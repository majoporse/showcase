namespace BusinessLayer.Services.Interfaces;

public interface IImageService
{
    public string GetImageExtension(string base64Image);
    public Task<bool> UpdateImageAsync(string base64Image, string? imageName, string newImageName);
    public Task<bool> SaveImageAsync(string base64Image, string imageName);
    public Task<string?> GetImageAsync(string imagePath);
    public string GetImageUrl(string imageName);
    public bool DeleteImage(string imageName);
}