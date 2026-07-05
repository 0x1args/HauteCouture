using HauteCouture.Shared.Databases.Postgres.Abstractions.Repositories;
using HauteCouture.Shared.Databases.Postgres.Abstractions.Transactions;
using HauteCouture.Shared.Databases.Postgres.Configurators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HauteCouture.Shared.Databases.Postgres.Registration;

/// <summary>
///     Registrar for data access with Postgres.
/// </summary>
public static class PostgresRegistrar
{
    /// <summary>
    ///     Maximum number of <see cref="DbContext"/> instances retained in the pool.
    /// </summary>
    public const int MaxDbContextPoolSize = 128;

    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers a pooled <see cref="DbContext"/> with <see cref="MaxDbContextPoolSize"/> along
        ///     with its configurator and the default <see cref="IRepository{TEntity,TKey}"/> implementation
        ///     backed by <see cref="PostgresRepository{TEntity,TKey}"/>.
        /// </summary>
        /// <typeparam name="TDbContext">The <see cref="DbContext"/> type to register.</typeparam>
        /// <typeparam name="TDbContextConfigurator">
        ///     The <see cref="IDbContextConfigurator{TDbContext}"/> implementation that configures
        ///     <typeparamref name="TDbContext"/>.
        /// </typeparam>
        public IServiceCollection AddPostgres<TDbContext, TDbContextConfigurator>()
            where TDbContext : DbContext
            where TDbContextConfigurator : class, IDbContextConfigurator<TDbContext>
        {
            services
                .AddPostgresBase<TDbContext, TDbContextConfigurator>()
                .AddScoped(typeof(IRepository<,>), typeof(PostgresRepository<,>));

            return services;
        }

        /// <summary>
        ///     Registers a pooled <see cref="DbContext"/> with <paramref name="poolSize"/> along
        ///     with its configurator and the default <see cref="IRepository{TEntity,TKey}"/> implementation
        ///     backed by <see cref="PostgresRepository{TEntity,TKey}"/>.
        /// </summary>
        /// <typeparam name="TDbContext">The <see cref="DbContext"/> type to register.</typeparam>
        /// <typeparam name="TDbContextConfigurator">
        ///     The <see cref="IDbContextConfigurator{TDbContext}"/> implementation that configures
        ///     <typeparamref name="TDbContext"/>.
        /// </typeparam>
        /// <param name="poolSize">
        ///     Maximum number of <see cref="DbContext"/> instances retained in the pool.
        ///     Instances exceeding this limit are discarded rather than returned to the pool.
        /// </param>
        public IServiceCollection AddPostgres<TDbContext, TDbContextConfigurator>(int poolSize)
            where TDbContext : DbContext
            where TDbContextConfigurator : class, IDbContextConfigurator<TDbContext>
        {
            services
                .AddPostgresBase<TDbContext, TDbContextConfigurator>(poolSize)
                .AddScoped(typeof(IRepository<,>), typeof(PostgresRepository<,>));

            return services;
        }

        /// <summary>
        ///     Registers a pooled <see cref="DbContext"/> with <see cref="MaxDbContextPoolSize"/> along
        ///     with its configurator, leaving repository registration entirely up to the caller.
        /// </summary>
        /// <typeparam name="TDbContext">The <see cref="DbContext"/> type to register.</typeparam>
        /// <typeparam name="TDbContextConfigurator">
        ///     The <see cref="IDbContextConfigurator{TDbContext}"/> implementation that configures
        ///     <typeparamref name="TDbContext"/>.
        /// </typeparam>
        /// <param name="configureRepository">
        ///     Callback responsible for registering the <see cref="IRepository{TEntity,TKey}"/> implementation.
        /// </param>
        public IServiceCollection AddPostgres<TDbContext, TDbContextConfigurator>(
            Action<IServiceCollection> configureRepository)
            where TDbContext : DbContext
            where TDbContextConfigurator : class, IDbContextConfigurator<TDbContext>
        {
            services.AddPostgresBase<TDbContext, TDbContextConfigurator>();
            configureRepository(services);

            return services;
        }

        /// <summary>
        ///     Registers a pooled <see cref="DbContext"/> with <paramref name="poolSize"/> along
        ///     with its configurator, leaving repository registration entirely up to the caller.
        /// </summary>
        /// <typeparam name="TDbContext">The <see cref="DbContext"/> type to register.</typeparam>
        /// <typeparam name="TDbContextConfigurator">
        ///     The <see cref="IDbContextConfigurator{TDbContext}"/> implementation that configures
        ///     <typeparamref name="TDbContext"/>.
        /// </typeparam>
        /// <param name="poolSize">
        ///     Maximum number of <see cref="DbContext"/> instances retained in the pool.
        /// </param>
        /// <param name="configureRepository">
        ///     Callback responsible for registering the <see cref="IRepository{TEntity,TKey}"/> implementation.
        /// </param>
        public IServiceCollection AddPostgres<TDbContext, TDbContextConfigurator>(
            int poolSize,
            Action<IServiceCollection> configureRepository)
            where TDbContext : DbContext
            where TDbContextConfigurator : class, IDbContextConfigurator<TDbContext>
        {
            services.AddPostgresBase<TDbContext, TDbContextConfigurator>(poolSize);
            configureRepository(services);

            return services;
        }

        private IServiceCollection AddPostgresBase<TDbContext, TDbContextConfigurator>(
            int poolSize = MaxDbContextPoolSize)
            where TDbContext : DbContext
            where TDbContextConfigurator : class, IDbContextConfigurator<TDbContext>
        {
            return services
                .AddDbContextPool<TDbContext>(ConfigureDbContext<TDbContext>, poolSize)
                .AddSingleton<IDbContextConfigurator<TDbContext>, TDbContextConfigurator>()
                .AddScoped<DbContext>(sp => sp.GetRequiredService<TDbContext>())
                .AddScoped<ITransactionalScope, PostgresTransactionalScope>();
        }
    }
    
    private static void ConfigureDbContext<TDbContext>(
        IServiceProvider provider,
        DbContextOptionsBuilder builder)
        where TDbContext : DbContext
    {
        var configurator = provider.GetRequiredService<IDbContextConfigurator<TDbContext>>();
        configurator.Configure((DbContextOptionsBuilder<TDbContext>)builder);
    }
}