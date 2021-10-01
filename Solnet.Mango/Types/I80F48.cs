using Solnet.Programs.Utilities;
using System;
using System.Globalization;
using System.Numerics;

namespace Solnet.Mango.Types
{
    /// <summary>
    /// Represents the fixed pointer type I80F48 from rust lang.
    /// </summary>
    public class I80F48
    {
        /// <summary>
        /// The length of the data type in bytes.
        /// </summary>
        public const int Length = 16;
        
        /// <summary>
        /// 
        /// </summary>
        public double Value;
        
        /// <summary>
        /// The original raw data.
        /// </summary>
        public byte[] Data;
        
        /// <summary>
        /// 
        /// </summary>
        private static readonly BigInteger Divisor = BigInteger.Pow(2, 48);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static I80F48 Deserialize(ReadOnlySpan<byte> data)
        {
            return new I80F48
            {
                Value = (double) data.GetBigInt(0, Length, true) / (double)Divisor,
                Data = data.ToArray()
            };
        }
    }
}