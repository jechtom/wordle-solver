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

            int maxDegreeOrParallelism = Environment.ProcessorCount;
            Console.WriteLine($"MaxDegreeOfParallelism: {maxDegreeOrParallelism}");

            using var writer = new StreamWriter(output);

            // prepare pipeline
            var calculateScoreBlock = new TransformBlock<string, (string word, int green, int yellow)>(w =>
            {
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
                return (w, totalGreen, totalYellow);
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = maxDegreeOrParallelism, });

            var writeActionBlock = new ActionBlock<(string word, int green, int yellow)>(v =>
            {
                writer.WriteLine($"{v.word};{v.green};{v.yellow}");
            }, new ExecutionDataflowBlockOptions() {  MaxDegreeOfParallelism = 1 });

            calculateScoreBlock.LinkTo(writeActionBlock, new DataflowLinkOptions()
            {
                PropagateCompletion = true
            });

            // send all words to pipeline and wait for the result
            Console.WriteLine($"Processing...");
            foreach (var w in allWords) calculateScoreBlock.Post(w);
            calculateScoreBlock.Complete();
            await writeActionBlock.Completion;
            Console.WriteLine($"Processing done.");
        }
    }
}
