namespace Fanex.Alpha.Before.Common.Services
{
    using System;
    using Logging;

    public static class FunctionRecorder
    {
        public static T RunFunctionAndTrackTime<T>(Func<T> func)
        {
            var startTime = DateTime.Now;
            var result = func();

            var endTime = DateTime.Now;
            var rumTime = (endTime - startTime).TotalMilliseconds;

            LogService.Info($"Run time: {rumTime}ms");
            return result;
        }
    }
}
