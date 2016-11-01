using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;

using xTile.Tiles;
using xTile.ObjectModel;

using Entoarox.Framework;

namespace MorePets
{
    public class MorePetsConfig : Config
    {
        public override T GenerateDefaultConfig<T>()
        {
            AdoptionPrice = 500;
            RepeatedAdoptionPenality = 0.1;
            UseMaxAdoptionLimit = false;
            MaxAdoptionLimit = 10;
            DebugMode = false;
            return this as T;
        }
        public bool DebugMode;
        private int _AdoptionPrice;
        public int AdoptionPrice
        {
            get
            {
                return _AdoptionPrice;
            }
            set
            {
                _AdoptionPrice = Math.Max(100, value);
            }
        }
        private double _RepeatedAdoptionPenality;
        public double RepeatedAdoptionPenality
        {
            get
            {
                return _RepeatedAdoptionPenality;
            }
            set
            {
                _RepeatedAdoptionPenality = Math.Max(0.0, Math.Min(0.9, value));
            }
        }
        public bool UseMaxAdoptionLimit;
        public int MaxAdoptionLimit;
    }
    public class MorePetsMod : Mod
    {
        internal static LocalizedContentManager content;
        private static string modPath;
        internal static int dogLimit = 1;
        internal static int catLimit = 1;
        internal static Random random;
        private static bool replaceBus = false;
        internal static MorePetsConfig Config;
        internal static DataLogger Logger = new DataLogger("MorePets");
        private static System.Version version = new System.Version(1,2,1);
        // DEV PROPERTIES
        internal static int offsetX = 0;
        internal static int offsetY = 0;
        public override void Entry(params object[] objects)
        {
            modPath = PathOnDisk;
            Config = new MorePetsConfig().InitializeConfig(BaseConfigPath);
            GameEvents.UpdateTick += GameEvents_UpdateTick;
            ControlEvents.ControllerButtonPressed += ControlEvents_ControllerButtonPressed;
            ControlEvents.MouseChanged += ControlEvents_MouseChanged;
            //LocationEvents.CurrentLocationChanged += LocationEvents_CurrentLocationChanged;
            Command.RegisterCommand("kill_pets", "Kills all the pets you adopted using ExtraPets, you monster").CommandFired += CommandFired_KillPets;
            if (Config.DebugMode)
            {
                // DEV COMMANDS, kept in should they be needed in the future
                Command.RegisterCommand("spawn_pet", "Spawns either a `dog` or a `cat` depending on the given name | spawn_pet <type> <skin>",new string[] { "type","skin"}).CommandFired += CommandFired_SpawnPet;
                Command.RegisterCommand("test_adoption", "Triggers the adoption dialogue").CommandFired += CommandFired_TestAdoption;
            }
            Logger.Info("MorePets "+version.ToString()+" by Entoarox, do not redistribute without permission");
            VersionChecker.AddCheck("MorePets", version, "https://raw.githubusercontent.com/Entoarox/Stardew-SMAPI-mods/master/Projects/VersionChecker/MorePets.json");
        }
        private static void PopulatePetSkins()
        {
            bool dogsDone = false;
            bool catsDone = false;
            dogLimit = 0;
            catLimit = 0;
            for (int c=1;c<int.MaxValue;c++)
            {
                if(!dogsDone)
                    if (File.Exists(Path.Combine(modPath, "pets", "dog_" + c + ".xnb")))
                        dogLimit++;
                    else
                        dogsDone = true;
                if (!catsDone)
                    if (File.Exists(Path.Combine(modPath, "pets", "cat_" + c + ".xnb")))
                        catLimit++;
                    else
                        catsDone = true;
                if (dogsDone && catsDone)
                    break;
            }
            Logger.Info("Found [" + catLimit + "] Cat and [" + dogLimit + "] Dog skins");
        }
        private static List<NPC> GetAllPets()
        {
            List<NPC> npcs = new List<NPC>();
            npcs.AddRange(Game1.getLocationFromName("FarmHouse").characters.FindAll(a => a is Pet));
            npcs.AddRange(Game1.getLocationFromName("Farm").characters.FindAll(a => a is Pet));
            return npcs;
        }
        internal static void GameEvents_UpdateTick(object s, EventArgs e)
        {
            if (!Game1.hasLoadedGame || Game1.CurrentEvent != null || Game1.fadeToBlack)
                return;
            if (content == null)
            {
                content = new LocalizedContentManager(Game1.content.ServiceProvider, modPath);
                PopulatePetSkins();
                replaceBus = true;
            }
            foreach (NPC npc in GetAllPets())
                if (npc.manners > 0 && npc.updatedDialogueYet == false)
                {
                    try
                    {
                        npc.sprite = new AnimatedSprite(content.Load<Texture2D>("pets/" + (npc is Dog ? "dog_" : "cat_") + npc.manners), 0, 32, 32);
                    }
                    catch
                    {
                        Logger.Error("Pet with unknown skin number found, using default: "+npc.manners);
                    }
                    npc.updatedDialogueYet = true;
                }
            if (replaceBus && Game1.getLocationFromName("BusStop") != null)
            {
                GameLocation bus = Game1.getLocationFromName("BusStop");
                bus.map.AddTileSheet(new TileSheet("MorePetsTilesheet",bus.map, "paths_objects_MorePetsTilesheet", new xTile.Dimensions.Size(2,2), new xTile.Dimensions.Size(16,16)));
                EntoFramework.GetContentRegistry().RegisterTexture("paths_objects_MorePetsTilesheet", Path.Combine(modPath, "box.png"));
                ILocationHelper helper = EntoFramework.GetLocationHelper();
                helper.SetStaticTile(bus, "Front", 1, 2, 0, "MorePetsTilesheet");
                helper.SetStaticTile(bus, "Front", 2, 2, 1, "MorePetsTilesheet");
                helper.SetStaticTile(bus, "Buildings", 1, 3, 2, "MorePetsTilesheet");
                helper.SetStaticTile(bus, "Buildings", 2, 3, 3, "MorePetsTilesheet");
                bus.setTileProperty(1, 3, "Buildings", "Action", "MorePetsAdoption");
                bus.setTileProperty(2, 3, "Buildings", "Action", "MorePetsAdoption");
                replaceBus = false;
            }
        }
        internal static void CommandFired_SpawnPet(object s, EventArgsCommand e)
        {
            if(e.Command.CalledArgs.Length==2)
            {
                Pet pet=null;
                int skin = Convert.ToInt32(e.Command.CalledArgs[1]);
                switch (e.Command.CalledArgs[0])
                {
                    case "dog":
                        if (dogLimit == 0 || skin > catLimit || skin < 1)
                            break;
                        pet = new Dog(Game1.player.getTileLocationPoint().X, Game1.player.getTileLocationPoint().Y);
                        pet.sprite = new AnimatedSprite(content.Load<Texture2D>("pets/dog_" + skin), 0, 32, 32);
                        break;
                    case "cat":
                        if (catLimit == 0 || skin > catLimit || skin < 1)
                            break;
                        pet = new Cat((int)Game1.player.position.X, (int)Game1.player.position.Y);
                        pet.sprite = new AnimatedSprite(content.Load<Texture2D>("pets/cat_" + skin), 0, 32, 32);
                        break;
                }
                if(pet!=null)
                {
                    pet.manners = 1; // Skin
                    pet.age = 1100;
                    pet.position = Game1.player.position;
                    Game1.currentLocation.addCharacter(pet);
                    Logger.Log("INFO","Pet spawned", ConsoleColor.Green);
                }
                else
                    Logger.Error("Was unable to spawn pet, did you make sure the <type> and <skin> are valid?");
            }
        }
        internal static void CommandFired_KillPets(object s, EventArgs e)
        {
            GameLocation farm = Game1.getLocationFromName("Farm");
            GameLocation house = Game1.getLocationFromName("FarmHouse");
            foreach (NPC pet in GetAllPets())
                if(pet.age>0)
                    if (farm.characters.Contains(pet))
                        farm.characters.Remove(pet);
                    else
                        house.characters.Remove(pet);
            Logger.Log("INFO","You actually killed them.. you FAT monster!", ConsoleColor.DarkRed);
        }
        internal static void CommandFired_TestAdoption(object s, EventArgs e)
        {
            if (dogLimit == 0 && catLimit == 0)
                return;
            random = new Random();
            AdoptQuestion.Show();
        }
        internal static void ControlEvents_ControllerButtonPressed(object sender, EventArgsControllerButtonPressed e)
        {
            if (e.ButtonPressed == Buttons.A)
                CheckForAction();
        }
        internal static void ControlEvents_MouseChanged(object sender, EventArgsMouseStateChanged e)
        {
            if (e.NewState.RightButton == ButtonState.Pressed && e.PriorState.RightButton != ButtonState.Pressed)
                CheckForAction();
        }
        internal static List<string> seasons = new List<string>(){ "spring", "summer", "fall", "winter" };
        private static void CheckForAction()
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
                    if ((dogLimit == 0 && catLimit == 0) || (Config.UseMaxAdoptionLimit && list.Count >= Config.MaxAdoptionLimit) || random.NextDouble() < Math.Min(0.9, list.Count * Config.RepeatedAdoptionPenality) || list.FindIndex(a => a.age == seed) != -1)
                        Game1.drawObjectDialogue("Just an empty box.");
                    else
                        AdoptQuestion.Show();
                }
            }
        }
    }
    internal class AdoptQuestion
    {
        private bool Cat;
        private int Skin;
        private Farmer Who=null;
        private AnimatedSprite Sprite;
        internal AdoptQuestion(bool cat, int skin)
        {
            Cat = cat;
            Skin = skin;
            Sprite= new AnimatedSprite(MorePetsMod.content.Load<Texture2D>("pets/"+(Cat ? "cat_" : "dog_") + Skin),28,32,32);
            Sprite.loop = true;
        }
        internal static void Show()
        {
            Random rnd = MorePetsMod.random;
            bool cat = MorePetsMod.catLimit == 0 ? false : MorePetsMod.dogLimit == 0 ? true : rnd.NextDouble() < 0.5;
            AdoptQuestion q = new AdoptQuestion(cat, rnd.Next(1, cat ? MorePetsMod.catLimit : MorePetsMod.dogLimit));
            GraphicsEvents.OnPostRenderHudEvent += q.Display;
            Game1.currentLocation.lastQuestionKey = "AdoptPetQuestion";
            Game1.currentLocation.createQuestionDialogue(
                "Oh dear, it looks like someone has abandoned a poor " + (cat ? "Cat" : "Dog") + " here! Perhaps you should pay Marnie " + MorePetsMod.Config.AdoptionPrice + " gold to give it a checkup so you can adopt it?",
                Game1.player.money < MorePetsMod.Config.AdoptionPrice?
                new Response[] {
                    new Response("n","Unfortunately I do not have the required "+MorePetsMod.Config.AdoptionPrice+" gold in order to do this.")
                }:
                new Response[] {
                    new Response("y","Yes, I really should adopt the poor animal!"),
                    new Response("n","No, I do not have the space to house it.")
                },
                q.Resolver, null);
        }
        internal void Display(object o, EventArgs e)
        {
            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is DialogueBox)
            {
                DialogueBox box = (DialogueBox)Game1.activeClickableMenu;
                Vector2 c = new Vector2(Game1.viewport.Width/2-128*Game1.pixelZoom, Game1.viewport.Height - box.height-(56*Game1.pixelZoom));
                Vector2 p = new Vector2(36 * Game1.pixelZoom + c.X, 32 * Game1.pixelZoom +c.Y);
                IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), (int)c.X, (int)c.Y, 40* Game1.pixelZoom, 40* Game1.pixelZoom, Color.White, 1, true);
                Game1.spriteBatch.Draw(Sprite.Texture, p, Sprite.SourceRect, Color.White, 0, new Vector2(Sprite.spriteWidth, Sprite.spriteHeight), Game1.pixelZoom, SpriteEffects.None, 0.991f);
                Sprite.Animate(Game1.currentGameTime, 28, 2, 500);
            }
            else
                GraphicsEvents.OnPostRenderHudEvent -= Display;
        }
        internal void Resolver(Farmer who, string answer)
        {
            GraphicsEvents.OnPostRenderHudEvent -= Display;
            if (answer == "n")
                return;
            Who = who;
            Game1.activeClickableMenu= new NamingMenu(Namer, "Choose a name");
        }
        internal void Namer(string petName)
        {
            NPC pet;
            Who.Money -= MorePetsMod.Config.AdoptionPrice;
            if (Cat)
            {
                pet = new Cat((int)Game1.player.position.X, (int)Game1.player.position.Y);
                pet.sprite = new AnimatedSprite(MorePetsMod.content.Load<Texture2D>("pets/cat_" + Skin), 0, 32, 32);
            }
            else
            {
                pet = new Dog(Game1.player.getTileLocationPoint().X, Game1.player.getTileLocationPoint().Y);
                pet.sprite = new AnimatedSprite(MorePetsMod.content.Load<Texture2D>("pets/dog_" + Skin), 0, 32, 32);
            }
            pet.name = petName;
            pet.manners = Skin;
            pet.age = Game1.year * 1000 + MorePetsMod.seasons.IndexOf(Game1.currentSeason) * 100 + Game1.dayOfMonth;
            pet.position = Game1.player.position;
            Game1.currentLocation.addCharacter(pet);
            (pet as Pet).warpToFarmHouse(Who);
            Game1.drawObjectDialogue("Marnie will bring " + petName + " to your house once they have their shots and been given a grooming.");
        }
    }
}
