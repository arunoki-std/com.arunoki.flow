# RF-004 hub follow-up — flow session prompt

You are running INSIDE `com.arunoki.flow` (its own git repo, cwd = `Packages/com.arunoki.flow`).
Single-module task. Goal: close the RF-004 leftover — EditMode characterization tests for
`Runtime/Hub/` (`FlowHub`, `GlobalHub`, `StaticBootstrap`). This is the explicit prerequisite for
host RF-005 step 3 (migrating `GlobalHub.Instance` consumers): without these tests the static
refactoring has no safety net. Risk of THIS task: low (tests only).

Source of truth for style and access strategy: `Specs~/RF-004-flow-tests.md` (done, 35 tests).
Follow it exactly: capture CURRENT behavior including bugs/quirks, do NOT fix; comment quirks
in-test with a `// RF-005` marker where the quirk is the static coupling itself.

## Before you start

1. Read `AGENTS.md`, `Specs~/RF-004-flow-tests.md`, `Runtime/Hub/**` (4 files, ~340 lines).
2. Ask the user to run the existing EditMode suite (`Arunoki.Flow.Tests`, 35 tests) — baseline
   must be green.
3. Create `Specs~/RF-004-hub-tests.md` (copy the RF-004 spec format; scope = this brief).

## Scope

New test files under `Tests/` only: `FlowHubTests.cs`, `GlobalHubTests.cs`,
`StaticBootstrapTests.cs` (+ `Doubles/` additions). No `Runtime/` changes unless a test is
blocked — document each such case in the spec instead of changing behavior.

## Test areas

### 1. `FlowHub` (plain CLR — no GameObjects; use `new FlowHub(context)` with a fake `IFlowContext`)

1. Construction: `FindPartsAt` collects public `IHubContainer` properties from the hub itself and
   from `Contexts.Root`; an `IDummy` context contributes nothing.
2. `TryInjectDependencies`: `IHubPart`/`IContextPart` doubles get hub/root context injected only
   when currently null (pre-set values survive).
3. Container ordering: `SortContainers` orders by `BuildOrder` — note `Handlers/Pipelines/
   Contexts/Managers/Services` are `short.MinValue + n` while `Any = 0`, so custom containers
   sort LAST. Pin the actual order of the six built-in containers.
4. `Register`: returns true iff at least one container consumed the entity; null throws
   `ArgumentNullException`. Same null contract for `Remove`/`IsConsumable`.
5. `Remove`/`RemoveAll`: `RemoveAll` also calls `Events.ClearAll()`; `Reset` calls
   `Events.Reset()`.
6. `TryFind<T>` finds a built-in container by type; false + default for absent type.
7. `Activate`/`Deactivate` lifecycle (via `BaseServiceExplicit`); `OnInit` adds `Contexts.Root`
   to `Contexts.Set`.

### 2. `GlobalHub` — the statics RF-005 targets (expect friction; the friction IS the finding)

Hazards to plan around, then pin in tests:

- `Instance` is set-once with NO reset API; `_isReady`/`OnReady` are also static. A second
  `new GlobalHub(...)` in the same test run throws `InvalidOperationException`. You MUST reset
  `Instance`, `_isReady`, and `OnReady` via reflection in `SetUp`/`TearDown`, and destroy the
  created `Main.Flow` GameObject — otherwise tests are order-dependent and unrepeatable.
  Document this reflection-reset necessity in the spec: it is direct evidence for RF-005.
- The ctor creates a `Main.Flow` GameObject, adds `RoutineHelper`, and calls `DontDestroyOnLoad`,
  which is not supported in EditMode — check whether it logs an error; if so, use
  `LogAssert.Expect` (do not move to PlayMode for this alone).

Tests:

1. Ctor sets `Instance`; second ctor throws; `IsAssemblyInitialized` reflects `Instance`.
2. `OnReady` fires exactly once on first `Activate()`, is nulled afterwards; a subscriber added
   after activation never fires (capture — this is current behavior).
3. `GetRoutine()` returns the `RoutineHelper` from the created GameObject.
4. `Init(hub, bootstrap)`: registers each bootstrap type into `Managers`, then activates the hub.

### 3. `StaticBootstrap` (pure CLR)

Use test-local static classes declared in dedicated namespaces inside the test assembly.

1. `Init(assembly, namespaces)`: collects ONLY static classes; namespace match is EXACT — a
   static class in `X.Y.Z` is skipped when filtering for `X.Y`. Pin this (it is the documented
   silent-skip hazard from `docs/flow.md` §5 / AGENTS.md).
2. Class constructors run on collection (assert via a static ctor side effect flag).
3. No-filter overload collects all static classes of the assembly; `GetTypes()` /
   `GetEnumerator()` expose the collected list.

## Do NOT

- No behavior fixes, no `Runtime/` edits, no `InternalsVisibleTo`.
- Never touch `.meta` files; new test files get their `.meta` from Unity — ask the user to focus
  the Editor once so metas generate before committing.
- No LINQ in tests' helper code where a plain loop is as clear (project convention).

## Finish

- Ask the user to run the full EditMode suite: old 35 + new hub tests, all green.
- Update `Specs~/RF-004-hub-tests.md` → status: done; note any blocked tests.
- One branch, one commit in this module's repo.
- Tell the user to: bump the flow submodule pointer in the parent repo; mark the RF-005
  prerequisite as satisfied in host `docs/MIGRATION_PLAN.md` (§4 item 8) and in
  `specs/refactoring/RF-005-reduce-static-coupling.md` (step 3 of the migration order).
