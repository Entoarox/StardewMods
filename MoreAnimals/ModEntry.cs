using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Entoarox.Framework;
using Entoarox.MorePetsAndAnimals.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using xTile.Dimensions;
using xTile.Tiles;

namespace Entoarox.MorePetsAndAnimals
{
    internal class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>Whether to replace the bus on the next opportunity.</summary>
        private bool ReplaceBus = true;

        /// <summary>The file extensions recognised by the mod.</summary>
        private readonly HashSet<string> ValidExtensions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            ".png",
            ".xnb"
        };

        /// <summary>Handles choosing from a set of available values.</summary>
        private Chooser Chooser;


        /*********
        ** Accessors
        *********/
        internal static ModConfig Config;
        internal static IModHelper SHelper;

        internal static Dictionary<AnimalType, AnimalSkin[]> Skins = new Dictionary<AnimalType, AnimalSkin[]>();
        internal static string[] Seasons = { "spring", "summer", "fall", "winter" };


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // init
            ModEntry.Config = helper.ReadConfig<ModConfig>();
            ModEntry.SHelper = helper;
            this.Chooser = new Chooser();

            // add commands
            helper.ConsoleCommands.Add("abandon_pet", "Remove a pet with the given name.", this.OnCommandReceived);
            helper.ConsoleCommands.Add("abandon_all_pets", "Remove all pets adopted using this mod, you monster.", this.OnCommandReceived);

            // load textures
            AnimalSkin[] skins = this.LoadSkins().ToArray();
            ModEntry.Skins = skins.GroupBy(skin => skin.AnimalType).ToDictionary(group => group.Key, group => group.ToArray());
            foreach (AnimalType type in Enum.GetValues(typeof(AnimalType)))
            {
                if (!ModEntry.Skins.ContainsKey(type))
                    ModEntry.Skins[type] = new AnimalSkin[0];
            }

            // print skin summary
            {
                StringBuilder summary = new StringBuilder();
                summary.AppendLine(
                    "Statistics:\n"
                    + "  Config:\n"
                    + $"    AdoptionPrice: {ModEntry.Config.AdoptionPrice}\n"
                    + $"    RepeatedAdoptionPenality: {ModEntry.Config.RepeatedAdoptionPenality}\n"
                    + $"    UseMaxAdoptionLimit: {ModEntry.Config.UseMaxAdoptionLimit}\n"
                    + $"    MaxAdoptionLimit: {ModEntry.Config.MaxAdoptionLimit}\n"
                    + $"    AnimalsOnly: {ModEntry.Config.AnimalsOnly}\n"
                    + "  Skins:"
                );
                foreach (KeyValuePair<AnimalType, AnimalSkin[]> pair in ModEntry.Skins)
                {
                    if (pair.Value.Length > 1)
                        summary.AppendLine($"    {pair.Key}: {pair.Value.Length} skins ({string.Join(", ", pair.Value.Select(p => Path.GetFileName(p.AssetKey)).OrderBy(p => p))})");
                }

                this.Monitor.Log(summary.ToString(), LogLevel.Trace);
            }

            // configure bus replacement
            if (ModEntry.Config.AnimalsOnly)
                this.ReplaceBus = false;
            if (this.ReplaceBus && !ModEntry.Skins[AnimalType.Dog].Any() && !ModEntry.Skins[AnimalType.Cat].Any())
            {
                this.ReplaceBus = false;
                this.Monitor.Log($"The `{nameof(ModConfig.AnimalsOnly)}` config option is set to false, but no dog or cat skins were found!", LogLevel.Error);
            }

            // hook events
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            if (this.ReplaceBus)
            {
                helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            }
        }


        /*********
        ** Protected methods
        *********/
        /// <summary>Get all pets in the game.</summary>
        private IEnumerable<Pet> GetAllPets()
        {
            foreach (NPC npc in Utility.getAllCharacters())
            {
                if (npc is Pet pet)
                    yield return pet;
            }
        }

        /// <summary>Get all farm animals in the game.</summary>
        private IEnumerable<FarmAnimal> GetFarmAnimals()
        {
            Farm farm = Game1.getFarm();
            if (farm == null)
                yield break;

            foreach (FarmAnimal animal in farm.getAllFarmAnimals())
                yield return animal;
        }

        private IEnumerable<AnimalSkin> LoadSkins()
        {
            foreach (FileInfo file in new DirectoryInfo(Path.Combine(this.Helper.DirectoryPath, "assets", "skins")).EnumerateFiles())
            {
                // check extension
                string extension = Path.GetExtension(file.Name);
                if (!this.ValidExtensions.Contains(extension))
                {
                    this.Monitor.Log($"Ignored skin `assets/skins/{file.Name}` with invalid extension (must be one of {string.Join(", ", this.ValidExtensions)}).", LogLevel.Warn);
                    continue;
                }

                // parse name
                string[] parts = Path.GetFileNameWithoutExtension(file.Name).Split(new[] { '_' }, 2);
                if (!AnimalSkin.TryParseType(parts[0], out AnimalType type))
                {
                    this.Monitor.Log($"Ignored skin `assets/skins/{file.Name}` with invalid naming convention (can't parse '{parts[0]}' as an animal type, expected one of {string.Join(", ", Enum.GetNames(typeof(AnimalType)))}).", LogLevel.Warn);
                    continue;
                }
                int skinID = 0;
                if (parts.Length == 2 && !int.TryParse(parts[1], out skinID))
                {
                    this.Monitor.Log($"Ignored skin `assets/skins/{file.Name}` with invalid naming convention (can't parse '{parts[1]}' as a number).", LogLevel.Warn);
                    continue;
                }

                // yield
                string assetKey = this.Helper.Content.GetActualAssetKey(Path.Combine("assets", "skins", file.Name));
                yield return new AnimalSkin(type, skinID, assetKey);
            }
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            // patch bus stop
            if (this.ReplaceBus && Game1.getLocationFromName("BusStop") != null)
            {
                this.Monitor.Log("Patching bus stop...", LogLevel.Trace);
                GameLocation bus = Game1.getLocationFromName("BusStop");
                bus.map.AddTileSheet(new TileSheet("MorePetsTilesheet", bus.map, this.Helper.Content.GetActualAssetKey("assets/box.png"), new Size(2, 2), new Size(16, 16)));
                bus.SetTile(1, 2, "Front", 0, "MorePetsTilesheet");
                bus.SetTile(2, 2, "Front", 1, "MorePetsTilesheet");
                bus.SetTile(1, 3, "Buildings", 2, "MorePetsTilesheet");
                bus.SetTile(2, 3, "Buildings", 3, "MorePetsTilesheet");
                bus.SetTileProperty(1, 3, "Buildings", "Action", "MorePetsAdoption");
                bus.SetTileProperty(2, 3, "Buildings", "Action", "MorePetsAdoption");
                this.ReplaceBus = false;
            }

            // set pet skins
            foreach (Pet pet in this.GetAllPets())
            {
                if (pet.Manners <= 0)
                    continue; // vanilla pet

                if (!pet.updatedDialogueYet)
                {
                    AnimalSkin skin = this.GetSkin(pet) ?? this.ChooseRandomSkin(pet);
                    if (skin != null)
                    {
                        pet.Sprite = new AnimatedSprite(skin.AssetKey, 0, 32, 32);
                        pet.Manners = skin.ID;
                    }
                    pet.updatedDialogueYet = true;
                }
            }

            // set farm animal skins
            foreach (FarmAnimal animal in this.GetFarmAnimals())
            {
                if (animal.meatIndex.Value <= 999)
                    continue; // vanilla animal

                AnimalSkin skin = this.GetSkin(animal) ?? this.ChooseRandomSkin(animal);
                if (skin != null && animal.Sprite.textureName.Value != skin.AssetKey)
                {
                    animal.Sprite = new AnimatedSprite(skin.AssetKey, 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                    animal.meatIndex.Value = skin.ID + 999;
                }
            }
        }

        /// <summary>Get the animal type for a character.</summary>
        /// <param name="npc">The animal to check.</param>
        /// <returns>Returns the animal type if valid, else null.</returns>
        private AnimalType? GetType(Character npc)
        {
            switch (npc)
            {
                case Cat _:
                    return AnimalType.Cat;

                case Dog _:
                    return AnimalType.Dog;

                case FarmAnimal animal:
                    {
                        // get type string
                        string typeStr = animal.type.Value;
                        if (animal.age.Value < animal.ageWhenMature.Value)
                            typeStr = $"Baby{animal.type.Value}";
                        else if (animal.showDifferentTextureWhenReadyForHarvest.Value && animal.currentProduce.Value <= 0)
                            typeStr = $"Sheared{animal.type.Value}";

                        // parse type
                        return AnimalSkin.TryParseType(typeStr, out AnimalType type)
                            ? type
                            : (AnimalType?)null;
                    }

                default:
                    return null;
            }
        }

        /// <summary>Get the current skin for an animal.</summary>
        /// <param name="npc">The animal to check.</param>
        private AnimalSkin GetSkin(Character npc)
        {
            // get type
            AnimalType? type = this.GetType(npc);
            if (type == null)
                return null;

            switch (npc)
            {
                case Pet pet:
                    return this.GetSkin(type.Value, skinID: pet.Manners);

                case FarmAnimal animal:
                    if (animal.meatIndex.Value <= 999)
                        return null;

                    return this.GetSkin(type.Value, skinID: animal.meatIndex.Value - 999);

                default:
                    return null;
            }
        }

        /// <summary>Get the current skin for an animal.</summary>
        /// <param name="type">The animal type.</param>
        /// <param name="skinID">The skin ID.</param>
        private AnimalSkin GetSkin(AnimalType type, int skinID)
        {
            if (!ModEntry.Skins.TryGetValue(type, out AnimalSkin[] skins) || !skins.Any())
                return null;

            return skinID >= 0
                ? skins.FirstOrDefault(p => p.ID == skinID)
                : null;
        }

        /// <summary>Choose a random skin for an animal.</summary>
        /// <param name="npc">The animal.</param>
        private AnimalSkin ChooseRandomSkin(Character npc)
        {
            AnimalType? type = this.GetType(npc);
            return type != null
                ? this.ChooseRandomSkin(type.Value)
                : null;
        }

        /// <summary>Choose a random skin for an animal type.</summary>
        /// <param name="type">The animal type.</param>
        private AnimalSkin ChooseRandomSkin(AnimalType type)
        {
            if (!ModEntry.Skins.TryGetValue(type, out AnimalSkin[] skins) || !skins.Any())
                return null;

            int skinID = this.Chooser.Choose(
                options: skins.Select(p => p.ID).ToArray(),
                previous: () => this.GetCurrentSkins(type)
            );
            return this.GetSkin(type, skinID);
        }

        /// <summary>Get the skins currently assigned to animals in the world.</summary>
        /// <param name="type">The animal type to check.</param>
        private IEnumerable<int> GetCurrentSkins(AnimalType type)
        {
            switch (type)
            {
                case AnimalType.Cat:
                case AnimalType.Dog:
                    return this
                        .GetAllPets()
                        .Where(pet => this.GetType(pet) == type)
                        .Select(pet => pet.Manners);

                default:
                    return this
                        .GetFarmAnimals()
                        .Where(animal => this.GetType(animal) == type)
                        .Select(p => p.meatIndex.Value);
            }
        }

        /// <summary>Raised after the player enters a command for this mod in the SMAPI console.</summary>
        /// <param name="command">The command name.</param>
        /// <param name="args">The command arguments.</param>
        private void OnCommandReceived(string command, string[] args)
        {
            switch (command)
            {
                case "abandon_pet":
                case "abandon_all_pets":
                    {
                        // validate
                        if (command == "abandon_pet" && args.Length == 0)
                        {
                            this.Monitor.Log("You must specify a pet name to remove.", LogLevel.Warn);
                            return;
                        }

                        // get pet selector
                        Func<Pet, bool> removePet = pet => pet.Age > 0;
                        if (command == "abandon_pet")
                            removePet = pet => string.Equals(pet.Name, args[0], StringComparison.InvariantCultureIgnoreCase);

                        // remove matching pet
                        int found = 0;
                        foreach (GameLocation location in Game1.locations)
                        {
                            if (location is Farm || location is FarmHouse)
                            {
                                location.characters.Filter(npc =>
                                {
                                    bool remove = npc is Pet pet && removePet(pet);
                                    if (remove)
                                    {
                                        this.Monitor.Log($"Removing {npc.Name}...", LogLevel.Info);
                                        found++;
                                    }

                                    return !remove;
                                });
                            }
                        }

                        this.Monitor.Log(found > 0 ? $"Done! Removed {found} pets." : "No matching pets found.", LogLevel.Info);
                    }
                    break;

                default:
                    this.Monitor.Log($"Unknown command '{command}'.", LogLevel.Error);
                    break;
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsPlayerFree && e.Button.IsActionButton() && Game1.currentLocation.doesTileHaveProperty((int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y, "Action", "Buildings") == "MorePetsAdoption")
                this.InteractWithBox();
        }

        /// <summary>Handle user interaction with the adoption box.</summary>
        private void InteractWithBox()
        {
            int seed = Game1.year * 1000 + Array.IndexOf(ModEntry.Seasons, Game1.currentSeason) * 100 + Game1.dayOfMonth;
            this.Chooser.Reset(seed);
            List<Pet> list = this.GetAllPets().ToList();
            if (ModEntry.Config.UseMaxAdoptionLimit && list.Count >= ModEntry.Config.MaxAdoptionLimit || this.Chooser.Random.NextDouble() < Math.Max(0.1, Math.Min(0.9, list.Count * ModEntry.Config.RepeatedAdoptionPenality)) || list.FindIndex(a => a.Age == seed) != -1)
                Game1.drawObjectDialogue("Just an empty box.");
            else
                AdoptQuestion.Show(this.Helper.Events, this.GetAllPets().ToArray(), this.Chooser);
        }
    }
}
