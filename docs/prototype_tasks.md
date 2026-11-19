# Squeeze Benz — Prototype task assignments

Planning date: November 10, 2026. Submission deadline: November 13, 2026.

This plan is based on `GDD Game programming.pdf`, `prototype_requirements.pdf`, and `proposal.md` in this directory. The GDD defines the gameplay; the prototype requirements define the submission. The proposal supplies supporting context where consistent with the GDD.

## Team assignment and workload

The GDD names Karim Bennis, The Vinh Vuong, James Luciano, and John Michael Pasiolan, but does not identify their technical strengths. Assign the actual names to these slots together before work begins; no ability ranking is inferred from the names.

| Slot | Capability assumption | Primary responsibility | Estimated focused effort |
| --- | --- | --- | --- |
| Developer A | Stronger technical contributor | Vehicle physics, run lifecycle, difficulty, integration | 12–17 hours |
| Developer B | Stronger technical contributor | Traffic generation, NPC behavior, police, scoring and economy logic | 16–20 hours |
| Developer C | Developing technical contributor | Menus, HUD, garage/store presentation, user-facing feedback | 10–14 hours |
| Developer D | Developing technical contributor | Level assembly, vehicle assets, audio/visual feedback, QA and submission documentation | 10–14 hours |

These are estimates, not measured capacity or a promise that all work fits. A/B own the systems with the most interacting rules and failure cases. C/D own bounded implementation work and deliver playable features, with short A/B reviews at integration points. Every developer implements, checks, and commits their own work. A's integration responsibility does not transfer everyone else's debugging to A.

## Prototype scope and decisions

Deliver a complete rough loop: start → drive and weave through traffic → gain score while difficulty rises → trigger/escape police → crash → see results and receive money → buy/select/upgrade a car → retry.

- Use one highway environment with reusable road sections, civilian traffic, a police car, and two playable cars with different stats. Boxes and simple materials are acceptable.
- Represent each defined core mechanic with a minimal implementation and visible or audible feedback. Reduce asset variety and polish before cutting mechanics.
- Use a fixed chase camera as the working assumption, plus the GDD's look-behind action. Adapt to the team's chosen camera if already decided. Engine/version and scene conventions must be agreed at kickoff; the task names below describe responsibilities, not a required engine API.
- Implement W/S/A/D, C for look behind, H for horn, and Esc for pause. Alternate key presets and rebinding are later work.
- For an endless game, the objective is survival and improving score. Display that objective at the start; a first police escape or difficulty milestone provides a visible intermediate success without ending the run.
- The GDD lists four driving stats in its mechanics section but five in its car profiles. Working interpretation: handling controls steering response; traction controls grip/sliding. Use all five and document the mapping. A owns the final tuning.
- The GDD does not specify what happens below minimum speed. Working prototype rule: show a warning below the threshold and remove the speed multiplier bonus, with no additional instant-loss condition. This is a proposed rule, not an existing GDD requirement; record the team's decision before tuning.
- Keep the posted police speed limit separate from the rising minimum-speed threshold. Eventually provoking police is consistent with the risk/reward loop; show both values clearly.
- The requirements say at least one playable level, while the grading table mentions three connected levels. D should confirm the expectation with the instructor early. Prepare three connected difficulty sections in the highway as a fallback, but do not assume these satisfy a requirement for three separate levels.
- A boost cooldown appears in the GDD's HUD list without a boost mechanic, input, or rule. Leave boost out pending a design decision; do not invent a full mechanic from the HUD label.
- Defer the full nine-car catalog, rarity presentation, cosmetic/parts systems beyond one stat upgrade, hostile civilian AI, minimap, tutorial level, selectable time of day, radio system, advanced graphics, and full settings/records menus. Their absence must not block the core loop.

## Shared agreements before implementation (all, 30–45 minutes)

