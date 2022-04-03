using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace WordleSolver
{
    internal class StartWordScoreSort
    {
        public static async Task HandleAsync(string input)
        {
            Console.WriteLine($"Input file: { input }");
            Console.WriteLine($"Reading input...");

            IEnumerable<string> readAllLinesEnumerable()
            {
                using var reader = new StreamReader(input);
                while (!reader.EndOfStream)
                {
                    yield return reader.ReadLine() ?? throw new InvalidOperationException();
                }
            }

            var aggregator = new SortTopAggregator<ScoredWord>(new ScoredWordComparer(), 10);

            readAllLinesEnumerable().Select(line =>
            {
                string[] splitted = line.Split(';');
                return new ScoredWord() { 
                    Word = splitted[0], 
                    Green = int.Parse(splitted[1]), 
                    Yellow = int.Parse(splitted[2]) 
                };
            })
                .Select(sw => sw.CalculateDefaultScore())
                .Aggregate(aggregator, (a, w) => a.UpdateWithValue(w));

            Console.WriteLine($"Done.");

            Console.WriteLine($"Results (from largest score):");
            foreach (var item in aggregator.GetSortedDescending())
            {
                Console.WriteLine($" - '{item.Word.ToUpperInvariant()}' score={item.Score}");
            }

            Console.WriteLine($"End of list.");
        }
    }
}
