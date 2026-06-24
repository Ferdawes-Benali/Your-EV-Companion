using System;

namespace SmartEVCompanion.Domain.Entities
{
    /// <summary>
    /// Represents an electric vehicle with its specifications.
    /// </summary>
    public class Vehicle
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Vehicle display name or model.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Total usable battery capacity in kWh.
        /// </summary>
        public double BatteryKwh { get; set; }

        /// <summary>
        /// Base consumption in Wh/km (at standard conditions).
        /// </summary>
        public double BaseConsumptionWhPerKm { get; set; }

        /// <summary>
        /// Minimum safe state of charge threshold, as percentage (e.g., 5%).
        /// Below this, vehicle should not be driven.
        /// </summary>
        public double SocReservePercent { get; set; } = 5.0;
    }
}
