namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents a <see cref="EventType"/> in the Mango <see cref="EventQueue"/>.
    /// </summary>
    public abstract class Event
    {
        /// <summary>
        /// Represents the layout of the <see cref="Event"/>.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the <see cref="Event"/> structure.
            /// </summary>
            internal const int Length = 200;

            /// <summary>
            /// The offset at which the event type begins.
            /// </summary>
            internal const int EventTypeOffset = 0;
            
            /// <summary>
            /// The offset at which the timestamp value begins.
            /// </summary>
            internal const int TimestampOffset = 8;
            
            /// <summary>
            /// The offset at which the sequence number value begins.
            /// </summary>
            internal const int SequenceNumberOffset = 16;
        }

        /// <summary>
        /// The event type.
        /// </summary>
        public EventType EventType;
        
        /// <summary>
        /// The timestamp of the fill event.
        /// </summary>
        public ulong Timestamp; 
       
        /// <summary>
        /// The sequence number.
        /// </summary>
        public ulong SequenceNumber;
    }
}