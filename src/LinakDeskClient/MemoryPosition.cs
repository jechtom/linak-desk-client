using System;
using System.Collections.Generic;
using System.Text;

namespace LinakDeskClient
{
    public struct MemoryPosition
    {
        public MemoryPosition(bool isSet, DeskHeight height)
        {
            IsSet = isSet;
            Height = height;
        }

        public static MemoryPosition Parse(byte[] data)
        {
            DeskHeight height = default;
            bool isSet = (data[0] == 1);
            if(isSet)
            {
                height = DeskHeight.Parse(data, 1);
            }
            return new MemoryPosition(isSet, height);
        }

        public DeskHeight Height { get; private set; }
        public bool IsSet { get; private set; }

        public override string ToString() => IsSet ? $"IsSet: {IsSet}; Height: {Height}" : $"IsSet: {IsSet}";
    }
}
