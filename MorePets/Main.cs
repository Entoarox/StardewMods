using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entoarox.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using xTile.ObjectModel;
using xTile.Tiles;

namespace MorePets
{
    public class MorePetsMod : Mod
    {
        internal static Random random;
        private static bool replaceBus = false;
        internal static MorePetsConfig Config;
        // DEV PROPERTIES
        internal static int offsetX = 0;
        internal static int offsetY = 0;
        internal static Texture2D[] CatTextures;
        internal static Texture2D[] DogTextures;
        public override void Entry(IModHelper helper)
        {
            // init
            Config = helper.ReadConfig<MorePetsConfig>();

            // load textures
            this.LoadPetSkins(out CatTextures, out DogTextures);
            Monitor.Log($"Found [{CatTextures.Length}] Cat and [{DogTextures.Length}] Dog skins", LogLevel.Info);

            // hook events
            GameEvents.UpdateTick += GameEvents_UpdateTick;
            ControlEvents.ControllerButtonPressed += ControlEvents_ControllerButtonPressed;
            ControlEvents.MouseChanged += ControlEvents_MouseChanged;
            //LocationEvents.CurrentLocationChanged += LocationEvents_CurrentLocationChanged;
            helper.ConsoleCommands.Add("kill_pets", "Kills all the pets you adopted using MorePets, you monster", this.CommandFired_KillPets);

            // add console commands
            if (Config.DebugMode)
            {
                // DEV COMMANDS, kept in should they be needed in the future
                helper.ConsoleCommands
                    .Add("spawn_pet", "Spawns either a `dog` or a `cat` depending on the given name | spawn_pet <type> <skin>", this.CommandFired_SpawnPet)
                    .Add("test_adoption", "Triggers the adoption dialogue", this.CommandFired_TestAdoption);
            }

            // check version
            Helper.RequestUpdateCheck("https://raw.githubusercontent.com/Entoarox/StardewMods/master/MorePets/update.json");
        }

