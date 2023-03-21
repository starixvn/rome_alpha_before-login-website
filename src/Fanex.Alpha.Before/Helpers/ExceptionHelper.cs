using System.Data.SqlClient;
using System.Text;
using Fanex.Data;
using Microsoft.AspNetCore.Diagnostics;

namespace Fanex.Alpha.Before.Helpers;

public static class ExceptionHelper
{
    private const string BreakLine = "\r\n";
    private const string TwoBreakLine = "\r\n\r\n";

    public static async Task<string> BuildError(HttpRequest request, IExceptionHandlerFeature exceptionHandlerFeature)
    {
        var exception = exceptionHandlerFeature.Error;
        var errorBuilder = new StringBuilder();
        errorBuilder
            .Append(exception.Message).Append(TwoBreakLine)
            .Append("PATH: ")
            .Append($"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}")
            .Append(TwoBreakLine)
            .Append($"REQUESTED URL: {exceptionHandlerFeature.Path}")
            .Append(TwoBreakLine)
            .Append("BODY: ")
            .Append(await GetRawBodyStringAsync(request))
            .Append(TwoBreakLine)
            .Append("HEADERS:").Append(BreakLine)
            .Append(GetHeaderValues(request))
            .Append(TwoBreakLine)
            .Append("COOKIES:").Append(BreakLine)
            .Append(GetCookiesValues(request))
            .Append(TwoBreakLine);

        if (exception is ObjectDbException objectDbException
            && exception.InnerException is SqlException sqlException)
        {
            errorBuilder.Append(BuildDbException(objectDbException, sqlException));
        }

        errorBuilder
            .Append("EXCEPTION: ")
            .Append(exception.ToString());

        return errorBuilder.ToString();
    }

    private static async Task<string> GetRawBodyStringAsync(HttpRequest request)
    {
        request.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(request.Body, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }

    private static string GetHeaderValues(HttpRequest request)
        => string.Join(BreakLine, request.Headers.Select(header => $"{header.Key}:{header.Value}"));

    private static string GetCookiesValues(HttpRequest request)
        => string.Join(BreakLine, request.Cookies.Select(header => $"{header.Key}:{header.Value}"));

    private static string BuildDbException(ObjectDbException dbException, SqlException sqlException)
    {
        var errorBuilder = new StringBuilder();
        errorBuilder
            .Append("DATABASE INFO:").Append(BreakLine)
            .Append("Server: ").Append(sqlException.Server).Append(BreakLine)
            .Append("DbName: ").Append(dbException.DbName).Append(BreakLine)
            .Append("SpName: ").Append(dbException.SpName).Append(BreakLine)
            .Append("Parameters: ").Append(BreakLine).Append(GetSpParameters(dbException.Parameters)).Append(BreakLine)
            .Append("Line: ").Append(sqlException.LineNumber).Append(TwoBreakLine);

        return errorBuilder.ToString();
    }

    private static string GetSpParameters(IDictionary<string, object> parameters)
        => string.Join(BreakLine, parameters.Select(p => $"{p.Key}={p.Value}"));

}