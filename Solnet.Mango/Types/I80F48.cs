using Solnet.Programs.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

namespace Solnet.Mango.Types
{
    /// <summary>
    /// Fixed point decimal that uses 128 bits of storage and the last 48bits are treated as fractional part.
    /// For human representation, consider a 128 bit integer divided by 2^48. 
    /// The smallest, non-negative, representable number is 1 / 2^48.
    /// </summary>
    public struct I80F48
    {
        private static readonly int FRACTIONAL_BITS = 48;
        private static readonly int MAX_SIZE = 128;
        private static readonly int BI_MAX_BIT_SIZE = MAX_SIZE - 1; // doesn't include sign bit

        // 2^48 constants
        private static readonly BigInteger MULTIPLIER_BI = new BigInteger(281_474_976_710_656);
        private static readonly decimal MULTIPLIER_DECIMAL = 281_474_976_710_656m;
        private static readonly double MULTIPLIER_DOUBLE = 281_474_976_710_656.0;

        private static readonly BigInteger MAX_BI = (new BigInteger(1) << 127) - 1; // 2^128 / 2 - 1   <==>  2^127 - 1
        private static readonly BigInteger MIN_BI = new BigInteger(-1) << 127; // 2^128 / 2 * -1       <==> -2^127
        private static readonly BigInteger ZERO_BI = new BigInteger(0);

        private static readonly BigInteger FRAC_BITS = MULTIPLIER_BI - 1; // 2^48 - 1
        private static readonly BigInteger INTEGER_BITS = MAX_BI ^ FRAC_BITS; // (2^128 - 1) xor (2^48 - 1)

        private static readonly BigInteger ONE_HUNDRED_BI = new BigInteger(100) * MULTIPLIER_BI;
        private static readonly BigInteger ONE_NEGATIVE_BI = -MULTIPLIER_BI;

        /// <summary>
        /// The length of the data type in bytes.
        /// </summary>
        public static int Length => 16;

        /// <summary>
        /// The maximum representable number as an <see cref="I80F48"/> type.
        /// </summary>
        public static I80F48 MaxValue => new I80F48(MAX_BI);

        /// <summary>
        /// The minimum representable number as an <see cref="I80F48"/> type.
        /// </summary>
        public static I80F48 MinValue => new I80F48(MIN_BI);

        /// <summary>
        /// The number 0 as an <see cref="I80F48"/> type.
        /// </summary>
        public static I80F48 Zero => new I80F48(ZERO_BI);

        /// <summary>
        /// The number 1 as an <see cref="I80F48"/> type.
        /// </summary>
        public static I80F48 One => new I80F48(MULTIPLIER_BI);

        /// <summary>
        /// The number -1 as an <see cref="I80F48"/> type.
        /// </summary>
        public static I80F48 NegativeOne => new I80F48(ONE_NEGATIVE_BI);

        /// <summary>
        /// The number 100 as an <see cref="I80F48"/> type.
        /// </summary>
        public static I80F48 OneHundred => new I80F48(ONE_HUNDRED_BI);


        private BigInteger _storage;

        /// <summary>
        /// Constructs a I80F48 number from a <see cref="byte[]"/> used as storage. 
        /// The given <see cref="byte[]"/> needs to have been previously converted to the I80F48 format.
        /// </summary>
        /// <param name="data">The number.</param>
        public I80F48(byte[] data)
        {
            if (data.Length != 16) throw new ArgumentOutOfRangeException();
            _storage = new BigInteger(data);
        }

        /// <summary>
        /// Constructs a I80F48 number from a <see cref="BigInteger"/> used as storage. 
        /// The given <see cref="BigInteger"/> needs to have been previously converted to the I80F48 format (multiplied by 2^48).
        /// </summary>
        /// <param name="data">The number.</param>
        public I80F48(BigInteger data)
        {
            if (data.GetBitLength() > BI_MAX_BIT_SIZE) throw new ArgumentOutOfRangeException();
            _storage = data;
        }

        /// <summary>
        /// Constructs a I80F48 number from a <see cref="ulong"/>.
        /// </summary>
        /// <param name="data">The number.</param>
        public I80F48(ulong data)
        {
            _storage = new BigInteger(data) << FRACTIONAL_BITS;
        }

        /// <summary>
        /// Constructs a I80F48 number from a <see cref="long"/>.
        /// </summary>
        /// <param name="data">The number.</param>
        public I80F48(long data)
        {
            _storage = new BigInteger(data) << FRACTIONAL_BITS;
        }

        /// <summary>
        /// Constructs a I80F48 number from a <see cref="int"/>.
        /// </summary>
        /// <param name="data">The number.</param>
        public I80F48(int data)
        {
            _storage = new BigInteger(data) << FRACTIONAL_BITS;
        }

        /// <summary>
        /// Constructs a I80F48 number from a <see cref="uint"/>.
        /// </summary>
        /// <param name="data">The number.</param>
        public I80F48(uint data)
        {
            _storage = new BigInteger(data) << FRACTIONAL_BITS;
        }

