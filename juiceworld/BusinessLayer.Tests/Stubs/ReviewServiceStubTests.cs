using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Installers;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using Infrastructure.Repositories;
using JuiceWorld.Entities;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace BusinessLayer.Tests.Stubs;

public class ReviewServiceStubTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IRepository<Review>> _reviewRepositoryMock;

    private readonly List<Review> _reviews = new()
    {
        new Review
        {
            Id = 1,
            Rating = 5,
            Body = "Amazing product!",
            ProductId = 101,
            UserId = 1
        },
        new Review
        {
            Id = 2,
            Rating = 3,
            Body = "It's okay.",
            ProductId = 102,
            UserId = 2
        }
    };

    private readonly IReviewService _reviewService;

    public ReviewServiceStubTests()
    {
        // Initialize mock repository
        _reviewRepositoryMock = new Mock<IRepository<Review>>();

        // Configure AutoMapper
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfileInstaller>());
        _mapper = config.CreateMapper();

        // Initialize the service with the mocked repository and mapper
        var cache = new MemoryCache(new MemoryCacheOptions());
        _reviewService = new ReviewService(_reviewRepositoryMock.Object, cache, _mapper);
    }

    [Fact]
    public async Task GetAllReviewsAsync_ExactMatch()
    {
        // Arrange
        _reviewRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_reviews);

        // Act
        var result = await _reviewService.GetAllReviewsAsync();

        // Assert
        var reviewDtos = result.ToList();
        Assert.Equal(_reviews.Count, reviewDtos.Count);
        Assert.All(_reviews, review => Assert.Contains(reviewDtos, dto => dto.Id == review.Id));
    }

    [Fact]
    public async Task GetReviewByIdAsync_ExactMatch()
    {
        // Arrange
        var reviewId = 1;
        _reviewRepositoryMock.Setup(repo => repo.GetByIdAsync(reviewId)).ReturnsAsync(_reviews[0]);

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
        var reviewDto = new ReviewDto
        {
            Id = 3,
            Rating = 4,
            Body = "Pretty good.",
            ProductId = 103,
            UserId = 3
        };

        var review = _mapper.Map<Review>(reviewDto);
        _reviewRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Review>(), null, true)).ReturnsAsync(review);

        // Act
        var result = await _reviewService.CreateReviewAsync(reviewDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(reviewDto.Id, result.Id);
        Assert.Equal(reviewDto.Rating, result.Rating);
        Assert.Equal(reviewDto.Body, result.Body);
        Assert.Equal(reviewDto.ProductId, result.ProductId);
        Assert.Equal(reviewDto.UserId, result.UserId);
    }

    [Fact]
    public async Task UpdateReviewAsync_Simple()
    {
        // Arrange
        var reviewDto = new ReviewDto
        {
            Id = 1,
            Rating = 4,
            Body = "Updated review.",
            ProductId = 101,
            UserId = 1
        };

        var review = _mapper.Map<Review>(reviewDto);
        _reviewRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Review>(), null, true)).ReturnsAsync(review);

        // Act
        var result = await _reviewService.UpdateReviewAsync(reviewDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(reviewDto.Id, result.Id);
        Assert.Equal(reviewDto.Rating, result.Rating);
        Assert.Equal(reviewDto.Body, result.Body);
        Assert.Equal(reviewDto.ProductId, result.ProductId);
        Assert.Equal(reviewDto.UserId, result.UserId);
    }

    [Fact]
    public async Task DeleteReviewByIdAsync_Simple()
    {
        // Arrange
        var reviewId = 1;
        _reviewRepositoryMock.Setup(repo => repo.DeleteAsync(reviewId, null, true)).ReturnsAsync(true);

        // Act
        var result = await _reviewService.DeleteReviewByIdAsync(reviewId);

        // Assert
        Assert.True(result);
    }
}
