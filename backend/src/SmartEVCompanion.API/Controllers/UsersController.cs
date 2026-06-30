using Microsoft.AspNetCore.Mvc;
using SmartEVCompanion.Application.Common.Interfaces;
using SmartEVCompanion.Application.Services;
using System;
using System.Threading.Tasks;

namespace SmartEVCompanion.API.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UsersController : ControllerBase
    {
        private readonly IEmailQueue _emailQueue;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IEmailQueue emailQueue,
            ILogger<UsersController> logger)
        {
            _emailQueue = emailQueue ?? throw new ArgumentNullException(nameof(emailQueue));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Email))
                    return BadRequest("Email is required");

                // Basic email format check
                if (!request.Email.Contains("@") || !request.Email.Contains("."))
                    return BadRequest("A valid email address is required");

                // NOTE: Remaining TODO items (duplicate checks, password hashing, persistence)
                // should be implemented when a user store and authentication are added.

                // For now, simulate user creation
                var userId = Guid.NewGuid();
                var displayName = request.DisplayName ?? "User";

                // Enqueue welcome email (non-blocking, fire-and-forget)
                var welcomeEmail = EmailMessageFactory.CreateWelcome(request.Email, displayName);
                _emailQueue.Enqueue(welcomeEmail);

                _logger.LogInformation("User registered: {Email}. Welcome email queued.", request.Email);

                return CreatedAtAction(nameof(Register), new { userId }, new
                {
                    id = userId,
                    email = request.Email,
                    displayName = displayName,
                    message = "User created successfully. Welcome email sent."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred during registration" });
            }
        }
    }

    /// <summary>
    /// Request model for user registration.
    /// </summary>
    public record CreateUserRequest(
        string Email,
        string? DisplayName = null
    );
}
