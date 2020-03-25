using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SerializeTest
{

    public class NodeSerializer<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) 
        {
            throw new NotSupportedException($"Deserializing not supported. Type={typeToConvert}.");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }

    }
}