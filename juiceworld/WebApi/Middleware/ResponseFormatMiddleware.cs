using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace WebApi.Middleware;

public class ResponseFormatMiddleware(RequestDelegate next)
{
    private const string JsonContentType = "application/json";
    private const string XmlContentType = "application/xml";
    private const string DefaultContentType = JsonContentType;

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.ContentType = DefaultContentType;

        var originalResponseBodyStream = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await next(context);

        memoryStream.Seek(0, SeekOrigin.Begin);

        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
        ConvertToFormat(context, ref responseBody);

        memoryStream.Seek(0, SeekOrigin.Begin);
        await originalResponseBodyStream.WriteAsync(Encoding.UTF8.GetBytes(responseBody));
        context.Response.Body = originalResponseBodyStream;
    }

    private static void ConvertToFormat(HttpContext context, ref string responseBody)
    {
        if (context.Request.Headers.Accept.Contains(XmlContentType))
        {
            context.Response.ContentType = XmlContentType;
            responseBody = ConvertJsonToXml(responseBody);
        }
    }

    private static string ConvertJsonToXml(string json)
    {
        var xml = XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(
            Encoding.UTF8.GetBytes(json), new XmlDictionaryReaderQuotas()));

        return xml.ToString();
    }
}