

using Fanex.Alpha.Before;
using Microsoft.AspNetCore;

var host = BuildWebHost(args);
host.Run();

static IWebHost BuildWebHost(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .UseUrls("http://0.0.0.0:5000")
        .Build();
