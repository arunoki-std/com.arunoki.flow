using System;
using System.Collections.Generic;
using Arunoki.Collections;
using NUnit.Framework;

namespace Arunoki.Flow.Tests
{
    public class SetsTypeCollectionTests
    {
        [Test]
        public void GetOrCreate_CachesByTypeAndMissingGetThrows()
        {
            var collection = new InspectableSetsTypeCollection<object>();

            var first = collection.GetOrCreate<FooKey>();
            var second = collection.GetOrCreate<FooKey>();

            Assert.That(second, Is.SameAs(first));
            Assert.That(collection.TryGet<BarKey>(out _), Is.False);
            Assert.That(() => collection.Get<BarKey>(), Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public void Operations_SpanAllSetsAndReportKeyCallbacks()
        {
            var elementRoot = new RecordingContainer<object>();
            var keyRoot = new RecordingContainer<Type>();
            var collection = new InspectableSetsTypeCollection<object>(elementRoot, keyRoot);
            var foo = new FooItem();
            var bar = new BarItem();

            Assert.That(collection.TryAdd(typeof(FooKey), foo), Is.True);
            Assert.That(collection.TryAdd(typeof(BarKey), bar), Is.True);

            var seen = new List<object>();
            collection.ForEach(seen.Add);

            Assert.That(collection.Count, Is.EqualTo(2));
            Assert.That(collection.Contains(foo), Is.True);
            Assert.That(collection.Any(item => ReferenceEquals(item, bar)), Is.True);
            Assert.That(seen, Is.EqualTo(new object[] { foo, bar }));
            Assert.That(keyRoot.Added, Is.EqualTo(new[] { typeof(FooKey), typeof(BarKey) }));

            Assert.That(collection.Remove(foo), Is.True);
            collection.Clear(typeof(BarKey));

            Assert.That(collection.Count, Is.Zero);
            Assert.That(elementRoot.Removed, Is.EqualTo(new object[] { foo, bar }));
            Assert.That(keyRoot.Removed, Is.EqualTo(new[] { typeof(BarKey) }));
        }

        [Test]
        public void ClearKey_LeavesStaleEmptySetInSetList()
        {
            var collection = new InspectableSetsTypeCollection<object>();
            var firstSet = collection.GetOrCreate<FooKey>();
            collection.TryAdd(typeof(FooKey), new FooItem());

            collection.Clear(typeof(FooKey));
            var secondSet = collection.GetOrCreate<FooKey>();
            collection.TryAdd(typeof(FooKey), new BarItem());

            // TODO [RF-008]: Clear(keyType) removes the cache entry but leaves the empty set in SetsList.
            Assert.That(secondSet, Is.Not.SameAs(firstSet));
            Assert.That(collection.InternalSetCount, Is.EqualTo(2));
            Assert.That(collection.Count, Is.EqualTo(1));
        }

        private sealed class InspectableSetsTypeCollection<T> : SetsTypeCollection<T>
        {
            public InspectableSetsTypeCollection() { }

            public InspectableSetsTypeCollection(
                IContainer<T> elementRoot,
                IContainer<Type> keyRoot
            )
                : base(elementRoot, keyRoot) { }

            public int InternalSetCount => SetsList.Count;
        }

        private sealed class FooKey { }

        private sealed class BarKey { }

        private sealed class FooItem { }

        private sealed class BarItem { }
    }
}
