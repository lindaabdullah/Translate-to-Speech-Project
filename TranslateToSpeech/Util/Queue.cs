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
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=storagehmla;AccountKey=CI+qdaSYWKUH5ptJ3szE9ndTGiqTJKiTZqppLZ/4LGDfwjhA6mG1NywgIR2iKJ/OBJZ2Fbjj0rfkxzAaEEMUCg==;EndpointSuffix=core.windows.net";
            QueueClient queueClient = new QueueClient(connectionString, queueName);
            queueClient.CreateIfNotExists();

            if (queueClient.Exists())
            {
                queueClient.SendMessage(message);
            }
        }
    }
}