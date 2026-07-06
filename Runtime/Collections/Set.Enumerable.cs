using System;
using System.Collections;
using System.Collections.Generic;
using Arunoki.Collections.Enumerators;

namespace Arunoki.Collections
{
    public partial class Set<TElement> : IEnumerable<TElement>
    {
        public MutableCastEnumerable<TElement, T> Cast<T>() => new(Elements);

        public MutableWhereCastEnumerable<TElement, T> Cast<T>(Func<T, bool> condition) =>
            new(Elements, condition);

        public MutableWhereEnumerable<TElement> Where(Func<TElement, bool> condition) =>
            new(Elements, condition);

        /// var (index, value)
        public MutableIndexedEnumerable<TElement> WithIndex() => new(Elements);

        public MutableEnumerator<TElement> GetEnumerator() => new(Elements);

        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() =>
            new MutableEnumerator<TElement>(Elements);

        IEnumerator IEnumerable.GetEnumerator() => new MutableEnumerator<TElement>(Elements);
    }
}
