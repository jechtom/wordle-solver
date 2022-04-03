using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace WordleSolver
{
    internal class StartWordScore
    {
        public static async Task HandleAsync(int length, string input, string output)
        {
            Console.WriteLine($"Length of words to filter: { length }");
            Console.WriteLine($"Input file: { input }");
            Console.WriteLine($"Output score file: { output }");
            Console.WriteLine($"Reading input to memory...");
            string[] allWords = await File.ReadAllLinesAsync(input);
            Console.WriteLine($"Done reading. Words: { allWords.Length }");

            using (var writer = new StreamWriter(output))
            {
                await ScoreWords(
                    allWords,
                    new Hint[0],
                    tryFindSolutionBasedOnHints: true,
                    new ActionBlock<ScoredWord>(v => writer.WriteLine($"{v.Word};{v.Green};{v.Yellow}"))
                );
            }

            Console.WriteLine($"Processing done.");
        }

        public static async Task ScoreWords(IEnumerable<string> allWords, Hint[] hints, bool tryFindSolutionBasedOnHints, ActionBlock<ScoredWord> targetBlock)
        {
            // limit possible word based on hints
            if (hints.Any() && tryFindSolutionBasedOnHints)
            {
                int countBeforeLimit = allWords.Count();
                allWords = allWords.Where(w => hints.All(h => h.CanBeWord(w))).ToArray();
                Console.WriteLine($"Words: {allWords.Count()} of {countBeforeLimit} (hints filtering applied)");
            }
            else
            {
                Console.WriteLine($"Words: {allWords.Count()} (no hint filtering)");
            }

            // gray letters based on hints
            var grayLetters = new HashSet<char>();
            foreach (var hint in hints)
            {
                grayLetters.UnionWith(hint.GetGrayAndNotPresentLetters());
            }

            int maxDegreeOrParallelism = Environment.ProcessorCount;
            Console.WriteLine($"MaxDegreeOfParallelism: {maxDegreeOrParallelism}");

            // prepare pipeline
            var calculateScoreBlock = new TransformBlock<string, ScoredWord>(w =>
            {
                // how many waster letters?
                int wasted = 0;
                for (int i = 0; i < w.Length; i++)
                {
                    // gray letter?
                    if (grayLetters.Contains(w[i]))
                    {
                        wasted++;
                        continue;
                    }

                    if (!tryFindSolutionBasedOnHints)
                    {
                        // letter used on this position before?
                        for (int j = 0; j < hints.Length; j++)
                        {
                            if (hints[j].Word[i] == w[i]) wasted++;
                            break;
                        }
                    }
                }

                wasted *= allWords.Count(); // score based on count of tested words

                // calculate score score for using this word as guess for all possible words
                int totalGreen = 0;
                int totalYellow = 0;
                foreach (var possibleWord in allWords)
                {
                    // exclude possible plural form, which is not used in wordle
                    if (possibleWord.EndsWith("s")) continue;

                    var score = MatchCalculator.CalculateScore(guess: w, word: possibleWord);
                    totalGreen += score.green;
                    totalYellow += score.yellow;
                }
                return new ScoredWord() with { Word = w, Green = totalGreen, Yellow = totalYellow, WastedOpportunities = wasted };
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = maxDegreeOrParallelism, });

            calculateScoreBlock.LinkTo(targetBlock, new DataflowLinkOptions()
            {
                PropagateCompletion = true
            });

            // send all words to pipeline and wait for the result
            Console.WriteLine($"Processing...");
            foreach (var w in allWords) calculateScoreBlock.Post(w);
            calculateScoreBlock.Complete();
            await targetBlock.Completion;
            Console.WriteLine($"Scoring completed.");
        }
    }
}
