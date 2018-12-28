## Release notes
## 1.4.6
Released 28 December 2018. (Thanks to Pathoschild!)

* Updated for the upcoming SMAPI 3.0.
* Updated translations. Thanks to Nanogamer7 (added German)!
* Fixed error loading shop portraits from content packs.
* Fixed typo.

## 1.4.5
Released 26 September 2018. (Thanks to Pathoschild!)

* Fixed error validating non-seasonal tilesheets in Stardew Valley 1.3.
* Fixed error overriding in-game locations.

## 1.4.4
Released 23 September 2018. (Thanks to Pathoschild!)

* Updated for Stardew Valley 1.3.
* Fixed Linux/Mac compatibility.
* Fixed error loading content packs which specify a format version like `1.2.9` instead of `1.2`.
* Removed custom update checks; replaced by standard SMAPI update alerts.
* Refactored internally.

## 1.4.3
Released 30 April 2018.

* Fixed issues with loader version handling, so both `1.2` and `1.2.0` are seen as valid.

## 1.4.2
Released 15 March 2018.

* Fixed issues with seasonal tilesheets. (Thanks to @13akoors for help with the debugging!)

## 1.4.1
Released 10 March 2018. (Thanks to Pathoschild!)

* Fixed support for legacy content packs.
* Fixed error when a content pack explicitly sets a location manifest field to `null`.
* Fixed error when patching due to a conflict with the save being loaded.

## 1.4
Released 01 March 2018. (Thanks to Pathoschild!)

* Updated for SMAPI 2.5+.
* Added support for standard content packs.
* Fixed deprecated code.

## 1.3.7
Released 26 January 2018.

* Fixed issue where seasonal tilesheets weren't applied if they were the only dynamic content.
* Added more debug logging.

## 1.3.6
Released 02 December 2017.

* Fixed controller support for custom shops.

## 1.3.5
Released 02 December 2017.

* Fixed sanity check for compatibility with mods like XNB Loader.

## 1.3.4
Released 02 December 2017.

* Fixed map object persistence broken in recent updates.

## 1.3.3
Released 02 December 2017.

* Fixed crashes in tilesheet logic.

## 1.3.2
Released 01 December 2017.

* Fixed issues in my mod build process that broke most of my mods.

## 1.3b
Released 01 December 2017.

* Updated for Entoarox Framework 2.0.

## 1.3
Released 23 August 2017. (Thanks to Pathoschild!)

* Updated for SMAPI 1.15.2.
* Added support for SMAPI translation files.
* Fixed deprecated events.

## 1.2.10
Released 16 April 2017. (Thanks to Pathoschild!)

* Updated for SMAPI 1.9.

## 1.2.9
Released 05 February 2017.

* Made some changes to the manifest converter for 1.1 manifests, it should now work with the 1.0+ branch of SMAPI.
* Added `Properties` section to `Tilesheets` entries for adding properties to tilesheets added this way.
* Made `FileName` property for `Tilesheets` entries optional. If omitted, ALL skips tilesheet modification and just applies the contents of `Properties`.
* Changed tile edit logging to be clearer as to what actually caused it to fail.
* Added a small optimisation to the `Teleporters` parsing logic that will have a great impact on parsing times if many Teleporters are created.
* Fixed a major issue with `Teleporters` that caused them to not work at all.

## 1.2.8
Released 21 January 2017.

* Fixed many interrelated bugs.

## 1.2.7
Released 20 January 2017.

* Fixed a bug which broke content packs in some cases.

## 1.2.6
Released 20 January 2017.

* Added handling for tile edits that reference a tilesheet that doesn't exist.
* Fixed issue that broke attempts to remove tiles.

## 1.2.5
Released 19 January 2017.

* Disabled condition collision logic for now pending reimplementation.
* Fixed adding tilesheets for use from within the Manifest only.
* Fixed the game crashing when trying to remove a tile that has already been removed previously.

## 1.2.4
Released 18 January 2017.

* Fixed custom shops added through location mods.

## 1.2.3
Released 18 January 2017.

* Fixed conditional tile edits not applied immediately after loading a save.

## 1.2.2
Released 16 December 2016.

