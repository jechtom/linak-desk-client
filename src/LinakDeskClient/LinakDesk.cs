using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using InTheHand.Bluetooth;

namespace LinakDeskClient
{
    public class LinakDesk
    {
        public LinakDesk(BluetoothDevice device)
        {
            BluetoothDevice = device ?? throw new ArgumentNullException(nameof(device));
            OnHeightOrSpeedChanged += (h) => Console.WriteLine($"Height and speed: {h}"); // debug
        }

        public BluetoothDevice BluetoothDevice { get; }
        public RemoteGattServer GattServer => BluetoothDevice.Gatt;
        public DeskState State { get; private set; }
        public ServicesAccessor Services { get; private set; }
        public DpgCommandDispatcher DpgCommandDispatcher { get; private set; }
        public Mover Mover { get; private set; }

        public async Task InitAsync()
        {
            // connect
            await GattServer.ConnectAsync();

            // init state
            State = new DeskState();
            State.DeviceId = BluetoothDevice.Id;

            // init services
            var services = new ServicesAccessor();
            await services.InitAsync(GattServer);
            Services = services;
            DpgCommandDispatcher = new DpgCommandDispatcher(services);
            Mover = new Mover(this);

            // subscribe to DPG data callback
            await SubscribeToNotification(Services.Dpg, DpgCommandDispatcher.OnDpgDataReceived, invokeRead: false);

            // read device name and desk type
            State.DeviceName = Encoding.UTF8.GetString(await Services.DeviceName.ReadValueAsync());
            State.DeskType = (DeskType)(await Services.Mask.ReadValueAsync())[0];

            // subscribe and read position/speed
            await SubscribeToNotification(Services.HeightSpeed, (data) =>
            {
                State.HeightAndSpeed = DeskHeightAndSpeed.Parse(data);
                OnHeightOrSpeedChanged?.Invoke(State.HeightAndSpeed);
            }, invokeRead: true);

            // subscribe and error event
            await SubscribeToNotification(Services.Error, (data) =>
            {
                State.ErrorsCount++;
                OnReceivedError?.Invoke();
                Console.WriteLine($"Error: 0x{BluetoothDebugHelpers.ByteArrayToString(data)}");
            }, invokeRead: false);

            // subscribe and service change event
            await SubscribeToNotification(Services.ServiceChanged, (data) =>
            {
                if (data == null)
                {
                    Console.WriteLine($"Service changed: null");
                    return;
                }
                Console.WriteLine($"Service changed: 0x{BluetoothDebugHelpers.ByteArrayToString(data)}");
            }, invokeRead: false);

            // invoke DPG requests
            State.DeskCapabilities = await DpgCommandDispatcher.InvokeGetCapabilitiesAsync();
            State.HeightOffset = await DpgCommandDispatcher.InvokeGetDeskOffsetAsync();
            State.Memory = new MemoryPositionsBank(State.DeskCapabilities.MemorySize);
            for (int i = 0; i < State.DeskCapabilities.MemorySize; i++)
            {
                var memoryPosition = await DpgCommandDispatcher.InvokeGetMemoryPositionsAsync(i + 1);
                State.Memory.SetByNumber(i + 1, memoryPosition);
            }

            Console.WriteLine(State.ToString());
        }

        public event Action<DeskHeightAndSpeed> OnHeightOrSpeedChanged;
        public event Action OnReceivedError;

        public async Task MoveUpAsync()
        {
            await Services.Control.WriteValueWithResponseAsync(new byte[] { 71, 0 }); // UP
        }

        public async Task MoveDownAsync()
        {
            await Services.Control.WriteValueWithResponseAsync(new byte[] { 70, 0 }); // DOWN
        }

        public async Task MoveStopAsync()
        {
            await Services.Control.WriteValueWithResponseAsync(new byte[] { 255, 0 }); // STOP
        }

        private async Task SubscribeToNotification(GattCharacteristic characteristic, Action<byte[]> callback, bool invokeRead)
        {
            if (characteristic == null) throw new ArgumentNullException(nameof(characteristic));

            // read initial value
            if (invokeRead)
            {
                byte[] firstReadData = await characteristic.ReadValueAsync();
                callback.Invoke(firstReadData);
            }

            // subscribe
            characteristic.CharacteristicValueChanged += (s, e) => callback(e.Value);
        }
    }
}
