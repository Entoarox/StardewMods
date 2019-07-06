## Release notes
## 2.4.6
Released 06 July 2019
* Fixed JsonAssets error again, nested interfaces are also invalid

## 2.4.5
Released 04 July 2019
* Fixed a error related to the JsonAssets integration

## 2.4.4
Released 04 July 2019
* Fixed the ICustomItem `TerrainFeature` and `GameLocation` support
* Fixed SMAPI listeners being removed unintentionally
* Fixed a cross-mod API error due to trying to setup to early
* Fixed the serializer detection being inverted, disabling on the wrong platforms

## 2.4.3
Released 04 July 2019
* Updated for the upcoming SMAPI 3.0 again.
* Detect when a SDV version uses a different serializer mechanic and disable the custom serializer on those versions (Android & similar)
* Deprecated the ability to add types to the save serializer
* Added new `world_reset` command with `bushes` and `characters` options, enables reloading relevant data from disk when triggered in a active save.
* Restored some lost ICustomItem supported types (`TerrainFeature` & `GameLocation`), due to technical limits `Building` support is not yet back.

## 2.4.2
Released 28 December 2018. (Thanks to Pathoschild!)

* Updated for the upcoming SMAPI 3.0.
* Removed unused `SkipCredits` feature (see [Skip Intro](https://www.nexusmods.com/stardewvalley/mods/533) instead).
* Fixed EF event marked obsolete when EF itself still uses it.
* Fixed error when warping in multiplayer.

## 2.4.1
Released 23 September 2018. (Thanks to Pathoschild and Slamerz!)

* Updated for Stardew Valley 1.3.
* Fixed Linux/Mac compatibility.
* Fixed compatibility with other mods which change player speed. These will now work so long as you don't also install a mod which changes player speed through the Entoarox Framework API.
* Removed custom update checks; replaced by standard SMAPI update alerts.
* Refactored internally.

## 2.4
Released 10 April 2018.

* Added `ICustomItem` support for buildings and locations. (Thanks to @spacechase0 for testing!)

## 2.3.1
Released 22 March 2018.

* Fixed Linux/Mac compatibility.

## 2.3
Released 22 March 2018.

* Added `ICustomItem` support for objects placed in the world.
* Added `ICustomItem` support for terrain features (like trees).
* Added `IDeserializationHandler` which lets an `ICustomItem` do extra checking on itself and have itself removed from the game completely if something goes wrong. (Due to game limitations, terrain features that cannot be restored at all are _always_ removed.)

## 2.2.2
Released 15 March 2018.

* Fixed error when using `world_bushreset` command when certain mods are installed.

## 2.2.1
Released 15 March 2018.

* Fixed error in `world_bushreset` command.

## 2.2
Released 02 March 2018.

* Added mod-provided API to let other SMAPI mods set player speed through Entoarox Framework.
* Added custom item API (for items in a chest or player inventory only so far).

## 2.1
Released 01 March 2018. (Thanks to Pathoschild!)

* Updated for SMAPI 2.5.
* Updated deprecated code.

## 2.0.6
Released 02 December 2017.

* Fixed issue with version checking.

## 2.0.5
Released 02 December 2017.

* Fixed player modifiers not being applied.

## 2.0.4
Released 02 December 2017.

* Fixed custom events.

## 2.0.3
Released 02 December 2017.

* Fixed content handler crashes in Advanced Location Loader and XNB Loader.

## 2.0.2
Released 01 December 2017.

* Fixed some bugs.
* Temporarily disabled the version checker while maintenance is performed on it.

## 2.0.1
Released 01 December 2017.

* Fixes content handling bug that prevented Advanced Location Loader and XNB Loader from loading XNB files.

## 2.0
Released 01 December 2017.

* Major update to Entoarox Framework; too many changes to list. Not backwards-compatible with older mods.
* Fixed all known bugs.

## 1.8
Released 23 August 2017. (Thanks to Pathoschild!)

* Updated for SMAPI 2.0.
* Rewrote content registry to use SMAPI's content API.
* Updated deprecated code.

## 1.7.10
Released 13 August 2017. (Thanks to Pathoschild!)

* Reconstructed lost code from compiled releases.
* Fixed various bugs.

## 1.7.9
Released 01 May 2017.

* Fixed content handling.

## 1.7.8
Released 01 May 2017.

* Fixed rendering issue. (Thanks to @ekffie for testing!)

## 1.7.7
Released 01 May 2017.

* Fixed bug in 1.7.5 changes.

## 1.7.6
Released 01 May 2017.

* Fixed bug.

## 1.7.5
Released 01 May 2017.

* Fixed bugs in Stardew Valley 1.2.

## 1.7.4
Released 29 April 2017.

* Fixed content handling bugs in Stardew Valley 1.2.

## 1.7.3
Released 27 April 2017.

* Fixed temporary texture disposal issue.

## 1.7.2
Released 27 April 2017.

* Fixed whitelisting for `Tilesheets/tools.xnb` broken in Stardew Valley 1.2.
* Fixed map content handling.
* Fixed temporary content not being interceptable anymore.

## 1.7.1
Released 24 April 2017.

* Fixed new content in Stardew Valley 1.2 not being interceptable.

## 1.7
Released 24 April 2017.

* Added a few trainer-like commands.
* Fixed compatibility with Stardew Valley 1.2.

## 1.6.7
Released 16 April 2017. (Thanks to Pathoschild!)

* Updated for SMAPI 1.9.

## 1.6.6
Released 12 February 2017.

* Added support for intercepting statically-loaded content without needing to manually reload them.

## 1.6.5
Released 08 February 2017.

* Fixed bugs in the UI code.
* Added config option to disable in-game update notifications.

## 1.6.4
Released 05 February 2017.

* Fixed bugs and made changes in the UI code.
* Changed `GameLocation` extension's tilesheet handling behaviour for tile setting to the way its predecessor handled them. 

## 1.6.3
Released 18 January 2017.

* Added a utility method needed by Advanced Location Loader 1.2.3.

## 1.6.2
Released 18 January 2017.

* Improved UI framework internals, should hopefully fix some issues.

## 1.6.1
Released 16 December 2016.

* Fixed error if a texture asset is loaded and unloaded fast enough.

## 1.6
Released 16 December 2016.

* Added a UI framework independent from the game's UI.

## 1.5.1
Released 09 November 2016.

* Updated for SMAPI 1.0.

## 1.5
Released 07 November 2016.

* Added event needed by Furniture Anywhere.

## 1.4.5
Released 07 November 2016.

* Fixed `ActionTriggered` event.
* Fixed right-clicking on the main menu.

## 1.4.4
Released 07 November 2016.

* Fixed init bug in the event registry.

## 1.4.3
Released 06 November 2016.

* Fixed content manager not normalising path separators.

## 1.4.2
Released 05 November 2016.

* Fixed issue in the `MoreEvents` module.

## 1.4.1
Released 04 November 2016.

* Fixed bugs.

## 1.4
Released 03 November 2016.

* Added new features.
* Fixed bugs and save crash.

## 1.3
Released 22 October 2016.

* Added `MessageBox` feature. (Thanks to @Kithio!)
* Added in-game update notifications.
* Temporarily disabled `PlayerHelper` skill experience functionality due to issues for some players.
* Fixed bugs.

## 1.2.3
Released 20 October 2016.

* Fixed issue where skill experience wasn't assigned.

## 1.2.2
Released 19 October 2016.

* Fixed error in `PlayerHelper` if you don't have a food and drink buff.

## 1.2.1
Released 19 October 2016.

* Fixed content registry issue that meant the SMAPI content manager override wasn't getting applied.

## 1.2
Released 19 October 2016.

* Added menu overlay feature. (Thanks to @Pathoschild for letting me include it in the framework!)
* Added a ton of other features.
* Fixed greenhouse warp issue for some players.
* Fixed sound issue.

## 1.1.4
Released 11 October 2016.

* Fixed custom tilesheets not loaded correctly in Advanced Location Loader.

## 1.1.3
Released 09 October 2016.

* Fixed `ReflectionHelper` issue with static methods.

## 1.1.2
Released 08 October 2016.

* Fixed greenhouse warp issue for some players.
* Improved logging.
* Fixed data logged through `DataLogger` not being logged through SMAPI too.

## 1.1.1
Released 04 October 2016.

* Fixed issue with debug mode config. (Thanks to @Thaddel707 for reporting it!)

## 1.1
Released 04 October 2016.

* Added `TypeRegistry` to let you register new types for serialization.
* Added config option to enable debug mode, which adds more detailed log messages.
* Added greenhouse and lighting fixes from Advanced Location Loader. (You can disable them by setting the `GamePatcher` config property to `false`.)
* Added new helper methods to `Entoarox.Framework.Reflection`.
* Added constant for installed Entoarox Framework version.

## 1.0
Released 02 October 2016.
