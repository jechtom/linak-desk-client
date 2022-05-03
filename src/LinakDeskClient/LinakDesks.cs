using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand.Bluetooth;

namespace LinakDeskClient
{
    public static class LinakDesks
    {
        public static async Task<IReadOnlyList<LinakBluetoothDevice>> GetPairedDevicesAsync() =>
             (await Bluetooth.GetPairedDevicesAsync()).Select(p => new LinakBluetoothDevice(p.Id, p.Name)).ToList();

        public static async Task<IReadOnlyList<LinakBluetoothDevice>> ScanDevicesAsync()
        {
            var linakDeskScanFilter = new BluetoothLEScanFilter();
            linakDeskScanFilter.Services.Add(LinakUuids.Services.DPG);
            var scanOptions = new RequestDeviceOptions();
            scanOptions.Filters.Add(linakDeskScanFilter);
            var devices = await Bluetooth.ScanForDevicesAsync(scanOptions);
            return devices.Select(p => new LinakBluetoothDevice(p.Id, p.Name)).ToList();
        }

        public static async Task<LinakDesk> GetDeskByDeviceIdAsync(string deviceId)
        {
            IReadOnlyCollection<BluetoothDevice> devices = await Bluetooth.GetPairedDevicesAsync();
            BluetoothDevice dev = devices.SingleOrDefault(d => string.Equals(d.Id, deviceId, StringComparison.OrdinalIgnoreCase));
            if (dev == null)
            {
                throw new InvalidOperationException($"Device {deviceId} not found.");
            }

            var linakServer = new LinakDesk(dev);

            return linakServer;
        }
    }
}
