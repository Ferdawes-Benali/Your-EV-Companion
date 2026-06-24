using SmartEVCompanion.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SmartEVCompanion.Infrastructure.ExternalServices
{
    /// <summary>
    /// Implements IIsochroneProvider using OpenRouteService API.
    /// Endpoint: https://api.openrouteservice.org/v2/isochrones/driving-car
    /// </summary>
    public class OrsIsochroneProvider : IIsochroneProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<OrsIsochroneProvider> _logger;

        public OrsIsochroneProvider(
            HttpClient httpClient,
            IConfiguration configuration,
            IMemoryCache memoryCache,
            ILogger<OrsIsochroneProvider> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves isochrone GeoJSON from OpenRouteService.
        /// Results are cached for 30 minutes.
        /// Returns null on any error (graceful degradation).
        /// </summary>
        public async Task<string?> GetIsochroneGeoJsonAsync(double lat, double lng, double rangeKm, CancellationToken ct)
        {
            try
            {
                // Create cache key
                var cacheKey = $"ors_{Math.Round(lat, 3)}_{Math.Round(lng, 3)}_{(int)rangeKm}";

                // Check cache first
                if (_memoryCache.TryGetValue(cacheKey, out string? cachedResult))
                {
                    _logger.LogDebug("Isochrone cache hit for {CacheKey}", cacheKey);
                    return cachedResult;
                }

                // Prepare request body
                var rangeMeters = (int)(rangeKm * 1000);
                var requestBody = new
                {
                    locations = new[] { new[] { lng, lat } },  // ORS expects [lng, lat]
                    range = new[] { rangeMeters },
                    range_type = "distance"
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                // Get API key from configuration
                var apiKey = _configuration["OrsApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogWarning("OrsApiKey not configured");
                    return null;
                }

                // Set authorization header
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // Call ORS API
                var response = await _httpClient.PostAsync(
                    "v2/isochrones/driving-car",
                    content,
                    ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "ORS API returned status {StatusCode}: {ReasonPhrase}",
                        response.StatusCode, response.ReasonPhrase);
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync(ct);

                // Cache the result for 30 minutes
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };
                _memoryCache.Set(cacheKey, responseContent, cacheOptions);

                _logger.LogDebug("Successfully retrieved and cached isochrone for {CacheKey}", cacheKey);
                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error calling ORS API");
                return null;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "ORS API call timed out");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling ORS API");
                return null;
            }
        }
    }
}
