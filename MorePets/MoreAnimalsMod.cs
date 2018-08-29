using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using Entoarox.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using xTile.ObjectModel;
using xTile.Tiles;

namespace Entoarox.MorePetsAndAnimals
{
    public class MoreAnimalsMod : Mod
    {

        private static bool replaceBus = true;
        private bool _TriggerAction = false;
        internal static Random random;
        internal static ModConfig Config;
        internal static IModHelper SHelper;
        private static readonly ListPool<Pet> Pool = new ListPool<Pet>();
        internal static LocalizedContentManager Content = new LocalizedContentManager(Game1.content.ServiceProvider, "Mods\\MoreAnimals\\skins");
        internal static Dictionary<string, List<int>> Indexes = new Dictionary<string, List<int>>()
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
            ["BabyDuck"] = new List<int>(),
        };
        public override void Entry(IModHelper helper)
        {
            // init
            Config = helper.ReadConfig<ModConfig>();
            SHelper = helper;
            // load textures
            this.LoadPetSkins();
            List<string> partial = new List<string>()
            {
                "Statistics:", Environment.NewLine,
                "  Config:", Environment.NewLine,
                "    AdoptionPrice: "+Config.AdoptionPrice.ToString(), Environment.NewLine,
                "    RepeatedAdoptionPenality: "+Config.RepeatedAdoptionPenality.ToString(), Environment.NewLine,
                "    UseMaxAdoptionLimit: "+Config.UseMaxAdoptionLimit.ToString(), Environment.NewLine,
                "    MaxAdoptionLimit: "+Config.MaxAdoptionLimit.ToString(), Environment.NewLine,
                "    AnimalsOnly: "+Config.AnimalsOnly.ToString(), Environment.NewLine,
                "  Skins:"
            };
            foreach (KeyValuePair<string, List<int>> pair in Indexes)
                if (pair.Value.Count > 1)
                    partial.Add($"{Environment.NewLine}    {pair.Key.PadRight(20)}: {pair.Value.Count - 1} skins");
            this.Monitor.Log(string.Join("", partial), LogLevel.Trace);
            helper.ConsoleCommands.Add("kill_pets", "Kills all the pets you adopted using this mod, you monster", this.CommandFired_KillPets);
            if (Config.AnimalsOnly)
                replaceBus = false;
            if (replaceBus && !Indexes["dog"].Any() && !Indexes["cat"].Any())
            {
                replaceBus = false;
                this.Monitor.Log("The `AnimalsOnly` config option is set to `false`, yet no dog or cat skins have been found!", LogLevel.Error);
            }
            // hook events
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
            if(replaceBus)
            {
                ControlEvents.ControllerButtonPressed += this.ControlEvents_ControllerButtonPressed;
                ControlEvents.MouseChanged += this.ControlEvents_MouseChanged;
            }
        }

        private void LoadPetSkins()
        {
            List<string> skins = new List<string>();
            foreach (var index in Indexes)
                if(!index.Key.Equals("cat") && !index.Key.Equals("dog"))
                    index.Value.Add(0);
            foreach (FileInfo file in new DirectoryInfo(Path.Combine(this.Helper.DirectoryPath, "skins")).EnumerateFiles("*.xnb"))
            {
                if (file.Name.Contains("_"))
                {
                    string name = Path.GetFileNameWithoutExtension(file.Name);
                    string[] split = name.Split('_');
                    if (Indexes.ContainsKey(split[0]))
                    {
                        skins.Add(name);
                        Indexes[split[0]].Add(Convert.ToInt32(split[1]));
                    }
                    else
                        this.Monitor.Log("Found unexpected file `"+file.Name+"`, if this is meant to be a skin file it has incorrect naming", LogLevel.Warn);
                }
                else if(!file.Name.Equals("BabyDuck.xnb"))
                    this.Monitor.Log("Found file `" + file.Name + "`, if this is meant to be a skin file it has incorrect naming", LogLevel.Warn);
            }
            this.Monitor.Log("Skin files found: " + string.Join(", ", skins), LogLevel.Trace);
        }
        public List<Pet> GetAllPets()
        {
            List<Pet> pets = new List<Pet>();
            List<NPC> npcs = new List<NPC>();
            foreach (NPC npc in Utility.getAllCharacters())
            {
                npcs.Add(npc);
            }
            pets = npcs.Where(a => a is Pet).Cast<Pet>().ToList();
            return pets;
        }

