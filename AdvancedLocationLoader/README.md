**Advanced Location Loader** lets mods easily add new locations to the game. It provides a
framework for adding or editing locations using only JSON (no programming needed), adds new map
tile actions (see below), and automatically fixes some issues with custom locations (like misplaced
greenhouse warps or in-location warp lighting glitches).

## Install
1. [Install the latest version of SMAPI](https://smapi.io).
2. [Install Entoarox Framework](https://www.nexusmods.com/stardewvalley/mods/2269).
3. [Install this mod from Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/2270).
4. Run the game using SMAPI.
5. Once installed, you can add location mods by unzipping them into `locations` in this mod's folder.

## Instructions for modders
A location mod consists of a folder containing a manifest.json file (see [documented
example](docs/manifest.md)), and XNB files for the custom map data.

This mod adds three new tile actions you can use in maps:

* `ALLShift <x> <y>`  
  Moves the farmer to the (x,y) tile within the current location. Can be used as a `Back` or
  `Buildings` tile action
* `ALLRandomMessage <a>|>b>|<...>`  
  Randomly displays one of the messages given when activated. Can be used as a Buildings tile action.
* `ALLReact <sound> <interval> <a>,<b>,<...>`  
  Plays a sound and replaces the activated tile with a animated one using `<a>,<b>,<...>` as the
  list of tile animations and <interval> for the duration each frame exists. After a single loop,
  the tile is restored to what it was before activation. Can be used as a Buildingstile action.

See also:
* [content pack format](docs/manifest.md)
* [custom actions](docs/actions.md)

## Compatibility
* For Stardew Valley 1.3.30 or later.
* Compatible with Linux, Mac, or Windows.
* No known mod conflicts.

## See also
* [Release notes](RELEASE-NOTES.md)
* [Nexus mod](http://www.nexusmods.com/stardewvalley/mods/2270)
* [Discussion thread](https://community.playstarbound.com/threads/smapi-advanced-location-loader.114124/)
