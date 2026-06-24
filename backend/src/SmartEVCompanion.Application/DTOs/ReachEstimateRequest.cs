using System;

namespace SmartEVCompanion.Application.DTOs
{
    /// <summary>
    /// Request to estimate the vehicle's reach (range) from a current location,
    /// optionally to a destination.
    /// </summary>
    public record ReachEstimateRequest(
        /// <summary>
        /// Current latitude.
        /// </summary>
        double Lat,

        /// <summary>
        /// Current longitude.
        /// </summary>
        double Lng,

        /// <summary>
        /// Current state of charge, as a percentage (0-100).
        /// </summary>
        double SocPercent,

        /// <summary>
        /// ID of the vehicle to estimate reach for.
        /// </summary>
        Guid VehicleId,

        /// <summary>
        /// Optional destination latitude. If provided with DestLng, 
        /// SoC at arrival will be calculated.
        /// </summary>
        double? DestLat = null,

        /// <summary>
        /// Optional destination longitude. If provided with DestLat,
        /// SoC at arrival will be calculated.
        /// </summary>
        double? DestLng = null,

        /// <summary>
        /// Driving condition affecting consumption (city: 1.00, highway: 1.15, cold: 1.25).
        /// Default is "city".
        /// </summary>
        string Condition = "city"
    );
}
