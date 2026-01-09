# Product Requirements Document (PRD)

## Project: **desert_towers**

**Repository:** [https://github.com/bradallenfisher/desert_towers](https://github.com/bradallenfisher/desert_towers)
**Status:** Greenfield (empty repo)
**Primary Goal:** Launchable, server-authoritative lane battler / tower-defense PvP game that can evolve into a Clash-like product

---

## 1. Product Vision

Build a **real-time, server-authoritative lane battler** inspired by Clash-style gameplay, with:

* solid multiplayer infrastructure
* clean gameplay foundation (movement, targeting, combat)
* legal, permissive codebase
* no architectural dead ends
* a clear path from local dev → public alpha → AWS autoscaling

The project prioritizes **launchability and iteration**, not perfect feature parity with Clash Royale.

---

## 2. Key Decisions (Locked In)

### Engine & Platform

* **Unity (LTS)** as the game engine
* Cross-platform by design (desktop dev → iOS/Android later)

### Networking

* **Photon Fusion 2**
* **Server Mode (authoritative)** from day one
* Dedicated headless server build

### Gameplay Foundation

* Start from a **permissively licensed (MIT) tower-defense / lane battler donor**
* Use it as a **gameplay starter kit** (movement, targeting, combat, visuals)
* Refactor gameplay to be server-authoritative

> Chosen gameplay donor:
> **Tomiinek/Unity_Tower_Defence (MIT licensed)**
> (used as a *donor*, not a base repo)

### Hosting

* **Initial dev & alpha:** Photon Cloud (free tier) + local / single server
* **Long-term:** AWS (autoscaling, GameLift or equivalent)

### Backend (Staged)

* **Phase 1:** none or minimal (Photon only)
* **Phase 2:** PlayFab (auth, profiles, progression)
* **Phase 3:** Full live-ops stack

---

## 3. Repo Strategy (Industry Standard)

### One canonical product repo

You **do not fork** donor repos.

Instead:

* Create and own **one clean repo**
* Import/selectively copy systems from donor projects
* Keep architecture coherent and shippable

### Repo Layout

```
desert_towers/
  Unity/                # Unity project root
    Assets/
    Packages/
    ProjectSettings/

  Shared/               # Pure C# gameplay/simulation logic (no UnityEngine)
    Combat/
    Units/
    Towers/
    Cards/
    Rules/

  ServerTools/          # Build + run scripts, Docker later
  Docs/                 # Architecture, runbooks, setup guides

  .gitignore
  .gitattributes
  README.md
```

---

## 4. Build Targets

### Client Build

* Unity client
* Runs visuals, UI, prediction/interpolation
* Sends **inputs only** to server

### Server Build

* Unity **headless** Linux build
* Runs authoritative simulation
* Validates all gameplay actions
* Replicates state via Photon Fusion

---

## 5. Gameplay Scope (Initial MVP)

### Arena

* 1–2 lanes
* Fixed paths (waypoints/splines)
* 1 main tower per side (expand later)

### Units

* Melee unit
* Ranged unit
* Simple targeting priority:

  * nearest enemy
  * tower > unit (configurable)

### Towers

* Range
* Fire rate
* Damage
* Targeting loop

### Spells

* Single AOE damage spell
* Server validates placement + cost

### Resources

* Elixir-style resource
* Regenerates over time
* Server-validated costs

### Match Rules

* Match length: ~120 seconds
* Win by:

  * destroying enemy tower
  * or higher tower HP on timeout

---

## 6. Multiplayer & Authority Model

### Server Authoritative Rules

* Server owns:

  * unit spawning
  * damage
  * HP
  * cooldowns
  * match timer
* Clients:

  * send **DeployCommand(cardId, position, tick)**
  * never propose state changes

### Photon Fusion Usage

* Server Mode
* NetworkRunner hosted on dedicated server
* Clients connect via Photon Cloud session discovery

---

## 7. Gameplay Donor Integration Plan

### What to Import from Tomiinek/Unity_Tower_Defence

* Unit movement controllers
* Targeting & aggro logic
* Attack timers & damage application
* Projectile logic / visuals
* Health components
* Basic UI concepts (not styling)

### What NOT to Import

* Scene structure wholesale
* GameManager singleton patterns
* Wave spawning logic (will be replaced by card deploys)
* Any client-authoritative assumptions

### Refactor Rules

* All gameplay rules migrate into `Shared/`
* Unity scripts become thin adapters:

  * rendering
  * animation
  * VFX
* Server runs the same logic headlessly

---

## 8. Minimal Launch Architecture

### Phase 1 — Local Playable

* 1 headless server (local machine)
* 2 clients
* Photon Cloud free tier
* Full match start → end flow

### Phase 2 — Public Alpha

* Single hosted server (VPS / EC2)
* Simple matchmaking service or Photon rooms
* Logging + crash reporting
* Guest authentication

### Phase 3 — AWS Autoscaling

* Dedicated server builds uploaded to AWS
* Match allocator service
* Autoscaling fleets
* Multi-region later

---

## 9. “Unpainted Corner” Server Allocation Contract

The client **never cares** where the server lives.

### Allocation Response (stable forever)

```json
{
  "matchId": "m-123",
  "endpoint": "ip-or-dns:port",
  "joinToken": "opaque-string",
  "region": "us-east"
}
```

### Implementation swaps over time:

* Local config → VM → AWS GameLift
* Client code unchanged

---

## 10. Security & Anti-Cheat (MVP Level)

Server validates:

* elixir costs
* deploy zones
* cooldowns
* damage application
* match timer

Client cannot:

* spawn units
* modify HP
* speed up time
* bypass costs

---

## 11. Development Milestones

### Milestone 1 — Infrastructure Smoke Test

* Unity project created
* Photon Fusion installed
* Headless server runs
* 2 clients connect

### Milestone 2 — Gameplay Core

* Import TD movement/targeting
* Server-side combat loop
* Towers + units interact correctly

### Milestone 3 — Card Deployment

* DeployCommand input
* Elixir validation
* Spawn logic on server

### Milestone 4 — Match Completion

* Win/loss detection
* Result display
* Logging

---

## 12. Cursor Kickoff Prompt (Use This)

```text
You are a senior Unity multiplayer engineer.

Goal:
Bootstrap a Unity LTS project for a server-authoritative lane battler using Photon Fusion 2 Server Mode.

Repo:
https://github.com/bradallenfisher/desert_towers

Requirements:
1. Create a Unity project inside /Unity.
2. Add Photon Fusion and configure Server Mode.
  - app id: dfea8cb8-d584-4004-997d-88286be31dad
3. Implement a headless dedicated server build target.
4. Create a minimal test scene where:
   - a server starts headless
   - two clients connect
   - server ticks authoritative time
5. Establish folder structure:
   - Unity/
   - Shared/ (pure C#, no UnityEngine)
   - ServerTools/
6. Stub IServerAllocator interface for future AWS use.
7. Document how to run:
   - server locally
   - two clients locally

Priorities:
- Correct architecture > visuals
- Clarity > optimization
- No client-authoritative logic
```


### PROMPT FOR THIS PRD
You are a senior Unity multiplayer engineer.

Goal:
Bootstrap a Unity LTS project for a server-authoritative lane battler using Photon Fusion 2 Server Mode.

Repo:
https://github.com/bradallenfisher/desert_towers

Requirements:
1. Create a Unity project inside /Unity.
2. Add Photon Fusion and configure Server Mode.
3. Implement a headless dedicated server build target.
4. Create a minimal test scene where:
   - a server starts headless
   - two clients connect
   - server ticks authoritative time
5. Establish folder structure:
   - Unity/
   - Shared/ (pure C#, no UnityEngine)
   - ServerTools/
6. Stub IServerAllocator interface for future AWS use.
7. Document how to run:
   - server locally
   - two clients locally

Priorities:
- Correct architecture > visuals
- Clarity > optimization
- No client-authoritative logic
