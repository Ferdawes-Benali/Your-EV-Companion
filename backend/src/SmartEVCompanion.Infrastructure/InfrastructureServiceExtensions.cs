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
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMemoryCache();

            // Typed clients: configured HttpClient + service registration in one
            services.AddHttpClient<IIsochroneProvider, OrsIsochroneProvider>(c =>
                c.BaseAddress = new Uri("https://api.openrouteservice.org/"));

            services.AddHttpClient<IEmailSender, ResendEmailSender>(c =>
                c.BaseAddress = new Uri("https://api.resend.com/"));

            services.AddSingleton<IVehicleRepository, InMemoryVehicleRepository>();
            services.AddSingleton<IStationRepository, InMemoryStationRepository>();
            services.AddScoped<IReachEstimatorService, ReachEstimatorService>();

            services.AddSingleton<InProcessEmailQueue>();
            services.AddSingleton<IEmailQueue>(sp => sp.GetRequiredService<InProcessEmailQueue>());
            services.AddHostedService<EmailDispatcherBackgroundService>();
            return services;
        }
    }
}
