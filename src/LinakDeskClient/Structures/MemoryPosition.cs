using System;
using System.Collections.Generic;
using System.Text;

namespace LinakDeskClient
{
    public struct MemoryPosition
    {
        public static MemoryPosition UnSet() => new MemoryPosition(isSet: false, height: default, counter: new IntValue(uint.MaxValue) /* max value required for unsetting */);
        public static MemoryPosition Set(DeskHeight height) => new MemoryPosition(isSet: true, height: height, counter: new IntValue(uint.MinValue) /* 0 required for setting */);

        public MemoryPosition(bool isSet, DeskHeight height, IntValue counter)
        {
            IsSet = isSet;
            Height = height;
            Counter = counter;
        }

        public static MemoryPosition Parse(byte[] data)
        {
            // if data[0] == 0x01
            // [0x01][2B height][4B counter] // height is set - counter is 0x00000000
            // else
            // [0x00][4B counter] // height is unset - counter is 0xFFFFFFFF

            DeskHeight height = default;
            IntValue counter;
            bool isSet = (data[0] == 1);
            if(isSet)
            {
                if (data.Length != 7 /* 1B isSet + 2B height + 4B counter */) throw new InvalidOperationException("Unexpected array length.");
                height = DeskHeight.Parse(data, 1);
                counter = IntValue.Parse(data, 3);
            }
            else
            {
                if (data.Length != 5 /* 1B isSet + 4B counter */) throw new InvalidOperationException("Unexpected array length.");
                height = default;
                counter = IntValue.Parse(data, 1);
            }
            return new MemoryPosition(isSet, height, counter);
        }

        public DeskHeight Height { get; private set; }
        public IntValue Counter { get; private set; }
        public bool IsSet { get; private set; }

        public override string ToString() => IsSet ? $"{Height}" : $"Unset";

        public byte[] Serialize()
        {
            var result = new byte[IsSet ? 7 /* 1B isSet + 2B height + 4B counter */ : 5 /* 1B isSet + 4B counter */];
            result[0] = (byte)(IsSet ? 1 : 0);
            if(IsSet)
            {
                byte[] heightBytes = Height.Serialize();
                Array.Copy(heightBytes, 0, result, 1, heightBytes.Length);

                byte[] counterBytes = Counter.Serialize();
                Array.Copy(counterBytes, 0, result, 3, counterBytes.Length);
            }
            else
            {
                byte[] counterBytes = Counter.Serialize();
                Array.Copy(counterBytes, 0, result, 1, counterBytes.Length);
            }

            return result;
        }
    }
}
