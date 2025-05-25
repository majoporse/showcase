using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Installers;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using JuiceWorld.Entities;
using JuiceWorld.Repositories;
using Microsoft.Extensions.Caching.Memory;
using TestUtilities.MockedObjects;
using Xunit;
using Assert = Xunit.Assert;

namespace BusinessLayer.Tests.Services;

public class ReviewServiceTests
{
    private readonly IReviewService _reviewService;

    public ReviewServiceTests()
    {
        var dbContextOptions = MockedDbContext.GetOptions();
        var dbContext = MockedDbContext.CreateFromOptions(dbContextOptions);
        var reviewRepository = new Repository<Review>(dbContext);
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfileInstaller>());
        var mapper = config.CreateMapper();
        var cache = new MemoryCache(new MemoryCacheOptions());
        _reviewService = new ReviewService(reviewRepository, cache, mapper);
    }

    [Fact]
    public async Task GetAllReviewsAsync_ExactMatch()
    {
        // Arrange
        var reviewIdsToRetrieve = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = await _reviewService.GetAllReviewsAsync();

        // Assert
        var reviewDtos = result.ToList();
        Assert.Equal(reviewIdsToRetrieve.Length, reviewDtos.Count);
        Assert.All(reviewIdsToRetrieve, id => Assert.Contains(reviewDtos, review => review.Id == id));
    }

    [Fact]
    public async Task GetReviewByIdAsync_ExactMatch()
    {
        // Arrange
        var reviewId = 1;

        // Act
        var result = await _reviewService.GetReviewByIdAsync(reviewId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(reviewId, result.Id);
    }

    [Fact]
    public async Task CreateReviewAsync_Simple()
    {
        // Arrange
        var review = new ReviewDto
        {
            Id = 40,
            UserId = 2,
            ProductId = 2,
            Rating = 2,
            Body = "Bad product!"
        };

        // Act
        var result = await _reviewService.CreateReviewAsync(review);

        // Assert
        Assert.NotNull(result);
        Assert.True(review.UserId == result.UserId && review.ProductId == result.ProductId &&
                    review.Rating == result.Rating && review.Body == result.Body);
    }

    [Fact]
    public async Task UpdateReviewAsync_Simple()
    {
        // Arrange
        var review = new ReviewDto
        {
            Id = 4,
            UserId = 2,
            ProductId = 2,
            Rating = 2,
            Body = "Bad product!"
        };

        // Act
        var result = await _reviewService.UpdateReviewAsync(review);

        // Assert
        Assert.NotNull(result);
        Assert.True(review.UserId == result.UserId && review.ProductId == result.ProductId &&
                    review.Rating == result.Rating && review.Body == result.Body);
    }

    [Fact]
    public async Task DeleteReviewByIdAsync_Simple()
    {
        // Arrange
        var reviewId = 1;

        // Act
        var result = await _reviewService.DeleteReviewByIdAsync(reviewId);

        // Assert
        Assert.True(result);
    }
}