        internal void GameEvents_UpdateTick(object s, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (replaceBus && Game1.getLocationFromName("BusStop") != null)
            {
                this.Monitor.Log("Patching bus stop...", LogLevel.Trace);
                GameLocation bus = Game1.getLocationFromName("BusStop");
                bus.map.AddTileSheet(new TileSheet("MorePetsTilesheet", bus.map, this.Helper.Content.GetActualAssetKey("box"), new xTile.Dimensions.Size(2, 2), new xTile.Dimensions.Size(16, 16)));
                bus.SetTile(1, 2, "Front", 0, "MorePetsTilesheet");
                bus.SetTile(2, 2, "Front", 1, "MorePetsTilesheet");
                bus.SetTile(1, 3, "Buildings", 2, "MorePetsTilesheet");
                bus.SetTile(2, 3, "Buildings", 3, "MorePetsTilesheet");
                bus.SetTileProperty(1, 3, "Buildings", "Action", "MorePetsAdoption");
                bus.SetTileProperty(2, 3, "Buildings", "Action", "MorePetsAdoption");
                replaceBus = false;
            }
            foreach (Pet npc in GetAllPets())
                if (npc.Manners > 0 && npc.updatedDialogueYet == false)
                {
                    try
                    {
                        string type = npc is Dog ? "dog" : "cat";
                        npc.Sprite = new AnimatedSprite(Content, $"{type}_{npc.Manners}", 0, 32, 32);
                    }
                    catch
                    {
                        this.Monitor.Log("Pet with unknown skin number found, using default: " + npc.Manners.ToString(), LogLevel.Error);
                    }
                    npc.updatedDialogueYet = true;
                }
            if (Game1.getFarm() == null)
                return;
            Random random = new Random();
            foreach(FarmAnimal animal in Game1.getFarm().getAllFarmAnimals())
            {
                string str = animal.type.Value;
                if (animal.age.Value < animal.ageWhenMature.Value)
                    str = "Baby" + animal.type.Value;
                else if (animal.showDifferentTextureWhenReadyForHarvest.Value && animal.currentProduce.Value <= 0)
                    str = "Sheared" + animal.type.Value;
                if (animal.meatIndex.Value < 999)
                    animal.meatIndex.Set(Indexes[str][random.Next(0, Indexes[str].Count)] + 999);
                else if (animal.meatIndex.Value > 999)
                {
                    try
                    {
                        Texture2D texture = this.Helper.Content.Load<Texture2D>(str + "_" + (animal.meatIndex.Value - 999).ToString());
                        if (animal.Sprite.Texture != texture)
                            animal.Sprite = new AnimatedSprite(Content, texture.Name, 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                    }
                    catch
                    {
                        this.Monitor.Log("Animal with unknown skin number found, using default instead: " + (animal.meatIndex.Value - 999).ToString(), LogLevel.Error);
                        if (str.Equals("BabyDuck"))
                            try
                            {
                                Texture2D texture = Content.Load<Texture2D>("BabyDuck");
                                if (animal.Sprite.Texture != texture)
                                    animal.Sprite = new AnimatedSprite(Content, texture.Name, 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                            }
                            catch
                            {
                                this.Monitor.Log("Encounted a issue trying to override the default texture for baby ducks with the custom one, using vanilla.", LogLevel.Error);
                                Texture2D texture = Game1.content.Load<Texture2D>("Animals\\BabyWhite Chicken");
                                if (animal.Sprite.Texture != texture)
                                    animal.Sprite = new AnimatedSprite(texture.Name, 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                            }
                        else
                        {
                            Texture2D texture = Game1.content.Load<Texture2D>("Animals\\" + str);
                            if (animal.Sprite.Texture != texture)
                                animal.Sprite = new AnimatedSprite(texture.Name, 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                        }
                    }
                }
                else if (animal.type.Value == "Duck" && animal.age.Value < animal.ageWhenMature.Value)
                {
                    try
                    {
                        Texture2D texture = Content.Load<Texture2D>("BabyDuck");
                        if (animal.Sprite.Texture != texture)
                            animal.Sprite = new AnimatedSprite(Content, texture.Name, 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                    }
                    catch
                    {
                        this.Monitor.Log("Encounted a issue trying to override the default texture for baby ducks with the custom one, using vanilla.", LogLevel.Error);
                        Texture2D texture = Game1.content.Load<Texture2D>("Animals\\BabyWhite Chicken");
                        if (animal.Sprite.Texture != texture)
                            animal.Sprite = new AnimatedSprite(texture.Name, 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                    }
                }
            }
        }
        internal void CommandFired_KillPets(string name, string[] args)
        {
            GameLocation farm = Game1.getLocationFromName("Farm");
            GameLocation house = Game1.getLocationFromName("FarmHouse");
            foreach (Pet pet in GetAllPets())
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
                CheckForAction();
        }
        private void ControlEvents_ControllerButtonReleased(object sender, EventArgsControllerButtonReleased e)
        {
            if (this._TriggerAction && e.ButtonReleased == Buttons.A)
            {
                this._TriggerAction = false;
                DoAction();
            }
        }
        private void ControlEvents_MouseChanged(object sender, EventArgsMouseStateChanged e)
        {
            if (e.NewState.RightButton == ButtonState.Pressed && e.PriorState.RightButton != ButtonState.Pressed)
                CheckForAction();
            if (this._TriggerAction && e.NewState.RightButton == ButtonState.Released)
            {
                this._TriggerAction = false;
                DoAction();
            }
        }
        internal static List<string> seasons = new List<string>() { "spring", "summer", "fall", "winter" };
        private void CheckForAction()
        {
            if (!Game1.hasLoadedGame)
                return;
            if (!Game1.player.UsingTool && !Game1.pickingTool && !Game1.menuUp && (!Game1.eventUp || Game1.currentLocation.currentEvent.playerControlSequence) && !Game1.nameSelectUp && Game1.numberOfSelectedItems == -1 && !Game1.fadeToBlack)
            {
                Vector2 grabTile = new Vector2((Game1.getOldMouseX() + Game1.viewport.X), (Game1.getOldMouseY() + Game1.viewport.Y)) / Game1.tileSize;
                if (!Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
                    grabTile = Game1.player.GetGrabTile();
                Tile tile = Game1.currentLocation.map.GetLayer("Buildings").PickTile(new xTile.Dimensions.Location((int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize), Game1.viewport.Size);
                PropertyValue propertyValue = null;
                if (tile != null)
                    tile.Properties.TryGetValue("Action", out propertyValue);
                if (propertyValue != null && "MorePetsAdoption".Equals(propertyValue))
                {
                    this._TriggerAction = true;
                }
            }
        }
        private void DoAction()
        {
            int seed = Game1.year * 1000 + seasons.IndexOf(Game1.currentSeason) * 100 + Game1.dayOfMonth;
            random = new Random(seed);
            List<Pet> list = GetAllPets();
            if ((Config.UseMaxAdoptionLimit && list.Count >= Config.MaxAdoptionLimit) || random.NextDouble() < Math.Max(0.1, Math.Min(0.9, list.Count * Config.RepeatedAdoptionPenality)) || list.FindIndex(a => a.Age == seed) != -1)
                Game1.drawObjectDialogue("Just an empty box.");
            else
                AdoptQuestion.Show();
        }
    }
}
