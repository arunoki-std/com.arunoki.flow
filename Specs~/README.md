# Specs (module-local)

Specs scoped to THIS package only. Format: copy the host project's `specs/TEMPLATE.md`.
Cross-cutting work (touching several packages or the game) is specced in the host
project: `Hillybombs/specs/` + roadmap in `Hillybombs/docs/MIGRATION_PLAN.md`.

The `~` suffix keeps Unity from importing this folder (no .meta noise).

## Status

- RF-003 (naming and hygiene, flow slice) — done: `package.json` description/version (0.2.0),
  `Sample` → `Samples~`. Canonical spec: `Hillybombs/specs/refactoring/RF-003-naming-and-hygiene.md`.
- RF-006 (small debt, flow slice) — done (2026-07-08): LINQ triage. `ActiveOperations.Clear()` —
  `Keys.ToArray()` → reused buffer (runtime frequency, called from `RoutineHelper.OnDisable`).
  `Utils.Strings.cs` (`JoinAsList`) — kept as debug/trace (subscription-time), marked with a comment.
  `EventBusUtility.Expressions/.Probe` — untouched (expression trees, not LINQ operators). Canonical
  spec: `Hillybombs/specs/refactoring/RF-006-small-debt.md`.
- RF-008 (collections standardization) — done (2026-07-08): deleted dead `SetsCollection<>` /
  `CustomSet<>`, killed the LINQ, documented single-thread contracts. Naming standardization:
  namespace → `Arunoki.Flow.Collections`; `Set<>` → `FlowSet<>`, `SetsTypeCollection<>` →
  `FlowSetsCollection<>`, custom `ISet<T>` → `IFlowSet<T>`. Spec:
  `RF-008-collections-standardization.md`.
