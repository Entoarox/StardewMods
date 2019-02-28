using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

using Entoarox.Framework;
using Entoarox.Framework.Events;
using Entoarox.MorePetsAndAnimals.Framework;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;

using xTile.Dimensions;
using xTile.Tiles;

using Newtonsoft.Json;

using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.MorePetsAndAnimals
{
    internal class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>Whether to replace the bus on the next opportunity.</summary>
        private bool ReplaceBus = true;
        private bool TriggerAction;

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
        internal static IMonitor SMonitor;
        internal static ModApi Api= new ModApi();

        internal static Dictionary<string, List<AnimalSkin>> Animals = new Dictionary<string, List<AnimalSkin>>();
        internal static Dictionary<string, List<AnimalSkin>> Pets = new Dictionary<string, List<AnimalSkin>>();
        internal static Dictionary<string, Type> PetTypes = new Dictionary<string, Type>();
        internal static Dictionary<Type, string> PetTypesRev = new Dictionary<Type, string>();
        internal static AnimalSkin BabyDuck;
        internal static string[] Seasons = { "spring", "summer", "fall", "winter" };
        internal static bool SkinsReady = false;

        internal static Dictionary<long, int> SkinMap;


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
            ModEntry.SMonitor = this.Monitor;

            // Event listeners
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.SaveLoaded += this.LoadSkinMap;
            helper.Events.GameLoop.Saving += this.SaveSkinMap;
            helper.Events.GameLoop.Saved += this.LoadSkinMap;

            // add commands
            helper.ConsoleCommands.Add("abandon_pet", "Remove a pet with the given name.", this.OnCommandReceived);
            helper.ConsoleCommands.Add("abandon_all_pets", "Remove all pets adopted using this mod, you monster.", this.OnCommandReceived);
            helper.ConsoleCommands.Add("list_animal_types", "Lists all animal types on your farm.", this.OnCommandReceived);
            helper.ConsoleCommands.Add("list_animal_skins", "Lists all animal skins used on your farm.", this.OnCommandReceived);
            helper.ConsoleCommands.Add("reset_animal_skins", "Lists all animal skins used on your farm.", this.OnCommandReceived);

            // Prepare BabyDuck override
            try
            {
                string asset = this.Helper.Content.GetActualAssetKey(Path.Combine("assets", "skins", "BabyDuck.png"));
                Game1.content.Load<Texture2D>(asset);
                BabyDuck = new AnimalSkin("BabyDuck", 0, asset);
            }
            catch (Exception e)
            {
                this.Monitor.Log("Unable to patch BabyDuck due to exception:", LogLevel.Error, e);
            }

            // Register default supported animal types
            Api.RegisterAnimalType("Blue Chicken");
            Api.RegisterAnimalType("Brown Chicken");
            Api.RegisterAnimalType("Brown Cow");
            Api.RegisterAnimalType("Dinosaur", false);
            Api.RegisterAnimalType("Duck");
            Api.RegisterAnimalType("Goat");
            Api.RegisterAnimalType("Pig");
            Api.RegisterAnimalType("Rabbit");
            Api.RegisterAnimalType("Sheep", true, true);
            Api.RegisterAnimalType("Void Chicken");
            Api.RegisterAnimalType("White Chicken");
            Api.RegisterAnimalType("White Cow");

            if(Config.ExtraTypes!=null && Config.ExtraTypes.Length>0)
            foreach (string type in Config.ExtraTypes)
                Api.RegisterAnimalType(type, false);

            // Register default supported pet types
            Api.RegisterPetType("cat", typeof(Cat));
            Api.RegisterPetType("dog", typeof(Dog));

            // configure bus replacement
            if (ModEntry.Config.AnimalsOnly)
                this.ReplaceBus = false;
            if(this.ReplaceBus)
            {
                try
                {

                    string asset = this.Helper.Content.GetActualAssetKey(Path.Combine("assets", "box.png"));
                    Game1.content.Load<Texture2D>(asset);
                }
                catch(Exception e)
                {
                    this.ReplaceBus = false;
                    this.Monitor.Log("Unable to patch BusStop due to exception:", LogLevel.Error, e);
                }
            }
        }

        public override object GetApi()
        {
            return Api;
        }

        internal static string Sanitize(string input)
        {
            input = input.ToLower().Replace(" ", "");
            return string.IsInterned(input) ?? input;
        }


        /*********
        ** Protected methods
        *********/
        /// <summary>Get all pets in the game.</summary>
        internal static IEnumerable<Pet> GetAllPets()
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

        private void LoadSkins()
        {
            List<string> types = new List<string>();
            types.AddRange(Animals.Keys);
            types.AddRange(Pets.Keys);
            string validTypes = string.Join(", ", types);
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
                string type = Sanitize(parts[0]);
                if(!Animals.ContainsKey(type) && !Pets.ContainsKey(type))
                {
                    this.Monitor.Log($"Ignored skin `assets/skins/{file.Name}` with invalid naming convention (can't parse '{parts[0]}' as an animal or pet type, expected one of {validTypes}).", LogLevel.Warn);
                    continue;
                }
                int index = 0;
                if(parts.Length!=2 && type!="babyduck")
                {
                    this.Monitor.Log($"Ignored skin `assets/skins/{file.Name}` with invalid naming convention (no skin ID found).", LogLevel.Warn);
                    continue;
                }
                if (parts.Length == 2 && !int.TryParse(parts[1], out index))
                {
                    this.Monitor.Log($"Ignored skin `assets/skins/{file.Name}` with invalid naming convention (can't parse '{parts[1]}' as a number).", LogLevel.Warn);
                    continue;
                }

                // yield
                string assetKey = this.Helper.Content.GetActualAssetKey(Path.Combine("assets", "skins", file.Name));
                if (Animals.ContainsKey(type))
                    Animals[type].Add(new AnimalSkin(type, index, assetKey));
                else
                    Pets[type].Add(new AnimalSkin(type, index, assetKey));
            }
            StringBuilder summary = new StringBuilder();
            summary.AppendLine(
                "Statistics:\n"
                + "  Config:\n" + JsonConvert.SerializeObject(ModEntry.Config, new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,

                }).Replace("\n","\n   ")
                + "\n  Registered types: "+validTypes
                + "\n  Animal Skins:"
            );
            foreach (KeyValuePair<string, List<AnimalSkin>> pair in ModEntry.Animals)
            {
                if (pair.Value.Count > 0)
                    summary.AppendLine($"    {pair.Key}: {pair.Value.Count} skins ({string.Join(", ", pair.Value.Select(p => Path.GetFileName(p.AssetKey)).OrderBy(p => p))})");
            }
            summary.AppendLine("  Pet Skins:");
            foreach (KeyValuePair<string, List<AnimalSkin>> pair in ModEntry.Pets)
            {
                if (pair.Value.Count > 0)
                    summary.AppendLine($"    {pair.Key}: {pair.Value.Count} skins ({string.Join(", ", pair.Value.Select(p => Path.GetFileName(p.AssetKey)).OrderBy(p => p))})");
            }
            this.Monitor.Log(summary.ToString(), LogLevel.Trace);
            if (this.ReplaceBus && !Pets.Any(a => a.Value.Count!=0))
            {
                this.ReplaceBus = false;
                this.Monitor.Log($"The `{nameof(ModConfig.AnimalsOnly)}` config option is set to false, but no pet skins were found!", LogLevel.Error);
            }
            SkinsReady = true;
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            if (!SkinsReady)
                this.LoadSkins();

            // patch bus stop
            if (this.ReplaceBus && Game1.getLocationFromName("BusStop") != null)
            {
                MoreEvents.ActionTriggered += this.OnActionTriggered;
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
            foreach (Pet pet in GetAllPets())
            {
                if (pet.Manners > 0)
                {
                    AnimalSkin skin = this.GetSkin(pet);
                    if (skin != null && pet.Sprite.textureName.Value!=skin.AssetKey)
                    {
                        pet.Sprite = new AnimatedSprite(skin.AssetKey, 0, 32, 32);
                        pet.Manners = skin.ID;
                    }
                }
            }

            // set farm animal skins
            foreach (FarmAnimal animal in this.GetFarmAnimals())
            {
                AnimalSkin skin = this.GetSkin(animal);
                if (skin != null && animal.Sprite.textureName.Value != skin.AssetKey)
                {
                    animal.Sprite = new AnimatedSprite(skin.AssetKey, 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                    SkinMap[animal.myID.Value] = skin.ID;
                }
                else if (skin == null)
                    SkinMap[animal.myID.Value] = 0;
            }
        }

        /// <summary>Get the skin to apply for an animal.</summary>
        /// <param name="npc">The animal to check.</param>
        private AnimalSkin GetSkin(Character npc)
        {
            switch (npc)
            {
                case Pet pet:
                    if (pet.Manners == 0)
                        return null;
                    return this.GetSkin(type: Sanitize(pet.GetType().Name), index: pet.Manners);

                case FarmAnimal animal:
                    {
                        // get type
                        string typeStr = animal.type.Value;
                        if (animal.age.Value < animal.ageWhenMature.Value)
                            typeStr = $"Baby{animal.type.Value}";
                        else if (animal.showDifferentTextureWhenReadyForHarvest.Value && animal.currentProduce.Value <= 0)
                            typeStr = $"Sheared{animal.type.Value}";
                        typeStr = Sanitize(typeStr);
                        if (!Animals.ContainsKey(typeStr))
                            return null;
                        // get index
                        int index = SkinMap.ContainsKey(animal.myID.Value) ? SkinMap[animal.myID.Value] : -1;
                        if (index == 0 && typeStr == "babyduck" && BabyDuck != null)
                            return BabyDuck;
                        if (index == 0)
                            return null;
                        // get skin
                        return this.GetSkin(type: typeStr, index: index);
                    }

                default:
                    return null;
            }
        }

        /// <summary>Get the skin to apply for an animal.</summary>
        /// <param name="type">The animal type.</param>
        /// <param name="index">The animal index.</param>
        private AnimalSkin GetSkin(string type, int index)
        {
            type = Sanitize(type);
            if (!ModEntry.Animals.TryGetValue(type, out List<AnimalSkin> skins) && !ModEntry.Pets.TryGetValue(type, out skins))
                return null;
            // get assigned skin
            if (index >= 1)
            {
                AnimalSkin skin = skins.FirstOrDefault(p => p.ID == index);
                if (skin != null)
                    return skin;

                this.Monitor.Log($"Found animal of type `{type}` with skin index {index}, but there's no matching file. Randomly assigning a new skin.", LogLevel.Warn);
            }

            // get random skin
            int skinID = ModEntry.Animals.ContainsKey(type) ? this.SkinRandom.Next(skins.Count + 1) - 1 : this.SkinRandom.Next(skins.Count);
            return skinID >= 0 ? skins[skinID] : null;
        }

        private void LoadSkinMap(object s, EventArgs e)
        {
            SkinMap = this.Helper.Data.ReadSaveData<Dictionary<long, int>>("animal-skins") ?? new Dictionary<long, int>();
        }

        private void SaveSkinMap(object s, EventArgs e)
        {
            this.Helper.Data.WriteSaveData("animal-skins", SkinMap);
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
                case "list_animal_types":
                    List<string> types = new List<string>();
                    foreach(FarmAnimal animal in GetFarmAnimals())
                    {
                        string type = Sanitize(animal.type.Value);
                        if (!types.Contains(type))
                            types.Add(type);
                    }
                    this.Monitor.Log($"Found animal types: {string.Join(", ", types)}", LogLevel.Info);
                    break;
                case "list_animal_skins":
                    List<string> skins = new List<string>();
                    foreach (FarmAnimal animal in GetFarmAnimals())
                    {
                        string type = Sanitize(animal.type.Value) + ':' + (SkinMap.ContainsKey(animal.myID.Value) ? SkinMap[animal.myID.Value] : 0);
                        if (!skins.Contains(type))
                            skins.Add(type);
                    }
                    this.Monitor.Log($"Found animal skins: {string.Join(", ", skins)}", LogLevel.Info);
                    break;
                case "reset_animal_skins":
                    SkinMap.Clear();
                    this.Monitor.Log($"Skins for all animals reset, new skins will be assigned automatically.", LogLevel.Info);
                    break;
                default:
                    this.Monitor.Log($"Unknown command '{command}'.", LogLevel.Error);
                    break;
            }
        }

        private void OnActionTriggered(object sender, EventArgsActionTriggered e)
        {
            if (e.Action.Equals("MorePetsAdoption"))
            {
                int seed = Game1.year * 1000 + Array.IndexOf(ModEntry.Seasons, Game1.currentSeason) * 100 + Game1.dayOfMonth;
                if (Config.DisableDailyLimit)
                    ModEntry.Random = new Random();
                else
                    ModEntry.Random = new Random(seed);
                List<Pet> list = GetAllPets().ToList();
                if (ModEntry.Config.UseMaxAdoptionLimit && list.Count >= ModEntry.Config.MaxAdoptionLimit || ModEntry.Random.NextDouble() < Math.Max(0.1, Math.Min(0.9, list.Count * ModEntry.Config.RepeatedAdoptionPenality)) || (!Config.DisableDailyLimit && list.FindIndex(a => a.Age == seed) != -1))
                    Game1.drawObjectDialogue(this.Helper.Translation.Get("EmptyBox"));
                else
                    AdoptQuestion.Show();
            }
        }
    }
}
