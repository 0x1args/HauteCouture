using HauteCouture.Shared.CQS.Behaviors;
using HauteCouture.Shared.CQS.Extensions;
using HauteCouture.Shared.CQS.Registration.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HauteCouture.Shared.CQS.Registration;

/// <summary>
///     Registrar for handlers, validators, and event consumers from a given assembly.
/// </summary>
public static class CqsRegistrar
{
    /// <param name="services">DI service collection.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers validators, command/query handlers from the specified assembly,
        ///     and configures the CQS pipeline behaviors.
        /// </summary>
        /// <param name="configureOptions">CQS configuration.</param>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddCqs(Action<CqsOptions> configureOptions)
        {
            var cqsOptions = new CqsOptions();
            configureOptions(cqsOptions);

            if (cqsOptions.Assembly is null)
            {
                throw new InvalidOperationException(
                    $"Assembly must be specified. Use {nameof(cqsOptions.FromAssembly)}() method in configuration.");
            }

            // Registers all handlers in the DI.
            services
                .AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(cqsOptions.Assembly);
                })
                .AddCqsHandlersFromAssembly(cqsOptions.Assembly);

            // The order of registration defines the pipeline execution order.
            // Diagnostic --> Authorization --> Validation --> Logging --> Performance --> Caching --> Transactional is a typical order.
            if (cqsOptions.DiagnosticEnabled)
            {
                // Adds diagnostic/tracing behavior. Requires calling the UseDiagnostics() method.
                services.AddDiagnosticBehavior();
            }
            if (cqsOptions.AuthorizationEnabled)
            {
                // Adds authorization behavior. Requires calling the UseAuthorization() method.
                services.AddAuthorizationBehavior();
            }
            if (cqsOptions.ValidationEnabled)
            {
                // Adds validation behavior. Requires calling the UseValidation() method.
                services.AddValidationBehavior();
            }
            if (cqsOptions.LoggingEnabled)
            {
                // Adds logging behavior. Requires calling the UseLogging() method.
                services.AddLoggingBehavior();
            }
            if (cqsOptions.PerformanceEnabled)
            {
                // Adds performance tracking behavior. Requires calling the UsePerformanceTracking() method.
                services.AddPerformanceBehavior();
            }
            if (cqsOptions.CachingEnabled)
            {
                // Adds caching behavior. Requires calling the UseCaching() method.
                services.AddCachingBehavior();
            }
            if (cqsOptions.TransactionsEnabled)
            {
                // Adds transactional behavior. Requires calling the UseTransactions() method.
                services.AddTransactionBehavior();
            }

            return services;
        }

        /// <summary>
        ///     Registers the diagnostic/tracing pipeline behavior.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddDiagnosticBehavior()
        {
            return services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DiagnosticBehavior<,>));
        }

        /// <summary>
        ///     Registers the logging pipeline behavior.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddLoggingBehavior()
        {
            return services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        }

        /// <summary>
        ///     Registers the performance tracking pipeline behavior.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddPerformanceBehavior()
        {
            return services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        }

        /// <summary>
        ///     Registers the caching pipeline behavior.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddCachingBehavior()
        {
            return services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        }

        /// <summary>
        ///     Registers the validation pipeline behavior.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddValidationBehavior()
        {
            return services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }

        /// <summary>
        ///     Registers the authorization pipeline behavior.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddAuthorizationBehavior()
        {
            return services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        }

        /// <summary>
        ///     Registers the transactional pipeline behavior.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddTransactionBehavior()
        {
            return services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionalBehavior<,>));
        }
    }
}