1. Choose A/B/C/D, engine/version, camera, scene ownership, collision layers, and input conventions. Reuse approaches taught in the other labs where possible, as required by the repository instructions.
2. Create a launchable shared scene and agree on units, road width, lanes, forward direction, and starter-car dimensions. D owns level placement; A owns the player; B owns traffic/police; C owns UI. Prefer separate prefabs/scenes to reduce editing conflicts.
3. Agree on a small shared interface. A owns run state (`Menu`, `Running`, `Paused`, `GameOver`) and a single crash/end-run entry point. B owns score, wallet, purchases, and run-result calculation. C reads values and invokes commands; UI must not independently award money or determine losses.
4. Define shared data: vehicle stats, difficulty settings, score/multiplier, speed, distance, minimum speed, posted speed limit, chase status, final results, wallet, owned cars, and selected car. A/B provide initial values or stubs so C/D can work immediately.
5. Agree on notifications such as run started, crash, score changed, chase started/escaped, and run ended. Exact implementation should follow the labs. Reset temporary state on retry; preserve session wallet, ownership, upgrades, and best score.
6. Use a temporary development menu or test scene to trigger crashes, chases, money grants, and difficulty stages. Keep test shortcuts out of the submitted player controls.

## Developer A — vehicle, difficulty, and integration

### A1. Shared foundation and run lifecycle — 2–3 hours

- Create the player root and controller scaffold; expose speed and distance for other systems.
- Implement start, pause/resume, crash, game over, and retry transitions. Disable driving and scoring outside an active run.
- Ensure a crash is processed once, freezes/ends gameplay immediately, and produces one results event. Reset the player, traffic, police, score, and difficulty on retry through each system's reset hook.
- Coordinate collision layers with B/D. Traffic collisions end the run. Define road-edge behavior explicitly; recommended prototype behavior is a physical barrier whose collision also ends the run.
- Acceptance: start → pause → resume → crash → retry works repeatedly; pause freezes gameplay timers and movement; multiple collision callbacks cannot produce duplicate rewards.
- Dependencies/handoff: publish state and crash/reset interfaces first so B/C can integrate without waiting for complete physics.

### A2. Weighty arcade vehicle controller — 4–5 hours

- Implement acceleration, braking, steering, and limited lateral grip using techniques from the labs.
- Read five vehicle stats from shared car data: top speed, acceleration, handling/steering response, braking force, and traction/grip. Convert the GDD's 1–5 ratings to tunable controller values; do not treat the ratings as physical units.
- Add a follow camera and hold-to-look-behind behavior. Connect H to D's horn sound/feedback.
- Coordinate steering/wheel rotation or body lean, moving wheels, brake lights, and engine feedback with D. Driving-game states replace irrelevant humanoid walk/jump examples in the assignment.
- Acceptance: the player can deliberately weave through a traffic gap, brake, and return to a stable heading. Two selected cars feel measurably different. Controls remain consistent at different frame rates and high speed does not pass through traffic without collision.
- Dependencies/handoff: use D's placeholder dimensions; provide C with speed/active car and D with normalized acceleration, steering, and braking values.

### A3. Difficulty progression and tuning — 2–3 hours

- Increase minimum speed and request denser traffic as distance grows. Expose stage values to B instead of allowing two independent difficulty curves.
- Supply an initial gentle stage, a busier stage, and a challenging stage; cap the curve and allow safe gaps to remain possible.
- Implement the agreed below-minimum-speed warning rule with B/C. Ensure the starter car can reach every configured minimum speed.
- Acceptance: progression is visible within a short demonstration, restarts at the initial stage, and remains playable at the maximum stage. It does not create impossible full-width walls through B's spawner.
- Dependencies/handoff: B consumes density/speed settings; C displays minimum speed and warnings; D places matching section markers if useful.

### A4. Integration and stability — 3–4 hours

- Integrate the owned systems early, review B's collision/police interactions and C's run-state bindings, and resolve shared-interface issues with their owners.
- Check object cleanup/reuse, camera bounds, collision reliability, pause behavior, and repeated restarts in the assembled level.
- Produce the first runnable build before final art work; reserve time for build-only errors on the target machine.
- Acceptance: at least five consecutive complete runs can be started, ended, and retried without stale objects, input loss, duplicate payouts, or scene errors.

### A5. Final handling pass and handoff — 1–2 hours

