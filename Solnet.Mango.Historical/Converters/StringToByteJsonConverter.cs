using System;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solnet.Mango.Historical.Converters
{
    /// <summary>
    /// Implements a <see cref="JsonConverter{T}"/> from <see cref="string"/> to <see cref="byte"/>, 
    /// using the <see cref="NumberStyles.Integer"/> and <see cref="NumberFormatInfo.InvariantInfo"/> conventions.
    /// </summary>
    public class StringToByteJsonConverter : JsonConverter<byte>
    {
        /// <inheritdoc cref="JsonConverter{T}.Read(ref Utf8JsonReader, Type, JsonSerializerOptions)"/>
        public override byte Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();

            var successful = byte.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var result);

            if (successful)
            {
                return result;
            }
            else
            {
                throw new JsonException();
            }
        }

        /// <inheritdoc cref="JsonConverter{T}.Write(Utf8JsonWriter, T, JsonSerializerOptions)"/>
        public override void Write(Utf8JsonWriter writer, byte value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
