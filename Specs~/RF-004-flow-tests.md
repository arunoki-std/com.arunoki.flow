# RF-004 (flow): Characterization tests for EventBus, StateMachine, Reactive, Collections

- Status: planned
- Scope: this package only — populate `Tests/` + fix its asmdef; no `Runtime/` changes unless a test is blocked (document each case)
- Risk: low
- Host spec: `Hillybombs/specs/refactoring/RF-004-test-infrastructure.md` (flow is step 2, after poolables)
- Sibling precedent: `com.arunoki.poolables/Specs~/RF-004-poolables-tests.md` (done, 19 tests) — follow its style: capture CURRENT behavior, including bugs, with comments; do not fix.

## Problem

Zero tests. RF-005 (unstatic `GlobalHub`), RF-008 (collections migration) and RF-001/003 host work
have no safety net. Package passport: `Hillybombs/docs/packages/com.arunoki.flow.md`.

## Infrastructure fix (do first)

`Tests/Arunoki.Flow.Tests.asmdef` already exists but is a broken placeholder:
`excludePlatforms: ["Editor"]` (the exact opposite of EditMode) and no test-framework reference.
Bring it to parity with `Arunoki.Poolables.Tests`:

- `includePlatforms: ["Editor"]`, `excludePlatforms: []`
- `optionalUnityReferences: ["TestAssemblies"]`
- keep the existing reference GUID (`Arunoki.Flow`); do not touch the asmdef `.meta`.

No `InternalsVisibleTo` in Runtime and none should be added. Access strategy:

- `EventBus.Channels`, `Channel.Subscribe/Publish/Add` are `protected internal` → reachable from
  test SUBCLASSES (`protected` half works across assemblies). Prefer small test subclasses
  (`TestableEventBus`, or drive `Channel<T>` via public `Signal.Emit`/`Trigger.Fire`/
  `ValueProperty.Set`) over reflection; reflection only as a last resort (poolables used it once).
- `StateNode` is `internal sealed` → test `StateMachine` through its public API only, with test
  states that record `OnEnter/OnExit/OnUpdate/OnStart` calls into a shared `List<string>` log.
  Asserting on the log IS the characterization of transition order.
