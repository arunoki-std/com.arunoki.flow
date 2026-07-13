# RF-004 (flow, follow-up): Characterization tests for Hub — FlowHub, GlobalHub, StaticBootstrap

- Status: done (2026-07-13; 54/54 EditMode tests green — 35 pre-existing + 19 new:
  11 `FlowHubTests`, 4 `GlobalHubTests`, 4 `StaticBootstrapTests`)
- Scope: this package only — new test files under `Tests/` (`FlowHubTests`, `GlobalHubTests`,
  `StaticBootstrapTests` + `Doubles/` additions); no `Runtime/` changes unless a test is blocked
  (document each case here)
- Risk: low (tests only)
- Host spec: `Hillybombs/specs/refactoring/RF-005-reduce-static-coupling.md` — these tests are the
  explicit prerequisite for RF-005 step 3 (migrating `GlobalHub.Instance` consumers)
- Sibling precedent: `Specs~/RF-004-flow-tests.md` (done, 35 tests) — same style: capture CURRENT
  behavior, including bugs/quirks, with comments; do not fix. Quirks that ARE the static coupling
  get an in-test `// RF-005` marker.

## Problem

`Runtime/Hub/` has zero coverage. Host RF-005 step 3 will move `GlobalHub.Instance` consumers to
injected access; without characterization tests the static refactoring has no safety net.

## Test areas (current behavior, quirks included)

### 1. `FlowHub` (`FlowHubTests`) — plain CLR, no GameObjects

Access strategy per RF-004: no `InternalsVisibleTo`; `Containers`/`TryInjectDependencies` are
`protected internal` → reached via a test subclass (`TestableFlowHub`).

1. Construction: `FindPartsAt` collects public `IHubContainer` properties from the hub itself
   (Events is an `EventBus`, not a container — 5 built-ins on plain `FlowHub`) and from
   `Contexts.Root`; an `IDummy` root contributes nothing.
2. `TryInjectDependencies`: `IHubPart`/`IContextPart` get hub / `Contexts.Root` injected only when
   currently null; pre-set values survive.
3. Container ordering: built-ins use `BuildOrder` = `short.MinValue + n`
   (Handlers=+1, Pipelines=+2, Contexts=+3, Managers=+4, Services=+5) while `Any = 0`, so custom
   containers (and `UpdatableContainer`, which does not override `GetBuildOrder`) sort LAST.
   Pinned order of the six built-ins (via a `FlowHub` subclass replicating `GlobalHub`'s
   `Managers` property): Handlers, Pipeline, Contexts, Managers, Services, Updater.
   Note: `List.Sort` is stable here only because n ≤ 16 triggers insertion sort — commented
   in-test.
4. `Register` returns true iff at least one container consumed the entity; duplicate registration
   returns false; null throws `ArgumentNullException` (same for `Remove`/`IsConsumable`).
5. `RemoveAll` also calls `Events.ClearAll()` (handler callbacks dropped via `OnChannelRemoved`);
   `Reset` calls `Events.Reset()` (auto-reset triggers reload).
6. `TryFind<T>` finds a built-in container by type; false + default for absent type.
7. `Activate`/`Deactivate` via `BaseServiceExplicit` propagate to containers
   (`ServiceWithElements<IHubContainer>` as `TargetService`); `OnInit` adds `Contexts.Root` to
   `Contexts.Set`.

### 2. `GlobalHub` (`GlobalHubTests`) — the statics RF-005 targets

**Finding (direct RF-005 evidence): the tests are impossible without reflection.**
`Instance` is set-once with NO reset API; `_isReady` and `OnReady` are also static. A second
`new GlobalHub(...)` in the same test run throws `InvalidOperationException`. Every test must
reset `Instance`, `_isReady`, `OnReady` via reflection in `SetUp`/`TearDown` AND destroy the
`Main.Flow` GameObject the ctor creates — otherwise tests are order-dependent and unrepeatable.
This reflection-reset block is marked `// RF-005` in `GlobalHubTests`.

