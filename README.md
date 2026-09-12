# Matcha Memory

A 2.5D memory card game on the theme of matcha, made in Unity by a three-person team during a two-week
academic game jam at Poznań University of Economics and Business (2025/2026).

Match pairs of cards to clear the board — each cleared board starts the next level. A time limit keeps
the pressure on, and every completed level adds time back. Matching several pairs in a row builds a combo
multiplier that raises your ranking score without affecting progress, so fast and clean play is rewarded
while slower players can still finish.

## Running the game

There is no prebuilt release, so the game runs from the Unity editor.

1. Install [Unity Hub](https://unity.com/download) and the **Unity 2019.4.40f1** editor
   (Unity Hub → *Installs* → *Install Editor* → *Archive*, or the
   [Unity download archive](https://unity.com/releases/editor/archive)).
2. Clone the repository:

   ```bash
   git clone https://github.com/roksanakanicka/Matcha_Memory.git
   ```

3. In Unity Hub: *Projects* → *Add* → *Add project from disk* → select the cloned folder.
4. Open `Assets/Scenes/MB.unity` — the first scene in the build — and press **Play**.

Scenes included in the build, in order: `MB` → `WK` → `Oswiecenie`.

## Controls

- **Mouse** — click cards to flip them and use the menus.

## Features

- Procedurally generated card grid for each level.
- Time limit with time added for every completed level.
- Combo multiplier that feeds a score ranking.
- Matcha-themed visuals and card art.

## Project structure

```
Assets/Scenes/       Game scenes
Assets/Scripts/      Game manager, card grid generation, cards and animations, ranking, menus, audio
Assets/CardAssets/   Card designs
Assets/Audio/        Music and sound effects
```

## Team

Made by a three-person team during a two-week game jam run by our lecturer; the games were presented
and graded in the final class.
