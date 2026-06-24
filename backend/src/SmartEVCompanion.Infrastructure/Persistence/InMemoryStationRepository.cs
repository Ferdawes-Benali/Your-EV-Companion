using SmartEVCompanion.Application.Stations;
using SmartEVCompanion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartEVCompanion.Infrastructure.Persistence
{
    /// <summary>
    /// In-memory implementation of IStationRepository for MVP.
    /// TODO: Replace with EF Core implementation once DbContext is configured.
    /// </summary>
    public class InMemoryStationRepository : IStationRepository
    {
        // Sample stations in Tunis area for MVP testing
        private static readonly List<Station> Stations = new()
        {
            new Station
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Name = "Station Carthage",
                Latitude = 36.8533,
                Longitude = 10.3297
            },
            new Station
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Name = "Station Marsa",
                Latitude = 36.7333,
                Longitude = 10.2167
            },
            new Station
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Name = "Station Kasserine",
                Latitude = 35.1676,
                Longitude = 8.8324
            },
            new Station
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Name = "Station Sfax",
                Latitude = 34.7406,
                Longitude = 10.7603
            }
        };

        public Task<Station?> GetByIdAsync(Guid id)
        {
            var station = Stations.FirstOrDefault(s => s.Id == id);
            return Task.FromResult(station);
        }

        public Task<List<(Guid Id, double Lat, double Lng)>> GetAllWithLocationAsync(CancellationToken ct)
        {
            var locations = Stations
                .Select(s => (s.Id, s.Latitude, s.Longitude))
                .ToList();
            return Task.FromResult(locations);
        }
    }
}
