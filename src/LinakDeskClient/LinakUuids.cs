using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinakDeskClient
{
    public static class LinakUuids
    {
        public sealed class Services
        {
            public static readonly Guid GENERIC_ACCESS = Guid.Parse("00001800-0000-1000-8000-00805F9B34FB");
            public static readonly Guid GENERIC_ATTRIBUTE = Guid.Parse("00001801-0000-1000-8000-00805f9b34fb");
            public static readonly Guid CONTROL = Guid.Parse("99FA0001-338A-1024-8A49-009C0215F78A");
            public static readonly Guid REFERENCE_INPUT = Guid.Parse("99FA0030-338A-1024-8A49-009C0215F78A");
            public static readonly Guid REFERENCE_OUTPUT = Guid.Parse("99FA0020-338A-1024-8A49-009C0215F78A");
            public static readonly Guid DPG = Guid.Parse("99FA0010-338A-1024-8A49-009C0215F78A");
        }

        public sealed class Characteristics
        {
            public static readonly Guid GENERIC_ACCESS__DEVICE_NAME = Guid.Parse("00002A00-0000-1000-8000-00805F9B34FB");
            public static readonly Guid GENERIC_ATTRIBUTE__SERVICE_CHANGED = Guid.Parse("00002A05-0000-1000-8000-00805F9B34FB");
            public static readonly Guid REFERENCE_INPUT__CTRL1 = Guid.Parse("99FA0031-338A-1024-8A49-009C0215F78A");
            public static readonly Guid REFERENCE_OUTPUT__HEIGHT_SPEED = Guid.Parse("99FA0021-338A-1024-8A49-009C0215F78A");
            public static readonly Guid REFERENCE_OUTPUT__MASK = Guid.Parse("99FA0029-338A-1024-8A49-009C0215F78A");
            public static readonly Guid DPG__DPG = Guid.Parse("99FA0011-338A-1024-8A49-009C0215F78A");
            public static readonly Guid CONTROL__CONTROL = Guid.Parse("99FA0002-338A-1024-8A49-009C0215F78A");
            public static readonly Guid CONTROL__ERROR = Guid.Parse("99FA0003-338A-1024-8A49-009C0215F78A");
        }
    }
}
