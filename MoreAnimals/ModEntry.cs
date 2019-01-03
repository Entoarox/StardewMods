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

        /// <summary>The RNG used for assigning a skin to an existing animal.</summary>
        private readonly Random SkinRandom = new Random();


        /*********
        ** Accessors
        *********/
        internal static Random Random;
        internal static ModConfig Config;
        internal static IModHelper SHelper;

        internal static Dictionary<AnimalType, AnimalSkin[]> Indexes = new Dictionary<AnimalType, AnimalSkin[]>();
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

            // add commands
            helper.ConsoleCommands.Add("abandon_pet", "Remove a pet with the given name.", this.OnCommandReceived);
            helper.ConsoleCommands.Add("abandon_all_pets", "Remove all pets adopted using this mod, you monster.", this.OnCommandReceived);

            // load textures
            AnimalSkin[] skins = this.LoadSkins().ToArray();
            ModEntry.Indexes = skins.GroupBy(skin => skin.AnimalType).ToDictionary(group => group.Key, group => group.ToArray());
            foreach (AnimalType type in Enum.GetValues(typeof(AnimalType)))
            {
                if (!ModEntry.Indexes.ContainsKey(type))
                    ModEntry.Indexes[type] = new AnimalSkin[0];
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
                foreach (KeyValuePair<AnimalType, AnimalSkin[]> pair in ModEntry.Indexes)
                {
                    if (pair.Value.Length > 1)
                        summary.AppendLine($"    {pair.Key}: {pair.Value.Length} skins ({string.Join(", ", pair.Value.Select(p => Path.GetFileName(p.AssetKey)).OrderBy(p => p))})");
                }

                this.Monitor.Log(summary.ToString(), LogLevel.Trace);
            }

            // configure bus replacement
            if (ModEntry.Config.AnimalsOnly)
                this.ReplaceBus = false;
            if (this.ReplaceBus && !ModEntry.Indexes[AnimalType.Dog].Any() && !ModEntry.Indexes[AnimalType.Cat].Any())
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
                int index = 0;
                if (parts.Length == 2 && !int.TryParse(parts[1], out index))
                {
                    this.Monitor.Log($"Ignored skin `assets/skins/{file.Name}` with invalid naming convention (can't parse '{parts[1]}' as a number).", LogLevel.Warn);
                    continue;
                }

                // yield
                string assetKey = this.Helper.Content.GetActualAssetKey(Path.Combine("assets", "skins", file.Name));
                yield return new AnimalSkin(type, index, assetKey);
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
                if (pet.Manners > 0 && !pet.updatedDialogueYet)
                {
                    AnimalSkin skin = this.GetSkin(pet);
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
                AnimalSkin skin = this.GetSkin(animal);
                if (skin != null && animal.Sprite.textureName.Value != skin.AssetKey)
                {
                    animal.Sprite = new AnimatedSprite(skin.AssetKey, 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                    animal.meatIndex.Value = skin.ID + 999;
                }
            }
        }

        /// <summary>Get the skin to apply for an animal.</summary>
        /// <param name="npc">The animal to check.</param>
        private AnimalSkin GetSkin(Character npc)
        {
            switch (npc)
            {
                case Pet pet:
                    return this.GetSkin(type: pet is Dog ? AnimalType.Dog : AnimalType.Cat, index: pet.Manners);

                case FarmAnimal animal:
                    {
                        // get type
                        string typeStr = animal.type.Value;
                        if (animal.age.Value < animal.ageWhenMature.Value)
                            typeStr = $"Baby{animal.type.Value}";
                        else if (animal.showDifferentTextureWhenReadyForHarvest.Value && animal.currentProduce.Value <= 0)
                            typeStr = $"Sheared{animal.type.Value}";
                        if (!AnimalSkin.TryParseType(typeStr, out AnimalType type))
                            return null;

                        // get index
                        int index = animal.meatIndex.Value > 999
                            ? animal.meatIndex.Value - 999
                            : -1;

                        // get skin
                        return this.GetSkin(type: type, index: index);
                    }

                default:
                    return null;
            }
        }

        /// <summary>Get the skin to apply for an animal.</summary>
        /// <param name="type">The animal type.</param>
        /// <param name="index">The animal index.</param>
        private AnimalSkin GetSkin(AnimalType type, int index)
        {
            if (!ModEntry.Indexes.TryGetValue(type, out AnimalSkin[] skins) || !skins.Any())
                return null;

            // get assigned skin
            if (index >= 0)
            {
                AnimalSkin skin = skins.FirstOrDefault(p => p.ID == index);
                if (skin != null)
                    return skin;

                this.Monitor.Log($"Found animal type {type} with index {index}, but there's no matching file. Reassigning a random skin to that animal.");
            }

            // get random skin
            return skins[this.SkinRandom.Next(skins.Length)];
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
            ModEntry.Random = new Random(seed);
            List<Pet> list = this.GetAllPets().ToList();
            if (ModEntry.Config.UseMaxAdoptionLimit && list.Count >= ModEntry.Config.MaxAdoptionLimit || ModEntry.Random.NextDouble() < Math.Max(0.1, Math.Min(0.9, list.Count * ModEntry.Config.RepeatedAdoptionPenality)) || list.FindIndex(a => a.Age == seed) != -1)
                Game1.drawObjectDialogue("Just an empty box.");
            else
                AdoptQuestion.Show(this.Helper.Events);
        }
    }
}
