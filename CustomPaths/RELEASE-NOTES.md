## Release notes
## 1.1.3
Not yet released. (Thanks to Pathoschild!)

* Updated for Stardew Valley 1.3.
* Fixed Linux/Mac compatibility.
* Removed custom update checks; replaced by standard SMAPI update alerts.
* Refactored internally.

## 1.1.2
Released 23 March 2018.

* Tweaked tooltip for speed-boosting paths.
* Fixed path pieces near the top of the screen disappearing when partially outside the viewport.

## 1.1.1
Released 22 March 2018.

* Fixed speed boosts not getting applied.

## 1.1
Released 22 March 2018.

* Added support for path speed boosts in `paths.json`. Paths with a speed boost will show the boost in their description.
* Optimised performance:
  * overhauled path connection logic;
  * textures are now only updated as needed;
  * implemented some JavasSript-inspired black magic so that initializing the connection & texture information has no negative impact on the draw loop.
* Fixed paths using weed description.

## 1.0
Released 22 March 2018.
