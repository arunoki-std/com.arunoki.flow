# RF-008 — flow session prompt

You are running INSIDE `com.arunoki.flow` (its own git repo). Single-module task. Read the full
spec first: `Specs~/RF-008-collections-standardization.md` — it is the source of truth; this file
is the execution brief. Risk: MEDIUM — this touches Hub/Events/StateMachine internals via the
collection types they consume. Go carefully, lean on the tests.

## Prerequisite (satisfied)

RF-004 tests are done (`Arunoki.Flow.Tests`, 35 EditMode, incl. `Set<>`/`SetsTypeCollection<>`
coverage as used by Hub/Events/StateMachine). Run them BEFORE you start (baseline green) and
after every change.

## Scope — all under `Runtime/Collections/`

11 `// TODO [RF-008]` markers across 8 files. Verified 2026-07-08:
`SetsTypeCollection.cs`, `Set.cs`, `Set.KeyValue.cs`, `SetsCollection.cs`, `CustomSet.cs`,
`Enumerators/MutableEnumerator.cs`, `Interfaces/ISet.cs`.

## Do

1. **Audit before deleting.** RF-007 usage data says only `Set<>`, `IContainer<>`, `Container<>`,
   `SetsTypeCollection<>`, `ReflectionUtils`, and the `Mutable*` enumerators are consumed. Re-grep
   call sites across flow to confirm, then delete genuinely dead types (`SetsCollection<>` is a
   removal candidate — verify first).
2. **Replace with BCL where behavior allows.** Keep a custom type ONLY with a written
   justification in-file. The one real distinguishing feature is safe removal of the current
   element during (reversed) iteration — confirm each call site actually relies on it before
   swapping in `HashSet<T>`/`Dictionary<,>`.
3. **Kill the real LINQ.** Only `SetsTypeCollection.cs` uses `System.Linq` (`using System.Linq`
   + `SetsCache.Keys.ToArray()` at line ~173). Replace with a plain preallocated loop/buffer.
   NOTE: the two-arg `.Where(condition, action)` calls in `SetsCollection.ISet.cs` and
   `CustomSet.cs` are a CUSTOM mutate-during-iteration helper, NOT `System.Linq` — do not "fix"
   those.
4. **Thread-safety contract.** For every surviving mutable type (`Set<>`, `Set<,>`,
   `SetsCollection<>`, `SetsTypeCollection<>`): either document `// single-thread only (Unity main
   thread)` as an explicit contract, or add synchronization where multithreaded use is real.
   `ReflectionUtils.PropsCache` is already concurrent-safe (`ConcurrentDictionary`) — leave it.
5. **Rename the `ISet<T>` collision** with `System.Collections.Generic.ISet<T>`
   (`Interfaces/ISet.cs` + all usages) to a non-colliding name.
6. Every `// TODO [RF-008]` must end up either resolved or converted into a documented contract —
   none left dangling.

## Do NOT

- No behavior changes to flow's public API.
- No LINQ in runtime replacements (project convention). Plain loops, preallocated buffers.
- No changes outside `Runtime/Collections/` unless a rename forces call-site edits elsewhere in
  flow — keep those minimal and obvious.

## Finish

- One branch, one commit in this module's repo.
- Flow EditMode suite green; project compiles; `grep -rn "TODO \[RF-008\]"` returns nothing.
- Update `Specs~/RF-008-collections-standardization.md` status → done and `Specs~/README.md`.
- Tell the user to bump the flow submodule pointer in the parent repo, and to mark RF-008 done in
  the host `docs/MIGRATION_PLAN.md`.
