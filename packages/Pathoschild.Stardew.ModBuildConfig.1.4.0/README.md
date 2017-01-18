**Stardew.ModBuildConfig** is an open-source NuGet package which automates the build configuration
for [Stardew Valley](http://stardewvalley.net/) [SMAPI](https://github.com/Pathoschild/SMAPI) mods.

The package...

* lets you write your mod once, and compile it on any computer. It detects the current platform
  (Linux, Mac, or Windows) and game install path, and injects the right references automatically.
* configures Visual Studio so you can debug into the mod code when the game is running (_Windows
  only_).
* packages the mod automatically into the game's mod folder when you build the code (_optional_).


## Contents
* [Install](#install)
* [Simplify mod development](#simplify-mod-development)
* [Troubleshoot](#troubleshoot)
* [Versions](#versions)

## Install
**When creating a new mod:**

1. Create an empty library project.
2. Reference the [`Pathoschild.Stardew.ModBuildConfig` NuGet package](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig).
3. [Write your code](http://canimod.com/guides/creating-a-smapi-mod).
4. Compile on any platform.

**When migrating an existing mod:**

1. Remove any project references to `Microsoft.Xna.*`, `MonoGame`, Stardew Valley,
   `StardewModdingAPI`, and `xTile`.
2. Reference the [`Pathoschild.Stardew.ModBuildConfig` NuGet package](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig).
3. Compile on any platform.

## Simplify mod development
### Package your mod into the game directory automatically
During development, it's helpful to have the mod files packaged into your `Mods` directory automatically each time you build. To do that:

1. Edit your mod's `.csproj` file.
2. Add this block of code at the bottom, right above the closing `</Project>` tag:

   ```xml
   <Target Name="AfterBuild">
      <PropertyGroup>
         <ModPath>$(GamePath)\Mods\$(TargetName)</ModPath>
      </PropertyGroup>
      <Copy SourceFiles="$(TargetDir)\$(TargetName).dll" DestinationFolder="$(ModPath)" />
      <Copy SourceFiles="$(TargetDir)\$(TargetName).pdb" DestinationFolder="$(ModPath)" Condition="Exists('$(TargetDir)\$(TargetName).pdb')" />
      <Copy SourceFiles="$(TargetDir)\$(TargetName).dll.mdb" DestinationFolder="$(ModPath)" Condition="Exists('$(TargetDir)\$(TargetName).dll.mdb')" />
      <Copy SourceFiles="$(ProjectDir)manifest.json" DestinationFolder="$(ModPath)" />
   </Target>
   ```
3. Optionally, edit the `<ModPath>` value to change the name, or add any additional files your mod needs.

That's it! Each time you build, the files in `<game path>\Mods\<mod name>` will be updated.

### Debug into the mod code
Stepping into your mod code when the game is running is straightforward, since this package injects the configuration automatically. To do it:

1. [Package your mod into the game directory automatically](#package-your-mod-into-the-game-directory-automatically).
2. Launch the project with debugging in Visual Studio or MonoDevelop.

This will deploy your mod files into the game directory, launch SMAPI, and attach a debugger automatically. Now you can step through your code, set breakpoints, etc.

## Troubleshoot
### "Failed to find the game install path"
If you see this error:

> Failed to find the game install path automatically; edit the *.csproj file and manually add a
> &lt;GamePath&gt; setting with the full directory path containing the Stardew Valley executable.

...the package couldn't find your game install path automatically. You'll need to specify the
game location:

1. Get the full folder path containing the Stardew Valley executable.
2. Add this to your `.csproj` file under the `<Project` line (with the correct game path):
   
   ```xml
   <PropertyGroup>
     <GamePath>C:\Program Files (x86)\GalaxyClient\Games\Stardew Valley</GamePath>
   </PropertyGroup>
   ```

The configuration will check your custom path first, then fall back to the default paths (so it'll
still compile on a different computer).

## Versions
1.3:
* Fixed detection of non-default game paths on 32-bit Windows.
* Removed support for SilVerPLuM (discontinued).
* Removed support for overriding the target platform (no longer needed since SMAPI crossplatforms mods automatically).

1.2:
* Added support for non-default game paths on Windows.

1.1:
* Added support for overriding the target platform.

1.0:
* Initial release.
* Added support for detecting the game path automatically.
* Added support for injecting XNA/MonoGame references automatically based on the OS.
* Added support for mod builders like SilVerPLuM.
