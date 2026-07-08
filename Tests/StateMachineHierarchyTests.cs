using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Arunoki.Flow.Tests
{
    public class StateMachineHierarchyTests
    {
        [Test]
        public void Wiring_ThrowsWhenParentIsMissingOrDefaultChildIsDuplicated()
        {
            Assert.That(
                () => new StateMachine<MissingParentContext>(new MissingParentContext()).Activate(),
                Throws.TypeOf<StateMachineException>()
            );

            Assert.That(
                () =>
                    new StateMachine<DuplicateDefaultContext>(
                        new DuplicateDefaultContext()
                    ).Activate(),
                Throws.TypeOf<RewriteOperationException>()
            );
        }

        [Test]
        public void Activate_EntersRootThenDefaultChild()
        {
            var context = new HierarchyContext();
            var machine = new StateMachine<HierarchyContext>(context);

            machine.Activate();

            Assert.That(context.Log, Is.EqualTo(new[] { "Root.Enter", "ChildA.Enter" }));
            Assert.That(machine.IsActive<HierarchyContext.Root>(), Is.True);
            Assert.That(machine.IsActiveLeaf<HierarchyContext.ChildA>(), Is.True);
        }

        [Test]
        public void SiblingTransition_ExitsOnlyBelowCommonAncestorAndEntersTarget()
        {
            var context = new HierarchyContext();
            var machine = new StateMachine<HierarchyContext>(context);
            machine.Activate();
            context.Log.Clear();

            machine.GoTo<HierarchyContext.ChildB>(pendingRequest: false);

            Assert.That(context.Log, Is.EqualTo(new[] { "ChildA.Exit", "ChildB.Enter" }));
            Assert.That(machine.IsActive<HierarchyContext.Root>(), Is.True);
            Assert.That(machine.IsActiveLeaf<HierarchyContext.ChildB>(), Is.True);
        }

        [Test]
        public void TransitionToAnotherRoot_ExitsOldPathAndUpdatesCurrentRoot()
        {
            var context = new HierarchyContext();
            var machine = new StateMachine<HierarchyContext>(context);
            machine.Activate();
            context.Log.Clear();

            machine.GoTo<HierarchyContext.OtherRoot>(pendingRequest: false);

            Assert.That(
                context.Log,
                Is.EqualTo(new[] { "ChildA.Exit", "Root.Exit", "OtherRoot.Enter" })
            );
            Assert.That(machine.IsActive<HierarchyContext.Root>(), Is.False);
            Assert.That(machine.IsActive<HierarchyContext.OtherRoot>(), Is.True);
        }

        [Test]
        public void Update_UpdatesActivePathParentToLeaf()
        {
            var context = new HierarchyContext();
            var machine = new StateMachine<HierarchyContext>(context);
            machine.Activate();
            context.Log.Clear();

            machine.Update();

            Assert.That(
                context.Log,
                Is.EqualTo(new[] { "Root.Start", "Root.Update", "ChildA.Start", "ChildA.Update" })
            );
        }

        public sealed class HierarchyContext
        {
            public readonly List<string> Log = new();

            public sealed class Root : RecordingState
            {
                public Root()
                    : base("Root", isDefault: true) { }
            }

            public sealed class OtherRoot : RecordingState
            {
                public OtherRoot()
                    : base("OtherRoot") { }
            }

            public sealed class ChildA : RecordingState
            {
                public ChildA()
                    : base("ChildA", isDefault: true, parentState: typeof(Root)) { }
            }

            public sealed class ChildB : RecordingState
            {
                public ChildB()
                    : base("ChildB", parentState: typeof(Root)) { }
            }
        }

        public sealed class MissingParentContext
        {
            public sealed class Root : RecordingState<MissingParentContext>
            {
                public Root()
                    : base("Root", isDefault: true) { }
            }

            public sealed class Orphan : RecordingState<MissingParentContext>
            {
                public Orphan()
                    : base("Orphan", isDefault: true, parentState: typeof(NotRegistered)) { }
            }

            public sealed class NotRegistered { }
        }

        public sealed class DuplicateDefaultContext
        {
            public sealed class Root : RecordingState<DuplicateDefaultContext>
            {
                public Root()
                    : base("Root", isDefault: true) { }
            }

            public sealed class ChildA : RecordingState<DuplicateDefaultContext>
            {
                public ChildA()
                    : base("ChildA", isDefault: true, parentState: typeof(Root)) { }
            }

            public sealed class ChildB : RecordingState<DuplicateDefaultContext>
            {
                public ChildB()
                    : base("ChildB", isDefault: true, parentState: typeof(Root)) { }
            }
        }

        public abstract class RecordingState : RecordingState<HierarchyContext>
        {
            protected RecordingState(string name, bool isDefault = false, Type parentState = null)
                : base(name, isDefault, parentState) { }
        }

        public abstract class RecordingState<TContext> : State<TContext>
            where TContext : class
        {
            private readonly string name;

            protected RecordingState(string name, bool isDefault = false, Type parentState = null)
                : base(isDefault, parentState)
            {
                this.name = name;
            }

            public override void OnEnter()
            {
                Log($"{name}.Enter");
            }

            public override void OnExit()
            {
                Log($"{name}.Exit");
                base.OnExit();
            }

            public override void OnUpdate()
            {
                base.OnUpdate();
                Log($"{name}.Update");
            }

            protected override void OnStart()
            {
                Log($"{name}.Start");
            }

            private void Log(string message)
            {
                if (Context is HierarchyContext hierarchy)
                    hierarchy.Log.Add(message);
            }
        }
    }
}
