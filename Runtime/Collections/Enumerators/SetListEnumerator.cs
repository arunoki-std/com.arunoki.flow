using System.Collections;
using System.Collections.Generic;

namespace Arunoki.Collections.Enumerators
{
    public struct SetListEnumerator<T> : IEnumerator<T>
    {
        private List<Set<T>> setsList;
        private int setIndex;
        private int itemIndex;

        public SetListEnumerator(List<Set<T>> setsList)
        {
            this.setsList = setsList;

            setIndex = 0;
            itemIndex = -1;
        }

        public T Current => setsList[setIndex][itemIndex];

        object IEnumerator.Current => Current!;

        public bool MoveNext()
        {
            if (setIndex >= setsList.Count)
                return false;

            ++itemIndex;
            if (itemIndex >= setsList[setIndex].Count)
            {
                itemIndex = 0;
                ++setIndex;
            }

            return setIndex < setsList.Count;
        }

        public void Reset()
        {
            setIndex = 0;
            itemIndex = -1;
        }

        public void Dispose()
        {
            setsList = null;
        }
    }
}
