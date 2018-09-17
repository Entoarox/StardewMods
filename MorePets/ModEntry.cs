using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entoarox.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;

namespace Entoarox.MorePetsAndAnimals
{
    internal class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        private static bool ReplaceBus = true;
        private bool TriggerAction;


        /*********
        ** Accessors
        *********/
        internal static Random Random;
        internal static ModConfig Config;
        internal static IModHelper SHelper;
        internal static Dictionary<string, List<int>> Indexes = new Dictionary<string, List<int>>
        {
            ["BabyBlue Chicken"] = new List<int>(),
            ["BabyBrown Chicken"] = new List<int>(),
            ["BabyBrown Cow"] = new List<int>(),
            ["BabyCow"] = new List<int>(),
            ["BabyGoat"] = new List<int>(),
            ["BabyPig"] = new List<int>(),
            ["BabyRabbit"] = new List<int>(),
            ["BabySheep"] = new List<int>(),
            ["BabyVoid Chicken"] = new List<int>(),
            ["BabyWhite Chicken"] = new List<int>(),
            ["BabyWhite Cow"] = new List<int>(),
            ["Blue Chicken"] = new List<int>(),
            ["Brown Chicken"] = new List<int>(),
            ["Brown Cow"] = new List<int>(),
            ["cat"] = new List<int>(),
            ["Cow"] = new List<int>(),
            ["Dinosaur"] = new List<int>(),
            ["dog"] = new List<int>(),
            ["Duck"] = new List<int>(),
            ["Goat"] = new List<int>(),
            // There can only be 1 horse in the vanilla game anyhow, and this mod currently does not provide a way to get more due to the functionality behind horses and potential issues with other mods
            //["horse"] = new List<int>(),
            ["Pig"] = new List<int>(),
            ["Rabbit"] = new List<int>(),
            ["ShearedSheep"] = new List<int>(),
            ["Sheep"] = new List<int>(),
            ["Void Chicken"] = new List<int>(),
            ["White Chicken"] = new List<int>(),
            ["White Cow"] = new List<int>(),
            // Special: MorePets separates baby ducks from baby white chickens (BabyDuck.xnb as a copy of BabyWhite Chicken.xnb is bundled because of this)
            ["BabyDuck"] = new List<int>()
        };
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
            // load textures
            this.LoadPetSkins();
            List<string> partial = new List<string>
            {
                "Statistics:", Environment.NewLine,
                "  Config:", Environment.NewLine,
                "    AdoptionPrice: " + ModEntry.Config.AdoptionPrice, Environment.NewLine,
                "    RepeatedAdoptionPenality: " + ModEntry.Config.RepeatedAdoptionPenality, Environment.NewLine,
                "    UseMaxAdoptionLimit: " + ModEntry.Config.UseMaxAdoptionLimit, Environment.NewLine,
                "    MaxAdoptionLimit: " + ModEntry.Config.MaxAdoptionLimit, Environment.NewLine,
                "    AnimalsOnly: " + ModEntry.Config.AnimalsOnly, Environment.NewLine,
                "  Skins:"
            };
            foreach (KeyValuePair<string, List<int>> pair in ModEntry.Indexes)
            {
                if (pair.Value.Count > 1)
                    partial.Add($"{Environment.NewLine}    {pair.Key.PadRight(20)}: {pair.Value.Count - 1} skins");
            }
            this.Monitor.Log(string.Join("", partial), LogLevel.Trace);
            helper.ConsoleCommands.Add("kill_pets", "Kills all the pets you adopted using this mod, you monster", this.CommandFired_KillPets);
            if (ModEntry.Config.AnimalsOnly)
                ModEntry.ReplaceBus = false;
            if (ModEntry.ReplaceBus && !ModEntry.Indexes["dog"].Any() && !ModEntry.Indexes["cat"].Any())
            {
                ModEntry.ReplaceBus = false;
                this.Monitor.Log("The `AnimalsOnly` config option is set to `false`, yet no dog or cat skins have been found!", LogLevel.Error);
            }

