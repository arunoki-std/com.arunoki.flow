using System.Collections.Generic;
using NUnit.Framework;

namespace Arunoki.Flow.Tests
{
    public class StateMachineTests
    {
        [Test]
        public void Activate_UsesAutoDiscoveredDefaultRootAndDuplicateAddIsIgnored()
        {
            var context = new FlatContext();
            var machine = new StateMachine<FlatContext>(context);
            machine.AddState<FlatContext.Root>();

            machine.Activate();

            Assert.That(context.Log, Is.EqualTo(new[] { "Root.Enter" }));
            Assert.That(machine.IsActive<FlatContext.Root>(), Is.True);
        }

        [Test]
        public void Activate_WithoutDefaultRootThrows()
        {
            var machine = new StateMachine<NoDefaultContext>(new NoDefaultContext());

            Assert.That(() => machine.Activate(), Throws.TypeOf<StateMachineException>());
        }

        [Test]
        public void StateBaseLifecycle_StartsOnFirstUpdateAndExitResets()
        {
            var context = new FlatContext();
            var machine = new StateMachine<FlatContext>(context);
            machine.Activate();
            context.Log.Clear();

            machine.Update();
            machine.Update();
            machine.Deactivate();

            Assert.That(
                context.Log,
                Is.EqualTo(
                    new[]
                    {
                        "Root.Start",
                        "Root.Update:False",
                        "Root.Update:True",
                        "Root.Exit:False",
                    }
                )
            );
        }

        [Test]
        public void PendingTransition_AppliesOnNextUpdateAndSkipsActivePathUpdateThatFrame()
        {
            var context = new FlatContext();
            var machine = new StateMachine<FlatContext>(context);
            machine.Activate();
            context.Log.Clear();

            Assert.That(machine.GoTo<FlatContext.Second>(), Is.True);
            Assert.That(machine.HasChangeRequest(), Is.True);
            machine.Update();
            machine.Update();

            Assert.That(
                context.Log,
                Is.EqualTo(
                    new[]
                    {
                        "Root.Exit:False",
                        "Second.Enter",
                        "Second.Start",
                        "Second.Update:False",
                    }
                )
            );
        }

        [Test]
        public void GoTo_ActiveLeafCancelsExistingPendingRequest()
        {
            var context = new FlatContext();
            var machine = new StateMachine<FlatContext>(context);
            machine.Activate();
            context.Log.Clear();

            machine.GoTo<FlatContext.Second>();
            var changed = machine.GoTo<FlatContext.Root>();

            // TODO [RF-004]: GoTo<T>() clears pendingState before it checks the active leaf.
            Assert.That(changed, Is.False);
            Assert.That(machine.HasChangeRequest(), Is.False);

            machine.Update();

            Assert.That(context.Log, Is.EqualTo(new[] { "Root.Start", "Root.Update:False" }));
        }

        [Test]
        public void TryGoToUnknownReturnsFalseButLateAddedStateThrowsWhenApplied()
        {
            var context = new FlatContext();
            var machine = new StateMachine<FlatContext>(context);
            machine.Activate();

            Assert.That(machine.TryGoTo<UnknownState>(), Is.False);
            Assert.That(machine.HasChangeRequest(), Is.False);

            machine.AddState<LateState>();

            Assert.That(machine.TryGoTo<LateState>(), Is.True);
            Assert.That(() => machine.Update(), Throws.TypeOf<StateMachineException>());
        }

        [Test]
        public void GoToBeforeActivate_EntersRequestedPathInsteadOfDefaultRoot()
        {
            var context = new FlatContext();
            var machine = new StateMachine<FlatContext>(context);

            machine.GoTo<FlatContext.Second>();
            machine.Activate();

            Assert.That(context.Log, Is.EqualTo(new[] { "Second.Enter" }));
            Assert.That(machine.IsActiveLeaf<FlatContext.Second>(), Is.True);
        }

        [Test]
        public void DeactivateAndQueriesReflectActiveLeafAndAssignableTypes()
        {
            var context = new FlatContext();
            var machine = new StateMachine<FlatContext>(context);
            machine.Activate();
            machine.GoTo<FlatContext.Second>(pendingRequest: false);
            context.Log.Clear();

            Assert.That(machine.Contains<IFlatMarker>(), Is.True);
            Assert.That(machine.IsActive<IFlatMarker>(), Is.True);
            Assert.That(machine.IsActiveLeaf<FlatContext.Second>(), Is.True);
            Assert.That(machine.GetActiveLeaf(), Is.TypeOf<FlatContext.Second>());

            machine.Deactivate();

            Assert.That(machine.IsActive<FlatContext.Second>(), Is.False);
            Assert.That(context.Log, Is.EqualTo(new[] { "Second.Exit:False" }));
        }

        public interface IFlatMarker { }

        public sealed class FlatContext
        {
            public readonly List<string> Log = new();

            public sealed class Root : RecordingState, IFlatMarker
            {
                public Root()
                    : base("Root", isDefault: true) { }
            }

            public sealed class Second : RecordingState, IFlatMarker
            {
                public Second()
                    : base("Second") { }
            }
        }

        public sealed class NoDefaultContext
        {
            public sealed class Root : RecordingState<NoDefaultContext>
            {
                public Root()
                    : base("Root") { }
            }
        }

        public sealed class UnknownState : RecordingState
        {
            public UnknownState()
                : base("Unknown") { }
        }

        public sealed class LateState : RecordingState
        {
            public LateState()
                : base("Late") { }
        }

        public abstract class RecordingState : RecordingState<FlatContext>
        {
            protected RecordingState(string name, bool isDefault = false)
                : base(name, isDefault) { }
        }

        public abstract class RecordingState<TContext> : State<TContext>
            where TContext : class
        {
            private readonly string name;

            protected RecordingState(string name, bool isDefault = false)
                : base(isDefault)
            {
                this.name = name;
            }

            public override void OnEnter()
            {
                Log($"{name}.Enter");
            }

            public override void OnExit()
            {
                base.OnExit();
                Log($"{name}.Exit:{IsFirstUpdatePassed}");
            }

            public override void OnUpdate()
            {
                base.OnUpdate();
                Log($"{name}.Update:{IsFirstUpdatePassed}");
            }

            protected override void OnStart()
            {
                Log($"{name}.Start");
            }

            private void Log(string message)
            {
                if (Context is FlatContext flat)
                    flat.Log.Add(message);
            }
        }
    }
}
