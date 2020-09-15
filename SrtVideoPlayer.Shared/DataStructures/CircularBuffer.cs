using System.Collections;
using System.Collections.Generic;

namespace SrtVideoPlayer.Shared.DataStructures
{
    class CircularBuffer<T> : IEnumerable<T>
    {
        public readonly int Capacity;

        private readonly Queue<T> Queue = new Queue<T>();

        public CircularBuffer(int capacity)
        {
            Capacity = capacity;
        }

        public CircularBuffer(int capacity, IEnumerable<T> source)
        {
            Capacity = capacity;
            foreach (var item in source)
                Write(item);
        }

        public bool IsEmpty => Queue.Count == 0;

        public bool IsFull => Queue.Count == Capacity;

        public void Write(T value)
        {
            Queue.Enqueue(value);
            if (Queue.Count > Capacity)
                Queue.Dequeue();
        }

        public T Read() => Queue.Dequeue();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in Queue)
                yield return item;
        }
    }
}
