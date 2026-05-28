# Blood Arena

## 1. Game Overview
**Blood Arena** is an intense, fast-paced 2-player local fighting game built exclusively for PC. Step into the arena and battle head-to-head in a visceral martial arts showdown where split-second timing, precision dodging, and brutal strikes determine the victor. Designed as a couch-multiplayer experience, two players share a single keyboard in a classic arcade-style setup.

## 2. Controls
Both players play locally on the same keyboard. The controls are fully mapped using Unity's New Input System.

| Action | Player 1 | Player 2 |
| :--- | :--- | :--- |
| **Move (Left/Right/Crouch/Jump)** | `W`, `A`, `S`, `D` | `Up`, `Left`, `Down`, `Right` Arrows |
| **Light Attack** | `Q` | `Right Ctrl` |
| **Heavy Attack** | `F` | `Numpad 0` |
| **Block / Parry** | `E` | `Right Shift` |
| **Dodge** | `C` | `Numpad Enter` |

*Pro-tip: Time your block perfectly as an enemy strike lands to trigger a parry, opening them up for a devastating counter-attack.*

## 3. How to Play
- **Game Loop:** A match is played in a "Best of 3 Rounds" format. Each round starts with both fighters at opposite ends of the arena with maximum health.
- **Round Rules:** A standard round lasts exactly 120 seconds. Players deal damage using light and heavy attacks. The round ends when a fighter's health reaches 0, resulting in a knockout (KO). If the timer runs out, the player with the highest remaining health wins the round. 
- **Sudden Death:** If the timer expires and both players have exactly the same health (or if both are KO'd simultaneously), the round is declared a Draw. If this draw pushes the match to a tiebreaker, the game enters Sudden Death where the first hit wins.
- **Winning the Match:** Win 2 rounds to be crowned the champion of the Blood Arena.

## 4. How to Build
To build the game from source:
1. Ensure you have **Unity 2022.3 LTS** installed via the Unity Hub.
2. Open the `Blood Arena` project.
3. In the top menu, go to **File > Build Settings**.
4. Confirm all 6 scenes are added to the "Scenes in Build" list in their correct order.
5. Set the **Target Platform** to `Windows, Mac, Linux`.
6. Set the **Target OS** to `Windows` and **Architecture** to `Intel 64-bit (x86_64)`.
7. Click **Build**, select a destination folder, and wait for the compilation to finish.

## 5. Running the Game
Once built, navigate to your destination folder and double-click the `Blood Arena.exe` file. Since it's a 2-player local game, you and your opponent will play side-by-side on the same keyboard.

## 6. Project Structure
Our project strictly follows the architecture laid out in Section 15.2 of the Game Design Document (GDD):

```text
Assets/
├── 01_Scenes/               # Bootstrap, Main Menu, Character Select, Arena maps
├── 02_Scripts/
│   ├── Fighters/            # State machine, combat system, input reading
│   ├── Managers/            # GameManager, AudioManager, RoundManager
│   └── Editor/              # Automation tools, setup wizards, QA scripts
├── 03_Assets/
│   ├── Audio/               # .ogg sound effects and music
│   ├── ScriptableObjects/   # SessionConfig, FighterDatabase, FighterData
│   ├── Materials/           # URP materials
│   └── Prefabs/             # Fighters (Ragnar, Darius)
└── 04_InputActions/         # InputActions_BloodArena.inputactions definition
```

## 7. Asset Credits
The following CC0/Open Source assets were utilized to bring Blood Arena to life:

| Asset | Source | License |
| :--- | :--- | :--- |
| *Arena Ambience Audio* | OpenGameArt.org | CC0 Public Domain |
| *Impact SFX Pack* | FreeSound.org | CC0 Public Domain |
| *Placeholder UI Font* | Google Fonts | SIL Open Font License |
| *Basic Fighter Models* | Kenney.nl | CC0 Public Domain |

*(Note: Replace with actual asset credits as production evolves.)*

## 8. Technical Notes
- **Engine Version:** Developed in Unity 2022.3 LTS.
- **Render Pipeline:** Universal Render Pipeline (URP 14.x) for optimized visual fidelity and performance.
- **Input System:** Unity's New Input System package.
- **Combat Logic:** Hitbox and hurtbox detection is explicitly driven by `FixedUpdate` (running at a fixed `0.01667` timestep / 60Hz) to ensure frame-accurate, deterministic combat.
- **Timers:** Game logic relying on time (like the `RoundManager` and `SlowMotionController`) strictly uses `WaitForSecondsRealtime` to ensure coroutines function correctly independently of `Time.timeScale` modifications.
- **Target Performance:** 60 FPS target at a 1920x1080 resolution.
