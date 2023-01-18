using Fanex.Data;
using Fanex.Data.SqlServer;

namespace Fanex.Alpha.Before.Extensions;

public static class ApplicationBuilderExtensions
{
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