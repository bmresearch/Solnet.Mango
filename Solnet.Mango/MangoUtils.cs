using Solnet.Mango.Models;
using Solnet.Mango.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango
{
    /// <summary>
    /// Implements extension methods and utilities to easily operate with Mango.
    /// </summary>
    public static class MangoUtils
    {

        /// <summary>
        /// Humanizes a native <see cref="I80F48"/> value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="decimals">The number of decimals for the asset.</param>
        /// <returns>The humanized value.</returns>
        public static double HumanizeNative(double value, byte decimals)
        {
            return value / Math.Pow(10, decimals);
        }

        /// <summary>
        /// Gets the weights to calculate account health ratios.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="tokenIndex">The index of the market.</param>
        /// <param name="healthType">The health type.</param>
        /// <returns>The weights structure.</returns>
        public static Weights GetWeights(MangoGroup mangoGroup, int tokenIndex, HealthType healthType)
        {
            return healthType switch
            {
                HealthType.Maintenance => new Weights
                {
                    SpotAssetWeight = mangoGroup.SpotMarkets[tokenIndex].MaintenanceAssetWeight.Value,
                    SpotLiabilityWeight = mangoGroup.SpotMarkets[tokenIndex].MaintenanceLiabilityWeight.Value,
                    PerpAssetWeight = mangoGroup.PerpetualMarkets[tokenIndex].MaintenanceLiabilityWeight.Value,
                    PerpLiabilityWeight = mangoGroup.PerpetualMarkets[tokenIndex].MaintenanceLiabilityWeight.Value,
                },
                HealthType.Initialization => new Weights
                {
                    SpotAssetWeight = mangoGroup.SpotMarkets[tokenIndex].InitializationAssetWeight.Value,
                    SpotLiabilityWeight = mangoGroup.SpotMarkets[tokenIndex].InitializationLiabilityWeight.Value,
                    PerpAssetWeight = mangoGroup.PerpetualMarkets[tokenIndex].InitializationAssetWeight.Value,
                    PerpLiabilityWeight = mangoGroup.PerpetualMarkets[tokenIndex].InitializationLiabilityWeight.Value,
                },
                _ => new Weights
                {
                    SpotAssetWeight = 1,
                    SpotLiabilityWeight = 1,
                    PerpAssetWeight = 1,
                    PerpLiabilityWeight = 1,
                }
            };
        }

    }
}