- Tune steering, braking, grip, camera, and starting difficulty with D's playtest findings.
- Record the controller/stat mapping and run-state rules briefly for the demo; verify that another teammate can explain and use them.
- Acceptance: the starter car is controllable immediately, and a player understands why a collision ended the run.

## Developer B — traffic, police, scoring, and economy

### B1. Civilian traffic and bounded spawning — 4–5 hours

- Implement a civilian NPC that follows its lane at a configurable speed. Use a few speeds/vehicle sizes for variation; lane-changing AI is unnecessary.
- Spawn ahead of the player, outside the immediate collision zone, and remove/recycle vehicles and road traffic left behind. Bound the active count.
- Use A's difficulty settings for density. Check lane occupancy and longitudinal spacing before placement. Reserve a sufficiently wide corridor across a look-ahead window; an empty lane in a single spawn row is not enough if successive rows block reachable movement.
- Start with same-direction traffic. Add one clearly separated oncoming lane only after the base spawner works, to represent the GDD's oncoming-traffic option at low cost.
- Acceptance: NPCs visibly move, traffic does not appear inside the player or other vehicles, and a several-minute drive does not continually increase the object count. Each tested difficulty stage offers gaps the current car can reach.
- Dependencies/handoff: use A/D's lane dimensions and reset hook. Tell D which prefab pivots/collider sizes are required.

### B2. Police state machine — 4–5 hours

- Implement `Patrol → Chase → Escaped/Reset`. Trigger only when a police car is present within a documented detection range and the player exceeds the posted limit.
- Use simple following/steering behavior compatible with the road; avoid a separate navigation framework for this prototype.
- Define tunable detection distance, pursuit speed, and an escape distance maintained for a short duration. Make escape possible with the starter car through spacing and tuning.
- Award one escape bonus per chase. Leaving detection range before a chase starts does not award a bonus. Police collision uses the same instant-crash rule as traffic; defer a separate arrest mechanic.
- Acceptance: driving below the limit does not trigger pursuit; speeding near police does; escaping clears the alert and awards exactly one bonus. Pause and retry stop/reset pursuit and timers correctly.
- Dependencies/handoff: C receives chase status/distance; D receives state changes for lights/siren; A reviews collision and movement integration.

### B3. Score, near misses, and run results — 3–4 hours

- Accumulate base score from distance; increase the multiplier with speed. Keep values in a tunable configuration and implement the agreed below-minimum rule.
- Add a small near-miss bonus after safely passing close to a vehicle, with one award per vehicle/pass. A crash must not count as a successful near miss.
- Include police escape bonuses and track distance, top speed, and session high score. Freeze results on game over.
- Convert final score to money with a simple documented rate and a single payout per run. Restart/menu navigation must not repeat the payout.
- Acceptance: faster driving earns more per equal distance; a near miss and police escape have distinct bonuses; pause/game over stop accumulation; displayed results reconcile with the awarded wallet amount.
- Dependencies/handoff: give C live score data, bonus notifications, and an immutable final result; give A the single result-finalization/payout integration point.

### B4. Minimal car ownership and upgrades — 3–4 hours

- Define two GDD cars in data: Rustbucket (speed 4, acceleration 4, handling 1, braking 2, traction 2) and Bodega (2, 2, 5, 2, 2). Rustbucket is initially owned; Bodega is purchasable. These are tradeoffs, not a strictly superior second car.
- Implement session wallet, owned-car checks, selected car, purchase validation, and one paid stat upgrade. A suitable prototype upgrade improves acceleration while retaining the car's braking/traction limitations; A tunes its effect.
- Reject insufficient-funds purchases, duplicate ownership purchases, and upgrades beyond the supported limit. Make the price reachable in a short demo.
- Acceptance: money earned in a run can buy/select the other car; the next run uses its stats. Buying/upgrading deducts the correct amount once and affects driving. State survives scene changes/retries during the session.
- Dependencies/handoff: C calls purchase/select/upgrade commands and displays their outcomes. Disk saving is deferred unless already available from a lab.

### B5. System checks and tuning — 2 hours

- Work through D's traffic, police, and economy cases; tune reachable escape conditions and safe traffic spacing.
- Document the scoring formula, payout rate, AI transitions, and spawn constraints for presentation.
- Acceptance: another developer can reproduce a chase, escape, payout, and purchase without developer shortcuts.

