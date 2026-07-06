namespace Arunoki.Collections
{
    public class Pair<TKey, TValue>
    {
        public readonly TKey Key;
        public readonly TValue Element;

        public Pair(TKey key, TValue element)
        {
            Key = key;
            Element = element;
        }

        public override string ToString() => $"({Key}, {Element})";
    }
}
