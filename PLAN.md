# Comet Crushers: Living Plan

Update this file after every phase. It is the single source of truth for plan, context, questions, and progress.

## Context
- CS4361.001 Team 6: Yadiel Valentín Ríos, Kyle Barnes, Daniel Carrasco, Brayden Edington, Wayne Le.
- Unity 6000.6.8f1, URP template. Remote: `YadValRio/Comet_Crushers`.
- Deliverable 2 is **due 2026-11-09 (today)**. The PDF lists Brayden's work (health bar, score display, bug fixes) and has blank entries for Daniel, Yadiel, Kyle and Wayne.
- We are covering Daniel's share. His task list is not known yet.
- Repo state at start: one commit ("Initial check-in"), only the URP template (`SampleScene`, `Assets/Settings`, `InputSystem_Actions`). No game scripts, and no other branches on origin.

## Phases
- [x] **1. Intake**: task list taken from Deliverable 1 (see Tasks below).
- [x] **2. Sync**: `git fetch --all` done. origin only has `main`, so the asteroid baseline is not in this repo yet.
- [ ] **3. Scope cut**: pick the smallest demonstrable set of tasks for today.
- [ ] **4. Build**: one task at a time, small commits, scripts in `Assets/Scripts/`.
- [ ] **5. Verify**: Play in Editor, Console clean, each task shown working.
- [ ] **6. Write-up**: draft Daniel's bullet for the Deliverable 2 PDF.

## Source: Deliverable 1 (`CS4361_ProjectProposal_Team6.pdf`)
**Goal:** 3D base-defense shooter. The player is a turret, asteroids come at the base, waves get harder, upgrades can be bought.
**Timeline:** Oct 28 placeholders + scene · Nov 4 basic shooter, asteroids destroyable · **Nov 9 progress report** · Nov 30 working demo (score + loss system, final models/scenes) · Dec 9 final report.
**Daniel Carrasco: turret controls, aiming camera, and integration.** Implement aiming and firing from the turret's perspective, and integrate the team's work in Unity.
Others, for integration: Kyle = 3D environment/visuals, Wayne = 3D models + projectile/hit/explosion effects, Yadiel = asteroid spawning/movement/collisions, Brayden = base health, score UI, start/game-over flow, testing.

## Tasks (Daniel, itemized)
Phase 4 build order. Each item should be playable and committed on its own.
- [x] **T1 Scene scaffold:** a minimal scene with ground, a turret placeholder (capsule/cube), a base placeholder, and lighting. Skip this if Kyle's scene arrives first.
- [x] **T2 Aiming camera:** first/third-person camera on the turret, mouse look with pitch/yaw clamps, cursor locked (new Input System, using `InputSystem_Actions`).
- [x] **T3 Firing:** fire button spawns a projectile along the aim direction, with a fire-rate cooldown, projectile lifetime, and a crosshair.
- [x] **T4 Hit hook:** projectile collision calls a shared `IDamageable`/`Hit()` interface, so Yadiel's asteroids and Wayne's effects plug in without edits to turret code.
- [ ] **T5 Integration:** pull teammates' work, resolve conflicts, wire turret into the main scene, fix bugs found in testing.
- [ ] **T6 Stretch (Nov 30):** upgrades (fire rate, damage) hooked to score.
- [ ] **T7 Write-up:** Daniel's bullet for the Deliverable 2 PDF (see Progress log).

## Open questions
1. ~~What are Daniel's assigned tasks?~~ Answered by Deliverable 1 (above). Please confirm the list and order.
2. The PDF says the baseline game (asteroids destroyed, health bar, score) exists, but origin/main has only the template. Is that work on someone's machine or another repo or branch? Does it need to be pulled before we build?
3. Is the deliverable the PDF text only, or does it also need a build or demo?

## Decisions
- Plan doc lives in the repo as `PLAN.md` (committed, shared).
- Edit scripts in VS Code, keep Unity open for Play-mode testing.

## Progress log
- 2026-11-09: Read the PDF, fetched remotes, created this file.
- 2026-11-09 note: the working tree shows `Assets/Editor/HubForceResolve.cs` deleted and a `SENTIS_ANALYTICS_ENABLED` define added in `ProjectSettings.asset`. Unity Hub injects a self-deleting helper script on import, so this is expected. Don't commit it unless the team wants it.
- 2026-11-09: Wrote `Assets/Scripts/`: `TurretController` (mouse aim + fire + crosshair), `Projectile`, `IDamageable`, `TestTarget`, `SceneScaffold`. `SceneScaffold` builds ground/base/turret/targets at Play time only when the scene has no `TurretController`, so the saved scene is untouched. T1-T4 are written but not yet play-tested. Next: click the Unity window so it imports the scripts, check the Console, press Play.
- 2026-11-09: T1-T4 play-tested in Unity by Daniel: aim, fire, crosshair, hits on test targets all work. Committed. Remaining: T5 integration (blocked until teammates push), T7 PDF line.
