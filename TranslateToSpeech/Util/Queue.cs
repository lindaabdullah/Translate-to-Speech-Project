using System; // Namespace for Console output
using System.Configuration; // Namespace for ConfigurationManager
using System.Threading.Tasks; // Namespace for Task
//using Azure.Identity;
using Azure.Storage.Queues; // Namespace for Queue storage types
using Azure.Storage.Queues.Models; // Namespace for PeekedMessage

//-------------------------------------------------
// Insert a message into a queue
//-------------------------------------------------

namespace TranslateToSpeech
{
    public class Queue
    {
        public static void InsertMessage(string queueName, string message)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=cmpe363finaltemp;AccountKey=DK97Jdg7cicqgz0kmvPykYuOfVJhOYYPcAM0by8PP4Ss7Kfh0Sok5Ao6Tq+j4j4py5UOmViY/GDUpSm3JDSW+g==;EndpointSuffix=core.windows.net";
            QueueClient queueClient = new QueueClient(connectionString, queueName);
            queueClient.CreateIfNotExists();

            if (queueClient.Exists())
            {
                queueClient.SendMessage(message);
            }
        }
    }
}