        /// <summary>
        /// Constructs a I80F48 number from a <see cref="byte"/>.
        /// </summary>
        /// <param name="data">The number.</param>
        public I80F48(byte data)
        {
            _storage = new BigInteger(data) << FRACTIONAL_BITS;
        }


        /// <summary>
        /// Constructs a I80F48 number from a <see cref="sbyte"/>.
        /// </summary>
        /// <param name="data">The number.</param>
        public I80F48(sbyte data)
        {
            _storage = new BigInteger(data) << FRACTIONAL_BITS;
        }

        /// <summary>
        /// Constructs a I80F48 number from a <see cref="decimal"/>.
        /// </summary>
        /// <param name="data">The number.</param>
        public I80F48(decimal data)
        {
            var intPrt = Math.Truncate(data);
            var val = new BigInteger(intPrt) << FRACTIONAL_BITS;
            val += new BigInteger((data - intPrt) * MULTIPLIER_DECIMAL);

            _storage = val;
        }

        /// <summary>
        /// Constructs a I80F48 number from a double-precision floating-point number.
        /// </summary>
        /// <param name="data">The number.</param>
        public I80F48(double data)
        {
            var intPrt = Math.Truncate(data);
            var val = new BigInteger(intPrt) << FRACTIONAL_BITS;
            val += new BigInteger((data - intPrt) * MULTIPLIER_DOUBLE);

            _storage = val;
        }

