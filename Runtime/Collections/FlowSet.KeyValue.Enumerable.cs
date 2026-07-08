using Arunoki.Flow.Collections.Enumerators;

namespace Arunoki.Flow.Collections
{
    public partial class FlowSet<TKey, TElement>
    {
        /// var (index, key, element)
        public MutablePairWithIndex<TKey, TElement> WithIndex() => new(Elements);

        /// var (key, value)
        public MutablePair<TKey, TElement> WithKey() => new(Elements);

        public MutablePairValueEnumerator<TKey, TElement> GetEnumerator() => new(Elements);
    }
}
