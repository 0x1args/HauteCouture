using HauteCouture.Example.Applications.AppServices;
using HauteCouture.Example.Applications.AppServices.Abstractions;
using HauteCouture.Example.Applications.Handlers;
using HauteCouture.Example.Infrastuctures.DataAccess.Configurators;
using HauteCouture.Example.Infrastuctures.DataAccess.DbContexts;
using HauteCouture.Shared.CQS.Registration;
using HauteCouture.Shared.Databases.Postgres.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace HauteCouture.Example.Providers;

/// <summary>
///     Provides extension methods for registering the Example services.
/// </summary>
public static class ServiceRegistrar
{
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers all services required by the Example.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddCore()
        {
            services
                .AddApplications()
                .AddCqs()
                .AddInfrastructures();

            return services;
        }

        /// <summary>
        ///     Registers infrastructure services, including the Postgres <see cref="SomethingDbContext"/>.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddInfrastructures()
        {
            services.AddPostgres<SomethingDbContext, SomethingDbContextConfigurator>();
            return services;
        }

        /// <summary>
        ///     Registers application-level services.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddApplications()
        {
            services
                .AddSingleton(TimeProvider.System)
                .AddScoped<ISomethingService, SomethingService>();

            return services;
        }

        /// <summary>
        ///     Registers the CQS pipeline.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddCqs()
        {
            services
                .AddCqs(options => options
                    .FromAssembly(AssemblyReference.Assembly)
                    .UseDiagnostics()
                    .UseAuthorization()
                    .UseValidation()
                    .UseLogging()
                    .UsePerformanceTracking()
                    .UseCaching()
                    .UseTransactions());

            return services;
        }
    }
}