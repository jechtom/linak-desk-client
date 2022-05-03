using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;

namespace LinakDeskClient.Cli
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand();

            var listCommand = new Command("list", "List paired bluetooth devices.");
            listCommand.SetHandler(async () =>
            {
                IReadOnlyCollection<LinakBluetoothDevice> devices = await LinakDesks.GetPairedDevicesAsync();
                Console.WriteLine($"Found {devices.Count} already paired devices. Use 'scan' to find new devices.");
                foreach (var dev in devices)
                {
                    Console.WriteLine($" - {dev}");
                }
            });
            rootCommand.AddCommand(listCommand);

            var scanCommand = new Command("scan", "Scan bluetooth devices.");
            scanCommand.SetHandler(async () =>
            {
                Console.WriteLine("Scanning...");
                IReadOnlyCollection<LinakBluetoothDevice> devices = await LinakDesks.ScanDevicesAsync();
                Console.WriteLine($"Scan found {devices.Count} devices.");
                foreach (var dev in devices)
                {
                    Console.WriteLine($" - {dev}");
                }
            });
            rootCommand.AddCommand(scanCommand);

            var infoCommand = new Command("info", "Gets info about device.");
            infoCommand.AddArgument(new Argument<string>("device", "Id of device to connect to. Case insensitive."));
            infoCommand.SetHandler(async (string device) =>
            {
                LinakDesk linakServer = await ConnectToDeskByDeviceIdAsync(device);

                Console.WriteLine("Done.");
            }, infoCommand.Arguments.ToArray());
            rootCommand.AddCommand(infoCommand);

            var moveCommand = new Command("move", "Move desk to given memory position.");
            moveCommand.AddArgument(new Argument<string>("device", "Id of device to connect to. Case insensitive."));
            moveCommand.AddArgument(new Argument<int>("memory", "Memory position. One-based indexed."));
            moveCommand.SetHandler(async (string device, int memoryPosition) =>
            {
                LinakDesk linakServer = await ConnectToDeskByDeviceIdAsync(device);

                Console.WriteLine($"Moving to memory position {memoryPosition}.");
                await linakServer.Mover.MoveToMemoryPositionAsync(memoryPosition);

                Console.WriteLine("Done.");
            }, moveCommand.Arguments.ToArray());
            rootCommand.AddCommand(moveCommand);

            await rootCommand.InvokeAsync(args);
        }

        private static async Task<LinakDesk> ConnectToDeskByDeviceIdAsync(string deviceId)
        {
            var linakDesk = await LinakDesks.GetDeskByDeviceIdAsync(deviceId);
            
            Console.WriteLine("Found. Connecting...");
            await linakDesk.InitAsync();
            Console.WriteLine("Connected.");
            return linakDesk;
        }
    }
}
