using System;
using System.Collections.Generic;
using System.Reflection;
using Arunoki.Collections;
using Arunoki.Flow.Events;

namespace Arunoki.Flow.Tests
{
    internal sealed class RecordingContainer<T> : IContainer<T>
    {
        public readonly List<T> Added = new();
        public readonly List<T> Removed = new();

        public void OnAdded(T element) => Added.Add(element);

        public void OnRemoved(T element) => Removed.Add(element);
    }

    internal struct TestDomainEvent : IDomainEvent
    {
        public IFlowContext Context { get; set; }
    }

    internal struct OtherDomainEvent : IDomainEvent
    {
        public IFlowContext Context { get; set; }
    }

    internal struct MissingDomainEvent : IDomainEvent
    {
        public IFlowContext Context { get; set; }
    }

    internal struct StaticDomainEvent : IDomainEvent
    {
        public IFlowContext Context { get; set; }
    }

    internal struct AutoTriggerEvent : IDomainEvent
    {
        public IFlowContext Context { get; set; }
    }

    internal struct ManualTriggerEvent : IDomainEvent
    {
        public IFlowContext Context { get; set; }
    }

    internal struct TestIntValueEvent : IValueEvent<int>
    {
        public IFlowContext Context { get; set; }
        public int Current { get; set; }
        public int Previous { get; set; }
    }

    internal struct TestFloatValueEvent : IValueEvent<float>
    {
        public IFlowContext Context { get; set; }
        public float Current { get; set; }
        public float Previous { get; set; }
    }

    internal struct TestStringDataEvent : IDataEvent<string>
    {
        public IFlowContext Context { get; set; }
        public string Data { get; set; }
    }

    internal sealed class TestableEventBus : EventBus
    {
        public int ChannelCount
        {
            get
            {
                var count = 0;
                foreach (var _ in Channels)
                    count++;
                return count;
            }
        }

        public bool ContainsChannel<TEvent>()
            where TEvent : struct, IEvent => Channels.Contains(typeof(TEvent));

        public bool TryGetChannel<TEvent>(out Channel channel)
            where TEvent : struct, IEvent => Channels.TryGet(typeof(TEvent), out channel);

        public void AddChannel(Channel channel) => Add(channel);
    }

    internal sealed class TestSignal<TEvent> : Signal<TEvent>
        where TEvent : struct, IDomainEvent
    {
        public int CallbackCount => Callbacks.Count;

        public void SetContext(IFlowContext context) => ((IContextPart)this).Set(context);
    }

    internal sealed class TestTrigger<TEvent> : Trigger<TEvent>
        where TEvent : struct, IDomainEvent
    {
        public TestTrigger(bool autoReset = false)
            : base(autoReset) { }

        public int CallbackCount => Callbacks.Count;
    }

    internal sealed class TestIntProperty : ValueProperty<TestIntValueEvent, int>
    {
        public TestIntProperty(int defaultValue = default, bool autoReset = false)
            : base(defaultValue, autoReset) { }

        public int CallbackCount => Callbacks.Count;
    }

    internal sealed class TestProgressProperty : ProgressProperty<TestFloatValueEvent>
    {
        public TestProgressProperty(bool autoReset = false)
            : base(autoReset) { }
    }

    internal sealed class TestStringProxy : ProxyValue<TestStringDataEvent, string>
    {
        public TestStringProxy()
            : base() { }

        public TestStringProxy(string data, bool autoReset = false)
            : base(data, autoReset) { }
    }

    internal sealed class TestFlowContext : IFlowContext
    {
        public TestSignal<TestDomainEvent> Domain { get; } = new();
        public TestSignal<OtherDomainEvent> Other { get; } = new();
        public TestTrigger<AutoTriggerEvent> AutoTrigger { get; } = new(autoReset: true);
        public TestTrigger<ManualTriggerEvent> ManualTrigger { get; } = new();
        public TestIntProperty IntProperty { get; } = new();
        public string Ignored => "not a channel";
    }

    internal static class StaticEventSource
    {
        public static TestSignal<StaticDomainEvent> StaticSignal { get; } = new();

        public static void Reset()
        {
            StaticSignal.Clear();
            StaticSignal.SetContext(null);
        }
    }

    internal sealed class NonStaticEventSource
    {
        public static TestSignal<StaticDomainEvent> StaticSignal { get; } = new();
    }

    internal sealed class RecordingHandler : IFlowHandler
    {
        private readonly string name;
        private readonly List<string> log;

        public RecordingHandler(string name, List<string> log)
        {
            this.name = name;
            this.log = log;
        }

        public IFlowContext LastContext { get; private set; }
        public int DomainPublicCount { get; private set; }
        public int DomainPrivateCount { get; private set; }

        public void On(ref TestDomainEvent evt)
        {
            DomainPublicCount++;
            LastContext = evt.Context;
            log?.Add($"handler-{name}");
        }

        private void OnPrivate(ref TestDomainEvent evt)
        {
            DomainPrivateCount++;
            LastContext = evt.Context;
        }
    }

    internal sealed class MissingEventHandler : IFlowHandler
    {
        public int MissingCount { get; private set; }

        private void OnMissing(ref MissingDomainEvent evt)
        {
            MissingCount++;
        }
    }

    internal sealed class ConditionalHandler : IFlowConditionHandler
    {
        private readonly List<string> log;

        public ConditionalHandler(List<string> log)
        {
            this.log = log;
        }

        public bool IsHandlingEvents { get; set; }
        public int Count { get; private set; }

        private void On(ref TestDomainEvent evt)
        {
            Count++;
            log.Add("conditional");
        }
    }

    internal sealed class IncompatibleHandler : IFlowHandler
    {
        private int On(ref TestDomainEvent evt) => 0;
    }

    internal static class StaticRecordingHandler
    {
        public static int PublicCount { get; private set; }
        public static int PrivateCount { get; private set; }

        public static void Reset()
        {
            PublicCount = 0;
            PrivateCount = 0;
        }

        public static void OnPublic(ref StaticDomainEvent evt)
        {
            PublicCount++;
        }

        private static void OnPrivate(ref StaticDomainEvent evt)
        {
            PrivateCount++;
        }
    }

    internal static class ReflectionTest
    {
        public static MethodInfo[] Method<T>(string name) =>
            new[] { typeof(T).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic) };
    }
}
