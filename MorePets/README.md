**More Animals** lets you to get more pets, and optionally randomises the appearance of your farm
animals based on the sprites you download.

## Install
1. [Install the latest version of SMAPI](https://smapi.io).
2. [Install Entoarox Framework](https://www.nexusmods.com/stardewvalley/mods/2269).
3. [Install this mod from Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/2274).
4. Run the game using SMAPI.

## Use
### More pets
Just visit the Bus Stop and go to the empty box next to the cart in the top-left.
Look inside the box and perhaps there will be a poor abandoned cat or dog there for you to adopt.
This mod comes bundled with 9 different cat and dog skins (author credits in spoiler) that are
randomly assigned each day. Once you adopt a pet that day, the box will be empty for the remainder
of that day.

The skin and type is randomised based on the date, and reloading your save won't get you a
different one. For each pet you have (including the one Marnie offers), there's a 10% chance (max
90%) for the box to be empty that day.

### Random animal skins
To randomise your farm animal appearances, download PNG or XNB animal sprite mods into this mod's
`assets/skins` folder. When you buy an animal from Marnie, it will be randomly assigned one of the
available skins. When you first install this mod, any existing animals will be randomly assigned
skins.

Each file in `assets/skins` should have a name in the form `<animal type>_<number>.png` or `.xnb`.
The `<number>` can be any number, and is just used to identify this skin so the animals remember
which one they have. The possible animal types are:

animal          | baby type          | adult type
--------------- | ------------------ | ----------
cat             | —                  | `Cat`
dog             | —                  | `Dog`
chicken (blue)  | `BabyBlueChicken`  | `BlueChicken`
chicken (brown) | `BabyBrownChicken` | `BrownChicken`
chicken (void)  | `BabyVoidChicken`  | `VoidChicken`
chicken (white) | `BabyWhiteChicken` | `WhiteChicken`
cow (brown)     | `BabyBrownCow`     | `BrownCow`
cow (white)     | `BabyWhiteCow`     | `WhiteCow`
dinosaur        | —                  | `Dinosaur`
duck            | `BabyDuck`         | `Duck`
goat            | `BabyGoat`         | `Goat`
pig             | `BabyPig`          | `Pig`
rabbit          | `BabyRabbit`       | `Rabbit`
sheep           | `BabySheep`        | `Sheep`<br />`ShearedSheep`

For example, create these files to have three random rabbit types:
```
assets/
   skins/
      BabyRabbit_1.png
      BabyRabbit_2.png
      BabyRabbit_3.png
      Rabbit_1.png
      Rabbit_2.png
      Rabbit_3.png
```

## Compatibility
* For Stardew Valley 1.3.30 or later.
* Compatible with Linux, Mac, or Windows.
* No known mod conflicts.

## See also
* [Release notes](RELEASE-NOTES.md)
* [Nexus mod](http://www.nexusmods.com/stardewvalley/mods/2274)
* [Discussion thread](https://community.playstarbound.com/threads/smapi-more-animals.125946/)
