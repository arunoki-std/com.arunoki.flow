using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Arunoki.Flow.Events;
using Arunoki.Flow.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Arunoki.Flow.Tests
{
    public class EventBusTests
    {
        [TearDown]
        public void ResetStaticDoubles()
        {
            StaticEventSource.Reset();
            StaticRecordingHandler.Reset();
        }

        [Test]
        public void RegisterSource_ContextCollectsPublicChannelPropertiesAndAssignsContext()
        {
            var bus = new TestableEventBus();
            var context = new TestFlowContext();

            bus.RegisterSource(context);

            Assert.That(bus.ContainsChannel<TestDomainEvent>(), Is.True);
            Assert.That(bus.ContainsChannel<OtherDomainEvent>(), Is.True);
            Assert.That(context.Domain.Context, Is.SameAs(context));

            if (Utils.IsDebug())
            {
                Assert.That(
                    () => context.Domain.SetContext(new TestFlowContext()),
                    Throws.TypeOf<InvalidOperationException>()
                );
            }
            else
            {
                Assert.That(() => context.Domain.SetContext(new TestFlowContext()), Throws.Nothing);
            }
        }

        [Test]
        public void RegisterSource_StaticTypesWrapContextAndUnregisterBySource()
        {
            var bus = new TestableEventBus();

            bus.RegisterSource(typeof(StaticEventSource));

            Assert.That(bus.ContainsChannel<StaticDomainEvent>(), Is.True);
            Assert.That(StaticEventSource.StaticSignal.Context, Is.Not.Null);
            Assert.That(
                StaticEventSource.StaticSignal.Context.GetType().Name,
                Is.EqualTo("StaticContextWrapper")
            );

            bus.UnregisterSource(typeof(StaticEventSource));

            Assert.That(bus.ContainsChannel<StaticDomainEvent>(), Is.False);
            Assert.That(StaticEventSource.StaticSignal.CallbackCount, Is.Zero);

            if (Utils.IsDebug())
            {
                Assert.That(
                    () => bus.RegisterSource(typeof(NonStaticEventSource)),
                    Throws.TypeOf<InvalidOperationException>()
                );
            }
            else
            {
                Assert.That(() => bus.RegisterSource(typeof(NonStaticEventSource)), Throws.Nothing);
            }
        }

        [Test]
        public void UnregisterSource_ContextRemovesOnlyMatchingChannels()
        {
            var bus = new TestableEventBus();
            var context = new TestFlowContext();
            bus.RegisterSource(context);
            bus.RegisterSource(typeof(StaticEventSource));

            bus.UnregisterSource(context);

            Assert.That(bus.ContainsChannel<TestDomainEvent>(), Is.False);
            Assert.That(bus.ContainsChannel<OtherDomainEvent>(), Is.False);
            Assert.That(bus.ContainsChannel<StaticDomainEvent>(), Is.True);
        }

        [Test]
        public void Subscribe_InstanceHandlerMatchesPublicAndNonPublicRefEventMethods()
        {
            var bus = new TestableEventBus();
            var context = new TestFlowContext();
            var handler = new RecordingHandler("one", null);
            bus.RegisterSource(context);

            var callbacks = bus.Subscribe(handler);
            context.Domain.Emit();

            Assert.That(callbacks.Count, Is.EqualTo(1));
            Assert.That(handler.DomainPublicCount, Is.EqualTo(1));
            Assert.That(handler.DomainPrivateCount, Is.EqualTo(1));
        }

        [Test]
        public void Subscribe_WhenEventChannelIsMissingSkipsHandlerMethod()
        {
            var bus = new TestableEventBus();
            var context = new TestFlowContext();
            var handler = new MissingEventHandler();
            bus.RegisterSource(context);

            if (Utils.IsTraceable())
            {
                LogAssert.Expect(
                    LogType.Warning,
                    new Regex(
                        "Event hub does not contain any channel capable of handling "
                            + "'Arunoki.Flow.Tests.MissingDomainEvent'"
                    )
                );
            }

            var callbacks = bus.Subscribe(handler);

            Assert.That(callbacks.Count, Is.Zero);
            Assert.That(handler.MissingCount, Is.Zero);
        }

        [Test]
        public void Subscribe_StaticHandlerCurrentlyIgnoresPublicStaticMethods()
        {
            var bus = new TestableEventBus();
            bus.RegisterSource(typeof(StaticEventSource));

            var callbacks = bus.Subscribe(typeof(StaticRecordingHandler));
            StaticEventSource.StaticSignal.Emit();

            // TODO [RF-004/RF-005]: static handlers bind NonPublic only; public static methods are skipped.
            Assert.That(callbacks.Count, Is.EqualTo(1));
            Assert.That(StaticRecordingHandler.PrivateCount, Is.EqualTo(1));
            Assert.That(StaticRecordingHandler.PublicCount, Is.Zero);
        }

        [Test]
        public void Subscribe_DuplicateAndWrongSignaturesFollowDebugGuards()
        {
            var bus = new TestableEventBus();
            var context = new TestFlowContext();
            var handler = new RecordingHandler("one", null);
            bus.RegisterSource(context);

            bus.Subscribe(handler);

            if (Utils.IsDebug())
            {
                Assert.That(
                    () => bus.Subscribe(handler),
                    Throws.TypeOf<MultipleEventSubscriptionException>()
                );
            }
            else
            {
                Assert.That(() => bus.Subscribe(handler), Throws.Nothing);
            }

            Assert.That(
                () => bus.Subscribe(new IncompatibleHandler()),
                Throws.TypeOf<IncompatibleEventHandlerException<TestDomainEvent>>()
            );
        }

        [Test]
        public void Publish_InvokesHandlersInSubscriptionOrderThenDelegatesAndAppliesContext()
        {
            var bus = new TestableEventBus();
            var context = new TestFlowContext();
            var log = new List<string>();
            var first = new RecordingHandler("first", log);
            var second = new RecordingHandler("second", log);
            var skipped = new ConditionalHandler(log) { IsHandlingEvents = false };
            bus.RegisterSource(context);
            bus.Subscribe(first);
            bus.Subscribe(second);
            bus.Subscribe(skipped);
            context.Domain.OnEvent += (ref TestDomainEvent evt) => log.Add("delegate");

            context.Domain.Emit();

            Assert.That(log, Is.EqualTo(new[] { "handler-first", "handler-second", "delegate" }));
            Assert.That(skipped.Count, Is.Zero);
            Assert.That(first.LastContext, Is.SameAs(context));
            Assert.That(second.LastContext, Is.SameAs(context));
        }

        [Test]
        public void UnsubscribeResetAndClearAllAffectRegisteredChannels()
        {
            var bus = new TestableEventBus();
            var context = new TestFlowContext();
            var handler = new RecordingHandler("one", null);
            bus.RegisterSource(context);
            bus.Subscribe(handler);

            bus.Unsubscribe(handler);

            Assert.That(context.Domain.CallbackCount, Is.Zero);

            context.AutoTrigger.Fire();
            context.ManualTrigger.Fire();
            bus.Reset();

            Assert.That(context.AutoTrigger.IsTriggered, Is.False);
            Assert.That(context.ManualTrigger.IsTriggered, Is.True);

            bus.Subscribe(handler);
            bus.ClearAll();

            Assert.That(bus.ChannelCount, Is.Zero);
            Assert.That(context.Domain.CallbackCount, Is.Zero);
        }
    }
}
