using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solnet.Mango.Historical.Converters
{
    /// <summary>
    /// Implements a <see cref="JsonConverter{T}"/> from <see cref="string"/> to <see cref="decimal"/>, 
    /// using the <see cref="NumberStyles.Float"/> and <see cref="NumberFormatInfo.InvariantInfo"/> conventions.
    /// </summary>
    public class StringToDecimalJsonConverter : JsonConverter<decimal>
    {
        /// <inheritdoc cref="JsonConverter{T}.Read(ref Utf8JsonReader, Type, JsonSerializerOptions)"/>
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();

            var successful = decimal.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out decimal result);

            if (successful)
            {
                return result;
            }else
            {
                throw new JsonException();
            }
        }

        /// <inheritdoc cref="JsonConverter{T}.Write(Utf8JsonWriter, T, JsonSerializerOptions)"/>
        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
