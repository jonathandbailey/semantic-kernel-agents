using Azure.Storage.Blobs;

namespace Todo.Core.Extensions;

public static class AzureStorageExtensions
{
    public static async Task<string> DownloadBlobAsync(this BlobContainerClient blobContainerClient, string blobName)
    {
        var blobClient = blobContainerClient.GetBlobClient(blobName);

        using var memoryStream = new MemoryStream();

        await blobClient.DownloadToAsync(memoryStream);
        memoryStream.Position = 0;

        using var reader = new StreamReader(memoryStream);

        return await reader.ReadToEndAsync();
    }
}