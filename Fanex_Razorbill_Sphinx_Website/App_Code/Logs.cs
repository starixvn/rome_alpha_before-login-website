// --------------------------------------------------------------------------
// <copyright file="Logs.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Jeff.Nguyen" date="2012.06.04">Improve performance, check StypeCop</change>
// </history>

using System.Diagnostics;
using System.Reflection;
using System.Web;

/// <summary>
/// TODO: Log exception.
/// </summary>
public static class Logs
{
    private static bool enableLogginService;

    static Logs()
    {
        enableLogginService = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["LoggingServiceUrl"]);
    }

    public static void Write(string msg)
    {
        Write(msg, ILogger.LoggingType.Error);
    }

    public static void Write(object msg)
    {
        Write(msg, ILogger.LoggingType.Error);
    }

    /// <summary>
    /// Write log function.
    /// </summary>
    /// <param name="msg">Error message.</param>
    /// <param name="type">Logging type.</param>
    public static void Write(string msg, ILogger.LoggingType type)
    {
        if (!enableLogginService)
        {
            return;
        }

        StackTrace f = new StackTrace();
        StackFrame s = f.GetFrame(1);
        MethodBase m = s.GetMethod();
        string eventLog = string.Empty;
        try
        {
            if (HttpContext.Current != null)
            {
                eventLog = string.Format(
                    "SERVER: {0}[{1}] -- [{2}] --",
                    HttpContext.Current.Server.MachineName,
                    HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath,
                    s.GetMethod());
            }
        }
        catch
        {
            eventLog = "[could not get file/function name] -- ";
        }

        ILogger.Service.WriteLog(eventLog + "\r\n" + msg, type);
    }

    /// <summary>
    /// Write log function.
    /// </summary>
    /// <param name="msg">Error message.</param>
    /// <param name="type">Logging type.</param>
    public static void Write(object msg, ILogger.LoggingType type)
    {
        if (!enableLogginService)
        {
            return;
        }

        StackTrace f = new StackTrace();
        StackFrame s = f.GetFrame(1);
        string eventLog = string.Empty;
        try
        {
            if (HttpContext.Current != null)
            {
                eventLog = string.Format(
                    "SERVER: {0}[{1}] -- [{2}] --",
                    HttpContext.Current.Server.MachineName,
                    HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath,
                    s.GetMethod());
            }
        }
        catch
        {
            eventLog = "[could not get file/function name] -- ";
        }

        ILogger.Service.WriteLog(eventLog + "\r\n" + msg.ToString(), type);
    }
}
