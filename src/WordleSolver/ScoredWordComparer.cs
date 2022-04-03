using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleSolver
{
    /// <summary>
    /// Compares <see cref="ScoredWord"/> based on <see cref="ScoredWord.Score"/> property.
    /// </summary>
    public class ScoredWordComparer : IComparer<ScoredWord>
    {
        public int Compare(ScoredWord x, ScoredWord y) => x.Score.CompareTo(y.Score);
    }
}
