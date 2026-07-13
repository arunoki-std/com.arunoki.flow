using System;
using System.Collections.Generic;
using System.Reflection;
using Arunoki.Flow.Basics;
using Arunoki.Flow.Globals;
using NUnit.Framework;
using UnityEngine;

namespace Arunoki.Flow.Tests
{
    public class GlobalHubTests
    {
        // RF-005: GlobalHub.Instance is set-once with NO reset API; _isReady and OnReady are
        // also static. Without this reflection reset a second `new GlobalHub(...)` anywhere in
        // the test run throws InvalidOperationException, and OnReady/_isReady leak across
        // tests, making them order-dependent and unrepeatable. This block is direct evidence
        // for RF-005 (reduce static coupling); see Specs~/RF-004-hub-tests.md.
        private static readonly FieldInfo InstanceField = typeof(GlobalHub).GetField(
            "<Instance>k__BackingField",
            BindingFlags.NonPublic | BindingFlags.Static
        );
        private static readonly FieldInfo IsReadyField = typeof(GlobalHub).GetField(
            "_isReady",
            BindingFlags.NonPublic | BindingFlags.Static
        );
        private static readonly FieldInfo OnReadyField = typeof(GlobalHub).GetField(
            "OnReady",
            BindingFlags.NonPublic | BindingFlags.Static
        );

        [SetUp]
        public void ResetGlobalHubStatics()
        {
            Assert.That(InstanceField, Is.Not.Null, "GlobalHub.Instance backing field moved?");
            Assert.That(IsReadyField, Is.Not.Null, "GlobalHub._isReady moved?");
            Assert.That(OnReadyField, Is.Not.Null, "GlobalHub.OnReady backing field moved?");

            ResetStatics();
        }

        [TearDown]
        public void DestroyMainFlowObjectsAndResetStatics()
        {
            // The ctor creates a "Main.Flow" GameObject that would leak into other tests.
            // In EditMode the ctor aborts BEFORE AddComponent<RoutineHelper> (see CreateHub),
            // so sweep scene objects by name instead of by component.
            foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
                if (go != null && go.name == "Main.Flow" && go.scene.IsValid())
                    UnityEngine.Object.DestroyImmediate(go);

            ResetStatics();
        }

        private static void ResetStatics()
        {
            InstanceField.SetValue(null, null);
            IsReadyField.SetValue(null, false);
            OnReadyField.SetValue(null, null);
        }

        // RF-005 finding: in this Unity version DontDestroyOnLoad THROWS in EditMode
        // (InvalidOperationException, "...can only be used in play mode..."), so the GlobalHub
        // ctor NEVER completes in EditMode tests. By the time it throws, Instance is already
        // set (set-once, no rollback), so tests recover the half-built hub from the static.
        // The throw happens before AddComponent<RoutineHelper>: GetRoutine() stays null and
        // the "Main.Flow" GameObject exists without its helper. Captured, not fixed.
        private static GlobalHub CreateHub()
        {
            Assert.That(
                () => new GlobalHub(),
                Throws
                    .TypeOf<InvalidOperationException>()
                    .With.Message.Contains("DontDestroyOnLoad")
            );

            var hub = GlobalHub.Instance;
            Assert.That(hub, Is.Not.Null, "failed ctor should still have set Instance");
            return hub;
        }

        [Test]
        public void Ctor_AbortsInEditModeButLeavesInstanceSetAndSecondCtorThrows()
        {
            Assert.That(GlobalHub.Instance, Is.Null);
            Assert.That(GlobalHub.IsAssemblyInitialized, Is.False);

            // RF-005: the aborted ctor leaves a half-initialized global singleton behind.
            var hub = CreateHub();

            Assert.That(GlobalHub.Instance, Is.SameAs(hub));
            Assert.That(GlobalHub.IsAssemblyInitialized, Is.True);

            Assert.That(
                () => new GlobalHub(),
                Throws.TypeOf<InvalidOperationException>().With.Message.Contains("already created")
            );
            Assert.That(GlobalHub.Instance, Is.SameAs(hub));
        }

        [Test]
        public void OnReady_FiresOnceOnFirstActivateAndLateSubscriberNeverFires()
        {
            var hub = CreateHub();
            var readyCount = 0;
            GlobalHub.OnReady += () => readyCount++;

            hub.Activate();

            Assert.That(readyCount, Is.EqualTo(1));
            // RF-005: the delegate is nulled after the first activation, so any subscriber
            // added later is silently lost — captured, not fixed.
            Assert.That(OnReadyField.GetValue(null), Is.Null);

            var lateCount = 0;
            GlobalHub.OnReady += () => lateCount++;
            hub.Deactivate();
            hub.Activate();

            Assert.That(readyCount, Is.EqualTo(1));
            Assert.That(lateCount, Is.Zero);
        }

        [Test]
        public void GetRoutine_ReturnsNullInEditModeBecauseCtorAbortsBeforeAddComponent()
        {
            var hub = CreateHub();

            Assert.That(hub.GetRoutine(), Is.Null);

            // The GameObject is created and configured before the DontDestroyOnLoad throw...
            GameObject mainFlow = null;
            foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
                if (go != null && go.name == "Main.Flow" && go.scene.IsValid())
                    mainFlow = go;

            Assert.That(mainFlow, Is.Not.Null);
            Assert.That(mainFlow.hideFlags, Is.EqualTo(HideFlags.NotEditable));
            // ...but the RoutineHelper is never attached.
            Assert.That(mainFlow.GetComponent<RoutineHelper>(), Is.Null);
        }

        [Test]
        public void Init_RegistersBootstrapTypesIntoManagersAndActivatesHub()
        {
            var hub = CreateHub();
            // List-backed ctor: the bootstrap exposes exactly these types, no cctor runs here.
            var bootstrap = new StaticBootstrap(new List<Type> { typeof(HubManagedStatic) });

            var result = GlobalHub.Init(hub, bootstrap);

            Assert.That(result, Is.SameAs(hub));
            Assert.That(hub.IsActive(), Is.True);

            var managed = new List<Type>();
            foreach (var type in hub.Managers)
                managed.Add(type);
            Assert.That(managed, Is.EqualTo(new[] { typeof(HubManagedStatic) }));
        }
    }
}
