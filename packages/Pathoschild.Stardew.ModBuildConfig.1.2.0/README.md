**Stardew.ModBuildConfig** is an open-source NuGet package which automates the build configuration
for crossplatform [Stardew Valley](http://stardewvalley.net/) mods that use SMAPI.

## Usage
Basically this package lets you write your mod once, and compile it on any computer. It detects
your current platform (Linux, Mac, or Windows) and game path, and injects the right references
automatically. You can also target a specific platform to create a mod package compatible with that
platform.

More specifically, the configuration...

1. detects the operating system and Stardew Valley path;
2. injects the right references to Stardew Valley, SMAPI, and XNA/MonoGame for your platform;
3. configures Visual Studio so you can launch the game for debugging (_Windows only_);
4. and adds a `GamePath` variable which can be used to script mod packaging if desired.

## Installation
### Creating a new mod
1. Create an empty library project.
2. Reference the [`Pathoschild.Stardew.ModBuildConfig` NuGet package](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig).
3. [Write your code](http://canimod.com/guides/creating-a-smapi-mod).
4. Compile on any platform.

### Migrating an existing mod
1. Remove any references to `Microsoft.Xna.*`, Stardew Valley, `StardewModdingAPI`, and xTile.
2. Reference the [`Pathoschild.Stardew.ModBuildConfig` NuGet package](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig).
3. Compile on any platform.

## Configuration
### Custom game path
If you customised where Stardew Valley is installed, you can specify where it is.

1. Get the full path to the directory containing the Stardew Valley executable.
2. Add this section to your `.csproj` file (anywhere before the added `<Import` line):
   
   ```
   <PropertyGroup>
     <GamePath>C:\Program Files (x86)\GalaxyClient\Games\Stardew Valley</GamePath>
   </PropertyGroup>
   ```

The configuration will check your custom path first, then fall back to the default paths. (That way
you can still compile it normally on a different computer.)

### Target platform
By default the build configuration will target your current platform (e.g. Linux, Mac, or Windows).
If you're compiling it for a different platform (and have the required dependencies installed), you
can manually override the platform detection.

You can define it...

* in your `.csproj` (anywhere before the added `<Import` line). Valid values are `Linux`, `Mac`, or
  `Windows`.
   
   ```
   <PropertyGroup>
     <GamePlatform>Windows</GamePlatform>
   </PropertyGroup>
   ```

* _or_ by setting one of these compile constant: `GAME_PLATFORM_LINUX`, `GAME_PLATFORM_MAC`, or
  `GAME_PLATFORM_WINDOWS`.
  * <small>In Visual Studio: right-click on the project and choose _Properties_. Click the _Build_
    tab, and enter the constants into the _Conditional compilation symbols_ field.</small>
  * <small>In MonoDevelop: right-click on the project and choose _Options_. Click the
    _Build » Compiler_ tab, and enter the constants into the _Define Symbols_ field.</small>

### Compatibility with mod builders
The configuration is designed for compatibility with third-party mod compilers. [Silverplum](https://github.com/rumangerst/SilVerPLuM)
is officially supported, and mod builds can set the following environment variables:

* `GAMEPATH`: overrides the Stardew Valley install path.
* `GAMEPLATFORM`: overrides the detected platform. Should be only of `Linux`, `Mac`, or `Windows`.

## Versions
* 1.0: initial release.
* 1.1: added support for targeting platforms.

## See also
* [NuGet package](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig)
* <s>Discussion thread</s>