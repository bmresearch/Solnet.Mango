using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solnet.Mango.Historical.Converters
{
    /// <summary>
    /// Implements a <see cref="JsonConverter{T}"/> from <see cref="string[]"/> to <see cref="decimal[]"/>, 
    /// using the <see cref="NumberStyles.Float"/> and <see cref="NumberFormatInfo.InvariantInfo"/> conventions.
    /// </summary>
    public class StringArrayToDecimalArrayJsonConverter : JsonConverter<decimal[]>
    {
        /// <inheritdoc cref="Read(ref Utf8JsonReader, Type, JsonSerializerOptions)"/>
        public override decimal[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType == JsonTokenType.StartArray)
            {
                var successfulStartArrayTokenRead = reader.Read();

                if (!successfulStartArrayTokenRead)
                    throw new JsonException();

                List<decimal> list = new ();

                while (reader.TokenType != JsonTokenType.EndArray)
                {
                    var s = reader.GetString();
                    var successful = decimal.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out decimal result);

                    if (successful)
                    {
                        list.Add(result);
                        var successfulTokenRead = reader.Read();
                        if (!successfulTokenRead)
                            throw new JsonException();
                        continue;
                    }

                    throw new JsonException();
                }
                return list.ToArray();
            }

            throw new JsonException();
        }

        /// <inheritdoc cref="Write(Utf8JsonWriter, decimal[], JsonSerializerOptions)"/>
        public override void Write(Utf8JsonWriter writer, decimal[] value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
