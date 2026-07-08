using System.Collections.Generic;
using Arunoki.Collections;
using NUnit.Framework;

namespace Arunoki.Flow.Tests
{
    public class SetTests
    {
        [Test]
        public void TryAdd_RejectsDuplicatesNullsAndReportsContainerCallbacks()
        {
            var root = new RecordingContainer<string>();
            var set = new Set<string>(root);

            Assert.That(set.TryAdd("first"), Is.True);
            Assert.That(set.TryAdd("first"), Is.False);
            Assert.That(set.TryAdd(null), Is.False);

            Assert.That(set.Count, Is.EqualTo(1));
            Assert.That(root.Added, Is.EqualTo(new[] { "first" }));

            Assert.That(set.Remove("first"), Is.True);
            Assert.That(root.Removed, Is.EqualTo(new[] { "first" }));

            var positives = new Set<int>(value => value > 0);
            Assert.That(positives.TryAdd(-1), Is.False);
            Assert.That(positives.TryAdd(1), Is.True);
        }

        [Test]
        public void Ordering_UsesOldestFirstIndexerAndForEachButRawListIsNewestFirst()
        {
            var set = new Set<string>();
            set.AddRange("oldest", "middle", "newest");

            var forEachOrder = new List<string>();
            set.ForEach(forEachOrder.Add);

            Assert.That(set[0], Is.EqualTo("oldest"));
            Assert.That(forEachOrder, Is.EqualTo(new[] { "oldest", "middle", "newest" }));

            // TODO [RF-008]: GetList exposes raw newest-first storage despite its doc comment.
            Assert.That(set.GetList(), Is.EqualTo(new[] { "newest", "middle", "oldest" }));
        }

        [Test]
        public void ForEach_ToleratesRemovingTheCurrentElement()
        {
            var set = new Set<string>();
            set.AddRange("oldest", "middle", "newest");
            var seen = new List<string>();

            set.ForEach(item =>
            {
                seen.Add(item);
                if (item == "middle")
                    set.Remove(item);
            });

            Assert.That(seen, Is.EqualTo(new[] { "oldest", "middle", "newest" }));
            Assert.That(set.Contains("middle"), Is.False);
            Assert.That(set.Count, Is.EqualTo(2));
        }

        [Test]
        public void RemoveOperationsAndClear_ReportRemovedElements()
        {
            var root = new RecordingContainer<string>();
            var set = new Set<string>(root);
            set.AddRange("one", "two", "three", "four");

            set.RemoveWhere(value => value.Length == 3);

            Assert.That(set.Contains("one"), Is.False);
            Assert.That(set.Contains("two"), Is.False);
            Assert.That(set.Count, Is.EqualTo(2));

            Assert.That(set.Remove("three"), Is.True);
            Assert.That(set.RemoveAt(10), Is.False);

            set.Clear();

            Assert.That(set.Count, Is.Zero);
            Assert.That(root.Removed, Is.EqualTo(new[] { "one", "two", "three", "four" }));
        }
    }
}
