using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LinakDeskClient.Cli
{
    internal class HeightParser
    {
        public static DeskHeight ParseHeightExpression(LinakDesk desk, string position)
        {
            var memoryPresetMatch = Regex.Match(position, "^m(?<number>[1-4])$"); // m1, m2, m3, m4
            if (memoryPresetMatch.Success)
            {
                int number = int.Parse(memoryPresetMatch.Groups["number"].Value);
                var memory = desk.State.Memory.GetByNumber(number);
                Console.WriteLine($"Moving to the memory preset {number} set to: {memory}");
                if (!memory.IsSet) throw new InvalidOperationException("Given memory preset is not set.");
                return memory.Height;
            }
            
            var cmHeightMatch = Regex.Match(position, "^(?<number>[0-9]+)cm$"); // 35cm
            if (cmHeightMatch.Success)
            {
                int number = int.Parse(cmHeightMatch.Groups["number"].Value);
                DeskHeight height = DeskHeight.FromCm(number);
                Console.WriteLine($"Moving to height: {height}");
                return height;
            }

            throw new InvalidOperationException($"Unable to parse height expression '{position}'. Use format 'm1' (memory preset) or '40cm' (height).");
        }
    }
}
