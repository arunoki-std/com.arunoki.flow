# Specs (module-local)

Specs scoped to THIS package only. Format: copy the host project's `specs/TEMPLATE.md`.
Cross-cutting work (touching several packages or the game) is specced in the host
project: `Hillybombs/specs/` + roadmap in `Hillybombs/docs/MIGRATION_PLAN.md`.

The `~` suffix keeps Unity from importing this folder (no .meta noise).

## Status

- RF-003 (naming and hygiene, flow slice) — done: `package.json` description/version (0.2.0),
  `Sample` → `Samples~`. Canonical spec: `Hillybombs/specs/refactoring/RF-003-naming-and-hygiene.md`.
- RF-008 (collections standardization) — done (2026-07-08): deleted dead `SetsCollection<>` /
  `CustomSet<>`, killed the LINQ in `SetsTypeCollection`, documented single-thread contracts on
  surviving mutable types, renamed custom `ISet<T>` → `IFlowSet<T>`. Spec:
  `RF-008-collections-standardization.md`.
