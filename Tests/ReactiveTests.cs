using System.Collections.Generic;
using Arunoki.Flow.Events;
using NUnit.Framework;

namespace Arunoki.Flow.Tests
{
    public class ReactiveTests
    {
        [Test]
        public void ValueProperty_PublishesOnlyOnChangeUnlessForcedAndResetDoesNotPublish()
        {
            var property = new TestIntProperty(defaultValue: 7);
            var events = new List<TestIntValueEvent>();
            property.OnEvent += (ref TestIntValueEvent evt) => events.Add(evt);

            property.Set(5);
            property.Set(5);
            property.Force(5);
            property.Reset();

            Assert.That(events.Count, Is.EqualTo(2));
            Assert.That(events[0].Current, Is.EqualTo(5));
            Assert.That(events[0].Previous, Is.Zero);
            Assert.That(events[1].Current, Is.EqualTo(5));
            Assert.That(events[1].Previous, Is.Zero);
            Assert.That(property.Value, Is.EqualTo(7));
            Assert.That(property.Previous, Is.EqualTo(7));
        }

        [Test]
        public void ValueProperty_ClearDropsSubscribersAndObservableUpdateRunsAfterCallbacks()
        {
            var bus = new TestableEventBus();
            var context = new TestFlowContext();
            var log = new List<string>();
            var handler = new ValueHandler(log);
            IObservableEventChannel<int> observable = context.IntProperty;
            observable.OnUpdated += (_, value) => log.Add($"updated:{value}");
            bus.RegisterSource(context);
            bus.Subscribe(handler);

            context.IntProperty.Set(1);
            context.IntProperty.Clear();
            context.IntProperty.Set(2);

            Assert.That(log, Is.EqualTo(new[] { "handler:1", "updated:1" }));
            Assert.That(context.IntProperty.CallbackCount, Is.Zero);
            Assert.That(context.IntProperty.Value, Is.EqualTo(2));
        }

        [Test]
        public void ProgressProperty_ClampsBeforeComparingAndReportsReadiness()
        {
            var property = new TestProgressProperty();
            var events = 0;
            property.OnEvent += (ref TestFloatValueEvent evt) => events++;

            property.Set(1.5f);
            property.Set(2.0f);

            Assert.That(property.Value, Is.EqualTo(1.0f));
            Assert.That(property.IsReady(), Is.True);
            Assert.That(events, Is.EqualTo(1));
        }

        [Test]
        public void Trigger_FiresOnceUntilReloadAndHonorsEventBusAutoReset()
        {
            var bus = new TestableEventBus();
            var context = new TestFlowContext();
            var events = 0;
            context.AutoTrigger.OnEvent += (ref AutoTriggerEvent evt) => events++;
            bus.RegisterSource(context);

            context.AutoTrigger.Fire();
            context.AutoTrigger.Fire();
            context.AutoTrigger.Reload();
            context.AutoTrigger.Fire();

            Assert.That(events, Is.EqualTo(2));
            Assert.That((bool)context.AutoTrigger, Is.True);

            bus.Reset();

            Assert.That(context.AutoTrigger.IsTriggered, Is.False);
        }

        [Test]
        public void Signal_EmitPublishes()
        {
            var signal = new TestSignal<TestDomainEvent>();
            var count = 0;
            signal.OnEvent += (ref TestDomainEvent evt) => count++;

            signal.Emit();

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void ProxyValue_PublishUpdatesDataBeforeEventAndResetRestoresInitialState()
        {
            var proxy = new TestStringProxy("initial");
            var eventData = new List<string>();
            var dataDuringEvent = new List<string>();
            proxy.OnEvent += (ref TestStringDataEvent evt) =>
            {
                eventData.Add(evt.Data);
                dataDuringEvent.Add(proxy.Data);
            };

            proxy.Publish("next");
            proxy.Reset();

            Assert.That(eventData, Is.EqualTo(new[] { "next" }));
            Assert.That(dataDuringEvent, Is.EqualTo(new[] { "next" }));
            Assert.That(proxy.Data, Is.EqualTo("initial"));
            Assert.That(proxy.IsNotEmpty, Is.True);

            var emptyProxy = new TestStringProxy();
            emptyProxy.Publish("value");
            emptyProxy.Reset();

            Assert.That(emptyProxy.Data, Is.Null);
            Assert.That(emptyProxy.IsNotEmpty, Is.False);
        }

        private sealed class ValueHandler : IFlowHandler
        {
            private readonly List<string> log;

            public ValueHandler(List<string> log)
            {
                this.log = log;
            }

            private void On(ref TestIntValueEvent evt)
            {
                log.Add($"handler:{evt.Current}");
            }
        }
    }
}
