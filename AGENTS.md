# com.arunoki.flow — Agent Instructions

Standalone UPM package (own git repo), embedded as a submodule in the Hillybombs project. You are working INSIDE this package: commits go to this repo; the host project updates its submodule pointer separately.

## Package

Events, signals, reactive properties, state machine, hub (DI-like composition). Depends only on `com.arunoki.collections`.

- Code: `Runtime/` (asmdef `Arunoki.Flow` → `Arunoki.Collections`)
- Areas: `Events/` (EventBus), `Reactive/` (Signals, `ValueProperty`), `StateMachine/`, `Hub/` (`FlowHub`, `GlobalHub`), `Basics/` (services, `ActiveOperations`)
- Extra docs: `Docs~/Index.md`, `Docs~/ScriptingDefineSymbols.md`
- Consumers (in host project): com.arunoki.core, game code

## Rules

- Allowed references: `Arunoki.Collections` only. Never reference core, poolables, or game code.
- `GlobalHub.Instance` is known debt (host RF-005): do not add NEW static/singleton access paths; prefer `IFlowContext` injection.
- Public API changes are breaking for core and the game: additive preferred; removals/renames need a spec and a version bump.
- Never edit or delete `.meta` files by hand; never invent GUIDs.
- Code style: standard Microsoft C# per `.editorconfig`; run formatter on changed files.
- Specs for this package live in `Specs~/`. Cross-cutting roadmap lives in the host project (`docs/MIGRATION_PLAN.md`).
- Shared skills are available via symlinks in `.agents/skills/` (targets live in the host repo; they dangle in a standalone clone — harmless). Module-specific skills get real folders here, named with the package prefix.
- `Sample/Test/` is misplaced (host RF-003 will move it to `Samples~/`) — don't build on it.

## Verification

The package alone does not compile — needs a Unity host. Ask the user to compile/run EditMode tests in the Hillybombs project. Tests (RF-004) live in `Tests/` with asmdef `Arunoki.Flow.Tests`.
