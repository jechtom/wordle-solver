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
                .Aggregate(aggregator, (a, w) => a.UpdateWithValue(w));

            Console.WriteLine($"Done.");

            Console.WriteLine($"Results (from largest score):");
            foreach (var item in aggregator.GetSortedDescending())
            {
                Console.WriteLine($" - '{item.Word?.ToUpperInvariant()}' score={item?.Score}");
            }

            Console.WriteLine($"End of list.");
        }

        class ScoredWord
        {
            public int Green { get; set; }
            public int Yellow { get; set; }
            public int Score => Green * 15 + Yellow * 10; // experimental scoring
            //public int Score => Green * 100 + Yellow * 10; // experimental scoring
            public string? Word { get; set; }
        }

        class ScoredWordComparer : IComparer<ScoredWord>
        {
            public int Compare(ScoredWord? x, ScoredWord? y) => 
                (x ?? throw new InvalidOperationException()).Score.CompareTo((y ?? throw new InvalidOperationException()).Score);
        }
    }
}
