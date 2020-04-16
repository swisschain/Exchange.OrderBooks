using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace OrderBooks.Common.Utils
{
    public class ConcurrentDictionary<TFirstKey, TSecondKey, TValue> : ConcurrentDictionary<TFirstKey, ConcurrentDictionary<TSecondKey, TValue>>
        where TValue : class
    {
        public IReadOnlyList<TValue> GetAll(TFirstKey firstKey)
        {
            if (TryGetValue(firstKey, out var secondKeysValues))
            {
                return secondKeysValues.Values.ToList().AsReadOnly();
            }

            return new List<TValue>();
        }

        public TValue Get(TFirstKey firstKey, TSecondKey secondKey)
        {
            if (TryGetValue(firstKey, out var secondKeysValues))
            {
                if (secondKeysValues.TryGetValue(secondKey, out var value))
                {
                    return value;
                }
            }

            return null;
        }

        public void Update(TFirstKey firstKey, TSecondKey secondKey, TValue value)
        {
            // exists
            if (TryGetValue(firstKey, out var secondKeysValues))
            {
                secondKeysValues[secondKey] = value;
            }
            // is not exist
            else
            {
                var newSecondKeysValues =
                    new ConcurrentDictionary<TSecondKey, TValue> { [secondKey] = value };

                this[firstKey] = newSecondKeysValues;
            }
        }
    }
}
