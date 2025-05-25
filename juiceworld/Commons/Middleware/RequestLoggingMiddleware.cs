using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Commons.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, string source)
{
    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var traceIdentifier = context.TraceIdentifier;

        // Log incoming request
        logger.LogInformation(
            "INCOMING_REQUEST - SOURCE: {Source}, TRACE_ID: {TraceId}, METHOD: {Method}, URL: {Url}, IP: {IP}",
            source,
            traceIdentifier,
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress);

        await next(context);

        stopwatch.Stop();

        // Log outgoing response
        logger.LogInformation(
            "OUTGOING_RESPONSE - SOURCE: {Source}, TRACE_ID: {TraceId}, METHOD: {Method}, URL: {Url}, IP: {IP}, STATUS: {Status}, DOWNTIME: {Downtime}ms",
            source,
            traceIdentifier,
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
}