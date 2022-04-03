using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleSolver
{
    public class MatchCalculator
    {
        /// <summary>
        /// Calculates number of greens and yellows for given <paramref name="guess"/> of <paramref name="word"/>.
        /// </summary>
        /// <param name="guess">Current guess.</param>
        /// <param name="word">Word to guess.</param>
        /// <remarks>
        /// Green score is for each correct letter on correct position.
        /// Yellow score is for each correct letter on incorrect position. 
        /// Letters with green score are excluded. For example "BAAAA" guess of "ZZZZB" will give you one yellow score for first letter.
        /// But "BAAAB" guess of "ZZZZB" will give you one green score for last letter but no yellow score for first one.
        /// </remarks>
        /// <returns>Number of green positions and yellow positions.</returns>
        public static (int green, int yellow) CalculateScore(string guess, string word)
        {
            int lenght = guess.Length;

            int green = 0, yellow = 0;
            for (int i = 0; i < lenght; i++)
            {
                // green - match on correct position
                if (guess[i] == word[i])
                {
                    green++;
                    continue;
                }

                // yellow - match on incorrect position; but only if this position is not guessed as green
                for (int j = 0; j < lenght; j++)
                {
                    if (j == i) continue; // exclude current position
                    if (guess[j] == word[j]) continue; // exclude green letters
                    if (guess[i] == word[j])
                    {
                        yellow++;
                        break;
                    }
                }
            }

            return (green, yellow);
        }
    }
}
