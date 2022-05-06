using System;
using System.Collections.Generic;
using System.Text;

namespace LinakDeskClient.Structures
{
    public struct DeskCapabilities
    {
        public DeskCapabilities(byte[] data)
        {
            MemorySize = data[0] & 0b111;
            Flags = (DeskCapabilitiesEnum)(data[0] & 0b11111000);
        }

        public int MemorySize { get; private set; }
        public DeskCapabilitiesEnum Flags { get; private set; }

        public override string ToString() => $"MemorySize={MemorySize};Flags=[{Flags}]";
    }
}
