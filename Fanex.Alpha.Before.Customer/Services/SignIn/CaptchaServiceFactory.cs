using Alpha.SliderCaptcha.Mvc;
using Fanex.Alpha.Before.Infrastructure.Extensions;

namespace Fanex.Alpha.Before.Customer.Services.SignIn
{
    public class CaptchaServiceFactory : ServiceFactory
    {
        public override Caching.ICacheService CacheService => CacheExtension.CacheService;
    }
}
