using SmartEVCompanion.Application.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace SmartEVCompanion.Application.Common.Interfaces
{
    /// <summary>
    /// Service to estimate vehicle reach based on location, battery state,
    /// and driving conditions, optionally calculating arrival SoC to a destination.
    /// </summary>
    public interface IReachEstimatorService
    {
        /// <summary>
        /// Estimates the vehicle's range and reachable charging stations from a location.
        /// 
        /// Calculation formula:
        ///   E_usable_kWh = battery_kwh × (soc_current% − soc_reserve%) / 100
        ///   c_Wh_per_km = base_consumption_wh_km × derating
        ///   range_km = (E_usable_kWh × 1000) / c_Wh_per_km
        ///   SoC_arrival% = soc_current% − (energy_needed_kWh / battery_kwh) × 100
        ///   reachable = SoC_arrival% >= soc_reserve%
        ///
        /// If destination is provided, calculates SoC at arrival.
        /// If isochrone provider fails, gracefully degrades with null IsochroneGeoJson.
        /// </summary>
        /// <param name="request">Request containing location, SoC, vehicle ID, and optional destination.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Result with range, reachable radius, SoC at arrival, and reachable station list.</returns>
        Task<ReachEstimateResult> EstimateAsync(ReachEstimateRequest request, CancellationToken ct);
    }
}
