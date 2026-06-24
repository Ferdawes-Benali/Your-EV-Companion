using System;
using System.Collections.Generic;

namespace SmartEVCompanion.Application.DTOs
{
    /// <summary>
    /// Result of a reach estimate, including range, reachable radius, 
    /// arrival SoC, and list of reachable stations.
    /// </summary>
    public record ReachEstimateResult(
        /// <summary>
        /// Estimated range in kilometers based on current SoC and vehicle consumption.
        /// Calculated as: (E_usable_kWh × 1000) / c_Wh_per_km
        /// </summary>
        double RangeKm,

        /// <summary>
        /// Conservative reachable radius (75% of range) for MVP station filtering.
        /// Used as a first-pass filter; full isochrone is available in IsochroneGeoJson.
        /// </summary>
        double ReachableRadiusKm,

        /// <summary>
        /// Predicted state of charge upon arrival at destination, as percentage.
        /// Null if no destination was provided.
        /// Calculated as: SoC_current% − (energy_needed_kWh / battery_kwh) × 100
        /// </summary>
        double? SocAtArrival,

        /// <summary>
        /// Whether the vehicle can reach the destination while maintaining reserve SoC.
        /// Null if no destination was provided.
        /// True if: SoC_arrival% >= soc_reserve%
        /// </summary>
        bool? CanReachDestination,

        /// <summary>
        /// GeoJSON representation of the isochrone (reachable area boundary) from ORS.
        /// Null if ORS call fails or is unavailable (graceful degradation).
        /// </summary>
        string? IsochroneGeoJson,

        /// <summary>
        /// List of charging station IDs reachable from current location within range.
        /// Uses MVP filter (straight-line distance ≤ reachable_radius_km).
        /// Infrastructure may apply more accurate PostGIS filtering.
        /// </summary>
        List<Guid> ReachableStationIds
    );
}
