# RF-008: Collections standardization (post-RF-007 follow-up)

- Status: done (2026-07-08) — RF-004 tests were the safety net (35 EditMode, incl. Collections coverage)
- Scope: `Runtime/Collections/` (ex `com.arunoki.collections`, merged per host RF-007)
- Risk: medium (touches flow internals: Hub, Events, StateMachine)
- Execution: single session inside `com.arunoki.flow`. Prompt: `Specs~/RF-008-PROMPT.md`.

## Problem

`Runtime/Collections/` is a custom collection library that largely duplicates the BCL
(`HashSet<T>`, `Dictionary<TKey,TValue>`, LINQ-free iteration helpers). Two debts, marked
in code with `// TODO [RF-008]`:

1. **Duplication.** Most types have standard equivalents. The one distinguishing feature is
   safe removal of the current element during (reversed) iteration — verify at call sites
   whether it is actually relied upon before replacing anything.
2. **No thread-safety.** All mutable types (`Set<>`, `Set<,>`, `SetsCollection<>`,
   `SetsTypeCollection<>`) mutate `List`/`Dictionary` state without synchronization.
   `ReflectionUtils.PropsCache` is the only concurrent-safe piece (`ConcurrentDictionary`).

## Goal

- Audit which `Arunoki.Collections` types are dead weight inside flow (RF-007 usage data:
  only `Set<>`, `IContainer<>`, `Container<>`, `SetsTypeCollection<>`, `ReflectionUtils`,
  and the Mutable* enumerators are consumed) — delete the rest.
  Done during merge: internal `Utilities/Utils` deleted — it was a strict subset of
  `Arunoki.Flow.Utilities.Utils` and collided with it once both landed in one assembly.
- Replace remaining types with BCL collections where behavior allows; keep a custom type
  only with a written justification (e.g. mutate-during-iteration semantics).
- Constraint: NO LINQ in runtime replacements (project convention, editor-only) — plain
  loops and preallocated buffers. `SetsTypeCollection.cs` currently violates this
  (`Keys.ToArray()`, `using System.Linq`) — fix as part of this spec.
- For types that survive: either document "single-thread only (Unity main thread)" as a
  contract, or add synchronization / concurrent variants where multithreaded use is planned.
- Resolve the `ISet<T>` name collision with `System.Collections.Generic.ISet<T>` (rename).
- `Unity.Collections` (Native*) containers are relevant only if job/Burst usage appears —
  not a default target (managed reference types dominate here).

## Non-goals

- No behavior changes to flow public API.
- Characterization tests first (RF-004, `Arunoki.Flow.Tests`) — do not start replacement
  before tests cover `Set<>`/`SetsTypeCollection<>` as used by Hub/Events/StateMachine.

## Verification

Flow tests green; project compiles; grep shows no remaining `// TODO [RF-008]` markers
(each one either resolved or converted into a documented contract).

## Outcome (2026-07-08)

- Deleted dead types `SetsCollection<>` (3 partials) and `CustomSet<>` — audit confirmed zero
  references outside their own files in Runtime/Tests.
- Kept `Set<>`, `Set<,>`, `SetsTypeCollection<>` and the `Mutable*` enumerators with in-file
  justification: callers rely on safe removal of the current element during reverse iteration
  (EventBus, Channel, HubContainer) and on `Container<>` root-propagation callbacks — neither
  offered by `HashSet<T>`/`Dictionary<,>`.
- Removed the only real LINQ (`using System.Linq` + `SetsCache.Keys.ToArray()` in
  `SetsTypeCollection.cs`); replaced with a preallocated `CopyTo` snapshot loop.
- Documented `// Contract: single-thread only (Unity main thread)` on every surviving mutable type;
  no synchronization added (flow consumers are main-thread). `ReflectionUtils.PropsCache` left as-is
  (already `ConcurrentDictionary`).
- Renamed custom `ISet<T>` → `IFlowSet<T>` (`Interfaces/IFlowSet.cs`) to resolve the collision with
  `System.Collections.Generic.ISet<T>`; two implementers updated, no external call sites existed.
- All 11 `// TODO [RF-008]` markers resolved; `grep` clean.

## Notes

Created 2026-07-06 during RF-007 execution. Blocked by RF-004 (tests).
