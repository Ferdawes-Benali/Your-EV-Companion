using Microsoft.AspNetCore.Mvc;
using SmartEVCompanion.Application.Common.Interfaces;
using SmartEVCompanion.Application.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace SmartEVCompanion.API.Controllers
{
    /// <summary>
    /// API endpoints for vehicle reach estimation (range and reachable charging stations).
    /// </summary>
    [ApiController]
    [Route("api/v1/reach")]
    public class ReachController : ControllerBase
    {
        private readonly IReachEstimatorService _reachEstimatorService;
        private readonly ILogger<ReachController> _logger;

        public ReachController(
            IReachEstimatorService reachEstimatorService,
            ILogger<ReachController> logger)
        {
            _reachEstimatorService = reachEstimatorService ?? throw new ArgumentNullException(nameof(reachEstimatorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Estimates vehicle reach (range) from current location and optionally to a destination.
        /// Returns estimated range, reachable radius, SoC at arrival, and nearby charging stations.
        /// 
        /// Public endpoint (no authentication required).
        /// </summary>
        /// <param name="request">Reach estimation request with location and vehicle data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Reach estimation result with range, reachable stations, and optional isochrone GeoJSON.</returns>
        [HttpPost("estimate")]
        [ProducesResponseType(typeof(ReachEstimateResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Estimate(
            [FromBody] ReachEstimateRequest request,
            CancellationToken ct)
        {
            try
            {
                if (request == null)
                    return BadRequest("Request cannot be null");

                var result = await _reachEstimatorService.EstimateAsync(request, ct);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Vehicle not found: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Request was cancelled");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Request was cancelled" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during reach estimation");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An unexpected error occurred" });
            }
        }
    }
}
