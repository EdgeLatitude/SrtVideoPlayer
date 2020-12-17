using System.Collections;
using System.Collections.Generic;

namespace SrtVideoPlayer.Shared.DataStructures
{
    class CircularBuffer<T> : IEnumerable<T>
    {
        public readonly int _capacity;

        private readonly Queue<T> _queue = new Queue<T>();

        public CircularBuffer(int capacity)
        {
            _capacity = capacity;
        }

        public CircularBuffer(int capacity, IEnumerable<T> source)
        {
            _capacity = capacity;
            foreach (var item in source)
                Enqueue(item);
        }

        public int Count => _queue.Count;

        public bool IsEmpty => _queue.Count == 0;

        public bool IsFull => _queue.Count == _capacity;

        public void Enqueue(T value)
        {
            _queue.Enqueue(value);
            if (_queue.Count > _capacity)
                _queue.Dequeue();
        }

        public T Dequeue() => _queue.Dequeue();

        public T Peek() => _queue.Peek();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _queue)
                yield return item;
        }
    }
}
