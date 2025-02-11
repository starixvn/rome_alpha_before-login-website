using Fanex.Alpha.Before.Constants;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.V8;

namespace Fanex.Alpha.Before.Extensions;

public static class WebOptimizerExtensions
{
    public static IServiceCollection AddAssetsBundle(this IServiceCollection services, IConfiguration configuration)
    {
        var siteGroup = configuration.GetValue<string>("SiteGroup");
        var cssSuffix = siteGroup switch
        {
            SiteGroupConstants.Site789y => "2",
            SiteGroupConstants.IBCIndo => "3",
            _ => string.Empty
        };

        services.AddJsEngineSwitcher(options => options.DefaultEngineName = V8JsEngine.EngineName).AddV8();


        services.AddWebOptimizer(pipeline =>
        {
            pipeline.CompileScssFiles();

            pipeline.AddScssBundle("/assets/bundles/bootstrap-ex.min.css", "assets/styles/vendors-extensions/login/bootstrap-ex.scss");
            pipeline.AddScssBundle("/assets/bundles/themes/default.min.css", $"assets/styles/themes/default{cssSuffix}.scss");
            pipeline.AddScssBundle("/assets/bundles/themes/default-dark.min.css", $"assets/styles/themes/default{cssSuffix}-dark.scss");
            pipeline.AddScssBundle("/assets/bundles/themes/event.min.css", $"assets/styles/themes/default{cssSuffix}.scss", $"assets/styles/themes/event{cssSuffix}.scss");
            pipeline.AddScssBundle("/assets/bundles/themes/um.min.css", "assets/styles/pages/um.scss");
            pipeline.AddScssBundle("/assets/bundles/themes/um-dark.min.css", "assets/styles/pages/um-dark.scss");
            pipeline.AddScssBundle("/assets/bundles/themes/um2.min.css", "assets/scripts/libs/flipclock/flipclock.css", "assets/styles/pages/um2.scss");
            pipeline.AddScssBundle("/assets/bundles/themes/um3.min.css", "assets/styles/pages/um3.scss");
            pipeline.AddScssBundle("/assets/bundles/themes/um3-dark.min.css", "assets/styles/pages/um3-dark.scss");
            pipeline.AddScssBundle("/assets/bundles/robotcaptcha.min.css", "assets/styles/pages/robotcaptcha.scss");
            pipeline.AddScssBundle("/assets/bundles/robotcaptcha-dark.min.css", "assets/styles/pages/robotcaptcha-dark.scss");
            pipeline.AddScssBundle("/assets/bundles/themes/error.min.css", "assets/styles/pages/error.scss");
            pipeline.AddScssBundle("/assets/bundles/themes/error-dark.min.css", "assets/styles/pages/error-dark.scss");
            pipeline.AddScssBundle("/assets/bundles/themes/limitedcountryaccess.min.css", "assets/styles/pages/limitedcountryaccess.scss");
            pipeline.AddScssBundle("/assets/bundles/themes/limitedcountryaccess-dark.min.css", "assets/styles/pages/limitedcountryaccess-dark.scss");

            pipeline.AddJavaScriptBundle("/assets/bundles/login.min.js",
                "assets/scripts/libs/jquery/jquery-1.11.3.js",
                "assets/scripts/libs/bootstrap/dropdown.js",
                "assets/scripts/libs/storage/store-with-json2.min.js",
                "assets/scripts/libs/detecas/Detecas.Base64.js",
                "assets/scripts/libs/detecas/Detecas.JSON.js",
                "assets/scripts/libs/cryptojs/aes.js",
                "assets/scripts/login/core.js",
                "assets/scripts/login/place-holder.js",
                "assets/scripts/login/resources.js",
                "assets/scripts/login/assetAppPaths.js",
                "assets/scripts/login/login.js");
            pipeline.AddJavaScriptBundle("/assets/bundles/um2.min.js",
             "assets/scripts/libs/jquery/jquery-1.11.3.js",
             "assets/scripts/libs/flipclock/flipclock.js",
             "assets/scripts/um/um.js");

            pipeline.AddCssBundle("/assets/bundles/alpha.slider-captcha.css", "assets/styles/alpha.slider-captcha.css");
            pipeline.AddJavaScriptBundle("/assets/bundles/alpha.slider-captcha.js", "assets/scripts/alpha.slider-captcha.js");
        });

        return services;
    }
}