**Finding #2 (confirmed by the first test run, 2026-07-13): `DontDestroyOnLoad` does not just
log in EditMode — it THROWS `InvalidOperationException`** ("can only be used in play mode ...
cannot be part of an editor script"). Consequences, all pinned in `GlobalHubTests`:

- The `GlobalHub` ctor NEVER completes in EditMode. By the time it throws, `Instance` is
  already set (set-once, no rollback) — the aborted ctor leaves a half-initialized global
  singleton behind, and a subsequent `new GlobalHub()` fails with "already created" even
  though the first construction itself failed. Direct RF-005 evidence.
- The throw happens before `AddComponent<RoutineHelper>`: `GetRoutine()` returns null, and the
  `Main.Flow` GameObject exists in the scene WITHOUT its helper (update pump never wired).
- Tests recover the half-built hub via `GlobalHub.Instance` (the `CreateHub` helper pins the
  throw, then reads the static); teardown sweeps `Main.Flow` objects BY NAME since no
  `RoutineHelper` exists to find.

1. Ctor aborts in EditMode (see Finding #2) but leaves `Instance` set; second ctor throws
   "already created"; `IsAssemblyInitialized` mirrors `Instance`.
2. `OnReady` fires exactly once on first `Activate()` and is nulled afterwards; a subscriber
   added after activation NEVER fires — captured, not fixed. (Activation of the half-built hub
   works: `InitParts` completed before the ctor aborted.)
3. `GetRoutine()` returns null in EditMode (see Finding #2); the `Main.Flow` GameObject exists
   (`HideFlags.NotEditable`) without a `RoutineHelper`. The happy path — routine on the created
   GameObject — is only observable in PlayMode and stays uncovered (non-goal).
4. `Init(hub, bootstrap)` registers each bootstrap type into `Managers`, then activates the hub.

### 3. `StaticBootstrap` (`StaticBootstrapTests`) — pure CLR

Test-local static classes live in dedicated namespaces inside the test assembly
(`Arunoki.Flow.Tests.BootstrapData`, `.BootstrapData.Nested`, `.BootstrapCctor`).

1. `Init(assembly, namespaces)` collects ONLY static classes; namespace match is EXACT — a static
   class in `X.Y.Z` is silently skipped when filtering for `X.Y` (the documented hazard from
   `docs/flow.md` §5 / AGENTS.md). Pinned.
2. Class constructors run on collection — observed via a side-effect log OUTSIDE the collected
   class (touching a static member of the class itself would trigger the cctor and void the
   test). `RunClassConstructor` runs once per domain, so this test is `[Order(1)]`-ed before the
   collect-all test; commented in-test.
3. No-filter `Init` collects all static classes of the assembly; an EMPTY namespace list also
   means "no filter" (`Count > 0` guard) — pinned; `GetTypes()`/`GetEnumerator()` expose the
   collected list.

## Non-goals

- No behavior fixes: set-once `Instance`, static `OnReady` one-shot, silent namespace skip,
  custom-containers-sort-last all stay as captured.
- No `Runtime/` edits, no `InternalsVisibleTo`, no PlayMode tests.
- No `RoutineHelper` coroutine coverage (needs the player update loop).

## Blocked tests

- `GlobalHub` happy-path construction (`GetRoutine()` returning a live `RoutineHelper`, the
  `Updater` pump wiring) is unreachable in EditMode: `DontDestroyOnLoad` throws there
  (Finding #2). Not moved to PlayMode per the task brief; the EditMode tests pin the aborted
  state instead. Revisit when RF-005 removes the ctor's Unity side effects.

## Plan

1. `Tests/Doubles/HubTestDoubles.cs` — contexts (plain / dummy / with channels / with custom
   container), recording `IHubContainer`, injectable part, plain service, `TestableFlowHub`,
   `HubWithManagers`, static classes for `Managers`/bootstrap.
2. `FlowHubTests` → `GlobalHubTests` → `StaticBootstrapTests`.
3. User runs full EditMode suite (35 old + new) in the Hillybombs host; agent cannot compile
   (see `AGENTS.md`).

## Verification

All tests green in Unity Test Runner (EditMode), run by the user. After green: status → done,
one commit on a dedicated branch; then bump the flow submodule pointer in the parent repo and
mark the RF-005 prerequisite satisfied in host `docs/MIGRATION_PLAN.md` (§4 item 8) and
`specs/refactoring/RF-005-reduce-static-coupling.md` (step 3 of the migration order).
