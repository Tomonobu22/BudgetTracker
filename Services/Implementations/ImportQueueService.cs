using BudgetTracker.Models;
using BudgetTracker.Services.Interfaces;
using System.Text.Json;
using Azure.Storage.Queues;

namespace BudgetTracker.Services.Implementations
{
    public class ImportQueueService : IImportQueueService
    {
        private readonly QueueClient _queueClient;
        public ImportQueueService(QueueServiceClient queueServiceClient, IConfiguration configuration)
        {
            string queueName = configuration["AzureStorage:QueueName"] ?? "imports";
            _queueClient = queueServiceClient.GetQueueClient(queueName);
            // New queue is created, if it doesn't already exists, by calling CreateIfNotExistsAsync method on the QueueClient instance.
            _queueClient.CreateIfNotExists();
        }

        public async Task<string?> DequeueImportAsync(int importId)
        {
            if (await _queueClient.ExistsAsync())
            {
                var message = await _queueClient.ReceiveMessageAsync();
                if (message != null)
                {
                    var importMessage = JsonSerializer.Deserialize<ImportQueueMessage>(message.Value.MessageText);
                    if (importMessage != null && importMessage.ImportId == importId)
                    {
                        await _queueClient.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt);
                        return message.Value.MessageText;
                    }
                }
            }
            return null;
        }

        public async Task EnqueueImportAsync(int importId)
        {
            var message = new ImportQueueMessage { ImportId = importId };
            string json = JsonSerializer.Serialize(message);
            await _queueClient.SendMessageAsync(json);
        }
    }
}
