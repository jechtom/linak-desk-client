using System;
using System.Collections.Generic;
using System.Text;

namespace LinakDeskClient
{
    /// <summary>
    /// Represents 32bits unsigned integer serialized as little endian 4B byte array.
    /// </summary>
    public struct IntValue
    {
        public uint Value;

        public static IntValue Parse(byte[] data, int startIndex)
        {
            uint value = (uint)(data[startIndex] + (data[startIndex + 1] << 8) + (data[startIndex + 2] << 16) + (data[startIndex + 3] << 24));
            return new IntValue(value);
        }

        public byte[] Serialize() => new byte[] {
            (byte)(Value & 0xFF),
            (byte)(Value >> 8 & 0xFF),
            (byte)(Value >> 16 & 0xFF),
            (byte)(Value >> 24 & 0xFF)
        };

        public IntValue(uint value)
        {
            Value = value;
        }

        public IntValue Increment() => new IntValue(Value + 1);
    }
}
