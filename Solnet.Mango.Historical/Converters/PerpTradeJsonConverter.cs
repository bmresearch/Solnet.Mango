using Solnet.Mango.Historical.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solnet.Mango.Historical.Converters
{
    public class PerpTradeJsonConverter : JsonConverter<PerpTrade>
    {
        private static readonly StringToBigIntegerJsonConverter stringToBigInt = new();
        private static readonly StringToUlongJsonConverter stringToUlong = new();
        private static readonly StringToDecimalJsonConverter stringToDecimal = new();
        private static readonly StringToDateTimeJsonConverter stringToDateTime = new();
        public override PerpTrade Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType == JsonTokenType.StartObject)
            {
                reader.Read();

                _ = reader.GetString();
                reader.Read();
                var ts = stringToDateTime.Read(ref reader, typeof(DateTime), options);
                reader.Read();

                _ = reader.GetString();
                reader.Read();
                var address = reader.GetString();
                reader.Read();

                _ = reader.GetString();
                reader.Read();
                var seqNum = stringToUlong.Read(ref reader, typeof(ulong), options);
                reader.Read();

                _ = reader.GetString();
                reader.Read();
                var makerFee = stringToDecimal.Read(ref reader, typeof(decimal), options);
                reader.Read();

                _ = reader.GetString();
                reader.Read();
                var takerFee = stringToDecimal.Read(ref reader, typeof(decimal), options);
                reader.Read();

                _ = reader.GetString();
                reader.Read();
                var takerSide = reader.GetString();
                reader.Read();

                _ = reader.GetString();
                reader.Read();
                var maker = reader.GetString();
                reader.Read();
                
                _ = reader.GetString();
                reader.Read();
                var makerOrderId = stringToBigInt.Read(ref reader, typeof(BigInteger), options);
                reader.Read();

                _ = reader.GetString();
                reader.Read();
                var taker = reader.GetString();
                reader.Read();

                _ = reader.GetString();
                reader.Read();
                var takerOrderId = stringToBigInt.Read(ref reader, typeof(BigInteger), options);
                reader.Read();

                _ = reader.GetString();
                reader.Read();
                var price = stringToDecimal.Read(ref reader, typeof(decimal), options);
                reader.Read();

                _ = reader.GetString();
                reader.Read();
                var quantity = stringToDecimal.Read(ref reader, typeof(decimal), options);
                reader.Read();

                _ = reader.GetString();
                reader.Read();
                var makerClientOrderId = stringToUlong.Read(ref reader, typeof(ulong), options);
                reader.Read();

                _ = reader.GetString();
                reader.Read();
                var takerClientOrderId = stringToUlong.Read(ref reader, typeof(ulong), options);
                reader.Read();

                return new PerpTrade
                {
                    Address = address,
                    Timestamp = ts,
                    SequenceNumber = seqNum,
                    MakerFee = makerFee,
                    TakerFee = takerFee,
                    TakerSide = takerSide,
                    Maker = maker,
                    MakerOrderId = makerOrderId,
                    Taker = taker,
                    TakerOrerId = takerOrderId,
                    Price = price,
                    Quantity = quantity,
                    MakerClientOrderId = makerClientOrderId,
                    TakerClientOrderId = takerClientOrderId
                };
            } else if (reader.TokenType == JsonTokenType.StartArray)
            {
                reader.Read();

                return null;
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, PerpTrade value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
