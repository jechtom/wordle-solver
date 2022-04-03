using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleSolver
{
    public class SortTopAggregator<T>
    {
        public SortTopAggregator(IComparer<T> comparer, int maxCount)
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            MaxCount = maxCount;
            _values = new List<T>(capacity: maxCount);
        }

        public int MaxCount { get; }
        public IComparer<T> Comparer { get; }

        private List<T> _values;
        private T? _minValue;

        public IEnumerable<T> GetSortedDescending() => _values.OrderByDescending(v => v, Comparer);

        public SortTopAggregator<T> UpdateWithValue(T newItem)
        {
            if(newItem == null) throw new ArgumentNullException(nameof(newItem));

            // first item
            if (_values.Count == 0)
            {
                _values.Add(newItem);
                _minValue = newItem;
                return this;
            }

            // not at max capacity
            if (_values.Count < MaxCount)
            {
                _values.Add(newItem);
                
                // update min value if current min value is greater than new item
                if(Comparer.Compare(_minValue, newItem) > 0)
                {
                    _minValue = newItem;
                }

                return this;
            }

            // at capacity and new item is less or equal to current min value - don't add it
            if(Comparer.Compare(_minValue, newItem) >= 0)
            {
                return this;
            }

            // replace min value
            for (int i = 0; i < _values.Count; i++)
            {
                if(Comparer.Compare(_values[i], _minValue) == 0)
                {
                    _values[i] = newItem;
                    break;
                }
            }

            // update min value
            _minValue = _values.Min(Comparer) ?? throw new InvalidOperationException();
            return this;
        }

    }
}
