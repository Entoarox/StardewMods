## Release notes
## 3.0.2
Released 28 march 2019

* Adds french localization.
* Adds spanish localization.
* Fixes a missed issue with xnb skins.

## 3.0.1
Released 18 march 2019.

* Adds italian localization.
* Fixes xnb files not being usable as skins.
* Fixes pet types without skins having a chance of being selected.
* Fixes the pet adoption box disappearing in some situations.

## 3.0
Released 3 March 2019.

* Rewrite of nearly all internal logic, fixing a ton of behavioural bugs in the process.
* *Warning:* This version is not MP compatible!

## 2.1
Released 28 December 2018. (Thanks to Pathoschild!)

* Added `abandon_pet` command to remove a specific pet, and renamed `kill_pets` to `abandon_all_pets` to match.
* Updated for the upcoming SMAPI 3.0.
* Fixed error when warping in multiplayer.

## 2.0.6
Released 03 November 2018. (Thanks to Pathoschild!)

* Fixed asset path error in 2.0.5.
* Fixed pets showing `Cat` or `Dog` as their display name.

## 2.0.5
Released 23 September 2018. (Thanks to Pathoschild and Slamerz!)

* Updated for Stardew Valley 1.3.
* Overhauled skin loading:
  * moved into conventional `assets` subfolder;
  * switched to PNG images by default (XNB files will still work);
  * rewrote skin logic to be more robust and handle more cases;
  * improved error messages;
* Fixed Linux/Mac compatibility.
* Removed custom update checks; replaced by standard SMAPI update alerts.
* Refactored internally.

## 2.0.4
Released 14 March 2018.

* Fixed seasonal tilesheet issue.

## 2.0.3
Released 06 March 2018.

* Fixed skins not being applied.
* Fixed animations not being applied.

## 2.0.2
Released 10 Dec 2017.

* Fixed skins not being applied.

## 2.0.1
Released 01 December 2017.

* Fixed issues in my mod build process that broke most of my mods.

## 2.0
Released 01 December 2017.

* Updated for Entoarox Framework 2.0.
* Added support for animal skins.

## 1.4
Released 23 August 2017. (Thanks to Pathoschild!)

* Updated for SMAPI 2.0.
* Updated deprecated code.
* Refactored how mod loads its custom textures.

## 1.3.2
Released 16 April 2017. (Thanks to Pathoschild!)

* Updated for SMAPI 1.9.

## 1.3.1
Released 12 February 2017.

* Fixed null-pointer error when editing the box tile.

## 1.3
Released 10 February 2017.

* Updated for latest Entoarox Framework and prepared for Stardew Valley 1.2.

## 1.2.1
Released 23 October 2016.

* Fixed adoption dialogue always saying price is 500g, even if you changed it in config.
* Fixed being able to adopt without having enough gold.

## 1.2
Released 19 October 2016.

* Added dependency on Entoarox Framework.
* Fixed pets showing up on the calendar.
* Added update notifications.

## 1.1.1
Released 19 October 2016.

* Fixed crash when you right-click in the title menu.
* Updated `spawn_pet` debug command to let let you spawn pets using any available skin.

## 1.1
Released 18 October 2016.

* Fixed skin detection not working correctly.
* Added config file with various options.
* Added debug commands for troubleshooting.
* Added support for customising the adoption box image (just replace the image in the mod directory).
* Moved pet files into a subfolder.
* Tweaked adoption box placement.

## 1.0
Released 18 October 2016.
