using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApplication1.Models;

public class JsonFloatConverter : JsonConverter<float>
{
    public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Handle both string and number types in JSON
        if (reader.TokenType == JsonTokenType.String && float.TryParse(reader.GetString(), out var result))
        {
            return result;
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetSingle();
        }

        throw new JsonException("Invalid JSON token for float value.");
    }

    public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
    {
        // Write the float value as a number
        writer.WriteNumberValue(value);
    }
}