        private void LoadPetSkins(out Texture2D[] catTextures, out Texture2D[] dogTextures)
        {
            List<Texture2D> catTextureList = new List<Texture2D>();
            List<Texture2D> dogTextureList = new List<Texture2D>();

            foreach (FileInfo file in new DirectoryInfo(Path.Combine(this.Helper.DirectoryPath, "pets")).EnumerateFiles("*.xnb"))
            {
                if (file.Name.StartsWith("cat_"))
                    catTextureList.Add(this.Helper.Content.Load<Texture2D>($"pets/{file.Name}"));
                else if (file.Name.StartsWith("dog_"))
                    dogTextureList.Add(this.Helper.Content.Load<Texture2D>($"pets/{file.Name}"));
            }

            catTextures = catTextureList.ToArray();
            dogTextures = dogTextureList.ToArray();
        }
        private List<NPC> GetAllPets()
        {
            List<NPC> npcs = new List<NPC>();
            npcs.AddRange(Game1.getLocationFromName("FarmHouse").characters.FindAll(a => a is Pet));
            npcs.AddRange(Game1.getLocationFromName("Farm").characters.FindAll(a => a is Pet));
            return npcs;
        }
        internal void GameEvents_UpdateTick(object s, EventArgs e)
        {
            if (!Game1.hasLoadedGame || Game1.CurrentEvent != null || Game1.fadeToBlack)
                return;

            foreach (NPC npc in GetAllPets())
                if (npc.manners > 0 && npc.updatedDialogueYet == false)
                {
                    try
                    {
                        var textures = npc is Dog ? DogTextures : CatTextures;
                        npc.sprite = new AnimatedSprite(textures[npc.manners], 0, 32, 32);
                    }
                    catch
                    {
                        Monitor.Log("Pet with unknown skin number found, using default: " + npc.manners, LogLevel.Error);
                    }
                    npc.updatedDialogueYet = true;
                }
            if (replaceBus && Game1.getLocationFromName("BusStop") != null)
            {
                GameLocation bus = Game1.getLocationFromName("BusStop");
                bus.map.AddTileSheet(new TileSheet("MorePetsTilesheet", bus.map, Helper.Content.GetActualAssetKey("box.png"), new xTile.Dimensions.Size(2, 2), new xTile.Dimensions.Size(16, 16)));
                bus.SetTile(1, 2, "Front", 0, "MorePetsTilesheet");
                bus.SetTile(2, 2, "Front", 1, "MorePetsTilesheet");
                bus.SetTile(1, 3, "Buildings", 2, "MorePetsTilesheet");
                bus.SetTile(2, 3, "Buildings", 3, "MorePetsTilesheet");
                bus.SetTileProperty(1, 3, "Buildings", "Action", "MorePetsAdoption");
                bus.SetTileProperty(2, 3, "Buildings", "Action", "MorePetsAdoption");
                replaceBus = false;
            }
        }
        internal void CommandFired_SpawnPet(string name, string[] args)
        {
            if (args.Length == 2)
            {
                Pet pet = null;
                int skin = Convert.ToInt32(args[1]);
                switch (args[0])
                {
                    case "dog":
                        if (!DogTextures.Any() || skin > DogTextures.Length || skin < 1)
                        {
                            Monitor.Log($"Unable to spawn {args[0]}, unknown skin number: {skin}", LogLevel.Error);
                            break;
                        }
                        pet = new Dog(Game1.player.getTileLocationPoint().X, Game1.player.getTileLocationPoint().Y);
                        pet.sprite = new AnimatedSprite(DogTextures[skin], 0, 32, 32);
                        break;
                    case "cat":
                        if (!CatTextures.Any() || skin > CatTextures.Length || skin < 1)
                        {
                            Monitor.Log($"Unable to spawn {args[0]}, unknown skin number: {skin}", LogLevel.Error);
                            break;
                        }
                        pet = new Cat((int)Game1.player.position.X, (int)Game1.player.position.Y);
                        pet.sprite = new AnimatedSprite(CatTextures[skin], 0, 32, 32);
                        break;
                    default:
                        Monitor.Log("Unable to spawn pet, unknown type: " + args[0], LogLevel.Error);
                        break;
                }
                if (pet != null)
                {
                    pet.manners = skin;
                    pet.age = 0;
                    pet.position = Game1.player.position;
                    Game1.currentLocation.addCharacter(pet);
                    Monitor.Log("Pet spawned", LogLevel.Alert);
                }
                else
                    Monitor.Log("Unable to spawn pet, did you make sure the <type> and <skin> are valid?", LogLevel.Error);
            }
        }
        internal void CommandFired_KillPets(string name, string[] args)
        {
            GameLocation farm = Game1.getLocationFromName("Farm");
            GameLocation house = Game1.getLocationFromName("FarmHouse");
            foreach (NPC pet in GetAllPets())
                if (pet.age > 0)
                    if (farm.characters.Contains(pet))
                        farm.characters.Remove(pet);
                    else
                        house.characters.Remove(pet);
            Monitor.Log("You actually killed them.. you FAT monster!", LogLevel.Alert);
        }
        internal void CommandFired_TestAdoption(string name, string[] args)
        {
            if (!DogTextures.Any() && !CatTextures.Any())
                return;
            random = new Random();
            AdoptQuestion.Show();
        }
        internal void ControlEvents_ControllerButtonPressed(object sender, EventArgsControllerButtonPressed e)
        {
            if (e.ButtonPressed == Buttons.A)
                CheckForAction();
        }
        internal void ControlEvents_MouseChanged(object sender, EventArgsMouseStateChanged e)
        {
            if (e.NewState.RightButton == ButtonState.Pressed && e.PriorState.RightButton != ButtonState.Pressed)
                CheckForAction();
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
                    int seed = Game1.year * 1000 + seasons.IndexOf(Game1.currentSeason) * 100 + Game1.dayOfMonth;
                    random = new Random(seed);
                    List<NPC> list = GetAllPets();
                    if ((!DogTextures.Any() && !CatTextures.Any()) || (Config.UseMaxAdoptionLimit && list.Count >= Config.MaxAdoptionLimit) || random.NextDouble() < Math.Max(0.1, Math.Min(0.9, list.Count * Config.RepeatedAdoptionPenality)) || list.FindIndex(a => a.age == seed) != -1)
                        Game1.drawObjectDialogue("Just an empty box.");
                    else
                        AdoptQuestion.Show();
                }
            }
        }
    }
}
