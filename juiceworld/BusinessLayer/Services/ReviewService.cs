using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Infrastructure.Repositories;
using JuiceWorld.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace BusinessLayer.Services;

public class ReviewService(
    IRepository<Review> reviewRepository,
    IMemoryCache memoryCache,
    IMapper mapper) : IReviewService
{
    private readonly string _cacheKeyPrefix = nameof(ReviewService);
    private string CacheKeyReview(int id) => $"{_cacheKeyPrefix}-Review{id}";

    public async Task<ReviewDto?> CreateReviewAsync(ReviewDto reviewDto)
    {
        var newReview = await reviewRepository.CreateAsync(mapper.Map<Review>(reviewDto));
        return newReview is null ? null : mapper.Map<ReviewDto>(newReview);
    }

    public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
    {
        var reviews = await reviewRepository.GetAllAsync();
        return mapper.Map<List<ReviewDto>>(reviews);
    }

    public async Task<ReviewDto?> GetReviewByIdAsync(int id)
    {
        var cacheKey = CacheKeyReview(id);
        if (!memoryCache.TryGetValue(cacheKey, out Review? value))
        {
            value = await reviewRepository.GetByIdAsync(id);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
            memoryCache.Set(cacheKey, value, cacheEntryOptions);
        }

        return value is null ? null : mapper.Map<ReviewDto>(value);
    }

    public async Task<ReviewDto?> UpdateReviewAsync(ReviewDto reviewDto)
    {
        var cacheKey = CacheKeyReview(reviewDto.Id);
        memoryCache.Remove(cacheKey);
        var updatedReview = await reviewRepository.UpdateAsync(mapper.Map<Review>(reviewDto));

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

        memoryCache.Set(cacheKey, updatedReview, cacheEntryOptions);
        return updatedReview is null ? null : mapper.Map<ReviewDto>(updatedReview);
    }

    public Task<bool> DeleteReviewByIdAsync(int id)
    {
        var cacheKey = CacheKeyReview(id);
        memoryCache.Remove(cacheKey);
        return reviewRepository.DeleteAsync(id);
    }
}