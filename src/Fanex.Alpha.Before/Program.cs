using Fanex.Alpha.Before;
using Microsoft.AspNetCore;

ThreadPool.SetMinThreads(100, 100);

var host = BuildWebHost(args);
host.Run();

static IWebHost BuildWebHost(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .Build();
