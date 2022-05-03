using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LinakDeskClient.DpgCommands
{
    public class CommandDispatcher
    {
        public CommandDispatcher(ServicesAccessor accessor)
        {
            Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        readonly SemaphoreSlim semaphore = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        readonly ManualResetEventSlim waitForData = new ManualResetEventSlim(initialState: false);
        byte[] lastData;

        public void OnDpgDataReceived(byte[] data)
        {
            lastData = data;
            waitForData.Set();
        }

        public async Task<byte[]> InvokeCommandAsync(byte[] request)
        {
            await semaphore.WaitAsync();
            try
            {
                waitForData.Reset();
                lastData = null;
                await Accessor.Dpg.WriteValueWithResponseAsync(request);
                waitForData.Wait();

                byte[] result = ValidateAndExtractMessage(lastData);
                lastData = null;
                return result;
            }
            finally
            {
                semaphore.Release();
            }
        }

        private byte[] ValidateAndExtractMessage(byte[] data)
        {
            // validate message
            if (data[0] != 0x01)
            {
                throw new InvalidOperationException($"DPG: Invalid data! Data=0x{BluetoothDebugHelpers.ByteArrayToString(lastData)}");
            }

            // confirmation, no data
            if (data[1] == 0x00)
            {
                return new byte[0];
            }

            // skip prefix header
            byte[] result = new byte[data.Length - 2];
            Array.Copy(data, 2, result, 0, result.Length);
            return result;
        }

        public ServicesAccessor Accessor { get; }

        private byte[] BuildCommandRequestRead(CommandTypes command) => new byte[] { 0x7F, (byte)command, 0x0 }; // read is: [0x7f, CMD, 0x0]
        private byte[] BuildCommandRequestWrite(CommandTypes command, byte[] data) => CombineTwoArrays(new byte[] { 0x7F, (byte)command, 0x80 }, data); // write is: [0x7f, CMD, 0x80, ... data]

        public async Task<DeskCapabilities> InvokeGetCapabilitiesAsync()
        {
            byte[] data = await InvokeCommandAsync(BuildCommandRequestRead(CommandTypes.GET_CAPABILITIES));
            var result = new DeskCapabilities(data);
            return result;
        }

        public async Task<DeskHeight> InvokeGetDeskOffsetAsync()
        {
            byte[] data = await InvokeCommandAsync(BuildCommandRequestRead(CommandTypes.DESK_OFFSET));
            var result = DeskHeight.Parse(data, 0);
            return result;
        }

        public async Task<MemoryPosition> InvokeGetMemoryPositionsAsync(int number)
        {
            if (number <= 0 || number > MemoryPositionsBank.MaximumPositions) throw new ArgumentOutOfRangeException("Invalid memory position number. Position on one-based.");
            byte[] command = BuildCommandRequestRead(CommandTypes.GET_SET_MEMORY_POSITION_1 + (number - 1));
            byte[] data = await InvokeCommandAsync(command);

            var result = MemoryPosition.Parse(data);
            return result;
        }

        private static T[] CombineTwoArrays<T>(T[] a1, T[] a2)
        {
            T[] arrayCombined = new T[a1.Length + a2.Length];
            Array.Copy(a1, 0, arrayCombined, 0, a1.Length);
            Array.Copy(a2, 0, arrayCombined, a1.Length, a2.Length);
            return arrayCombined;
        }
    }
}

