using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinakDeskClient
{
    public class MemoryPositionsBank
    {
        public const int MaximumPositions = 4;

        public MemoryPositionsBank(int capacity)
        {
            if(capacity < 0 || capacity > MaximumPositions) throw new ArgumentOutOfRangeException(nameof(capacity));

            MaxPositions = capacity;
            positions = new MemoryPosition[capacity];
            for (int i = 0; i < capacity; i++)
            {
                positions[i] = new MemoryPosition(isSet: false, height: default);
            }
        }

        public int MaxPositions { get; }

        private MemoryPosition[] positions;

        public MemoryPosition Position1 => GetByNumber(1);
        public MemoryPosition Position2 => GetByNumber(2);
        public MemoryPosition Position3 => GetByNumber(3);
        public MemoryPosition Position4 => GetByNumber(4);

        public MemoryPosition GetByNumber(int number) => positions[number - 1];
        public MemoryPosition SetByNumber(int number, MemoryPosition position) => positions[number - 1] = position;

        public override string ToString() => string.Join(", ", positions.Select((m, i) => $"[{i+1}]=>{{{m}}}"));
    }
}
