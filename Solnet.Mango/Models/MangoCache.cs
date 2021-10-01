using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using System;
using System.Collections.Generic;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class MangoCache
    {
        /// <summary>
        /// 
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// 
            /// </summary>
            internal const int Length = 1608;
            
            /// <summary>
            /// 
            /// </summary>
            internal const int MetadataOffset = 0;
            
            /// <summary>
            /// 
            /// </summary>
            internal const int PriceCachesOffset = 8;

            /// <summary>
            /// 
            /// </summary>
            internal const int RootBankCachesOffset = 368;
            
            /// <summary>
            /// 
            /// </summary>
            internal const int PerpMarketCachesOffset = 1040;
        }

        /// <summary>
        /// The account's metadata.
        /// </summary>
        public MetaData Metadata;

        /// <summary>
        /// 
        /// </summary>
        public List<PriceCache> PriceCaches;

        /// <summary>
        /// 
        /// </summary>
        public List<RootBankCache> RootBankCaches;

        /// <summary>
        /// 
        /// </summary>
        public List<PerpMarketCache> PerpetualMarketCaches;
        
        /// <summary>
        /// Deserialize a span of bytes into a <see cref="RootBankCache"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="RootBankCache"/> structure.</returns>
        public static MangoCache Deserialize(byte[] data)
        {
            if (data.Length != Layout.Length) throw new ArgumentException("data length is invalid");
            ReadOnlySpan<byte> span = data.AsSpan();
            
            List<PriceCache> priceCaches = new(Constants.MaxPairs);
            ReadOnlySpan<byte> priceCachesBytes = span.Slice(Layout.PriceCachesOffset, PriceCache.Layout.Length * Constants.MaxPairs);

            for (int i = 0; i < Constants.MaxPairs - 1; i++)
            {
                PriceCache priceCache = PriceCache.Deserialize(priceCachesBytes.Slice(i * PriceCache.Layout.Length, PriceCache.Layout.Length));
                priceCaches.Add(priceCache);
            }
            
            List<RootBankCache> rootBankCaches = new(Constants.MaxTokens);
            ReadOnlySpan<byte> rootBankCachesBytes = span.Slice(Layout.RootBankCachesOffset, RootBankCache.Layout.Length * Constants.MaxPairs);
            
            for (int i = 0; i < Constants.MaxPairs - 1; i++)
            {
                RootBankCache rootBankCache = RootBankCache.Deserialize(rootBankCachesBytes.Slice(i * RootBankCache.Layout.Length, RootBankCache.Layout.Length));
                rootBankCaches.Add(rootBankCache);
            }
            
            List<PerpMarketCache> perpMarketCaches = new(Constants.MaxPairs);
            ReadOnlySpan<byte> perpMarketCachesBytes = span.Slice(Layout.RootBankCachesOffset, PerpMarketCache.Layout.Length * Constants.MaxPairs);
            
            for (int i = 0; i < Constants.MaxPairs - 1; i++)
            {
                PerpMarketCache perpMarketCache = PerpMarketCache.Deserialize(perpMarketCachesBytes.Slice(i * PerpMarketCache.Layout.Length, PerpMarketCache.Layout.Length));
                perpMarketCaches.Add(perpMarketCache);
            }
            
            return new MangoCache
            {
                Metadata = MetaData.Deserialize(span.GetSpan(Layout.MetadataOffset, MetaData.Layout.Length)),
                PriceCaches = priceCaches,
                RootBankCaches = rootBankCaches,
                PerpetualMarketCaches = perpMarketCaches
            };
        }
    }
}