using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LinakDeskClient.Cli
{
    internal class MemorySetExpressionParser
    {
        public static IEnumerable<(int position, MemoryPosition value)> ParseMemoryExpression(LinakDesk desk, string expression)
        {
            var expressionParts = expression.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var positionsToSet = new List<(int position, MemoryPosition value)>();
            foreach (var expressionPart in expressionParts)
            {
                var match = Regex.Match(expressionPart, "^(?<key>[1-4]+)=(?<value>.+)$");
                if (!match.Success) throw new InvalidOperationException($"Cannot parse part of expression: {expressionPart}");

                if (!int.TryParse(match.Groups["key"].Value, out int memoryPosition)) throw new InvalidOperationException("Can't parse number of memory position.");
                string value = match.Groups["value"].Value;
                if (string.Equals(value, "reset"))
                {
                    Console.WriteLine($"Will reset memory position {memoryPosition}.");
                    positionsToSet.Add((memoryPosition, MemoryPosition.UnSet()));
                }
                else if (string.Equals(value, "current"))
                {
                    Console.WriteLine($"Will set memory position {memoryPosition} to current desk position. It is { desk.State.Height }.");
                    positionsToSet.Add((memoryPosition, MemoryPosition.Set(desk.State.Height)));
                }
                else if (int.TryParse(value, out int positionCm))
                {
                    var position = DeskHeight.FromCm(positionCm);
                    Console.WriteLine($"Will set memory position {memoryPosition} to position { position }.");
                    positionsToSet.Add((memoryPosition, MemoryPosition.Set(desk.State.Height)));
                }
                else
                {
                    throw new InvalidOperationException($"Can't parse given value part of expression: {value}");
                }
            }

            return positionsToSet;
        }
    }
}
