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

## Configure
The mod creates a `config.json` file in its mod folder the first time you run it. You can open that
file in a text editor to configure the mod.

These are the available settings:

setting                    | what it affects
-------------------------- | -------------------
`AnimalsOnly`              | True to disable the pet adoption box. Default false.
`AdoptionPrice`            | The gold you must pay to adopt a pet. Default 500, minimum 100.
`MaxAdoptionLimit`         | If `UseMaxAdoptionLimit` is true, the maximum number of pets you can adopt (including the original pet). Default 10.
`UseMaxAdoptionLimit`      | Whether to apply the `MaxAdoptionLimit` limit. Default false.
`RepeatedAdoptionPenality` | A penalty which reduces the chance of a pet in the adoption box based on the number of pets you already have. (The default pet-in-box chance is 90%.) For example, `0.1` represents a 10% penalty per pet; after 5 pets, the chance is 90% - 50% or 40%. Note that this won't reduce the pet-in-box chance lower 10%. Default 0.1, must be 0–0.9 inclusive.
`UseBalancedDistribution`  | Whether to try to assign an even distribution of skins and pet types, if possible. Default false.

## Compatibility
* For Stardew Valley 1.3.30 or later.
* Compatible with Linux, Mac, or Windows.
* No known mod conflicts.

## See also
* [Release notes](RELEASE-NOTES.md)
* [Nexus mod](http://www.nexusmods.com/stardewvalley/mods/2274)
* [Discussion thread](https://community.playstarbound.com/threads/smapi-more-animals.125946/)
