using Org.BouncyCastle.Asn1.Crmf;
using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using System;
using System.Collections.Generic;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class UserActiveAssets
    {
        /// <summary>
        /// 
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the <see cref="UserActiveAssets"/> structure.
            /// </summary>
            internal const int Length = 30;
            
            /// <summary>
            /// 
            /// </summary>
            internal const int SpotOffset = 0;
            
            /// <summary>
            /// 
            /// </summary>
            internal const int PerpetualsOffset = 15;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public List<bool> Spot;

        /// <summary>
        /// 
        /// </summary>
        public List<bool> Perpetuals;

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="MangoGroup"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="MangoGroup"/> structure.</returns>
        public static UserActiveAssets Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length) throw new ArgumentException("data length is invalid");

            List<bool> spot = new(Constants.MaxPairs);
            ReadOnlySpan<byte> spotBytes = data.Slice(Layout.SpotOffset, Constants.MaxPairs);

            for (int i = 0; i < Constants.MaxPairs - 1; i++)
            {
                spot.Add(spotBytes.GetU8(i) == 1);
            }
            
            List<bool> perpetuals = new(Constants.MaxPairs);
            ReadOnlySpan<byte> perpetualsBytes = data.Slice(Layout.PerpetualsOffset, Constants.MaxPairs);
            
            for (int i = 0; i < Constants.MaxPairs - 1; i++)
            {
                perpetuals.Add(perpetualsBytes.GetU8(i) == 1);
            }
            
            return new UserActiveAssets
            {
                Spot = spot,
                Perpetuals = perpetuals,
            };
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class HealthCache
    {
        /// <summary>
        /// 
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the <see cref="UserActiveAssets"/> structure.
            /// </summary>
            internal const int Length = 30;

            /// <summary>
            /// 
            /// </summary>
            internal const int UserActiveAssetsOffset = 0;

            /// <summary>
            /// 
            /// </summary>
            internal const int SpotOffset = 30;
        }

        /// <summary>
        /// 
        /// </summary>
        public UserActiveAssets ActiveAssets;

        /// <summary>
        /// 
        /// </summary>
        public List<Tuple<I80F48, I80F48>> Spot;
        
        /// <summary>
        /// 
        /// </summary>
        public List<Tuple<I80F48, I80F48>> Perpetual;

        /// <summary>
        /// 
        /// </summary>
        public I80F48 Quote;
        
        /// <summary>
        /// 
        /// </summary>
        public List<I80F48> Health;
        
        /// <summary>
        /// Deserialize a span of bytes into a <see cref="HealthCache"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="HealthCache"/> structure.</returns>
        public static HealthCache Deserialize(ReadOnlySpan<byte> data)
        {
            List<Tuple<I80F48, I80F48>> spot = new ();
            uint spotSize = data.GetU32(Layout.SpotOffset);
            data = data[..sizeof(uint)];
            ReadOnlySpan<byte> spotBytes = data[..(I80F48.Length * 2 * (int)spotSize)];
            for (int i = 0; i < spotSize; i++)
            {
                ReadOnlySpan<byte> bytes = spotBytes.Slice(i * I80F48.Length * 2, I80F48.Length * 2);
                I80F48 first = I80F48.Deserialize(bytes[..I80F48.Length]);
                I80F48 second = I80F48.Deserialize(bytes[^I80F48.Length..]);
                spot.Add(new Tuple<I80F48, I80F48>(first, second));
            }

            data = data[spotBytes.Length..];
            List<Tuple<I80F48, I80F48>> perp = new ();
            uint perpSize = data.GetU32(0);
            data = data[..sizeof(uint)];
            ReadOnlySpan<byte> perpBytes = data[(I80F48.Length * 2 * (int)perpSize)..];
            
            for (int i = 0; i < perpSize; i++)
            {
                ReadOnlySpan<byte> bytes = perpBytes.Slice(i * I80F48.Length * 2, I80F48.Length * 2);
                I80F48 first = I80F48.Deserialize(bytes[..I80F48.Length]);
                I80F48 second = I80F48.Deserialize(bytes[^I80F48.Length..]);
                perp.Add(new Tuple<I80F48, I80F48>(first, second));
            }
            data = data[perpBytes.Length..];
            I80F48 quote = I80F48.Deserialize(data[I80F48.Length..]);

            List<I80F48> health = new();
            for (int i = 0; i < 2; i++)
            {
                health.Add(I80F48.Deserialize(data.Slice(i * I80F48.Length, I80F48.Length)));
            }
            
            return new HealthCache
            {
                ActiveAssets = UserActiveAssets.Deserialize(data.Slice(Layout.UserActiveAssetsOffset, UserActiveAssets.Layout.Length)),
                Spot = spot,
                Perpetual = perp,
                Quote = quote,
                Health = health
            };
        }
    }
}