using BusinessLayer.DTOs;

namespace BusinessLayer.Services.Interfaces;

public interface IReviewService
{
    Task<ReviewDto?> CreateReviewAsync(ReviewDto reviewDto);
    Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();
    Task<ReviewDto?> GetReviewByIdAsync(int id);
    Task<ReviewDto?> UpdateReviewAsync(ReviewDto reviewDto);
    Task<bool> DeleteReviewByIdAsync(int id);
}