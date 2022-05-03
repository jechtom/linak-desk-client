using InTheHand.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LinakDeskClient
{
    internal static class BluetoothDebugHelpers
    {
        public static async Task<string> RetrieveDeviceDebugInfoAsync(BluetoothDevice device)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Device: Id={device.Id}; Name=\"{device.Name}\"");
            var server = device.Gatt;
            sb.AppendLine($"Server: Conencted={server.IsConnected}; AutoConnect={server.AutoConnect}; Mtu={server.Mtu}; PreferredPhy={server.PreferredPhy}");
            var services = await server.GetPrimaryServicesAsync() ?? new List<GattService>();
            sb.AppendLine("Server hierarchy:");
            await PrintOutServicesAsync(sb, services, level: 1);

            return sb.ToString();
        }

        private static async Task PrintOutServicesAsync(StringBuilder sb, IEnumerable<GattService> services, int level)
        {
            string prefix = GetLevelPrefix(level);

            sb.AppendLine($"{prefix}Services ({services.Count()}):{(services.Any() ? string.Empty : " [no services]")}");
            foreach (var service in services)
            {
                await PrintOutServiceAsync(sb, service, level + 1);
            }
        }

        private static async Task PrintOutServiceAsync(StringBuilder sb, GattService service, int level)
        {
            string prefix = GetLevelPrefix(level);

            IReadOnlyList<GattCharacteristic> characteristics = await service.GetCharacteristicsAsync() ?? new List<GattCharacteristic>();
            IReadOnlyList<GattService> includedServices = await service.GetIncludedServicesAsync() ?? new List<GattService>();

            sb.AppendLine($"{prefix}- Service IsPrimary={service.IsPrimary}; Uuid={service.Uuid.Value}; HasCharacteristics={characteristics.Any()}; HasInlucededServices={includedServices.Any()}");

            if(characteristics.Any()) await PrintOutCharacteristicsAsync(sb, characteristics, level + 1);
            if(includedServices.Any()) await PrintOutServicesAsync(sb, includedServices, level + 1);
        }

        private static async Task PrintOutCharacteristicsAsync(StringBuilder sb, IReadOnlyList<GattCharacteristic> characteristics, int level)
        {
            string prefix = GetLevelPrefix(level);

            sb.AppendLine($"{prefix}Characteristics ({characteristics.Count()}):{(characteristics.Any() ? string.Empty : " [no characteristics]")}");
            foreach (var characteristic in characteristics)
            {
                await PrintOutCharacteristicAsync(sb, characteristic, level + 1);
            }
        }

        private static async Task PrintOutCharacteristicAsync(StringBuilder sb, GattCharacteristic characteristic, int level)
        {
            string prefix = GetLevelPrefix(level);
            
            IReadOnlyList<GattDescriptor> descriptors = await characteristic.GetDescriptorsAsync() ?? new List<GattDescriptor>();

            bool valueSuccess = false;
            byte[] value;
            try
            {
                value = await characteristic.ReadValueAsync();
                valueSuccess = true;
            }
            catch
            {
                value = null;
            }
            sb.AppendLine($"{prefix}- Characteristic Uuid={characteristic.Uuid.Value}; Value={ GetValueString(valueSuccess, value) }; HasDescriptors={descriptors.Any()}");
            sb.AppendLine($"{prefix}  Properties: {characteristic.Properties}");

            if(descriptors.Any()) await PrintOutDescriptorsAsync(sb, descriptors, level + 1);
        }

        private static async Task PrintOutDescriptorsAsync(StringBuilder sb, IReadOnlyList<GattDescriptor> descriptors, int level)
        {
            string prefix = GetLevelPrefix(level);
            sb.AppendLine($"{prefix}Descriptors ({descriptors.Count()}):{(descriptors.Any() ? string.Empty : " [no descriptors]")}");
            foreach (var descriptor in descriptors)
            {
                await PrintOutDescriptorAsync(sb, descriptor, level + 1);
            }
        }

        private static async Task PrintOutDescriptorAsync(StringBuilder sb, GattDescriptor descriptor, int level)
        {
            string prefix = GetLevelPrefix(level);

            bool valueSuccess = false;
            byte[] value;
            try
            {
                value = await descriptor.ReadValueAsync();
                valueSuccess = true;
            }
            catch
            {
                value = null;
            }

            sb.AppendLine($"{prefix}- Descriptor Uuid={descriptor.Uuid.Value}; Value={ GetValueString(valueSuccess, value) }");
            await Task.CompletedTask;
        }

        private static string GetValueString(bool success, byte[] value)
        {
            if (!success) return "[failed to read]";
            if (value == null) return "[null]";
            string hex = ByteArrayToString(value);
            string str = Encoding.ASCII.GetString(value);
            return $"0x{hex} (Length={value.Length}B; Str=\"{str}\")";
        }

        private static string GetLevelPrefix(int level) => new string(' ', level * 2);

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
