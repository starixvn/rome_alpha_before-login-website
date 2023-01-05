// This Startup file is based on ASP.NET Core new project templates and is included
// as a starting point for DI registration and HTTP request processing pipeline configuration.
// This file will need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core startup files, see https://docs.microsoft.com/aspnet/core/fundamentals/startup

using Alpha.SliderCaptcha.Core;
using Fanex.Alpha.Before.Configurations;
using Fanex.Alpha.Before.Helpers;
using Fanex.Alpha.Before.Repositories;
using Fanex.Alpha.Before.Services;
using Fanex.Alpha.Before.Services.FPS;
using Fanex.Alpha.Before.Services.SignIn;
using Fanex.Alpha.Before.Web.Extensions;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using TelegramBot.Client.Services;

namespace Fanex.Alpha.Before.Web
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
            services.AddSession();

            services.AddONEbookUM(Configuration);
            services.AddFanexLogging(Configuration);
            services.AddRedisCache(Configuration.GetSection("fanex.caching.redis"));

            var commonOptions = new CommonOptions();
            Configuration.GetSection(CommonOptions.Common).Bind(commonOptions);
            services.AddSingleton(commonOptions);

            var automationTestOptions = new AutomationTestOptions();
            Configuration.GetSection(AutomationTestOptions.AutomationTest).Bind(automationTestOptions);
            services.AddSingleton(automationTestOptions);


            var captchaOptions = new CaptchaOptions(
                Configuration.GetValue<bool>("SliderCaptcha:LogEnabled"),
                Configuration.GetValue<string>("SliderCaptcha:ImagePath"),
                Configuration.GetValue<int>("SliderCaptcha:AcceptRange"),
                Configuration.GetValue<int>("SliderCaptcha:DragTimeout"),
                Configuration.GetValue<int>("SliderCaptcha:VerifyTimeout")
            );
            services.AddSingleton(captchaOptions);

            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<ISignInRepository, SignInRepository>();
            services.AddSingleton<IRobotDetector, RobotDetector>();
            services.AddSingleton<ICacheService, CacheService>();
            // services.AddSingleton<ITokenService, TokenService>();

            // services.AddSingleton<IHttpContextService, HttpContextService>();
            // services.AddSingleton<IRandomService, RandomService>();
            // services.AddSingleton<IFileProvider, FileProvider>();
            // services.AddSingleton<ICaptchaService, CaptchaService>();
            services.AddSingleton<IKickoutCustomerService, KickoutCustomerService>();
            services.AddSingleton<IPHelper, IPHelperImpl>();
            services.AddSingleton<IPService, IpServiceImpl>();
            services.AddHttpClient<INotificationService, NotificationService>();
            services.AddSingleton<ILimitedCountryAccessService, LimitedCountryAccessService>();
            services.AddSingleton<ISignInService, SignInService>();

            services.AddControllersWithViews();
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

            app.UseTheme();
            app.UseRequestCulture();
            app.UseLimitedCountryAccess();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Auth}/{action=Index}/{id?}");
            });
        }
    }
}
