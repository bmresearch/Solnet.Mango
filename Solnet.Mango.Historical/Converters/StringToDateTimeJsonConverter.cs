using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solnet.Mango.Historical.Converters
{
    /// <summary>
    /// Implements a <see cref="JsonConverter{T}"/> from <see cref="string"/> to <see cref="DateTime"/>.
    /// </summary>
    public class StringToDateTimeJsonConverter : JsonConverter<DateTime>
    {
        /// <inheritdoc cref="Read(ref Utf8JsonReader, Type, JsonSerializerOptions)"/>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();

            var success = DateTime.TryParse(s, out var date);
            if (success)
            {
                return date;
            }

            return DateTime.UnixEpoch;
        }

        /// <inheritdoc cref="Write(Utf8JsonWriter, DateTime, JsonSerializerOptions)"/>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