- Debug-only throws are gated by `Utils.IsDebug()` (public) — branch assertions on it, as
  poolables did (in-editor test runs have it true; don't hardcode).

Everything under test is plain CLR — no GameObjects, no PlayMode, no scene teardown issues.
Only static shared state: the accessor cache inside `EventBusUtility` (read-only, harmless).

## Test areas (current behavior, quirks included)

### 1. Collections — `Set<T>` (`SetTests`)

Foundation for EventBus; ex `com.arunoki.collections`, merged per host RF-007. RF-008 plans to
replace these types — these tests are the safety net for that migration.

1. `TryAdd`: rejects duplicates (returns false) and nulls (default `IsConsumable` is
   `element is not null`); custom `consumablePredicate` respected; `IContainer` callbacks
   `OnAdded`/`OnRemoved` fire.
2. Ordering quirk — capture, don't fix: internal storage is `Insert(0)` (newest first), but the
   public indexer reverses (`set[0]` = OLDEST); `ForEach` iterates oldest→newest; **`GetList()`
   returns the raw internal list (newest first) while its doc comment claims "oldest to newest"**
   — assert actual order with a comment linking `// TODO [RF-008]`.
3. Mutation during iteration: `ForEach` tolerates removal of the CURRENT element (the guarded
   index check) — this is the one feature RF-008 says BCL lacks; pin it.
4. `RemoveWhere`, `Remove`, `RemoveAt` (out-of-range returns false), `Clear` fires
   `OnRemoved` per element.

### 2. Collections — `SetsTypeCollection<T>` (`SetsTypeCollectionTests`)

1. `GetOrCreate` caches by type; `Get<T>` on a missing key throws `KeyNotFoundException`;
   `TryGet`.
2. `Count` sums across sets; `Contains`/`Remove`/`Any`/`ForEach` span all sets;
   `rootKeyContainer.OnAdded/OnRemoved` fire on key create/clear.
3. Stale-entry bug — capture, don't fix: `Clear(keyType)` removes the set from `SetsCache` but
   **leaves it in `SetsList`**; a following `GetOrCreate` for the same key appends a SECOND set
   to `SetsList`. Assert the observable consequences (e.g. `Count` still correct because the
   stale set is empty) with a comment; candidate fix belongs to RF-008.

### 3. EventBus + Channel (`EventBusTests`)

Test events: local `struct`s implementing `IEvent`/`IDomainEvent`; contexts: plain-CLR
`IFlowContext` fakes exposing public `Channel` properties; handlers: classes with
`void On(ref TEvent evt)`-shaped methods.

1. `RegisterSource(IFlowContext)`: public instance `Channel` properties get collected; channel
   `Context` is set; re-setting a non-null `Context` throws `InvalidOperationException`
   (debug only).
2. `RegisterSource(Type)`: static source wrapped in `StaticContextWrapper`; non-static type
   throws `InvalidOperationException` (debug only); `UnregisterSource(Type)` removes exactly
   that source's channels; `UnregisterSource(IFlowContext)` likewise by `Equals`.
3. `Subscribe(IFlowHandler)` reflection contract: matches methods with a SINGLE `ref TEvent`
   parameter, instance public AND non-public; returns one `Callback` per matched channel;
   events without a registered channel are silently skipped (warning only under
   `ARUNOKI_TRACE`).
4. Static-handler asymmetry — capture, don't fix: `Subscribe(Type)` binds
   `Static | NonPublic` only, so **public static handler methods are silently ignored**
   (while static event SOURCES are scanned `Public | Static`). Pin with a comment.
5. Duplicate subscription of the same target to the same channel throws
   `MultipleEventSubscriptionException` (debug only); handler method with a wrong signature
   picked into a group throws `IncompatibleEventHandlerException` at `Callback` construction.
6. Publish semantics (drive via `Signal<T>.Emit`): `IFlowHandler` callbacks fire in
   subscription order (list is `Insert(0)` + reverse iteration), THEN `OnEvent` delegates;
   `IFlowConditionHandler.IsHandlingEvents == false` skips the callback; event instance
   carries the channel's `Context`.
7. `Unsubscribe` removes all of a handler's callbacks across channels; `Reset()` resets only
   channels that are `IResettable` with `AutoReset() == true`; `ClearAll`/`UnsubscribeAll`
   clear channels (and `OnChannelRemoved` clears callbacks).

### 4. Reactive (`ReactiveTests`)

1. `ValueProperty`: `Set(same value)` does NOT publish; `Set(new)` publishes with
   `Current`/`Previous` filled; `Force` publishes even when unchanged; `Reset()` restores
   `defaultValue` to both `Value` and `Previous` WITHOUT publishing; `Clear()` drops
   subscribers and resets; `IObservableEventChannel.OnUpdated` fires after the event.
2. `ProgressProperty`: input clamped to [0,1] BEFORE the equality check (so `Set(1.5f)` after
   `Set(1f)` does not publish); `IsReady()` at ~1.0.
3. `Trigger`: `Fire()` publishes once; repeat `Fire()` is a no-op until `Reload()`; implicit
   `bool` conversion; `AutoReset` flag honored via `EventBus.Reset()`.
4. `Signal.Emit` publishes (thin, covered by area 3 — one smoke test).
5. `ProxyValue`: constructor with initial data sets `Data`/`IsNotEmpty`; `Publish(data)`
   updates `Data` then publishes; `Reset()` restores initial data or empties.

### 5. StateMachine — flat (`StateMachineTests`)

Context: plain class with test states; states log lifecycle calls.

1. Setup: `AddStatesFrom(context)` auto-discovers nested `IState<TContext>` types (implicit in
   ctor unless context is `IDummy`); duplicate `AddState` is silently ignored; `Activate()`
   enters the default root; no default root → `StateMachineException`.
2. `State` base lifecycle: `OnStart` fires on FIRST `OnUpdate`; `IsFirstUpdatePassed` becomes
   true only on the SECOND update; `OnExit` resets both.
3. Pending transitions: `GoTo<T>()` sets `HasChangeRequest()`; applied on next `Update()`;
   quirk — the `Update()` that applies a transition SKIPS `OnUpdate` of the active path for
   that frame. Pin it.
4. Quirk — capture, don't fix: `GoTo<T>()` when `T` is already the active leaf returns false
   AND silently CANCELS any other pending request (`pendingState = null` happens first).
5. `GoTo(pendingRequest: false)` transitions immediately; `TryGoTo` on unknown state returns
   false without registering a request; `GoTo` to a state missing from `Ancestors` throws
   `StateMachineException` on apply.
6. `GoTo` before `Activate()`: `ApplyRequestOnStart` enters the requested path instead of the
   default root.
7. `Deactivate()` exits the active path leaf→root and clears state; `IsActive`/`IsActiveLeaf`/
   `Contains`/`GetActiveLeaf` — including by interface (`IsAssignableFrom`).

### 6. StateMachine — hierarchy (`StateMachineHierarchyTests`)

1. Wiring: substate (`State(parentState:)`) attaches to parent; parent missing →
   `StateMachineException`; second default child under one parent → `RewriteOperationException`.
2. Entering a parent falls through the default-child chain (`EnterDefaultPath`, recursive).
3. LCA transitions: between sibling substates only the nodes below the common ancestor
   exit/enter — the parent's `OnExit`/`OnEnter` must NOT fire; assert exact order from the
   log (exit leaf-up, enter top-down, then default-path of target).
4. Transition to a substate in another root: full exit of the old path, full enter of the new;
   `currentRoot` follows (observable via `IsActive<TRoot>()`).
5. `Update()` updates the whole active path parent→leaf (one log assertion).

## Non-goals

- No behavior fixes: `GetList()` order mismatch, `SetsList` stale entry, static-handler
  binding-flags asymmetry, `GoTo` cancel-on-active-leaf all stay as captured.
- No Hub coverage: `FlowHub`/`GlobalHub`/`StaticBootstrap`/`RoutineHelper` need the Unity
  update loop and drag in RF-005 static debt — separate phase after RF-005 shapes up.
  Same for `BaseService`/`ActiveOperations` (thin, hub-coupled).
- No RF-008 work (BCL migration, thread-safety) — these tests are its prerequisite.
- No PlayMode tests, no CI, no coverage targets.
- `Sample/Test/` untouched (moves to `Samples~` in host RF-003; don't build on it).

## Plan

1. Fix `Tests/Arunoki.Flow.Tests.asmdef` (see Infrastructure fix).
2. Test doubles in `Tests/Doubles/`: event structs, `IFlowContext` fakes, recording states,
   handler classes (incl. one static, one `IFlowConditionHandler`).
3. Test files per area: `SetTests`, `SetsTypeCollectionTests`, `EventBusTests`,
   `ReactiveTests`, `StateMachineTests`, `StateMachineHierarchyTests`.
4. Order: Collections → EventBus → Reactive → StateMachine (each layer builds on the previous).
5. Run `dotnet format` / CSharpier on new files only.

## Verification

All tests green in Unity Test Runner (EditMode), run by the user in the Hillybombs host.
Agent cannot compile — see `AGENTS.md`. After green: update host
`specs/refactoring/RF-004-test-infrastructure.md` execution record and this spec's status.

## Notes

- Passport: `Hillybombs/docs/packages/com.arunoki.flow.md`; game-side usage: `docs/flow.md`.
- Every "capture, don't fix" item above should carry an in-test comment naming the follow-up
  spec (RF-008 for collections, RF-005/RF-006 where relevant) — mirrors the poolables
  capacity-sentinel pattern.
- After green: RF-008 collections migration and RF-005 hub work become safe; host RF-001 next
  per `docs/MIGRATION_PLAN.md`.
