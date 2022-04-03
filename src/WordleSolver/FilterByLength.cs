using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace WordleSolver
{
    internal class FilterByLength
    {
        public static async Task HandleAsync(int length, string input, string output)
        {
            Console.WriteLine($"Length of words to filter: { length }");
            Console.WriteLine($"Input file: { input }");
            Console.WriteLine($"Output file: { output }");
            Console.WriteLine($"Processing...");

            int inputCount = 0, outputCount = 0;

            using var reader = new StreamReader(input);
            using var writer = new StreamWriter(output);

            IEnumerable<string> filterFunc(string input)
            {
                inputCount++;
                if (input.Length != length) yield break;
                outputCount++;
                yield return input;
            }

            var filterBlock = new TransformManyBlock<string, string>(filterFunc);
            var writeBlockAction = new ActionBlock<string>(writer.WriteLine);

            filterBlock.LinkTo(writeBlockAction, new DataflowLinkOptions()
            {
                PropagateCompletion = true
            });

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;
                filterBlock.Post(line);
            }
            Console.WriteLine("Read completed.");
            filterBlock.Complete();
            await writeBlockAction.Completion;
            Console.WriteLine("Write completed.");
            Console.WriteLine($"Input words count: { inputCount }");
            Console.WriteLine($"Output words count: { outputCount }");
        }
    }
}
