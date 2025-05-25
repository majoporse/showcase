using System.Security.Claims;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Commons.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Customer))]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    private const string ApiBaseName = "Review";

    [HttpPost]
    [OpenApiOperation(ApiBaseName + nameof(CreateReview))]
    public async Task<ActionResult<ReviewDto>> CreateReview(ReviewDto review)
    {
        var result = await reviewService.CreateReviewAsync(review);
        return result == null ? Problem() : Ok(result);
    }

    [HttpGet]
    [OpenApiOperation(ApiBaseName + nameof(GetAllReviews))]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAllReviews()
    {
        var result = await reviewService.GetAllReviewsAsync();
        return Ok(result);
    }

    [HttpGet("{reviewId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(GetReview))]
    public async Task<ActionResult<ReviewDto>> GetReview(int reviewId)
    {
        var result = await reviewService.GetReviewByIdAsync(reviewId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPut]
    [OpenApiOperation(ApiBaseName + nameof(UpdateReview))]
    public async Task<ActionResult<ReviewDto>> UpdateReview(ReviewDto review)
    {
        var result = await reviewService.UpdateReviewAsync(review);

        if (result == null)
            return NotFound();

        if (User.IsInRole(UserRole.Admin.ToString())) return Ok(result);

        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? "", out var userId)) return Unauthorized();

        return review.UserId != userId ? NotFound() : Ok(result);
    }

    [HttpDelete("{reviewId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(DeleteReview))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<bool>> DeleteReview(int reviewId)
    {
        var result = await reviewService.DeleteReviewByIdAsync(reviewId);
        return result ? Ok() : NotFound();
    }
}