* Fixed Linux/Mac crash due to all tilesheets being considered non-existent.

## 1.2.1
Released 16 December 2016.

* Fixed warp overrides.
* Improved debug logging.

## 1.2
Released 16 December 2016.

* Updated for Entoarox Framework 1.6.
* Rewrote mod from scratch.
* Added support for intercepting load requests for XNB files in the game's `Content` folder.

## 1.1.25
Released 05 November 2016.

* Updated for latest Entoarox Framework.

## 1.1.24
Released 19 October 2016.

* Updated for Entoarox Framework 1.2.

## 1.1.22
Released 10 October 2016.

* Updated for latest Entoarox Framework.

## 1.1.21
Released 04 October 2016.

* Updated for Entoarox Framework 1.1.

## 1.1.20
Released 02 October 2016.

* Updated for Stardew Valley 1.1.
* Added dependency on Entoarox Framework.

## 1.1.13
Released 26 July 2016.

* Fixed bug in lighting code.

## 1.1.12
Released 24 July 2016.

* Fixed issue where NPCs didn't pathfind to custom locations correctly.

## 1.1.11
Released 24 July 2016.

* Fixed issue with TitleMenu being both present and not-present at once.

## 1.1.10
Released 24 July 2016.

* Fixed game locking up for locations that do their own content loading.

## 1.1.9
Released 21 July 2016.

* The mines are now ignored by the in-location warp detection, as CA's modified lighting code for the mines is conflicting with my in-location warp fixes. (That is, you can now see in the mines again.)

## 1.1.8
Released 21 July 2016.

* Fixed the in-game "update available" notification causing the game to lock up.
* The update notification is now prettier, and added some sugar for people in a hurry.

## 1.1.7
Released 20 July 2016.

* Patched in a fix for the lighting glitch that occurs when shifting or warping within a location.
* Added in-game notification for patch integrity errors.
* Added `alwaysPatch` config option that forces ALL's own patches to be applied even if no ALL mod is loaded.
* Added beta version notice when using a beta version of ALL.
* Forced `alwaysPatch` to be `true` no matter what the config file says if the `debugMode` config option is `true`.
* Fixed the greenhouse again, the fix was somehow lost during refactoring.
* Fixed `day 30` detection not detecting a certain edge case.
* Fixed `day 30` detection having false positives.
* Fixed error if ALL can't connect to the version-checking server.
* Fixed log messages not added in the order that ALL outputs them. (This should make logging a lot less confusing.)
* Ignored the config file and set all config options to `true` in beta versions of ALL.

## 1.1.6
Released 11 July 2016.

* Fixed crash when loading a save in any season other then Spring. (Thanks to @Androxilogin for reporting it and @Jinxiewinxie for helping troubleshoot!)

## 1.1.5
Released 11 July 2016.

* Replaced old manifest property-verification code with the new property-verification code everywhere.
* Rewrote quite a few debug messages for greater clarity and more targeted problem solving.
* Slightly improved rendering of stacks in shops.
* Fixed a bug in custom tiles that caused the behaviour of sheeted and non-sheeted tiles to be switched around.

## 1.1.4
Released 10 July 2016.

* Fixed crash if a new save is created while ALL is active.

## 1.1.3
Released 07 July 2016.

* Added the `ALLReact` Building-tile action.
* Fixed custom tilesheets not being handled properly.

## 1.1.2
Released 06 July 2016.

* Improved custom patch algorithm. (Slower PCs should notice a decent improvement in save loading time.)
* Added `ALLRandomMessage` tile action.
* Disable mod if Farmhand is installed, since the mod isn't compatible with Farmhand.
* Added download URL to update notifications.

## 1.1.1
Released 29 May 2016.

* Fixed tilesheets loaded through ALL not work.

## 1.1
Released 24 May 2016.

* Lots of updates and changes.
* Lots more protection against things going wrong.
* Added handling for broken content packs that cause a 'day 30' crash.

## ???
Released 30 April 2016.

* Fixed greenhouse warp issue.
* Fixed ruined-greenhouse still shown after repairing greenhouse.

## ???
Released 30 April 2016.

* Fixed minecarts not always working.
* Fixed greenhouse entrance not being detected properly.
