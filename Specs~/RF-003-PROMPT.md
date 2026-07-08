# RF-003 — flow session prompt

You are running INSIDE `com.arunoki.flow` (its own git repo). Do only this module's slice of
RF-003. The canonical spec lives in the host repo: `specs/refactoring/RF-003-naming-and-hygiene.md`
— read it for full context, but this file is your work order. This is the biggest slice.

## Do

1. `package.json`: replace the placeholder `"description": "Reusable package"` with an honest
   one (what flow actually is: events/signals/state machine/hub, plus the merged
   `Runtime/Collections`). Bump `"version"` `0.1.0 → 0.2.0`.
2. `git mv Sample Samples~` — the `~` suffix hides it from Unity import (Unity convention).
   Carry `Sample.meta` with it. First verify nothing references the sample content.
   NOTE: the real EditMode `Tests/` folder (RF-004, 35 tests) is separate — leave it untouched.
3. rootNamespace is already correct (`Arunoki.Flow`). Do NOT touch the coexisting
   `Arunoki.Collections.*` namespaces — they are deliberate (collections merged into flow per
   RF-007), not a mismatch to "fix".

## Do NOT

- No code namespace renames. No changes outside this module. No touching `Tests/`.

## Finish

- One branch, one commit in this module's repo.
- Ask the user to run the flow EditMode suite (should stay green — 35 tests).
- Update this module's `Specs~/README.md` status line for RF-003.
- Tell the user to bump the flow submodule pointer in the parent repo.
