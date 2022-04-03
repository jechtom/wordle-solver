using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleSolver
{
    public record Hint
    {
        public string Word { get; init; }
        public HintValue[] Hints { get; init; }
        
        public static Hint ParseHints(string word, string hintsString)
        {
            if (word.Length != hintsString.Length) throw new ArgumentException("Invalid hints name.", nameof(hintsString));
            var hints = new HintValue[word.Length];
            for (int i = 0; i < word.Length; i++)
            {
                hints[i] = hintsString[i] switch
                {
                    '.' => HintValue.GrayAndNotPresent,
                    'x' => HintValue.Yellow,
                    'X' => HintValue.Green,
                    _ => throw new InvalidOperationException($"Invalid hint character: '{hintsString[i]}'; valid characters: ['.', 'x', 'X']")
                };
            }

            // fix hints from GrayAndNotPresent to GrayAndPresentOnOtherLocationAsGreen
            for (int i = 0; i < word.Length; i++)
            {
                if (hints[i] != HintValue.GrayAndNotPresent) continue;
                for (int j = 0; j < word.Length; j++)
                {
                    if (i == j) continue;
                    if(word[i] == word[j] && hints[j] == HintValue.Green)
                    {
                        // fix - location is gray, but because letter is green already on other location
                        hints[i] = HintValue.GrayAndPresentOnOtherLocationAsGreen;
                        break;
                    }
                }
            }

            return new Hint() with
            {
                Word = word,
                Hints = hints
            };
        }

        /// <summary>
        /// Gets if given <paramref name="testWord"/> is possible word to guess based on this hint.
        /// </summary>
        /// <param name="testWord">Possible word to guess.</param>
        public bool CanBeWord(string testWord)
        {
            int length = Word.Length;

            for (int i = 0; i < length; i++)
            {
                // hint for this letter is green but tested word letter is different => no match
                if (Hints[i] == HintValue.Green && testWord[i] != Word[i]) return false;

                // hint is yellow but tested word don't have this letter in it => no match
                if (Hints[i] == HintValue.Yellow && !testWord.Contains(Word[i])) return false;

                // hint is gray and tested word have this letter in it => no match
                if (Hints[i] == HintValue.GrayAndNotPresent && testWord.Contains(Word[i])) return false;
            }

            return true;
        }

        public IEnumerable<char> GetGrayAndNotPresentLetters() => Word
            .Where((letter, index) => Hints[index] == HintValue.GrayAndNotPresent);
    }
}
