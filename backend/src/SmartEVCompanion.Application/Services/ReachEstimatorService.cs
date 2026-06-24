using SmartEVCompanion.Application.Common.Interfaces;
using SmartEVCompanion.Application.DTOs;
using SmartEVCompanion.Application.Stations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartEVCompanion.Application.Services
{
    /// <summary>
    /// Implements reach estimation using battery, consumption, and location data.
    /// Calculates estimated range and filters reachable charging stations.
    /// </summary>
    public class ReachEstimatorService : IReachEstimatorService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IIsochroneProvider _isochroneProvider;
        private readonly IStationRepository _stationRepository;

        public ReachEstimatorService(
            IVehicleRepository vehicleRepository,
            IIsochroneProvider isochroneProvider,
            IStationRepository stationRepository)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _isochroneProvider = isochroneProvider ?? throw new ArgumentNullException(nameof(isochroneProvider));
            _stationRepository = stationRepository ?? throw new ArgumentNullException(nameof(stationRepository));
        }

        /// <summary>
        /// Estimates vehicle reach using the formulas:
        ///   E_usable_kWh  = battery_kwh × (soc_current% − soc_reserve%) / 100
        ///   c_Wh_per_km   = base_consumption_wh_km × derating
        ///   range_km      = (E_usable_kWh × 1000) / c_Wh_per_km
        ///   SoC_arrival%  = soc_current% − (energy_needed_kWh / battery_kwh) × 100
        ///   reachable     = SoC_arrival% >= soc_reserve%
        /// </summary>
        public async Task<ReachEstimateResult> EstimateAsync(ReachEstimateRequest request, CancellationToken ct)
        {
            // Fetch vehicle specs
            var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId, ct);
            if (vehicle == null)
                throw new InvalidOperationException($"Vehicle with ID {request.VehicleId} not found.");

            // Calculate usable energy
            var usableEnergy = vehicle.BatteryKwh * (request.SocPercent - vehicle.SocReservePercent) / 100;

            // Determine derating factor based on condition
            var derating = GetDeratingFactor(request.Condition);

            // Calculate consumption (Wh/km) under current conditions
            var consumptionWhPerKm = vehicle.BaseConsumptionWhPerKm * derating;

            // Calculate range
            var rangeKm = (usableEnergy * 1000) / consumptionWhPerKm;

            // Calculate reachable radius (MVP uses 75% of range)
            var reachableRadiusKm = rangeKm * 0.75;

            // If destination provided, calculate SoC at arrival
            double? socAtArrival = null;
            bool? canReachDestination = null;
            if (request.DestLat.HasValue && request.DestLng.HasValue)
            {
                var distanceKm = CalculateHaversineDistance(
                    request.Lat, request.Lng,
                    request.DestLat.Value, request.DestLng.Value);

                var energyNeededKwh = (distanceKm * consumptionWhPerKm) / 1000;
                socAtArrival = request.SocPercent - (energyNeededKwh / vehicle.BatteryKwh) * 100;
                canReachDestination = socAtArrival >= vehicle.SocReservePercent;
            }

            // Get isochrone GeoJSON (graceful fallback if unavailable)
            string? isochroneGeoJson = null;
            try
            {
                isochroneGeoJson = await _isochroneProvider.GetIsochroneGeoJsonAsync(
                    request.Lat, request.Lng, rangeKm, ct);
            }
            catch
            {
                // Graceful degradation: continue without isochrone
                isochroneGeoJson = null;
            }

            // Get all stations and filter by straight-line distance (MVP approach)
            var reachableStations = await GetNearbyStationsAsync(
                request.Lat, request.Lng, reachableRadiusKm, ct);

            return new ReachEstimateResult(
                RangeKm: rangeKm,
                ReachableRadiusKm: reachableRadiusKm,
                SocAtArrival: socAtArrival,
                CanReachDestination: canReachDestination,
                IsochroneGeoJson: isochroneGeoJson,
                ReachableStationIds: reachableStations);
        }

        /// <summary>
        /// Gets derating factor based on driving condition.
        /// </summary>
        private double GetDeratingFactor(string condition) => condition.ToLower() switch
        {
            "highway" => 1.15,
            "cold" => 1.25,
            "city" or _ => 1.00
        };

        /// <summary>
        /// Calculates straight-line distance between two coordinates (Haversine formula).
        /// Returns distance in kilometers.
        /// </summary>
        private double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusKm = 6371;

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Asin(Math.Sqrt(a));
            return EarthRadiusKm * c;
        }

        private double ToRadians(double degrees) => degrees * Math.PI / 180;

        /// <summary>
        /// MVP approach: filters stations by straight-line distance.
        /// Future versions can use PostGIS for more accurate road-based distance.
        /// </summary>
        private async Task<List<Guid>> GetNearbyStationsAsync(
            double lat, double lng, double radiusKm, CancellationToken ct)
        {
            var allStations = await _stationRepository.GetAllWithLocationAsync(ct);
            var reachableStations = new List<Guid>();

            foreach (var station in allStations)
            {
                var distance = CalculateHaversineDistance(lat, lng, station.Lat, station.Lng);
                if (distance <= radiusKm)
                {
                    reachableStations.Add(station.Id);
                }
            }

            return reachableStations;
        }
    }
}
