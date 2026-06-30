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

                // Basic validation:
                if (request.Rating.HasValue && (request.Rating < 1 || request.Rating > 5))
                    return BadRequest("Rating must be between 1 and 5");

                // Require either a rating or a review text to avoid empty submissions
                if (!request.Rating.HasValue && string.IsNullOrWhiteSpace(request.ReviewText))
                    return BadRequest("Either a rating or a review text is required");

                // NOTE: Remaining TODO items (persisting to DB, user/station existence checks,
                // spam/profanity checks) are left for full implementation once a persistence
                // layer and user management are added.

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
