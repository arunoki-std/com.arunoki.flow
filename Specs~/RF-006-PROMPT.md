# RF-006 — flow session prompt

You are running INSIDE `com.arunoki.flow` (its own git repo). Do only flow's slice of RF-006.
Canonical spec in the host repo: `specs/refactoring/RF-006-small-debt.md` — read for context, this
file is your work order. Risk: low. It is pure LINQ triage — no TODO items land in flow.

Convention: no LINQ on runtime hot paths (per-frame / per-event / pooling). One-time init and
debug-only branches may keep LINQ for readability — verify call frequency, then mark with a comment.
Run the flow EditMode suite (35) before and after; it must stay green.

## Do

1. **`Runtime/Basics/ActiveOperations.cs:97`** — `foreach (var id in operations.Keys.ToArray())`.
   This is callable at runtime frequency. Replace the `Keys.ToArray()` snapshot with a plain
   preallocated buffer / reused list (the `ToArray()` is there to allow mutation during iteration —
   preserve that semantic). Drop `using System.Linq` from the file if nothing else needs it.
2. **`Runtime/Utilities/Utils.Strings.cs`** — `using System.Linq`, used for trace/string formatting.
   Presumed debug/formatting only. Verify it is not called per-frame; if so, KEEP and add a comment
   ("debug/trace formatting — not a hot path"). Replace only if you find it on a hot path.

## Do NOT — important

- `Runtime/Events/Utilities/EventBusUtility.Expressions.cs` and `.Probe.cs` use
  `System.Linq.Expressions` (expression trees), NOT `System.Linq` collection operators. These are
  subscription-setup machinery — LEAVE THEM ENTIRELY. Do not "remove LINQ" here.
- No behavior changes to flow's public API. No changes outside flow.

## Finish

- Branch + commit in this module's repo.
- Flow EditMode suite green; project compiles.
- Update `Specs~/README.md` with the RF-006 (flow slice) status line.
- Tell the user to bump the flow submodule pointer in the parent repo.
