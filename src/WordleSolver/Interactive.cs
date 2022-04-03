using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace WordleSolver
{
    internal class Interactive
    {
        public static async Task HandleAsync(int length, string input)
        {
            Console.WriteLine($"Length of words to filter: { length }");
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

            List<string> allWords = readAllLinesEnumerable().ToList();
            Console.WriteLine($"Loaded {allWords.Count()} words.");

            var hints = new List<Hint>();

            // initial hints
            while (TryGetHint(out Hint hint))
            {
                hints.Add(hint);
            }

            while (true)
            {
                // score
                var aggregatorFindSolution = new SortTopAggregator<ScoredWord>(new ScoredWordComparer(), 15);
                var aggregatorExplore = new SortTopAggregator<ScoredWord>(new ScoredWordComparer(), 15);

                Console.WriteLine($"Find solution...");

                await StartWordScore.ScoreWords(
                    allWords, hints.ToArray(),
                    tryFindSolutionBasedOnHints: true,
                    new ActionBlock<ScoredWord>(v => aggregatorFindSolution.UpdateWithValue(v.CalculateDefaultScore()))
                );

                Console.WriteLine($"Done.");

                WriteAggregatorOptions(aggregatorFindSolution);

                /* skip exploration phase

                Console.WriteLine($"Exploration...");

                await StartWordScore.ScoreWords(
                    allWords, hints.ToArray(),
                    tryFindSolutionBasedOnHints: false,
                    new ActionBlock<ScoredWord>(v => aggregatorExplore.UpdateWithValue(v.CalculateDefaultScore()))
                );

                Console.WriteLine($"Done.");

                WriteAggregatorOptions(aggregatorExplore);
                */

                // add hint
                Hint hint;
                while(!TryGetHint(out hint))
                {
                }

                hints.Add(hint);
            }
        }

        private static bool TryGetHint(out Hint hint)
        {
            Console.WriteLine($"Enter word/result mask (.=gray, x=yellow, X=green) - ex. 'DAILY/.x.x.'):");
            string[] wordAndMask = (Console.ReadLine() ?? string.Empty).Split('/');
            if(wordAndMask.Length != 2)
            {
                hint = new Hint();
                return false;
            }
            string word = wordAndMask[0].ToLowerInvariant();
            string mask = wordAndMask[1];
            hint = Hint.ParseHints(word, mask);
            return true;
        }

        private static void WriteAggregatorOptions(SortTopAggregator<ScoredWord> aggregatorFindSolution)
        {
            Console.WriteLine($"Results (from largest score):");
            foreach (var item in aggregatorFindSolution.GetSortedDescending())
            {
                Console.WriteLine($" - '{item.Word?.ToUpperInvariant()}' score={item.Score}");
            }

            Console.WriteLine($"End of list.");
        }
    }
}