        /// <summary>
        /// Constructs a I80F48 number from a floating-point number.
        /// </summary>
        /// <param name="data">The number.</param>
        public I80F48(float data)
        {
            var intPrt = Math.Truncate(data);
            var val = new BigInteger(intPrt) << FRACTIONAL_BITS;
            val += new BigInteger((data - intPrt) * MULTIPLIER_DOUBLE);

            _storage = val;
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
        public byte[] GetBytes() => Serialize(this);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static I80F48 Deserialize(ReadOnlySpan<byte> data)
        {
            var bigInt = data.GetBigInt(0, Length, false);
            return new I80F48(bigInt);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Serialize(I80F48 data)
        {
            var buf = new byte[16];

            var count = buf.WriteBigInt(data._storage, 0, false);

            if (!data.IsPositive && count != 16)
            {
                //buf[count - 1] = (byte)(buf[count - 1] & 0x7F);
                //buf[15] = 0x80;
                while (count < 16) buf[count++] = 0xFF;
            }

            return buf;
        }

        public I80F48 Min(I80F48 other) => this > other ? other : this;

        public I80F48 Max(I80F48 other) => this > other ? this : other;

        /// <summary>
        /// Indicates whether the number represented is positive or not.
        /// </summary>
        public bool IsPositive => _storage.Sign >= 0;

        /// <summary>
        /// Indicates whether the number represented is zero or not.
        /// </summary>
        public bool IsZero => _storage.Sign == 0;


        /// <summary>
        /// Converts the I80F48 fixed point to a <see cref="decimal"/>.
        /// </summary>
        /// <returns>The number as a <see cref="decimal"/> .</returns>
        public decimal ToDecimal()
        {
            var div = BigInteger.DivRem(_storage, MULTIPLIER_BI, out BigInteger rem);

            var res = (decimal)div;
            res += (decimal)rem / MULTIPLIER_DECIMAL;

            return res;
        }

        /// <summary>
        /// Converts the I80F48 fixed point to a double-precision floating-point number.
        /// </summary>
        /// <returns>The number as a <see cref="double"/> .</returns>
        public double ToDouble()
        {
            var div = BigInteger.DivRem(_storage, MULTIPLIER_BI, out BigInteger rem);

            var res = (double)div;
            res += (double)rem / MULTIPLIER_DOUBLE;

            return res;
        }

        /// <summary>
        /// Returns the absolute number of the I80F48 number.
        /// </summary>
        /// <returns>The absolute number as an I80F48 type.</returns>
        public I80F48 Abs()
        {
            if (_storage.Sign < 0) return -this;
            return this;
        }
        /// <summary>
        /// Returns the largest integral value less than or equal to the I80F48 number.
        /// </summary>
        /// <returns>The largest integral value less than or equal to the I80F48 number as an I80F48 type.</returns>
        public I80F48 Floor() => new I80F48((_storage >> 48) << 48);

        /// <summary>
        /// Returns the smallest integral value that is greater than or equal to the I80F48 number.
        /// </summary>
        /// <returns>The smallest integral value that is greater than or equal to the I80F48 number as an I80F48 type.</returns>
        public I80F48 Ceiling()
        {
            var f = Floor();
            if (this > f) f++;

            return f;
        }
        /// <summary>
        /// Rounds a I80F48 value to the nearest integral value, and rounds midpoint values to the nearest even number.
        /// </summary>
        /// <returns>The integer nearest of the given number as an I80F48 type.</returns>
        public I80F48 Round()
        {
            var d = ToDecimal();

            d = Math.Round(d);

            return new I80F48(d);
        }

        /// <summary>
        /// Rounds a I80F48 value to a specified number of fractional digits, and rounds midpoint values to the nearest even number.
        /// </summary>
        /// <param name="decimals">The number of decimal places in the return value.</param>
        /// <returns>The number nearest to the given number that contains a number of fractional digits equal to decimals as an I80F48 type.</returns>
        public I80F48 Round(int decimals)
        {
            var d = ToDecimal();

            d = Math.Round(d, decimals);

            return new I80F48(d);
        }


        /// <summary>
        /// Gets the integer part of the I80F48 number.
        /// </summary>
        /// <returns>The integer part.</returns>
        public I80F48 GetIntegerPart()
        {
            return new I80F48(_storage & INTEGER_BITS);
        }

        /// <summary>
        /// Gets the integer part of the I80F48 number.
        /// </summary>
        /// <returns>The integer part.</returns>
        public BigInteger GetIntegerPartBI()
        {
            return _storage >> FRACTIONAL_BITS;
        }

        /// <summary>
        /// Gets the fractional part of the I80F48 number.
        /// </summary>
        /// <returns>The fractional part.</returns>
        public I80F48 GetFractionalPart()
        {
            return new I80F48(_storage & FRAC_BITS);
        }

        /// <summary>
        /// Gets the fractional part of the I80F48 number.
        /// </summary>
        /// <returns>The fractional part.</returns>
        public decimal GetFractionalPartD()
        {
            return (decimal)(_storage & FRAC_BITS) / MULTIPLIER_DECIMAL;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToDecimal().ToString();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is I80F48 other)
                return _storage.Equals(other._storage);
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _storage.GetHashCode();
        }

        public static I80F48 operator -(I80F48 rhs) => new I80F48(-rhs._storage);

        public static I80F48 operator +(I80F48 rhs) => new I80F48(+rhs._storage);

        public static I80F48 operator --(I80F48 rhs)
        {
            rhs._storage = rhs._storage - MULTIPLIER_BI;
            return rhs;
        }
        public static I80F48 operator ++(I80F48 rhs)
        {
            rhs._storage = rhs._storage + MULTIPLIER_BI;
            return rhs;
        }
        public static I80F48 operator +(I80F48 lhs, I80F48 rhs) => new I80F48(lhs._storage + rhs._storage);
        public static I80F48 operator -(I80F48 lhs, I80F48 rhs) => new I80F48(lhs._storage - rhs._storage);
        public static I80F48 operator *(I80F48 lhs, I80F48 rhs) => new I80F48((lhs._storage * rhs._storage) >> FRACTIONAL_BITS);
        public static I80F48 operator /(I80F48 lhs, I80F48 rhs) => new I80F48((lhs._storage << FRACTIONAL_BITS) / rhs._storage);
        public static bool operator ==(I80F48 lhs, I80F48 rhs) => lhs._storage == rhs._storage;
        public static bool operator !=(I80F48 lhs, I80F48 rhs) => lhs._storage != rhs._storage;
        public static bool operator <(I80F48 lhs, I80F48 rhs) => lhs._storage < rhs._storage;
        public static bool operator >(I80F48 lhs, I80F48 rhs) => lhs._storage > rhs._storage;
        public static bool operator <=(I80F48 lhs, I80F48 rhs) => lhs._storage <= rhs._storage;
        public static bool operator >=(I80F48 lhs, I80F48 rhs) => lhs._storage >= rhs._storage;


        public static explicit operator decimal(I80F48 val) => val.ToDecimal();
        public static explicit operator double(I80F48 val) => val.ToDouble();
        public static explicit operator float(I80F48 val) => (float)val.ToDouble();

        public static explicit operator long(I80F48 val) => (long)val.GetIntegerPartBI();
        public static explicit operator ulong(I80F48 val) => (ulong)val.GetIntegerPartBI();

        public static explicit operator int(I80F48 val) => (int)val.GetIntegerPartBI();
        public static explicit operator uint(I80F48 val) => (uint)val.GetIntegerPartBI();

        public static explicit operator byte(I80F48 val) => (byte)val.GetIntegerPartBI();
        public static explicit operator sbyte(I80F48 val) => (sbyte)val.GetIntegerPartBI();

        public static explicit operator I80F48(float val) => new I80F48(val);
        public static explicit operator I80F48(double val) => new I80F48(val);
        public static explicit operator I80F48(decimal val) => new I80F48(val);

        public static explicit operator I80F48(int val) => new I80F48(val);
        public static explicit operator I80F48(uint val) => new I80F48(val);

        public static explicit operator I80F48(long val) => new I80F48(val);
        public static explicit operator I80F48(ulong val) => new I80F48(val);

        public static explicit operator I80F48(byte val) => new I80F48(val);
        public static explicit operator I80F48(sbyte val) => new I80F48(val);
    }
}