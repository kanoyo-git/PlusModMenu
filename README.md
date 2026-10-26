# Plus Mod Menu

![Plus Mod Menu](docs/gamebanana-cover.png)

A mod menu for Baldi's Basics Plus 0.14.5 by Kanoyo. Characters, items, events, player, scenes, teleporter and the Classic, Yet Again modifiers, all on one clipboard in the style of the game's Options screen.

## Installation

1. Install BepInEx 5 into the game folder.
2. Put `PlusModMenu.dll` from the archive into `BepInEx/plugins`, directly or in any subfolder. The whole mod is this one file: no other libraries are needed.
3. Start the game and press **Tab** to open the menu.

## Russian language

The menu switches to Russian when CyrillicPlus is installed: it adds Cyrillic to the game's fonts. You can also pick the language in the menu settings.

## HL1 Movement

**Player → Movement → HL1 Movement** switches the player to Half-Life 1 movement (WON version: no bunnyhop speed cap, holding jump jumps again on landing).

- **Left mouse button:** jump (holding it bunnyhops). Clicking things works as usual. E doesn't jump.
- **Left Ctrl:** duck. **E:** pick up an item or a small piece of furniture (a chair, a small table, a desk) and put it down again. All three keys can be changed next to the switch; the jump key is also next to Jump Anywhere, which uses it too.
- Sprint and stamina are off; the staminometer becomes a speedometer in HL units (320 is normal running speed).
- **E** also carries decorations without colliders: globes, plant pots, papers, lunch trays.
- **Yes clip:** wedge a prop between yourself and a wall (hold it, or walk into one standing against the wall) and keep pushing forward while jumping. After a jump or two the prop pushes you through the wall into the room behind it.
- **Props:** what you let go of falls, slides and comes to rest on whatever is under it; big furniture that doesn't fit where you drop it is pushed out of the wall instead of getting stuck in your hands. Whatever stood on furniture you lift falls off it, items, coins and notebooks included. A prop pushed or thrown into a window breaks it. NPCs walk around furniture you moved, as around a wall.
- The camera is unlocked while it's on; turning HL1 Movement off puts the camera, jumping and speed settings back.

## Cheats and tools

- **Player → Survival:** God Mode (Baldi and the lethal touch of Classic, Yet Again can't catch you), YTP, lives and grade.
- **Items → General:** use items anywhere (in the Pitstop and Johnny's store, out of bounds), a Gravity Gun that may lie on the floor among the other items, and a button that gives one. The Gravity Gun lifts any furniture, lockers and tables included (not activities, notebooks, doors or elevators); using it again throws what it holds, the pick up key (E) puts it down, and a click of the mouse wheel switches turning on: the mouse then turns the held object instead of the view, until the next click; a double click straightens it (upright, squared to a quarter turn). Thrown furniture knocks NPCs aside. It stays in the inventory.
- **Other → World → Johnny's store:** free store, a 100% sale on everything.
- **Other → World → Field trips:** auto-win minigames: a field trip minigame is won perfectly right after its title.
- **Other → World:** solve all activities (math machines, balloon busters, matches, as if the player got them right), reveal the whole map, NPC arrows on the map, each in its character's colour, open, lock or unlock all doors, break all windows, open locked rooms, open all exits, finish the floor.
- **Scenes → Seed:** the current seed, and a fixed seed for the floors loaded from the scene manager.
- **Characters:** Dr. Reflex and The Test.
- **Events:** Lockdown, Mystery Room, Time Out, BalderDash and Student Shuffle, with their settings.
- **Other → Stickers:** the sticker in each of the four slots, glue on each slot, adding stickers to the sticker book.
- **42** (Other) now covers every balloon: math machine balloons all show 42, the balloon buster counts up to 42 of 42, and match pairs are pairs of 42s.
- **Teleporter → Notebooks** works from F2 on: a notebook still in its activity gets the activity solved first.
- **Teleporter → Classes:** a button for every classroom of the floor.
- **Other → World → Elevator:** fast elevator: the elevator between floors (zoom, doors, results board, waits) runs eight times faster.
- **Items → Inventory slot editor** offers the students' lost toys as well.
- **Debug Mode** (Other → Player data, off by default) shows a debug panel: the seed, the cell under the player, the floor's event plan, Baldi's anger.

## Key binds

Right-click any switch, button or choice in the menu and press a key: in the game that key flips the switch, presses the button or moves to the next choice. The key is shown in grey after the name. Right-click again to change it, or press Esc while it waits to remove the bind. Binds don't work while the menu is open. A notice at the top of the screen shows what the key did, for example "God Mode: on".

## Keeping a switch on

Alt+click a switch to keep it on after the game restarts: it turns on, its name turns green, and it is switched on again at every start. Alt+click it again to stop keeping it (it stays as it is).

## Notes

- Saves made with the mod go to a separate profile with the `__ModMenu` suffix, and no records are written.
- Menu settings (open key, language, jump keys, binds, toggles kept on) are in the BepInEx config, `BepInEx/config/kanoyo.plusmodmenu.cfg`. You can edit it while the game is closed.
- Presets, hotkeys and soundfonts live in `Mods/Kanoyo/PlusModMenu` in the game's data folder.

## Building

Needs the .NET SDK (tested with 10) and the game with BepInEx 5 installed.

```
dotnet build PlusModMenu.sln -c Release -p:GamePath="/path/to/Baldi's Basics Plus"
```

The mod is built to `dist/PlusModMenu/PlusModMenu.dll`. Add `-p:DeployToGame=true` to also copy it into the game's `BepInEx/plugins`.

## Credits

Based on [ModMenu by Fasguy](https://github.com/Fasguy/BaldisBasicsModMenu-archive) for Baldi's Basics Classic Remastered.

## License

[MIT](LICENSE). The bundled Newtonsoft.Json and SharpZipLib are under their own MIT licenses, see `lib/`.
