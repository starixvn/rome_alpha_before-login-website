using System.Collections;
using Fanex.Logging;

namespace Fanex.Alpha.Before.Services
{
    public static class FunctionRecorder
    {
        public static T RunFunctionAndTrackTime<T>(Func<T> func, string functionName)
        {
            var startTime = DateTime.Now;
            var result = func();

            var endTime = DateTime.Now;
            var runTime = (endTime - startTime).TotalMilliseconds;

            var message = $"{functionName} - Run time: {runTime}ms";
            var isEnumerable = typeof(IEnumerable).IsAssignableFrom(typeof(T));
            if (isEnumerable)
            {
                message += $" - Item Count: {(result as IEnumerable<object>).Count()}";
            }

            Logger.Log.Info(message);
            return result;
        }
    }
}
