// This Startup file is based on ASP.NET Core new project templates and is included
// as a starting point for DI registration and HTTP request processing pipeline configuration.
// This file will need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core startup files, see https://docs.microsoft.com/aspnet/core/fundamentals/startup

using Alpha.SliderCaptcha.AspNetCore;
using Fanex.Alpha.Before.Helpers;
using Fanex.Alpha.Before.Repositories;
using Fanex.Alpha.Before.Services;
using Fanex.Alpha.Before.Services.FPS;
using Fanex.Alpha.Before.Services.SignIn;
using Fanex.Alpha.Before.Extensions;
using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TelegramBot.Client.Services;

namespace Fanex.Alpha.Before
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.Cookie.Name = ".Alpha.Before.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.IsEssential = true;
            });

            services.AddFanexLogging(Configuration)
                 .AddONEbookUM(Configuration)
                 .AddCustomOptions(Configuration)
                 .AddNetThrottling(Configuration)
                 .AddSliderCaptcha(Configuration)
                 .AddCustomHealthChecks()
                 .AddControllersWithViews()
                 .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<ISignInRepository, SignInRepository>();
            services.AddSingleton<IRobotDetector, RobotDetector>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<IDistributedCacheService, DistributedCacheService>();

            services.AddSingleton<IKickoutCustomerService, KickoutCustomerService>();
            services.AddSingleton<IPHelper, IPHelperImpl>();
            services.AddSingleton<IPService, IpServiceImpl>();
            services.AddHttpClient<INotificationService, NotificationService>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(Configuration.GetValue<string>("TelegramBotUri"));
                httpClient.Timeout = TimeSpan.FromMinutes(5);
            });
            services.AddSingleton<ILimitedCountryAccessService, LimitedCountryAccessService>();
            services.AddSingleton<ISignInService, SignInService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Common/Error");
            }

            app.UseFanexDataWithSqlServer(Configuration, env);
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            HttpContextHelper.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            app.UseViewMode();
            app.UseRequestCulture();
            app.UseLimitedCountryAccess();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Auth}/{action=Index}/{id?}");
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }
}
