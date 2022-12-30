using Fanex.Data;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.AspNetCore;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.RabbitMQ;
using Fanex.Logging.Sentry;
using ONEbook.UM.AspNetCore;
using ONEbook.UM.AspNetCore.Filters;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Services;
using ONEbook.UM.Transport;
using Sentry;

namespace Fanex.Alpha.Before.Web.Extensions;

public static class CustomExtensions
{
    public static IServiceCollection AddONEbookUM(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new UMClientConfiguration().Initialize(configuration));
        services.AddSingleton<CheckUMFilterAttribute>();
        services.AddSingleton<IUMCacheService, UMCacheService>();
        services.AddSingleton<IUMRestClient, UMRestClient>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // For WEB 
        services.AddSingleton<IHttpContextHelper, HttpContextHelper>(); // For WEB 
        services.AddSingleton<IMaintenanceService, MaintenanceService>();

        return services;
    }

    public static IServiceCollection AddFanexLogging(this IServiceCollection services, IConfiguration configuration)
    {
        var logEngines = new List<ILogEngine>
        {
            new RabbitMQLogging(configuration["Fanex.Logging:RabbitMQConnectionStrings"])
        };

        var sentryUrl = configuration["Fanex.Logging:SentryUrl"];
        if (!string.IsNullOrWhiteSpace(sentryUrl))
        {
            logEngines.Add(new SentryLogging(new SentryEngineOptions
            {
                Dsn = new Dsn(sentryUrl)
            }));
        }

        LogManager
                .SetDefaultLogCategory(configuration["Fanex.Logging:DefaultCategory"])
                .Use(logEngines);

        services.AddSingleton(Logger.Log);

        ExceptionHandler
            .UseConfig(new LoggingConfiguration()
            .GetHttpHeaderInfo()
            .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo));

        return services;
    }

    public static IApplicationBuilder UseFanexDataWithSqlServer(this IApplicationBuilder app, IConfiguration configuration, IWebHostEnvironment env)
    {
        var connectionStrings = configuration
            .GetSection("ConnectionStrings")
            .GetChildren()
            .ToDictionary(connection => connection.Key,
                        connection => new ConnectionConfiguration(connection.Key, connection.Value));
        var resourcePath = Path.Combine(env.ContentRootPath, "App_Data", "SPNames.xml");
        DbSettingProviderManager
            .StartNewSession()
            .WithSqlServer(resourcePath)
            .Use(connectionStrings)
            .Run();
        return app;
    }
}