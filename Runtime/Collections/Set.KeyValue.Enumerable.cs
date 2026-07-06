using Arunoki.Collections.Enumerators;

namespace Arunoki.Collections
{
    public partial class Set<TKey, TElement>
    {
        /// var (index, key, element)
        public MutablePairWithIndex<TKey, TElement> WithIndex() => new(Elements);

        /// var (key, value)
        public MutablePair<TKey, TElement> WithKey() => new(Elements);

        public MutablePairValueEnumerator<TKey, TElement> GetEnumerator() => new(Elements);
    }
}
