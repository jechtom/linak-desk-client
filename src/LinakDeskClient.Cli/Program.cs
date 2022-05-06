using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LinakDeskClient.Cli
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand("LINAK DESK CLI");

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

            var scanCommand = new Command("scan", "Scan for bluetooth devices.");
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
                LinakDesk desk = await ConnectToDeskByDeviceIdAsync(device);

                Console.WriteLine("Done.");
            }, infoCommand.Arguments.ToArray());
            rootCommand.AddCommand(infoCommand);

            var moveCommand = new Command("move", "Move desk to given memory position.");
            moveCommand.AddArgument(new Argument<string>("device", "Id of device to connect to. Case insensitive."));
            moveCommand.AddArgument(new Argument<string>("position", "Memory position preset (m1, m2, m3, m3) or height in cm (with 'cm' suffix - example: '40cm')."));
            moveCommand.SetHandler(async (string device, string position) =>
            {
                LinakDesk desk = await ConnectToDeskByDeviceIdAsync(device);

                DeskHeight height = HeightParser.ParseHeightExpression(desk, position);

                Console.WriteLine($"Moving to memory position {height}.");
                await desk.Mover.MoveToPositionAsync(height);

                Console.WriteLine("Done.");
            }, moveCommand.Arguments.ToArray());
            rootCommand.AddCommand(moveCommand);

            var setCommand = new Command("set", "Set/unset memory position or positions.");
            setCommand.AddArgument(new Argument<string>("device", "Id of device to connect to. Case insensitive."));
            setCommand.AddArgument(new Argument<string>("expression", "Set expression. One or more (';' delimited) expressions in format 'key=value' where 'key' is number (1-4 memory positions) and 'value' is desk height (in CM) or 'reset' (to unset position) or 'current' (to capture current table height). Examples: 1=current or 1=15;2=70 or 1=reset;2=reset;3=reset."));
            setCommand.SetHandler(async (string device, string expression) =>
            {
                LinakDesk desk = await ConnectToDeskByDeviceIdAsync(device);

                IEnumerable<(int position, MemoryPosition value)> positionsToSet = MemorySetExpressionParser.ParseMemoryExpression(desk, expression);

                foreach (var positionToSet in positionsToSet)
                {
                    Console.WriteLine($"Setting memory position {positionToSet.position} to {positionToSet.value}.");
                    await desk.DpgCommandDispatcher.InvokeSetMemoryPositionsAsync(positionToSet.position, positionToSet.value);
                }

                Console.WriteLine("Done.");
            }, setCommand.Arguments.ToArray());
            rootCommand.AddCommand(setCommand);

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
