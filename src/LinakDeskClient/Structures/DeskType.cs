using System;
using System.Collections.Generic;
using System.Text;

namespace LinakDeskClient
{
    public enum DeskType
    {
        Invalid = 0,
        Desk = 1 << 0,
        LegRest = 1 << 6,
        BackRest = 1 << 7
    }
}
