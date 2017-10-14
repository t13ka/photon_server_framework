using System.Collections.Generic;

namespace YourGame.Server.LoadShedding
{
    internal class ValueHistory : Queue<int>
    {
        private readonly int capacity;

        public ValueHistory(int capacity)
            : base(capacity)
        {
            this.capacity = capacity;
        }

        public void Add(int value)
        {
            if (Count == capacity)
            {
                Dequeue();
            }

            Enqueue(value);
        }
    }
}