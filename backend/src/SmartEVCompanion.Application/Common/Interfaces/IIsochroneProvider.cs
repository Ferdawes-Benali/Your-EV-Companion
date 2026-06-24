using System.Threading;
using System.Threading.Tasks;

namespace SmartEVCompanion.Application.Common.Interfaces
{
    /// <summary>
    /// Provides isochrone (reachable area boundary) data from an external routing service.
    /// An isochrone is a polygon showing all points reachable within a given distance/time.
    /// </summary>
    public interface IIsochroneProvider
    {
        /// <summary>
        /// Retrieves the isochrone GeoJSON for a given location and range.
        /// 
        /// Returns null if the call fails or times out (graceful degradation).
        /// Implementations should cache results to avoid redundant API calls.
        /// </summary>
        /// <param name="lat">Latitude of the starting point.</param>
        /// <param name="lng">Longitude of the starting point.</param>
        /// <param name="rangeKm">Maximum range in kilometers.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>GeoJSON string representing the isochrone, or null on failure.</returns>
        Task<string?> GetIsochroneGeoJsonAsync(double lat, double lng, double rangeKm, CancellationToken ct);
    }
}
