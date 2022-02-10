using Solnet.Programs.Utilities;
using System;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents the metadata of an account in Mango Markets.
    /// </summary>
    public class MetaData
    {
        /// <summary>
        /// The layout for the <see cref="MetaData"/> structure.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the <see cref="MetaData"/> structure.
            /// </summary>
            internal const int Length = 8;

            /// <summary>
            /// The offset at which the data type value begins.
            /// </summary>
            internal const int DataTypeOffset = 0;

            /// <summary>
            /// The offset at which the version value begins.
            /// </summary>
            internal const int VersionOffset = 1;

            /// <summary>
            /// The offset at which the boolean which defines if the account is initialized value begins.
            /// </summary>
            internal const int IsInitializedOffset = 2;

            /// <summary>
            /// The offset at which the metadata extra info begins.
            /// </summary>
            internal const int ExtraInfoOffset = 3;

            /// <summary>
            /// The length of the extra info.
            /// </summary>
            internal const int ExtraInfoLength = 5;
        }

        /// <summary>
        /// The type of account.
        /// </summary>
        public DataType DataType;

        /// <summary>
        /// The version.
        /// </summary>
        public byte Version;

        /// <summary>
        /// Whether the account is initialized or not.
        /// </summary>
        public bool IsInitialized;

        /// <summary>
        /// Extra info.
        /// </summary>
        public byte[] ExtraInfo;

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="MetaData"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="MetaData"/> structure.</returns>
        public static MetaData Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"data length is invalid, expected {Layout.Length} but got {data.Length}");

            DataType dataType = (DataType)Enum.Parse(typeof(DataType), data.GetU8(Layout.DataTypeOffset).ToString());

            return new MetaData
            {
                DataType = dataType,
                Version = data.GetU8(Layout.VersionOffset),
                IsInitialized = data.GetU8(Layout.IsInitializedOffset) == 1,
                ExtraInfo = data.GetBytes(Layout.ExtraInfoOffset, Layout.ExtraInfoLength)
            };
        }
    }
}