using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlanningCenter.Api.Client.Models.JsonApi.Core
{
    /// <summary>
    /// Allows numbers and other primitives to be deserialized as strings.
    /// </summary>
    public class FlexibleStringJsonConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                // Try to get as long, then double
                if (reader.TryGetInt64(out long l))
                    return l.ToString();
                if (reader.TryGetDouble(out double d))
                    return d.ToString(System.Globalization.CultureInfo.InvariantCulture);
                // Fallback: read as string (should not usually occur for numbers)
                return reader.GetDouble().ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
            {
                return reader.GetBoolean().ToString();
            }
            else if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            else
            {
                // fallback: try to read as string
                return reader.GetString();
            }
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
