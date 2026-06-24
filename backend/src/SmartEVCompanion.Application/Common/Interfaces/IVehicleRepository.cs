using SmartEVCompanion.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartEVCompanion.Application.Common.Interfaces
{
    /// <summary>
    /// Repository for persisting and retrieving vehicle entities.
    /// </summary>
    public interface IVehicleRepository
    {
        /// <summary>
        /// Retrieves a vehicle by its ID.
        /// </summary>
        /// <param name="id">The vehicle ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The vehicle if found; null otherwise.</returns>
        Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken ct);
    }
}
