using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace WordleSolver
{
    internal class BeeSolver
    {
        public static async Task HandleAsync(string input, string letters)
        {
            letters = letters.ToLowerInvariant();

            const int length = 7;
            const int pangramValueMask = 0b1111111;

            // per each letter, create bit mask
            Dictionary<char, int> lettersDictionaryMasks = letters
                .Select((ch, index) => (ch, index))
                .ToDictionary(v => v.ch, v => 1 << v.index);

            char letterMain = letters[0];

            Console.WriteLine($"Input file: { input }");
            if (letters.Length != length) throw new InvalidOperationException($"Invalid letters length. Expected {length} letters.");
            Console.WriteLine($"Input letters: { string.Join("; ", letters.ToUpperInvariant().ToArray()) } (first is core letter)");
            Console.WriteLine($"Processing...");

            using var reader = new StreamReader(input);

            int getScore(string word)
            {
                if (word.Length < 4) return 0;
                int mask = 0;
                for (int i = 0; i < word.Length; i++)
                {
                    if (!lettersDictionaryMasks.TryGetValue(word[i], out int maskCurrent)) return 0;
                    mask |= maskCurrent;
                }
                if ((mask & 1) == 0) return 0; // no core letter
                return word.Length - 3 + ((mask == pangramValueMask) ? 7 : 0);
            }

            var scoreBlock = new TransformBlock<string, (string, int)>(word => (word, getScore(word)), new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            });

            var aggregator = new SortTopAggregator<(string word, int score)>(new StringIntSorter(), maxCount: 30);

            var sortActionBlock = new ActionBlock<(string word, int score)>(scoredWord =>
            {
                if (scoredWord.score == 0) return;
                aggregator.UpdateWithValue(scoredWord);
            }, new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = 1
            });

            scoreBlock.LinkTo(sortActionBlock, new DataflowLinkOptions()
            {
                PropagateCompletion = true
            });

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;
                scoreBlock.Post(line);
            }
            Console.WriteLine("Read completed.");
            scoreBlock.Complete();
            await sortActionBlock.Completion;
            Console.WriteLine("Scoring completed.");
            Console.WriteLine($"Top words:");
            foreach (var item in aggregator.GetSortedDescending())
            {
                Console.WriteLine($" - \"{item.word.ToUpperInvariant()}\": {item.score} points");
            }
            Console.WriteLine($"Done.");

        }

        class StringIntSorter : IComparer<(string word, int score)>
        {
            public int Compare((string word, int score) x, (string word, int score) y) => x.score.CompareTo(y.score);
        }
    }
}
