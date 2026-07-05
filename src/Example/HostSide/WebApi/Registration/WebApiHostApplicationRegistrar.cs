using HauteCouture.Shared.WebApi.Endpoints;
using HauteCouture.Shared.WebApi.Registration;

namespace HauteCouture.Example.HostSide.Public.WebApi.Registration;

/// <summary>
///     Provides extension methods for configuring the Example web API
///     HTTP request pipeline.
/// </summary>
public static class WebApiHostApplicationRegistrar
{
    extension(WebApplication app)
    {
        /// <summary>
        /// Configures the HTTP request pipeline for the Example web API.
        /// </summary>
        /// <returns>The same <see cref="WebApplication"/> instance for chaining.</returns>
        public WebApplication UseExampleWebApi()
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseWebModules();
            app.MapEndpoints();

            return app;
        }
    }
}