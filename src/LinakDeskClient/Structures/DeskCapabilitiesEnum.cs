using System;

namespace LinakDeskClient.Structures
{
    [Flags]
    public enum DeskCapabilitiesEnum
    {
        AutoUp = 1 << 3,
        AutoDown = 1 << 4,
        BluetoothAllowed = 1 << 5,
        HasDisplay = 1 << 6,
        HasLight = 1 << 7
    }
}