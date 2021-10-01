using Solnet.Programs.Utilities;
using System;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents the header of an <see cref="EventQueue"/>.
    /// </summary>
    public class QueueHeader
    {
        #region Layout
        
        /// <summary>
        /// Represents the layout of the <see cref="QueueHeader"/> data structure.
        /// </summary>
        internal class Layout
        {
            /// <summary>
            /// The size of the data for a queue header structure.
            /// </summary>
            internal const int Length = 32;
            
            /// <summary>
            /// The offset at which the value of the queue's metadata begins.
            /// </summary>
            internal const int MetadataOffset = 0;

            /// <summary>
            /// The offset at which the value of the queue's head begins.
            /// </summary>
            internal const int HeadOffset = 8;

            /// <summary>
            /// The offset at which the value of the queue's count begins.
            /// </summary>
            internal const int CountOffset = 16;

            /// <summary>
            /// The offset at which the value of the queue's next sequence number begins.
            /// </summary>
            internal const int NextSequenceNumberOffset = 24;
        }

        #endregion
        
        /// <summary>
        /// The metadata of the account.
        /// </summary>
        public MetaData Metadata;

        /// <summary>
        /// The _
        /// </summary>
        public ulong Head;

        /// <summary>
        /// The number of _
        /// </summary>
        public ulong Count;

        /// <summary>
        /// The value which defines the next sequence number.
        /// </summary>
        public ulong NextSequenceNumber;

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="QueueHeader"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The QueueHeader structure.</returns>
        public static QueueHeader Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length) throw new Exception("data length is invalid.");

            return new QueueHeader
            {
                Metadata = MetaData.Deserialize(data.Slice(Layout.MetadataOffset, MetaData.Layout.Length)),
                Head = data.GetU64(Layout.HeadOffset),
                Count = data.GetU64(Layout.CountOffset),
                NextSequenceNumber = data.GetU64(Layout.NextSequenceNumberOffset)
            };
        }
        
    }
}