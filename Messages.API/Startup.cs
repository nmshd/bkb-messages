using System;
using Enmeshed.BuildingBlocks.API.Extensions;
using Messages.API.Extensions;
using Messages.API.JsonConverters;
using Messages.Application;
using Messages.Application.Extensions;
using Messages.Infrastructure.EventBus;
using Messages.Infrastructure.Persistence;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messages.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApplicationOptions>(_configuration.GetSection("ApplicationOptions"));

            services.AddCustomAspNetCore(_configuration, _env, options =>
            {
                options.Authentication.Audience = "messages";
                options.Authentication.Authority = _configuration.GetAuthorizationConfiguration().Authority;
                options.Authentication.ValidIssuer = _configuration.GetAuthorizationConfiguration().ValidIssuer;

                options.Cors.AllowedOrigins = _configuration.GetCorsConfiguration().AllowedOrigins;
                options.Cors.ExposedHeaders = _configuration.GetCorsConfiguration().ExposedHeaders;

                options.HealthChecks.SqlConnectionString = _configuration.GetSqlDatabaseConfiguration().ConnectionString;

                options.Json.Converters.Add(new FileIdJsonConverter());
                options.Json.Converters.Add(new MessageIdJsonConverter());
            });

            services.AddCustomApplicationInsights();

            services.AddCustomFluentValidation(_ => { });

            services.AddPersistence(options =>
            {
                options.DbOptions.DbConnectionString = _configuration.GetSqlDatabaseConfiguration().ConnectionString;

                options.BlobStorageOptions.ConnectionString = _configuration.GetBlobStorageConfiguration().ConnectionString;
                options.BlobStorageOptions.ContainerName = "messages";
            });

            services.AddEventBus(_configuration.GetEventBusConfiguration());

            services.AddApplication();

            return services.ToAutofacServiceProvider();
        }

        public void Configure(IApplicationBuilder app, TelemetryConfiguration telemetryConfiguration)
        {
            telemetryConfiguration.DisableTelemetry = !_configuration.GetApplicationInsightsConfiguration().Enabled;

            app.ConfigureMiddleware(_env);
        }
    }
}
