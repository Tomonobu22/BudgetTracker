using System;
using System.Threading.Tasks;
using Azure.Storage.Queues.Models;
using BudgetTracker.Core.Models;
using BudgetTracker.Core.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Functions;

public class ImportQueueTrigger
{
    private readonly IImportProcessingService _importProcessingService;

    public ImportQueueTrigger(IImportProcessingService importProcessingService)
    {
        _importProcessingService = importProcessingService;
    }

    [Function(nameof(ImportQueueTrigger))]
    public async Task Run([QueueTrigger("imports", Connection = "AzureWebJobsStorage")] string message)
    {
        var importQueueMessage = System.Text.Json.JsonSerializer.Deserialize<ImportQueueMessage>(message);
        if (importQueueMessage == null)
        {
            throw new ArgumentNullException(nameof(importQueueMessage), "ImportQueueMessage is null");
        }
        await _importProcessingService.ProcessImportAsync(importQueueMessage.ImportId);
    }
}