            // hook events
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
            if (ModEntry.ReplaceBus)
            {
                ControlEvents.ControllerButtonPressed += this.ControlEvents_ControllerButtonPressed;
                ControlEvents.MouseChanged += this.ControlEvents_MouseChanged;
            }
        }


        /*********
        ** Protected methods
        *********/
        private IEnumerable<Pet> GetAllPets()
        {
            foreach (NPC npc in Utility.getAllCharacters())
            {
                if (npc is Pet pet)
                    yield return pet;
            }
        }

        private void LoadPetSkins()
        {
            List<string> skins = new List<string>();
            foreach (KeyValuePair<string, List<int>> index in ModEntry.Indexes)
                if (!index.Key.Equals("cat") && !index.Key.Equals("dog"))
                    index.Value.Add(0);
            foreach (FileInfo file in new DirectoryInfo(Path.Combine(this.Helper.DirectoryPath, "skins")).EnumerateFiles("*.xnb"))
            {
                if (file.Name.Contains("_"))
                {
                    string name = Path.GetFileNameWithoutExtension(file.Name);
                    string[] split = name.Split('_');
                    if (ModEntry.Indexes.ContainsKey(split[0]))
                    {
                        skins.Add(name);
                        ModEntry.Indexes[split[0]].Add(Convert.ToInt32(split[1]));
                    }
                    else
                        this.Monitor.Log("Found unexpected file `" + file.Name + "`, if this is meant to be a skin file it has incorrect naming", LogLevel.Warn);
                }
                else if (!file.Name.Equals("BabyDuck.xnb"))
                    this.Monitor.Log("Found file `" + file.Name + "`, if this is meant to be a skin file it has incorrect naming", LogLevel.Warn);
            }

            this.Monitor.Log("Skin files found: " + string.Join(", ", skins), LogLevel.Trace);
        }

        private void GameEvents_UpdateTick(object s, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (ModEntry.ReplaceBus && Game1.getLocationFromName("BusStop") != null)
            {
                this.Monitor.Log("Patching bus stop...", LogLevel.Trace);
                GameLocation bus = Game1.getLocationFromName("BusStop");
                bus.map.AddTileSheet(new TileSheet("MorePetsTilesheet", bus.map, this.Helper.Content.GetActualAssetKey("box"), new Size(2, 2), new Size(16, 16)));
                bus.SetTile(1, 2, "Front", 0, "MorePetsTilesheet");
                bus.SetTile(2, 2, "Front", 1, "MorePetsTilesheet");
                bus.SetTile(1, 3, "Buildings", 2, "MorePetsTilesheet");
                bus.SetTile(2, 3, "Buildings", 3, "MorePetsTilesheet");
                bus.SetTileProperty(1, 3, "Buildings", "Action", "MorePetsAdoption");
                bus.SetTileProperty(2, 3, "Buildings", "Action", "MorePetsAdoption");
                ModEntry.ReplaceBus = false;
            }

            foreach (Pet npc in this.GetAllPets())
            {
                if (npc.Manners > 0 && npc.updatedDialogueYet == false)
                {
                    try
                    {
                        string type = npc is Dog ? "dog" : "cat";
                        npc.Sprite = new AnimatedSprite(this.Helper.Content.GetActualAssetKey($"skins/{type}_{npc.Manners}"), 0, 32, 32);
                    }
                    catch
                    {
                        this.Monitor.Log("Pet with unknown skin number found, using default: " + npc.Manners, LogLevel.Error);
                    }

                    npc.updatedDialogueYet = true;
                }
            }

            if (Game1.getFarm() == null)
                return;
            Random random = new Random();
            foreach (FarmAnimal animal in Game1.getFarm().getAllFarmAnimals())
            {
                string str = animal.type.Value;
                if (animal.age.Value < animal.ageWhenMature.Value)
                    str = "Baby" + animal.type.Value;
                else if (animal.showDifferentTextureWhenReadyForHarvest.Value && animal.currentProduce.Value <= 0)
                    str = "Sheared" + animal.type.Value;
                if (animal.meatIndex.Value < 999)
                    animal.meatIndex.Value = ModEntry.Indexes[str][random.Next(0, ModEntry.Indexes[str].Count)] + 999;
                else if (animal.meatIndex.Value > 999)
                {
                    try
                    {
                        string assetKey = this.Helper.Content.GetActualAssetKey($"skins/{str}_{animal.meatIndex.Value - 999}");
                        if (animal.Sprite.textureName.Value != assetKey)
                            animal.Sprite = new AnimatedSprite(assetKey, 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                    }
                    catch
                    {
                        this.Monitor.Log("Animal with unknown skin number found, using default instead: " + (animal.meatIndex.Value - 999), LogLevel.Error);
                        if (str.Equals("BabyDuck"))
                            try
                            {
                                string assetKey = this.Helper.Content.GetActualAssetKey("skins/BabyDuck");
                                if (animal.Sprite.textureName.Value != assetKey)
                                    animal.Sprite = new AnimatedSprite(assetKey, 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                            }
                            catch
                            {
                                this.Monitor.Log("Encounted a issue trying to override the default texture for baby ducks with the custom one, using vanilla.", LogLevel.Error);
                                string assetKey = this.Helper.Content.GetActualAssetKey("Animals\\BabyWhite Chicken", ContentSource.GameContent);
                                if (animal.Sprite.textureName.Value != assetKey)
                                    animal.Sprite = new AnimatedSprite(assetKey, 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                            }
                        else
                        {
                            string assetKey = this.Helper.Content.GetActualAssetKey($"Animals\\{str}", ContentSource.GameContent);
                            if (animal.Sprite.textureName.Value != assetKey)
                                animal.Sprite = new AnimatedSprite(assetKey, 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                        }
                    }
                }
                else if (animal.type.Value == "Duck" && animal.age.Value < animal.ageWhenMature.Value)
                {
                    try
                    {
                        string assetKey = this.Helper.Content.GetActualAssetKey("skins/BabyDuck");
                        if (animal.Sprite.textureName.Value != assetKey)
                            animal.Sprite = new AnimatedSprite(assetKey, 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                    }
                    catch
                    {
                        this.Monitor.Log("Encounted a issue trying to override the default texture for baby ducks with the custom one, using vanilla.", LogLevel.Error);
                        string assetKey = this.Helper.Content.GetActualAssetKey("Animals\\BabyWhite Chicken", ContentSource.GameContent);
                        if (animal.Sprite.textureName.Value != assetKey)
                            animal.Sprite = new AnimatedSprite(assetKey, 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                    }
                }
            }
        }

        private void CommandFired_KillPets(string name, string[] args)
        {
            GameLocation farm = Game1.getLocationFromName("Farm");
            GameLocation house = Game1.getLocationFromName("FarmHouse");
            foreach (Pet pet in this.GetAllPets())
                if (pet.Age > 0)
                    if (farm.characters.Contains(pet))
                        farm.characters.Remove(pet);
                    else
                        house.characters.Remove(pet);
            this.Monitor.Log("You actually killed them.. you FAT monster!", LogLevel.Alert);
        }

        private void ControlEvents_ControllerButtonPressed(object sender, EventArgsControllerButtonPressed e)
        {
            if (e.ButtonPressed == Buttons.A)
                this.CheckForAction();
        }

        private void ControlEvents_ControllerButtonReleased(object sender, EventArgsControllerButtonReleased e)
        {
            if (this.TriggerAction && e.ButtonReleased == Buttons.A)
            {
                this.TriggerAction = false;
                this.DoAction();
            }
        }

        private void ControlEvents_MouseChanged(object sender, EventArgsMouseStateChanged e)
        {
            if (e.NewState.RightButton == ButtonState.Pressed && e.PriorState.RightButton != ButtonState.Pressed)
                this.CheckForAction();
            if (this.TriggerAction && e.NewState.RightButton == ButtonState.Released)
            {
                this.TriggerAction = false;
                this.DoAction();
            }
        }

        private void CheckForAction()
        {
            if (!Game1.hasLoadedGame)
                return;
            if (!Game1.player.UsingTool && !Game1.pickingTool && !Game1.menuUp && (!Game1.eventUp || Game1.currentLocation.currentEvent.playerControlSequence) && !Game1.nameSelectUp && Game1.numberOfSelectedItems == -1 && !Game1.fadeToBlack)
            {
                Vector2 grabTile = new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / Game1.tileSize;
                if (!Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
                    grabTile = Game1.player.GetGrabTile();
                Tile tile = Game1.currentLocation.map.GetLayer("Buildings").PickTile(new Location((int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize), Game1.viewport.Size);
                PropertyValue propertyValue = null;
                if (tile != null)
                    tile.Properties.TryGetValue("Action", out propertyValue);
                if (propertyValue != null && "MorePetsAdoption".Equals(propertyValue)) this.TriggerAction = true;
            }
        }

        private void DoAction()
        {
            int seed = Game1.year * 1000 + Array.IndexOf(ModEntry.Seasons, Game1.currentSeason) * 100 + Game1.dayOfMonth;
            ModEntry.Random = new Random(seed);
            List<Pet> list = this.GetAllPets().ToList();
            if (ModEntry.Config.UseMaxAdoptionLimit && list.Count >= ModEntry.Config.MaxAdoptionLimit || ModEntry.Random.NextDouble() < Math.Max(0.1, Math.Min(0.9, list.Count * ModEntry.Config.RepeatedAdoptionPenality)) || list.FindIndex(a => a.Age == seed) != -1)
                Game1.drawObjectDialogue("Just an empty box.");
            else
                AdoptQuestion.Show();
        }
    }
}
