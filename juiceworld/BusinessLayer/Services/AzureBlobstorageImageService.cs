using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BusinessLayer.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Services;

public class AzureBlobstorageImageService : IImageService
{
    private const string ContainerName = "images";
    private readonly BlobContainerClient _containerClient;
    private readonly ILogger<ImageService> logger;

    public AzureBlobstorageImageService(ILogger<ImageService> logger, BlobServiceClient blobServiceClient)
    {
        this.logger = logger;
        _containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        _containerClient.CreateIfNotExists(PublicAccessType.Blob);
    }

    private static readonly Dictionary<string, string> MimeTypes = new()
    {
        { "/9j/", ".jpg" },
        { "iVBORw0KGgo", ".png" },
        { "R0lGODlh", ".gif" },
        { "R0lGODdh", ".gif" }
    };

    public string GetImageExtension(string base64Image)
    {
        foreach (var mimeType in MimeTypes)
            if (base64Image.StartsWith(mimeType.Key))
                return mimeType.Value;

        return string.Empty;
    }
    
    public string GetImageUrl(string imageName)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(imageName);
            return blobClient.Uri.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<bool> SaveImageAsync(string base64Image, string imageName)
    {
        try
        {
            var imageBytes = Convert.FromBase64String(base64Image);
            var blobClient = _containerClient.GetBlobClient(imageName);

            using var stream = new MemoryStream(imageBytes);
            await blobClient.UploadAsync(stream, overwrite: true);

            return true;
        }
        catch (Exception e)
        {
            logger.LogError($"ERROR - unable to upload image {imageName} to Azure Blob Storage \n ERROR-MESSAGE: {e.Message}");
            return false;
        }
    }

    public async Task<string?> GetImageAsync(string imageName)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(imageName);

            if (!await blobClient.ExistsAsync())
                return null;

            var downloadInfo = await blobClient.DownloadContentAsync();
            var imageBytes = downloadInfo.Value.Content.ToArray();

            return Convert.ToBase64String(imageBytes);
        }
        catch (Exception e)
        {
            logger.LogError($"ERROR - unable to retrieve image {imageName} from Azure Blob Storage \n ERROR-MESSAGE: {e.Message}");
            return null;
        }
    }

    public bool DeleteImage(string imageName)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(imageName);
            blobClient.DeleteIfExists();
            return true;
        }
        catch (Exception e)
        {
            logger.LogError($"ERROR - unable to delete image {imageName} from Azure Blob Storage \n ERROR-MESSAGE: {e.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateImageAsync(string base64Image, string? imageName, string newImageName)
    {
        if (imageName != null)
        {
            var oldBlobClient = _containerClient.GetBlobClient(imageName);
            await oldBlobClient.DeleteIfExistsAsync();
        }

        return await SaveImageAsync(base64Image, newImageName);
    }
}
