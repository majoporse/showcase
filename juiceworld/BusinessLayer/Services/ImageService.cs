using BusinessLayer.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Services;

public class ImageService(ILogger<ImageService> logger) : IImageService
{
    public const string ImgFolderPath = "Images";

    private string ImagePath(string imageName) => Path.Combine(BusinessConstants.WebRootPath, ImgFolderPath, imageName);

    private static readonly Dictionary<string, string> MimeTypes = new()
    {
        // enough to determine the image type
        // source https://stackoverflow.com/questions/57976898/how-to-get-mime-type-from-base-64-string
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

    

    public async Task<bool> SaveImageAsync(string base64Image, string imageName)
    {
        Directory.CreateDirectory(Path.Combine(BusinessConstants.WebRootPath, ImgFolderPath));
        var imageBytes = Convert.FromBase64String(base64Image);
        var filePath = ImagePath(imageName);
        try
        {
            await File.WriteAllBytesAsync(filePath, imageBytes);
        }
        catch (Exception e)
        {
            logger.LogError($"ERROR - unable to write to file {filePath} \n ERROR-MESSAGE: {e.Message}");
            return false;
        }

        return true;
    }

    public async Task<string?> GetImageAsync(string imagePath)
    {
        var filePath = ImagePath(imagePath);
        if (!File.Exists(filePath)) return null;
        return Convert.ToBase64String(await File.ReadAllBytesAsync(filePath));
    }

    public string GetImageUrl(string imageName)
    {
        return ImagePath(imageName);
    }

    public bool DeleteImage(string imageName)
    {
        try
        {
            File.Delete(ImagePath(imageName));
        }
        catch (Exception e)
        {
            logger.LogError($"ERROR - unable to delete file {imageName} \n ERROR-MESSAGE: {e.Message}");
            return false;
        }

        return true;
    }

    public Task<bool> UpdateImageAsync(string base64Image, string? imageName, string newImageName)
    {
        if (imageName != null)
        {
            var oldImagePath = ImagePath(imageName);
            if (File.Exists(oldImagePath))
            {
                File.Delete(oldImagePath);
            }
        }
        return SaveImageAsync(base64Image, newImageName);
    }
}
