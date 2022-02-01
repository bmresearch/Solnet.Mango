using Solnet.Programs.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

namespace Solnet.Mango.Types
{
    /// <summary>
    /// Represents the fixed pointer type I80F48 from rust lang.
    /// </summary>
    public class I80F48
    {
        private static int FRACTIONS = 48;
        private static int MAX_SIZE = 128;
        private static BigInteger MULTIPLIER_BI = new BigInteger(281_474_976_710_656);
        private static decimal MULTIPLIER_DECIMAL = 281_474_976_710_656m;
        private static double MULTIPLIER_DOUBLE = 281_474_976_710_656.0;

        public static I80F48 One = new I80F48(1m);
        public static I80F48 NegativeOne = new I80F48(-1m);
        public static I80F48 Zero = new I80F48(0m);
        public static I80F48 OneHundred = new I80F48(100m);
        public static I80F48 MaxValue = new I80F48(BigInteger.Parse("170141183460469231731687303715884105727"));
        public static I80F48 MinValue = new I80F48(BigInteger.Parse("-170141183460469231731687303715884105728"));

        /// <summary>
        /// The length of the data type in bytes.
        /// </summary>
        public const int Length = 16;

        /// <summary>
        /// 
        /// </summary>
        public double Value;

        public decimal Decimal;

        /// <summary>
        /// The original raw data.
        /// </summary>
        private byte[] _data;

        private BigInteger _storage;

        /// <summary>
        /// 
        /// </summary>
        private static readonly BigInteger Divisor = BigInteger.Pow(2, 48);

        /// <summary>
        /// 
        /// </summary>
        public I80F48() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public I80F48(decimal data)
        {
            var intPrt = Math.Truncate(data);
            var val = new BigInteger(intPrt) << FRACTIONS;
            val += new BigInteger((data - intPrt) * MULTIPLIER_DECIMAL);

            _storage = val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public I80F48(BigInteger data)
        {
            if (data.GetByteCount() > Length)
                throw new ArgumentOutOfRangeException("big integer byte count is too large");

            _storage = data;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public decimal ToDecimal()
        {
            var div = BigInteger.DivRem(_storage, MULTIPLIER_BI, out BigInteger rem);

            var res = (decimal)div;
            res += (decimal)rem / MULTIPLIER_DECIMAL;

            return res;
        }

        public bool IsPositive() => _storage > MULTIPLIER_BI;

        public I80F48 Min(I80F48 other)
        {
            return this > other ? other : this; 
        }

        public I80F48 Max(I80F48 other)
        {
            return this < other ? other : this;
        }

        public I80F48 Abs()
        {
            if (this < Zero)
                return this * NegativeOne;
            return this;
        }

        public I80F48 Floor()
        {
            return new I80F48(Math.Round(ToDecimal(), 0));
        }

        public I80F48 Ceil()
        {
            return new I80F48(Math.Round(ToDecimal(), 3));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BigInteger GetData() => _storage;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes() => _data;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static I80F48 Deserialize(ReadOnlySpan<byte> data)
        {
            var bigInt = data.GetBigInt(0, Length, false);
            return new I80F48
            {
                _storage = bigInt,
                _data = data.ToArray(),
                Value = (double) bigInt / (double)Divisor,
            };
        }

        public static I80F48 operator -(I80F48 rhs) => new I80F48(-rhs._storage);
        public static I80F48 operator +(I80F48 rhs) => new I80F48(+rhs._storage);
        public static I80F48 operator +(I80F48 lhs, I80F48 rhs) => new I80F48(lhs._storage + rhs._storage);

        public static I80F48 operator -(I80F48 lhs, I80F48 rhs) => new I80F48(lhs._storage - rhs._storage);

        public static I80F48 operator *(I80F48 lhs, I80F48 rhs) => new I80F48((lhs._storage * rhs._storage) >> FRACTIONS);

        public static I80F48 operator /(I80F48 lhs, I80F48 rhs) => new I80F48((lhs._storage << FRACTIONS) / rhs._storage);

        public static bool operator >(I80F48 lhs, I80F48 rhs) => lhs.ToDecimal() > rhs.ToDecimal() ? true : false;
        public static bool operator <(I80F48 lhs, I80F48 rhs) => rhs.ToDecimal() > lhs.ToDecimal() ? true : false;
    }
}