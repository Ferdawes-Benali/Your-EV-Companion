using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartEVCompanion.Application.Common.Interfaces;
using SmartEVCompanion.Application.Stations;
using SmartEVCompanion.Application.Services;
using SmartEVCompanion.Infrastructure.Email;
using SmartEVCompanion.Infrastructure.ExternalServices;
using SmartEVCompanion.Infrastructure.Persistence;

namespace SmartEVCompanion.Infrastructure
{
    /// <summary>
    /// Extension methods for registering Infrastructure layer services.
    /// </summary>
    public static class InfrastructureServiceExtensions
    {
        /// <summary>
        /// Adds all Infrastructure services to the DI container.
        /// Call this in Program.cs: builder.Services.AddInfrastructureServices(builder.Configuration);
        /// </summary>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add memory caching for isochrone results
            services.AddMemoryCache();

            // Register HTTP clients
            services.AddHttpClient("ORS", c =>
                c.BaseAddress = new Uri("https://api.openrouteservice.org/"));

            services.AddHttpClient("Resend", c =>
                c.BaseAddress = new Uri("https://api.resend.com/"));

            // Register external services (Reach Estimator)
            services.AddScoped<IIsochroneProvider, OrsIsochroneProvider>();

            // Register repositories (MVP: in-memory implementations)
            services.AddSingleton<IVehicleRepository, InMemoryVehicleRepository>();
            services.AddSingleton<IStationRepository, InMemoryStationRepository>();

            // Register application services (Reach Estimator)
            services.AddScoped<IReachEstimatorService, ReachEstimatorService>();

            // Register email services
            services.AddSingleton<InProcessEmailQueue>();
            services.AddSingleton<IEmailQueue>(sp => sp.GetRequiredService<InProcessEmailQueue>());
            services.AddScoped<IEmailSender, ResendEmailSender>();
            services.AddHostedService<EmailDispatcherBackgroundService>();

            return services;
        }
    }
}
