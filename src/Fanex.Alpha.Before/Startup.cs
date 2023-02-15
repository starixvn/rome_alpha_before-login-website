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
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TelegramBot.Client.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;
using Fanex.Caching.Redis;

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
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddDataProtection()
               .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(Configuration.GetValue<string>("RedisConfiguration")), "alpha-before-login");

            services.AddFanexLogging(Configuration)
                 .AddONEbookUM(Configuration)
                 .AddRedisCache(Configuration.GetSection("Fanex.Caching.Redis"))
                 .AddCustomOptions(Configuration)
                 .AddNetThrottling(Configuration)
                 .AddSliderCaptcha(Configuration)
                 .AddCustomHealthChecks()
                 .AddControllersWithViews()
                 .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddAssetsBundle(Configuration);

            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<ISignInRepository, SignInRepository>();
            services.AddSingleton<IRobotDetector, RobotDetector>();
            services.AddSingleton<IDistributedCacheService, DistributedCacheService>();

            services.AddSingleton<IKickoutCustomerService, KickoutCustomerService>();
            services.AddSingleton<IPHelper, IPHelperImpl>();
            services.AddSingleton<IFPSHelper, FPSHelper>();
            services.AddSingleton<IEncryptUrlHelper, EncryptUrlHelper>();
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

            app.UseForwardedHeaders();

            app.UseFanexDataWithSqlServer(Configuration, env);
            app.UseWebOptimizer();
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
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                }).AllowAnonymous();
                endpoints.MapControllerRoute("default", "{controller=Auth}/{action=Index}/{id?}");
            });
        }
    }
}
