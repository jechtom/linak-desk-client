using LinakDeskClient.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinakDeskClient
{
    public class DeskState
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public DeskHeight Height => HeightAndSpeed.Height;
        public DeskHeightAndSpeed HeightAndSpeed { get; set; }
        public DeskType DeskType { get; set; }
        public DeskCapabilities DeskCapabilities { get; set; }
        public MemoryPositionsBank Memory { get; set; }
        public DeskHeight HeightOffset { get; set; }
        public int ErrorsCount { get; set; }

        public override string ToString() => $"Desk \"{DeviceName}\" (Id={DeviceId}, DeskType={DeskType})\nCapabilities: {DeskCapabilities}\nMemory: {Memory}\nOffset: {HeightOffset}\nPosition: {HeightAndSpeed}";
    }
}
