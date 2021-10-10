using Solnet.Mango.Models.Events;
using Solnet.Mango.Models.Perpetuals;
using Solnet.Programs.Utilities;
using System;
using System.Collections.Generic;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents the an <see cref="EventQueue"/> in Mango's <see cref="PerpMarket"/>.
    /// </summary>
    public class EventQueue
    {
        /// <summary>
        /// The header of the event queue, which specifies item count etc.
        /// </summary>
        public QueueHeader Header;

        /// <summary>
        /// The events in the queue.
        /// </summary>
        public IList<Event> Events;

        /// <summary>
        /// Deserialize a span of bytes into an <see cref="EventQueue"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The Event Queue structure.</returns>
        public static EventQueue Deserialize(byte[] data)
        {
            ReadOnlySpan<byte> span = data.AsSpan();
            QueueHeader header = QueueHeader.Deserialize(span[..QueueHeader.Layout.Length]);

            ReadOnlySpan<byte> headLessData = span.Slice(
                QueueHeader.Layout.Length,
                span.Length - QueueHeader.Layout.Length);

            int numElements = headLessData.Length / Event.Layout.Length;
            List<Event> events = new(numElements);

            for (int i = 0; i < numElements; i++)
            {
                long idx = ((long)header.Head + (long)header.Count + numElements - 1 - i) % numElements;
                long evtOffset = idx * Event.Layout.Length;

                EventType evtType =
                    (EventType)Enum.Parse(typeof(EventType), headLessData.GetU8((int)evtOffset).ToString());
                Event evt = null;

                switch (evtType)
                {
                    case EventType.Fill:
                        evt = FillEvent.Deserialize(headLessData.Slice((int)evtOffset, Event.Layout.Length));
                        break;
                    case EventType.Out:
                        evt = OutEvent.Deserialize(headLessData.Slice((int)evtOffset, Event.Layout.Length));
                        break;
                    case EventType.Liquidate:
                        evt = LiquidateEvent.Deserialize(headLessData.Slice((int)evtOffset, Event.Layout.Length));
                        break;
                }

                if (evt == null) continue;
                events.Add(evt);
            }
            return new EventQueue
            {
                Header = header,
                Events = events
            };
        }

        /// <summary>
        /// Deserialize a span of bytes into an <see cref="EventQueue"/> instance that contains only the new events
        /// since the given sequence number.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <param name="lastSequenceNumber">The previous queue's next sequence number.</param>
        /// <returns>The Event Queue structure.</returns>
        public static EventQueue DeserializeSince(ReadOnlySpan<byte> data, ulong lastSequenceNumber = 0)
        {
            QueueHeader header = QueueHeader.Deserialize(data[..QueueHeader.Layout.Length]);

            ReadOnlySpan<byte> headLessData = data.Slice(
                QueueHeader.Layout.Length,
                data.Length - QueueHeader.Layout.Length);

            int numElements = headLessData.Length / Event.Layout.Length;

            // Calculate number of missed events
            // Account for u32 & ring buffer overflows
            const long modulo = 0x100000000;
            long missedEvents = (long)(header.NextSequenceNumber - lastSequenceNumber + modulo) % modulo;

            if (missedEvents > numElements)
            {
                missedEvents = numElements - 1;
            }

            long startSequence = ((long)header.NextSequenceNumber - missedEvents + modulo) % numElements;

            // Define boundary indexes in ring buffer [start;end]
            long endIdx = ((long)header.Head + (long)header.Count) % numElements;
            long startIdx = (endIdx - missedEvents + numElements) % numElements;

            List<Event> events = new();

            for (int i = 0; i < missedEvents; i++)
            {
                long idx = (startIdx + i) % numElements;
                long evtOffset = idx * Event.Layout.Length;

                EventType evtType =
                    (EventType)Enum.Parse(typeof(EventType), headLessData.GetU8((int)evtOffset).ToString());
                Event evt = null;

                switch (evtType)
                {
                    case EventType.Fill:
                        evt = FillEvent.Deserialize(headLessData.Slice((int)evtOffset, Event.Layout.Length));
                        break;
                    case EventType.Out:
                        evt = OutEvent.Deserialize(headLessData.Slice((int)evtOffset, Event.Layout.Length));
                        break;
                    case EventType.Liquidate:
                        evt = LiquidateEvent.Deserialize(headLessData.Slice((int)evtOffset, Event.Layout.Length));
                        break;
                }

                if (evt == null) continue;
                evt.SequenceNumber = (ulong)(startSequence + i) % modulo;
                events.Add(evt);
            }

            return new EventQueue
            {
                Header = header,
                Events = events
            };
        }
    }
}