## Developer C — menus, HUD, and player feedback

### C1. Menu and gameplay navigation — 2–3 hours

- Build a basic main menu with Play, Garage/Store, Controls, and Quit where supported. Show the survival/high-score objective and basic controls.
- Build pause/resume, retry, and return-to-menu controls using A's state interface. Returning to menu abandons the run without a second payout.
- Build game over with score, distance, top speed, escape bonus, money earned, high-score indication, Retry, and Main Menu.
- Acceptance: a new player can start, pause, resume, lose, retry, and return to the menu without using editor controls. Buttons do not remain active behind overlays.
- Dependencies/handoff: begin with placeholder values; connect to A's lifecycle and B's final results when available.

### C2. Readable live HUD — 3–4 hours

- Show current speed, minimum speed, posted speed limit, score, multiplier, distance, and high score. Label speed units consistently.
- Display a clear below-minimum warning and police chase status/distance. Keep these visually distinct.
- Show short notifications for near misses, police escape rewards, and progression milestones. Present the current objective near the start.
- Acceptance: every displayed value matches the underlying system, remains readable over the road, and resets correctly after retry. Warnings communicate meaning with text/icons as well as color.
- Dependencies/handoff: bind to A/B's exposed data/events. D supplies a simple coherent color/font treatment.

### C3. Garage/store UI — 2–3 hours

- Show the two cars, five stat bars, price, wallet, ownership, and selected car. Provide Buy, Select, and one Upgrade button.
- Disable invalid actions and display a short success/failure message from B's command result.
- Acceptance: the player can spend earned money and change the next run's car entirely through the UI. Repeated clicks cannot bypass B's validation or cause duplicate spending.
- Dependencies/handoff: use B's economy/data interface; let A verify the selected car reaches the controller.

### C4. UI verification and demo instructions — 3–4 hours

- Check screen scaling at the team's target resolution and one smaller resolution, pause focus, repeated retry, and garage/menu transitions.
- Verify objective, controls, speed-limit rule, and instant-crash rule are understandable. Add a concise controls/help panel rather than a separate tutorial level.
- Record UI limitations and provide D with screenshots and a short menu/demo walkthrough.
- Acceptance: a teammate unfamiliar with the UI completes the entire loop without verbal instructions.

## Developer D — level, presentation, QA, and documentation

### D1. Playable highway and prefab setup — 3–4 hours

- Build one highway test level with agreed lanes, road edges/barriers, repeatable sections, player start, and camera clearance. Coordinate section recycling with A so the road does not end during normal play.
- Assemble simple player, civilian, and police prefabs with consistent dimensions and distinguishable silhouettes/colors. A/B own their movement scripts.
- Apply sun-bleached road materials plus a restrained neon/cyberpunk palette. Add lane markings and a few repeated roadside props.
- Acceptance: A's player and B's traffic use the same lane geometry; road seams are driveable; traffic and police are recognizable at speed. Greybox assets have a consistent scale and style.
- Dependencies/handoff: share dimensions/prefabs early. Do not wait for final models to provide the test level.

### D2. Feedback for mechanics — 2–3 hours

- Add simple engine pitch/volume changes, horn, braking lights/skid feedback, moving wheels/body lean, collision flash/sound, and police lights/siren using A/B's exposed values/events.
- Work with C to ensure score bonuses, purchases/upgrades, and difficulty progression also have basic visible feedback.
- Use available lab assets or permitted free assets; background music is optional. Track every external asset/tutorial actually used.
- Acceptance: accelerating, braking/steering, crashing, police pursuit/escape, and progression each have a perceivable response. Pausing stops relevant looping audio; retry does not stack sirens/engine sounds.

### D3. Playtest and build verification — 3–4 hours

- Maintain the checklist below and a short issue list with reproduction steps, expected/actual behavior, severity, and owner. C fixes UI issues; A/B fix their systems.
- Test each difficulty stage, both cars, near misses, police escapes, purchases/upgrades, and repeated restarts. Run the actual exported build, not just the editor scene.
- Measure frame rate on the intended demo machine and check that the run remains stable for several minutes. Flag growth in active traffic/road objects to A/B.
- Acceptance: critical issues are resolved or explicitly documented, and the team has a reproducible demo route through all mechanics.

