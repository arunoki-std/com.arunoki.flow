using System;
using System.Collections.Generic;
using System.Reflection;
using Arunoki.Flow.Globals;
using Arunoki.Flow.Tests.BootstrapCctor;
using Arunoki.Flow.Tests.BootstrapData;
using Arunoki.Flow.Tests.BootstrapData.Nested;
using NUnit.Framework;

namespace Arunoki.Flow.Tests
{
    public class StaticBootstrapTests
    {
        private static Assembly TestAssembly => typeof(StaticBootstrapTests).Assembly;

        // RunClassConstructor runs a cctor at most once per domain, so this test must run
        // before the collect-all tests below (which also collect CctorProbe). [Order] keeps
        // the before/after assertion meaningful within this fixture.
        [Test]
        [Order(1)]
        public void Init_RunsClassConstructorsOfCollectedStaticClasses()
        {
            // The side-effect log lives OUTSIDE the probe class: touching a static member of
            // the probe itself would trigger its cctor and void the test. RunClassConstructor
            // is once-per-domain, so on a suite re-run without a domain reload the effect
            // cannot be re-observed — skip instead of failing.
            if (BootstrapSideEffects.Log.Contains(nameof(CctorProbe)))
                Assert.Ignore(
                    "CctorProbe's cctor already ran in this domain (suite re-run without a "
                        + "domain reload); reload the domain to re-observe."
                );

            var bootstrap = new StaticBootstrap(TestAssembly, "Arunoki.Flow.Tests.BootstrapCctor");

            Assert.That(bootstrap.GetTypes(), Is.EqualTo(new[] { typeof(CctorProbe) }));
            Assert.That(BootstrapSideEffects.Log, Contains.Item(nameof(CctorProbe)));
        }

        [Test]
        [Order(2)]
        public void Init_CollectsOnlyStaticClassesAndNamespaceMatchIsExact()
        {
            var bootstrap = new StaticBootstrap(TestAssembly, "Arunoki.Flow.Tests.BootstrapData");

            var types = bootstrap.GetTypes();
            Assert.That(types, Contains.Item(typeof(OuterStatic)));
            // Non-static class in the matching namespace is skipped.
            Assert.That(types, Has.No.Member(typeof(NotStatic)));
            // Namespace match is EXACT: X.Y.Z is SILENTLY skipped when filtering for X.Y —
            // the documented silent-skip hazard (docs/flow.md §5 / AGENTS.md). Captured.
            Assert.That(types, Has.No.Member(typeof(NestedStatic)));
        }

        [Test]
        [Order(3)]
        public void Init_WithoutFilterCollectsAllStaticClassesOfTheAssembly()
        {
            var bootstrap = new StaticBootstrap(TestAssembly);

            var types = bootstrap.GetTypes();
            // Superset across namespaces: bootstrap fixtures AND unrelated test doubles.
            Assert.That(types, Contains.Item(typeof(OuterStatic)));
            Assert.That(types, Contains.Item(typeof(NestedStatic)));
            Assert.That(types, Contains.Item(typeof(CctorProbe)));
            Assert.That(types, Contains.Item(typeof(StaticEventSource)));
            Assert.That(types, Has.No.Member(typeof(NotStatic)));

            // Quirk — capture, don't fix: an EMPTY namespace list also means "no filter"
            // (the Count > 0 guard), not "match nothing".
            var emptyFilter = new StaticBootstrap();
            emptyFilter.Init(TestAssembly, new List<string>());
            Assert.That(emptyFilter.GetTypes(), Is.EqualTo(types));
        }

        [Test]
        [Order(4)]
        public void GetTypesAndGetEnumerator_ExposeTheCollectedList()
        {
            var bootstrap = new StaticBootstrap(TestAssembly, "Arunoki.Flow.Tests.BootstrapData");

            var enumerated = new List<Type>();
            foreach (var type in bootstrap)
                enumerated.Add(type);

            Assert.That(enumerated, Is.EqualTo(bootstrap.GetTypes()));
            Assert.That(enumerated, Is.EqualTo(new[] { typeof(OuterStatic) }));
        }
    }

    /// Side-effect sink for cctor observation; static on purpose (also gets collected by the
    /// no-filter test — harmless).
    internal static class BootstrapSideEffects
    {
        public static readonly List<string> Log = new();
    }
}

namespace Arunoki.Flow.Tests.BootstrapData
{
    internal static class OuterStatic { }

    internal sealed class NotStatic { }
}

namespace Arunoki.Flow.Tests.BootstrapData.Nested
{
    internal static class NestedStatic { }
}

namespace Arunoki.Flow.Tests.BootstrapCctor
{
    internal static class CctorProbe
    {
        static CctorProbe()
        {
            Tests.BootstrapSideEffects.Log.Add(nameof(CctorProbe));
        }
    }
}
