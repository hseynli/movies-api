using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Text;

namespace API.ResponseWriters;

public class HealthResponseWriter
{
    public Task WriteResponse(HttpContext context, HealthReport result)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        var options = new JsonWriterOptions
        {
            Indented = true
        };
        using (var stream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(stream, options))
            {
                var healthCheck = result.Entries.FirstOrDefault().Value;
                writer.WriteStartObject();
                writer.WriteString("Status", healthCheck.Status.ToString());
                writer.WriteString("Description", healthCheck.Description);
                writer.WriteEndObject();
            }
            var json = Encoding.UTF8.GetString(stream.ToArray());
            return context.Response.WriteAsync(json);
        }
    }
}