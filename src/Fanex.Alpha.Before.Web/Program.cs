

using Fanex.Alpha.Before.Web;
using Microsoft.AspNetCore;

var host = BuildWebHost(args);
host.Run();

static IWebHost BuildWebHost(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .Build();
