using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleSolver
{
    public record struct ScoredWord
    {
        public string Word;
        public int Green;
        public int Yellow;
        public int Score;

        /// <summary>
        /// Gets how many letters has been used even if we know these are not present in guessed word based on previous hints.
        /// </summary>
        public int WastedOpportunities;

        public ScoredWord CalculateDefaultScore() => this with { Score = Green * 15 + Yellow + 10 - WastedOpportunities * 100 };
    }
}
