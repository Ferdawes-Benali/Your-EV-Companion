using Microsoft.AspNetCore.Mvc;
using SmartEVCompanion.Application.Common.Interfaces;
using SmartEVCompanion.Application.Services;
using System;
using System.Threading.Tasks;

namespace SmartEVCompanion.API.Controllers
{
    /// <summary>
    /// API endpoints for managing charging station reviews.
    /// Demonstrates email integration (Review Confirmation email on submission).
    /// </summary>
    [ApiController]
    [Route("api/v1/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IEmailQueue _emailQueue;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(
            IEmailQueue emailQueue,
            ILogger<ReviewsController> logger)
        {
            _emailQueue = emailQueue ?? throw new ArgumentNullException(nameof(emailQueue));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Submits a review for a charging station and sends confirmation email.
        /// This is a simplified example for demonstration.
        /// </summary>
        [HttpPost("submit")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitReview([FromBody] SubmitReviewRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Review request cannot be null");

                if (string.IsNullOrWhiteSpace(request.StationName) || string.IsNullOrWhiteSpace(request.UserEmail))
                    return BadRequest("Station name and user email are required");

                // TODO: In a real implementation, this would:
                // 1. Validate the request (rating 1-5, text not empty, etc.)
                // 2. Check that user exists
                // 3. Check that station exists
                // 4. Save review to database
                // 5. Update station rating
                // 6. Check for spam/profanity

                // For now, simulate review saving
                var reviewId = Guid.NewGuid();

                // Enqueue review confirmation email (non-blocking, fire-and-forget)
                var confirmationEmail = EmailMessageFactory.CreateReviewConfirmation(request.UserEmail, request.StationName);
                _emailQueue.Enqueue(confirmationEmail);

                _logger.LogInformation("Review submitted for {StationName} by {Email}. Confirmation email queued.", 
                    request.StationName, request.UserEmail);

                return CreatedAtAction(nameof(SubmitReview), new { reviewId }, new
                {
                    id = reviewId,
                    stationName = request.StationName,
                    rating = request.Rating,
                    message = "Review submitted successfully. Confirmation email sent."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting review");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while submitting the review" });
            }
        }
    }

    /// <summary>
    /// Request model for submitting a station review.
    /// </summary>
    public record SubmitReviewRequest(
        /// <summary>
        /// Name of the charging station being reviewed.
        /// </summary>
        string StationName,

        /// <summary>
        /// Email of the user submitting the review.
        /// </summary>
        string UserEmail,

        /// <summary>
        /// Rating (1-5 stars). Optional for this example.
        /// </summary>
        int? Rating = null,

        /// <summary>
        /// Review text. Optional for this example.
        /// </summary>
        string? ReviewText = null
    );
}
