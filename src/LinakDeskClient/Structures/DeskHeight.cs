using System;

namespace LinakDeskClient
{
    public struct DeskHeight
    {
        public DeskHeight(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }

        public static DeskHeight Parse(byte[] data, int startIndex) =>
            new DeskHeight()
            {
                Value = data[startIndex] + (data[startIndex + 1] << 8)
            };
        public override string ToString() => $"{(double)Value/100:0.00}cm";

        public byte[] Serialize() => new byte[] {
                (byte)(Value & 0xFF), 
                (byte)(Value >> 8 & 0xFF)
            };

        public static DeskHeight FromCm(double cm) => new DeskHeight((int)(cm * 100));
    }
}