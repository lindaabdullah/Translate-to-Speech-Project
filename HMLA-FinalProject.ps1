# connect to azure
Connect-AzAccount

# importing modules
Import-Module Az.Storage
Import-Module Az.Sql

# ask user if they have their own translator and speech services
$question = Read-Host "Do you have your own translator and speech services? yes or no"

if ($question -eq "yes"){
    $translatekey = Read-Host -Prompt "Input your translator service key:"
    $translateregion = Read-Host -Prompt "Input your translator region:"
    $speechkey = Read-Host -Prompt "Input your speech service key:"
    $speechregion = Read-Host -Prompt "Input your speech service region:"
}

# retrieve subscription ID
$SubscriptionId = (Get-AzContext).Subscription.id

# suppress the breaking change warning messages in Azure PowerShell
Set-Item Env:\SuppressAzurePowerShellBreakingChangeWarnings "true"

# Set subscription
Set-AzContext -SubscriptionId $subscriptionId

# resource group name
$resourceGroupName = "FinalProject-HMLA"

#location
$location = "East US"

#variables to be used for the database and database server:
# server name
$serverName = "server-hmlafinalproj"
#database name
$databaseName = "HMLA-DatabaseFinalproj"
#allowed ips
$startIp = "0.0.0.0"
$endIp = "255.255.255.255"
# database login information
$adminSqlLogin = "lindadb"
$password = "LindaAbdullah2001"


#create new resource group
New-AzResourceGroup -Name $resourceGroupName -Location "East US"


##----- We created the translator and speech services manually from Azure Portal -----##


# Web app variables
$webappname="webapp-CMPE363-FinalProject-HMLA"

$WAppServicePLan = "ASP-CMPE363FinalProjectHMLA-a1589"

# setting environment variables so user does not need to change them manually from the website's code
$WAppSettings =@{
                'TRANSLATE_KEY' =  '$translatekey '; 
                'TRANSLATE_REGION' = '$translateregion';
                'SPEECH_KEY' = '$speechkey';
                'SPEECH_REGION' = '$speechregion';
}

# deploy code from github to webapp
$gitrepo = "https://github.com/lindaabdullah/CMPE363_FinalProject.git"
$webappname="webapp-finalproject-HMLA"
$PropertiesObject = @{
    repoUrl = "$gitrepo";
    branch = "master";
    isManualIntegration = "true";
}

# creates a new webapp
New-AzWebApp -ResourceGroupName $resourceGroupName -Name $webappname -Location $location -AppServicePlan $WAppServicePLan -GitRepositoryPath $gitrepo

# deployment part ---->
# deploying once from github but everytime the GitHub repository is updated, we click "sync" in the deployment center
Set-AzResource -Properties $PropertiesObject -ResourceGroupName $resourceGroupName -ResourceType Microsoft.Web/sites/sourcecontrols -ResourceName $webappname/web -ApiVersion 2015-08-01 -Force

# set environment variables for code to use
Set-AzWebApp -AppSettings $WAppSettings -Name $webappname -ResourceGroupName $resourceGroupName


# Function app variables
$FAppServicePlan = "ASP-Functionapp-HMLA"
$FunctionAppName = "HMLAFunctionapp"
$storageAccount = "storagehmla"
$tier = "Standard"

# required function app settings
$FunctionAppSettings = @{
    ServerFarmId="/subscriptions/$SubscriptionId/resourceGroups/$resourceGroupName/providers/Microsoft.Web/serverfarms/$FAppServicePlan";
    alwaysOn=$True;
}


#========Creating Azure Storage Account========
New-AzStorageAccount -ResourceGroupName $resourceGroupName -AccountName $storageAccount -Location $location -SkuName "Standard_LRS"


#========Creating App Service Plan============
New-AzAppServicePlan -ResourceGroupName $resourceGroupName -Name $FAppServicePlan -Location $location -Tier $tier

#========Creating Azure Function========
$FunctionAppResource = Get-AzResource | Where-Object { $_.ResourceName -eq $FunctionAppName -And $_.ResourceType -eq "Microsoft.Web/Sites" }

New-AzResource -ResourceType 'Microsoft.Web/Sites' -ResourceName $FunctionAppName -kind 'functionapp' -Location $location -ResourceGroupName $resourceGroupName -Properties $FunctionAppSettings -force


# Create an Azure SQL DB---->

# create server
$server = New-AzSqlServer -ResourceGroupName $resourceGroupName `
    -ServerName $serverName `
    -Location $location `
    -SqlAdministratorCredentials $(New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $adminSqlLogin, $(ConvertTo-SecureString -String $password -AsPlainText -Force))

# create a server firewall rule that allows access from the specified IP range
$serverFirewallRule = New-AzSqlServerFirewallRule -ResourceGroupName $resourceGroupName `
    -ServerName $serverName `
    -FirewallRuleName "AllowedIPs" -StartIpAddress $startIp -EndIpAddress $endIp

# create a blank database with an S0 performance level
$database = New-AzSqlDatabase  -ResourceGroupName $resourceGroupName `
    -ServerName $serverName `
    -DatabaseName $databaseName `
    -RequestedServiceObjectiveName "S0" `
    -SampleName "AdventureWorksLT"


$create = "
CREATE TABLE log (
	id int PRIMARY KEY IDENTITY(1, 1),
	text nvarchar(255) NOT NULL,
	detected_language nvarchar(255) NOT NULL,
	translation nvarchar(255) NOT NULL,
	target_language nvarchar(255) NOT NULL,
	created_at DATE DEFAULT CURRENT_TIMESTAMP
)"

# create table "log"
Invoke-Sqlcmd -ServerInstance "server-hmlafinalproj.database.windows.net" -Database $databaseName -Query $create -Username $adminSqlLogin -Password $password -Verbose 
   


# <------ configuring Function App settings so the function is able to access environment variables from the funciton app---->

#========Retrieving Keys========
$keys = Get-AzStorageAccountKey -ResourceGroupName $resourceGroupName -AccountName $storageAccount
$accountKey = $keys | Where-Object { $_.KeyName -eq 'Key1' } | Select-Object -ExpandProperty Value
$storageAccountConnectionString = 'DefaultEndpointsProtocol=https;AccountName='+$storageAccount+';AccountKey='+$accountKey


#========Defining Azure Function Settings========
$AppSettings =@{
    'AzureWebJobsDashboard' = $storageAccountConnectionString;
    'AzureWebJobsStorage' = $storageAccountConnectionString;
    'FUNCTIONS_EXTENSION_VERSION' = '~3';
    'FUNCTION_APP_EDIT_MODE'= 'readwrite';
    'FUNCTIONS_WORKER_RUNTIME' = 'dotnet';
    'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING' = $storageAccountConnectionString;
    'WEBSITE_CONTENTSHARE' = $storageAccount;
    'DB_CONNECTIONSTRING' = 'Server=tcp:server-hmlafinalproj.database.windows.net,1433;Initial Catalog=HMLA-DatabaseFinalproj;Persist Security Info=False;User ID=lindadb;Password=LindaAbdullah2001;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;';
} 

# set the function app's settings
Set-AzWebApp -Name $FunctionAppName -ResourceGroupName $resourceGroupName -AppSettings $AppSettings