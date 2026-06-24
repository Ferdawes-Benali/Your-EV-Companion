using SmartEVCompanion.Application.Common.Interfaces;
using SmartEVCompanion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartEVCompanion.Infrastructure.Persistence
{
    /// <summary>
    /// In-memory implementation of IVehicleRepository for MVP.
    /// TODO: Replace with EF Core implementation once DbContext is configured.
    /// </summary>
    public class InMemoryVehicleRepository : IVehicleRepository
    {
        // Sample vehicles for MVP testing
        private static readonly Dictionary<Guid, Vehicle> Vehicles = new()
        {
            {
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                new Vehicle
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Name = "Tesla Model 3",
                    BatteryKwh = 60,
                    BaseConsumptionWhPerKm = 150,
                    SocReservePercent = 5
                }
            },
            {
                Guid.Parse("00000000-0000-0000-0000-000000000002"),
                new Vehicle
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Name = "Nissan Leaf",
                    BatteryKwh = 40,
                    BaseConsumptionWhPerKm = 170,
                    SocReservePercent = 5
                }
            }
        };

        public Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            Vehicles.TryGetValue(id, out var vehicle);
            return Task.FromResult(vehicle);
        }
    }
}
