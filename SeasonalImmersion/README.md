**Seasonal Immersion** changes buildings to fit the current season. Buildings can be snow-covered
in winter, overgrown with vines in summer, etc.

## Install
1. [Install the latest version of SMAPI](https://smapi.io).
2. [Install this mod from Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/2273).
3. Download a content pack (see below).
4. 
   **On Windows:** copy the ContentPack.zip into the mod's folder.  
   **On Linux or Mac:** unzip the ContentPack.zip into the mod's folder.
5. Run the game using SMAPI.

Suggested content packs (you can only install one):
* [Hobbit House](http://community.playstarbound.com/attachments/contentpack-zip.168340) by [LemonEX](http://community.playstarbound.com/members/lemonex.28207/) (see [example](http://i.imgur.com/NrJm1IV.png))
* [Lumisteria Houses](http://community.playstarbound.com/resources/pans-seasonal-buildings.4310/download?version=19177) by [Panwicker](http://community.playstarbound.com/members/panwicker.673982/) (see [example](http://community.playstarbound.com/threads/pans-seasonal-houses.126329/#post-3053111) or [credit](http://www.nexusmods.com/stardewvalley/mods/196))
* [Elle's Seasonal Buildings](https://www.nexusmods.com/stardewvalley/mods/1993)﻿ by [junimods](http://community.playstarbound.com/members/junimods.733912/) (see [example and credit](http://community.playstarbound.com/threads/elles-seasonal-non-seasonal-building-replacements.127106/))

## Create a content pack
To create a new content pack:
1. Create four folders under the mod's folder: `spring`, `summer`, `fall`, and `winter`.
2. Add PNGs with the same name as the content to replace in each folder. For example, add `Mill.png`
   to override the `Content/Buildings/Mill.xnb` file.
3. You can do this for any file under `Content/Buildings`, `TerrainFeatures/Flooring`, and
   `TileSheets/Craftables`. For craftables only, you can add `Craftables_indoors.png` or
   `Craftables_outdoors.png` (or both); if you add `Craftables.png`, it'll apply both indoors and
   outdoors.
4. Create a `manifest.json` file next to the season folders with this content (replace all-caps text
   with your not-all-caps information):

   ```json
   {
     "Name": "MOD NAME HERE",
     "Author": "AUTHOR NAME HERE",
     "Version":"1.0.0",
     "Description": "BRIEF DESCRIPTION HERE",
   }
   ```
5. Optionally zip it into a `ContentPack.zip` folder (with the `manifest.json` and season folders
   at the root) to share.

## Compatibility
* For Stardew Valley 1.3.30 or later.
* Compatible with Linux, Mac, or Windows.
* No known mod conflicts.

## See also
* [Release notes](RELEASE-NOTES.md)
* [Nexus mod](http://www.nexusmods.com/stardewvalley/mods/2273)
* [Discussion thread](https://community.playstarbound.com/threads/smapi-seasonal-immersion.125683/)