### D4. Submission documentation and packaging — 2–3 hours

- Create a short overview with actual names/roles, implemented mechanics, controls, run objective, known limitations, and build/run instructions.
- Record tutorial URLs actually used, asset sources/credits, and any external-package instructor approvals. The assignment requires prior validation for packages outside class scope; prefer lab techniques to keep scope small.
- Coordinate a short demo: start → drive/progress → near miss → police chase/escape → crash/results → purchase/select/upgrade → retry.
- Check that each developer's Git commits reflect their own contributions. Assemble the final build and documentation in the submission format required by the course.
- Acceptance: the build launches on the target machine, roles and sources are documented, and every teammate can explain their implementation. The course prohibits fully AI-generated solutions and extensive AI coding; this task plan does not replace the team's implementation work.

## Order of work and integration checkpoints

| Time | A | B | C | D | Required checkpoint |
| --- | --- | --- | --- | --- | --- |
| Nov 10 / first work session | Foundation, interfaces, controller start | Traffic scaffold and data definitions | Menu/HUD with placeholders | Highway/prefabs and requirements clarification | Shared scene launches; units, interfaces, ownership, and scope agreed |
| Nov 11 | Drive, collide, pause, retry; difficulty output | Civilian spawning, score, police prototype | Live HUD and game-over flow | Cohesive road, first feedback, first playtest | Playable start → weave → crash → results → retry loop merged |
| Nov 12 | Difficulty/handling tuning and build integration | Police escape, near misses, payout, purchase/upgrade logic | Store/garage wired; warnings/bonus feedback | Finish feedback, exported-build QA, documentation draft | All core mechanics represented; complete earning → spending → driving loop |
| Nov 13, before submission | Fix critical gameplay/build defects | Fix critical NPC/economy defects | Fix navigation/readability defects | Final verification, package, demo rehearsal | Freeze features, verify exported build, submit with buffer |

The critical path is shared dimensions/interfaces → driving and traffic → collision/results → payout/store → complete loop. Police and presentation can progress alongside it. Integrate small working increments at least daily; do not merge four isolated feature sets only on the deadline day.

If behind, cut model detail, decoration, music, extra menus, extra cars, and sophisticated AI first. Retain minimal versions of the defined core mechanics. Add no stretch features until the entire acceptance checklist passes.

## Acceptance checklist and requirement coverage

| Check | Pass condition | Owners |
| --- | --- | --- |
| Core driving and player feedback | Accelerate, brake, steer, look behind, horn; motion/brake/crash feedback; car stats affect movement | A + D |
| Single-mistake loss | One collision ends the run once and opens results | A + C |
| NPC behavior | Civilian lane driving and police patrol/chase/escape are visible | B |
| Difficulty/progression | Minimum speed and traffic rise through visible stages; usable gaps remain | A + B |
| Scoring | Speed multiplier, near-miss bonus, police bonus, distance, and high score update correctly | B + C |
| Economy and car choice | One payout per run; affordable car purchase, selection, and one upgrade affect the next run | B + C + A |
| Menu/HUD | Start, pause/resume, game over, retry, garage/store; live state is readable | C |
| Level and visual direction | Continuous highway supports all mechanics; consistent placeholders and basic audio/FX | D + A |
| Clear goal | Survival/high-score objective is shown; a progression milestone or police escape is recognizable | C + D |
| Stability | Exported build works; five restarts work; several-minute run has bounded objects and acceptable performance | All; D coordinates |
| Teamwork/documentation | Named roles, controls, sources/tutorial links, package approvals where applicable, known issues, and meaningful individual commits | All; D assembles |

Before submission, also test these edge cases: crash during a chase; pause just before escape; retry while a bonus message is visible; two collisions in one frame; purchase with insufficient funds; repeatedly click Buy/Upgrade; select a different car then retry; abandon a run to menu; and verify high score/wallet persist across scene transitions during the session.
