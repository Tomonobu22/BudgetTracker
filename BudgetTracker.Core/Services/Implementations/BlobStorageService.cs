using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BudgetTracker.Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BudgetTracker.Core.Services.Implementations
{
    public class BlobStorageService: IBlobStorageService
    {
        private readonly BlobContainerClient _blobContainerClient;

        public BlobStorageService(BlobServiceClient blobServiceClient, IConfiguration configuration) 
        {
            var containerName = configuration["AzureStorage:ContainerName"];
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        }

        public async Task<Stream> DownloadAsync(string blobName, CancellationToken cancellationToken = default)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            var response = await blobClient.DownloadAsync(cancellationToken);
            return response.Value.Content;
        }

        public async Task UploadAsync(Stream fileStream, string blobName, string contentType, CancellationToken cancellationToken = default)
        {
            await _blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(fileStream, new BlobUploadOptions { 
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
            }, cancellationToken);
        }
    }
}
