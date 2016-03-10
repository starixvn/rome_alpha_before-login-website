namespace Fanex.Alpha.Before.Infrastructure.Extensions
{
    using System;
    using System.Configuration;
    using System.Linq;

    internal static class LogExtensionFilter
    {
        private static readonly string[] SensitiveParameters;
        private static readonly string[] InfoLogFilterMessages;
        private static string sensitiveValue = "******";

        static LogExtensionFilter()
        {
            var splitChars = new char[] { ',', ';' };

            string sensitiveString = ConfigurationManager.AppSettings["SensitiveParameters"];
            if (string.IsNullOrWhiteSpace(sensitiveString))
            {
                sensitiveString = "password,seccode,securitycode";
            }

            SensitiveParameters = sensitiveString.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);

            string infoMessages = ConfigurationManager.AppSettings["InfoLogFilterMessages"];
            InfoLogFilterMessages = string.IsNullOrWhiteSpace(infoMessages)
                ? new string[0] :
                infoMessages.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
        }

        internal static bool IsInfoLog(string message)
        {
            return InfoLogFilterMessages.Any(info => message.IndexOf(info, StringComparison.OrdinalIgnoreCase) > -1);
        }

        internal static bool IsWarnLog(string source)
        {
            return source.StartsWith("App_Web_", StringComparison.OrdinalIgnoreCase);
        }

        internal static string CensorSensitiveInformation(string message)
        {
            if (string.IsNullOrEmpty(message) || SensitiveParameters.Length == 0)
            {
                return message;
            }

            string splitChar = "&";
            string[] paramsArray = message.Split(new[] { splitChar }, StringSplitOptions.RemoveEmptyEntries);

            int length;
            if ((length = paramsArray.Length) == 0)
            {
                return message;
            }

            for (int i = 0; i < length; i++)
            {
                var parameter = paramsArray[i].ToLower();
                if (SensitiveParameters.Any(sensitive => parameter.Contains(sensitive)))
                {
                    var indexOf = parameter.IndexOf('=');
                    if (indexOf >= 0)
                    {
                        paramsArray[i] = string.Format("{0}{1}", paramsArray[i].Substring(0, indexOf + 1), sensitiveValue);
                    }
                }
            }

            return string.Join(splitChar, paramsArray);
        }
    }
}