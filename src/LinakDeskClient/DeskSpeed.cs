using System;

namespace LinakDeskClient
{
    public struct DeskSpeed
    {
        public int Value { get; private set; }

        public static DeskSpeed Parse(byte[] data, int startIndex) =>
            new DeskSpeed()
            {
                Value = (Int16)(data[startIndex] + (data[startIndex + 1] << 8))
            };
        public override string ToString() => Value.ToString();

    }
}