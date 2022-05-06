namespace LinakDeskClient
{
    public struct DeskHeightAndSpeed
    {
        public static DeskHeightAndSpeed Parse(byte[] data) => new DeskHeightAndSpeed()
        {
            Height = DeskHeight.Parse(data, 0),
            Speed = DeskSpeed.Parse(data, 2)
        };

        public DeskHeight Height { get; private set; }
        public DeskSpeed Speed { get; private set; }

        public override string ToString() => $"Height: {Height}; Speed: {Speed}";
    }
}