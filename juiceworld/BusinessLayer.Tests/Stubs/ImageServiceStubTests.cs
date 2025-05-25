using BusinessLayer.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BusinessLayer.Tests.Stubs;

public class ImageServiceTests
{
    private const string TestImageBase64 = "iVBORw0KGgoAAAANSUhEUgAA"; // Example base64 image string
    private const string TestImageName = "testImage.png";
    private readonly ImageService _imageService;

    public ImageServiceTests()
    {
        // Mocking ILogger<ImageService>
        Mock<ILogger<ImageService>> loggerMock = new();

        // Initializing ImageService
        _imageService = new ImageService(loggerMock.Object);
    }

    [Fact]
    public void GetImageExtension_ValidImageExtension_ReturnsCorrectExtension()
    {
        // Arrange
        var base64Image = TestImageBase64;

        // Act
        var extension = _imageService.GetImageExtension(base64Image);

        // Assert
        Assert.Equal(".png", extension);
    }

    [Fact]
    public void GetImageExtension_InvalidImage_ReturnsEmptyString()
    {
        // Arrange
        var invalidBase64Image = "invalidbase64string";

        // Act
        var extension = _imageService.GetImageExtension(invalidBase64Image);

        // Assert
        Assert.Equal(string.Empty, extension); // We expect an empty string for invalid data
    }

    [Fact]
    public async Task SaveImageAsync_ValidImage_ReturnsTrue()
    {
        // Arrange
        var base64Image = TestImageBase64;
        var imageName = TestImageName;

        // Act
        var result = await _imageService.SaveImageAsync(base64Image, imageName);

        // Assert
        Assert.True(result);
        Assert.True(File.Exists(Path.Combine("Images", imageName)));

        // Clean up after test
        File.Delete(Path.Combine("Images", imageName));
    }

    [Fact]
    public async Task GetImageAsync_ExistingImage_ReturnsBase64String()
    {
        // Arrange
        var base64Image = TestImageBase64;
        var imageName = TestImageName;
        await _imageService.SaveImageAsync(base64Image, imageName);

        // Act
        var result = await _imageService.GetImageAsync(imageName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(base64Image, result);

        // Clean up after test
        File.Delete(Path.Combine("Images", imageName));
    }

    [Fact]
    public async Task GetImageAsync_NonExistingImage_ReturnsNull()
    {
        // Arrange
        var imageName = "nonExistingImage.png";

        // Act
        var result = await _imageService.GetImageAsync(imageName);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteImageAsync_ExistingImage_ReturnsTrue()
    {
        // Arrange
        var base64Image = TestImageBase64;
        var imageName = TestImageName;
        await _imageService.SaveImageAsync(base64Image, imageName);

        // Act
        var result = _imageService.DeleteImage(imageName);

        // Assert
        Assert.True(result);
        Assert.False(File.Exists(Path.Combine("Images", imageName)));
    }

    [Fact]
    public void DeleteImageAsync_NonExistingImage_ReturnsTrue()
    {
        // Arrange
        var imageName = "nonExistingImage.png";

        // Act
        var result = _imageService.DeleteImage(imageName);

        // Assert
        Assert.True(result); // It should return true even if the file doesn't exist
    }

    [Fact]
    public async Task UpdateImageAsync_ValidImage_UpdateSuccess()
    {
        // Arrange
        var base64Image = TestImageBase64;
        var imageName = "oldImage.png";
        var newImageName = "newImage.png";
        await _imageService.SaveImageAsync(base64Image, imageName);

        // Act
        var result = await _imageService.UpdateImageAsync(base64Image, imageName, newImageName);

        // Assert
        Assert.True(result);
        Assert.False(File.Exists(Path.Combine("Images", imageName))); // Old image should be deleted
        Assert.True(File.Exists(Path.Combine("Images", newImageName))); // New image should be saved

        // Clean up after test
        File.Delete(Path.Combine("Images", newImageName));
    }
}