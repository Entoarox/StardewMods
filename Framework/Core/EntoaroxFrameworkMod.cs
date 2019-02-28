using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Entoarox.Framework.Core.AssetHandlers;
using Entoarox.Framework.Core.Serialization;
using Entoarox.Framework.Events;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;
using SObject = StardewValley.Object;

namespace Entoarox.Framework.Core
{
    internal class EntoaroxFrameworkMod : Mod
    {
        /*********
        ** Fields
        *********/
        private static readonly List<string> Farms = new List<string> { "standard", "river", "forest", "hilltop", "wilderniss" };
        private static string Verify;
        private JsonSerializer Serializer;
        private EventArgsActionTriggered ActionInfo;
        private Item PrevItem;
        private static Vector2? LastTouchAction;

        /****
        ** Serializer
        ****/
        public static bool SerializerInjected;
        public static List<Type> SerializerTypes = new List<Type>();
        private XmlSerializer MainSerializer;
        private XmlSerializer FarmerSerializer;
        private XmlSerializer LocationSerializer;
        private static readonly Type[] SerialiserTypes = new Type[]
        {
            typeof(Tool),
            typeof(GameLocation),
            typeof(Duggy),
            typeof(Bug),
            typeof(BigSlime),
            typeof(Ghost),
            typeof(Child),
            typeof(Pet),
            typeof(Dog),
            typeof(Cat),
            typeof(Horse),
            typeof(GreenSlime),
            typeof(LavaCrab),
            typeof(RockCrab),
            typeof(ShadowGuy),
            typeof(SquidKid),
            typeof(Grub),
            typeof(Fly),
            typeof(DustSpirit),
            typeof(Quest),
            typeof(MetalHead),
            typeof(ShadowGirl),
            typeof(Monster),
            typeof(JunimoHarvester),
            typeof(TerrainFeature)
        };
        private static readonly Type[] FarmerTypes = new Type[]
        {
            typeof(Tool)
        };
        private static readonly Type[] LocationTypes = new Type[]
        {
            typeof(Tool),
            typeof(Duggy),
            typeof(Ghost),
            typeof(GreenSlime),
            typeof(LavaCrab),
            typeof(RockCrab),
            typeof(ShadowGuy),
            typeof(Child),
            typeof(Pet),
            typeof(Dog),
            typeof(Cat),
            typeof(Horse),
            typeof(SquidKid),
            typeof(Grub),
            typeof(Fly),
            typeof(DustSpirit),
            typeof(Bug),
            typeof(BigSlime),
            typeof(BreakableContainer),
            typeof(MetalHead),
            typeof(ShadowGirl),
            typeof(Monster),
            typeof(JunimoHarvester),
            typeof(TerrainFeature)
        };


        /*********
        ** Accessors
        *********/
        internal static ModConfig Config;
        internal static IMonitor Logger;
        internal static IModHelper SHelper;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Serializer = new JsonSerializer();
            this.Serializer.Converters.Add(new RectangleConverter());
            this.Serializer.Formatting = Formatting.None;
            this.Serializer.NullValueHandling = NullValueHandling.Ignore;
            this.Serializer.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
            this.Serializer.ContractResolver = new ReadonlyContractResolver();
            DeferredAssetHandler typeHandler = new DeferredAssetHandler();
            helper.Content.AssetEditors.Add(typeHandler);
            helper.Content.AssetEditors.Add(typeHandler);
            helper.Content.AssetEditors.Add(new DeferredTypeHandler());
            helper.Content.AssetLoaders.Add(new XnbLoader());
            helper.Content.AssetEditors.Add(new TextureInjector());
            helper.Content.AssetEditors.Add(new DictionaryInjector());
            EntoaroxFrameworkMod.SHelper = this.Helper;
            EntoaroxFrameworkMod.Logger = this.Monitor;
            EntoaroxFrameworkMod.Config = this.Helper.ReadConfig<ModConfig>();

            this.Helper.ConsoleCommands.Add("world_bushreset", "Resets bushes in the whole game, use this if you installed a map mod and want to keep using your old save.", this.Commands);
            if (EntoaroxFrameworkMod.Config.TrainerCommands)
            {
                helper.ConsoleCommands
                    .Add("farm_settype", "farm_settype <type> | Enables you to change your farm type to any of the following: " + string.Join(",", EntoaroxFrameworkMod.Farms), this.Commands)
                    .Add("farm_clear", "farm_clear | Removes ALL objects from your farm, this cannot be undone!", this.Commands)
                    .Add("player_warp", "player_warp <location> <x> <y> | Warps the player to the given position in the game.", this.Commands);
            }

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.Saved += this.OnSaved;
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;
        }

        public override object GetApi()
        {
            return new EntoaroxFrameworkAPI();
        }


        /*********
        ** Protected methods
        *********/

        /// <summary>Raised after the player releases a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button.IsActionButton() && Game1.activeClickableMenu == null && !Game1.player.UsingTool && !Game1.pickingTool && !Game1.menuUp && (!Game1.eventUp || Game1.currentLocation.currentEvent.playerControlSequence) && !Game1.nameSelectUp && Game1.numberOfSelectedItems == -1 && !Game1.fadeToBlack)
            {
                this.ActionInfo = null;
                Vector2 grabTile = new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / Game1.tileSize;
                if (!Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
                    grabTile = Game1.player.GetGrabTile();
                Tile tile = Game1.currentLocation.map.GetLayer("Buildings").PickTile(new Location((int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize), Game1.viewport.Size);
                PropertyValue propertyValue = null;
                tile?.Properties.TryGetValue("Action", out propertyValue);
                if (propertyValue != null)
                {
                    string[] split = ((string)propertyValue).Split(' ');
                    string[] args = new string[split.Length - 1];
                    Array.Copy(split, 1, args, 0, args.Length);
                    MoreEvents.FireActionTriggered(new EventArgsActionTriggered(Game1.player, split[0], args, grabTile));
                }
            }
        }

        private void Commands(string command, string[] args)
        {
            if (!Game1.hasLoadedGame)
            {
                EntoaroxFrameworkMod.Logger.Log("You need to load a game before you can use this command.", LogLevel.Error);
                return;
            }

            switch (command)
            {
                case "world_bushreset":
                    foreach (GameLocation loc in Game1.locations)
                    {
                        loc.largeTerrainFeatures.Filter(a => !(a is Bush));
                        if ((loc.IsOutdoors || loc.Name.Equals("BathHouse_Entry") || loc.treatAsOutdoors.Value) && loc.map.GetLayer("Paths") != null)
                        {
                            for (int x = 0; x < loc.map.Layers[0].LayerWidth; ++x)
                            {
                                for (int y = 0; y < loc.map.Layers[0].LayerHeight; ++y)
                                {
                                    Tile tile = loc.map.GetLayer("Paths").Tiles[x, y];
                                    if (tile != null)
                                    {
                                        Vector2 vector2 = new Vector2(x, y);
                                        switch (tile.TileIndex)
                                        {
                                            case 24:
                                                if (!loc.terrainFeatures.ContainsKey(vector2))
                                                    loc.largeTerrainFeatures.Add(new Bush(vector2, 2, loc));
                                                break;
                                            case 25:
                                                if (!loc.terrainFeatures.ContainsKey(vector2))
                                                    loc.largeTerrainFeatures.Add(new Bush(vector2, 1, loc));
                                                break;
                                            case 26:
                                                if (!loc.terrainFeatures.ContainsKey(vector2))
                                                    loc.largeTerrainFeatures.Add(new Bush(vector2, 0, loc));
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "farm_settype":
                    if (args.Length == 0)
                        this.Monitor.Log("Please provide the type you wish to change your farm to.", LogLevel.Error);
                    else if (EntoaroxFrameworkMod.Farms.Contains(args[0]))
                    {
                        Game1.whichFarm = EntoaroxFrameworkMod.Farms.IndexOf(args[0]);
                        EntoaroxFrameworkMod.Logger.Log($"Changed farm type to `{args[0]}`, please sleep in a bed then quit&restart to finalize this change.", LogLevel.Alert);
                    }
                    else
                        EntoaroxFrameworkMod.Logger.Log("Unknown farm type: " + args[0], LogLevel.Error);

                    break;
                case "farm_clear":
                    if (EntoaroxFrameworkMod.Verify == null && args.Length == 0)
                    {
                        EntoaroxFrameworkMod.Verify = new Random().Next().ToString("XXX");
                        EntoaroxFrameworkMod.Logger.Log($"This will remove all objects, natural and user-made from your farm, use `farm_clear {EntoaroxFrameworkMod.Verify}` to verify that you actually want to do this.", LogLevel.Alert);
                    }
                    else if (!args[0].Equals(EntoaroxFrameworkMod.Verify))
                    {
                        EntoaroxFrameworkMod.Verify = null;
                        EntoaroxFrameworkMod.Logger.Log("Verification failed, attempt to remove objects has been cancelled.", LogLevel.Error);
                    }
                    else
                    {
                        EntoaroxFrameworkMod.Verify = null;
                        Farm farm = Game1.getFarm();
                        farm.objects.Clear();
                    }

                    break;
                case "player_warp":
                    try
                    {
                        int x = Convert.ToInt32(args[1]);
                        int y = Convert.ToInt32(args[2]);
                        Game1.warpFarmer(args[0], x, y, false);
                        this.Monitor.Log("Player warped.", LogLevel.Alert);
                    }
                    catch (Exception err)
                    {
                        this.Monitor.Log("A error occured trying to warp: ", LogLevel.Error, err);
                    }

                    break;
            }
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.SetupSerializer();
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            this.EnforceSerializer();
            if (!Context.IsWorldReady)
                return;
            if (Game1.player.CurrentItem == null && this.PrevItem != null || Game1.player.CurrentItem != null && !Game1.player.CurrentItem.Equals(this.PrevItem))
            {
                ItemEvents.FireActiveItemChanged(new EventArgsActiveItemChanged(this.PrevItem, Game1.player.CurrentItem));
                this.PrevItem = Game1.player.CurrentItem;
            }

            IModHelperExtensions.PlayerModifierHelper.UpdateTick();
            if (Context.IsPlayerFree)
            {
                Vector2 playerPos = new Vector2(Game1.player.getStandingX() / Game1.tileSize, Game1.player.getStandingY() / Game1.tileSize);
                if (EntoaroxFrameworkMod.LastTouchAction != playerPos)
                {
                    string text = Game1.currentLocation.doesTileHaveProperty((int)playerPos.X, (int)playerPos.Y, "TouchAction", "Back");
                    EntoaroxFrameworkMod.LastTouchAction = playerPos;
                    if (text != null)
                    {
                        string[] split = text.Split(' ');
                        string[] args = new string[split.Length - 1];
                        Array.Copy(split, 1, args, 0, args.Length);
                        this.ActionInfo = new EventArgsActionTriggered(Game1.player, split[0], args, playerPos);
                        MoreEvents.FireTouchActionTriggered(this.ActionInfo);
                    }
                }
            }
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // read data
            if (Context.IsMainPlayer)
            {
                this.Monitor.Log("Unpacking custom objects...", LogLevel.Trace);
                ItemEvents.FireBeforeDeserialize();
                IDictionary<string, InstanceState> data = this.Helper.Data.ReadSaveData<Dictionary<string, InstanceState>>("custom-items");
                if (data == null)
                {
                    // read from legacy mod file
                    FileInfo legacyFile = new FileInfo(Path.Combine(Constants.CurrentSavePath, "Entoarox.Framework", "CustomItems.json"));
                    if (legacyFile.Exists)
                        data = JsonConvert.DeserializeObject<Dictionary<string, InstanceState>>(File.ReadAllText(legacyFile.FullName));
                }

                if (data == null)
                    data = new Dictionary<string, InstanceState>();
                this.RestoreItems(data);
                ItemEvents.FireAfterDeserialize();
            }
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                this.Monitor.Log("Packing custom objects...", LogLevel.Trace);
                ItemEvents.FireBeforeSerialize();
                Dictionary<string, InstanceState> data = new Dictionary<string, InstanceState>();
                foreach (GameLocation loc in Game1.locations)
                {
                    foreach (Chest chest in loc.Objects.Values.OfType<Chest>())
                        this.Serialize(data, chest.items);
                }

                this.Serialize(data, Game1.player.Items);
                FarmHouse house = Game1.getLocationFromName("FarmHouse") as FarmHouse;

                if (house.fridge.Value != null)
                    this.Serialize(data, house.fridge.Value.items);
                this.Helper.Data.WriteSaveData("custom-items", data);
                ItemEvents.FireAfterSerialize();
            }
        }

        /// <summary>Raised after the game finishes writing data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaved(object sender, SavedEventArgs e)
        {
            // delete legacy mod data (migrated into save file by this point)
            {
                FileInfo legacyFile = new FileInfo(Path.Combine(Constants.CurrentSavePath, "Entoarox.Framework", "CustomItems.json"));
                if (legacyFile.Exists)
                    legacyFile.Delete();

                DirectoryInfo legacyDir = new DirectoryInfo(Path.Combine(Constants.CurrentSavePath, "Entoarox.Framework"));
                if (legacyDir.Exists && !legacyDir.GetFileSystemInfos().Any())
                    legacyDir.Delete();
            }

            // read data
            this.Monitor.Log("Unpacking custom objects...", LogLevel.Trace);
            ItemEvents.FireBeforeDeserialize();
            IDictionary<string, InstanceState> data = this.Helper.Data.ReadSaveData<Dictionary<string, InstanceState>>("custom-items") ?? new Dictionary<string, InstanceState>();
            this.RestoreItems(data);
            ItemEvents.FireAfterDeserialize();
        }

        private void RestoreItems(IDictionary<string, InstanceState> data)
        {
            foreach (GameLocation loc in Game1.locations)
            {
                foreach (Chest chest in loc.Objects.Values.OfType<Chest>())
                    this.Deserialize(data, chest.items);
            }

            FarmHouse house = (FarmHouse)Game1.getLocationFromName("FarmHouse");
            this.Deserialize(data, Game1.player.Items);
            this.Deserialize(data, house.fridge.Value.items);
        }

        private void Serialize(IDictionary<string, InstanceState> data, IList<Item> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                if (item is ICustomItem)
                {
                    string id = Guid.NewGuid().ToString();
                    int counter = 0;
                    while (data.ContainsKey(id) && counter++ < 25)
                        id = Guid.NewGuid().ToString();
                    if (counter >= 25)
                        throw new TimeoutException("Unable to assign a GUID to all items!");
                    SObject obj = new SObject
                    {
                        Stack = item.getStack(),
                        ParentSheetIndex = 0,
                        Type = id,
                        name = "(Entoarox.Framework.ICustomItem)",
                        Price = item.salePrice()
                    };
                    if (item is Placeholder pitm)
                        data.Add(id, new InstanceState(pitm.Id, pitm.Data));
                    else
                        data.Add(id, new InstanceState(item.GetType().AssemblyQualifiedName, JToken.FromObject(item, this.Serializer)));

                    items[i] = obj;
                }
            }
        }

        private void Deserialize(IDictionary<string, InstanceState> data, IList<Item> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                if (item is SObject obj && obj.name.Equals("(Entoarox.Framework.ICustomItem)"))
                {
                    if (data.ContainsKey(obj.Type))
                    {
                        string cls = data[obj.Type].Type;
                        Type type = Type.GetType(cls);
                        if (type == null)
                        {
                            this.Monitor.Log("Unable to deserialize custom item, type does not exist: " + cls, LogLevel.Error);
                            items[i] = new Placeholder(cls, data[obj.Type].Data);
                        }
                        else if (!typeof(Item).IsAssignableFrom(type))
                        {
                            this.Monitor.Log("Unable to deserialize custom item, class does not inherit from StardewValley.Item in any form: " + cls, LogLevel.Error);
                            items[i] = new Placeholder(cls, data[obj.Type].Data);
                        }
                        else if (!type.GetInterfaces().Contains(typeof(ICustomItem)))
                        {
                            this.Monitor.Log("Unable to deserialize custom item, item class does not implement the ICustomItem interface: " + cls, LogLevel.Error);
                            items[i] = new Placeholder(cls, data[obj.Type].Data);
                        }
                        else
                            try
                            {
                                items[i] = (Item)data[obj.Type].Data.ToObject(type, this.Serializer);
                            }
                            catch (Exception err)
                            {
                                this.Monitor.Log("Unable to deserialize custom item of type " + cls + ", unknown error:\n" + err, LogLevel.Error);
                                items[i] = new Placeholder(cls, data[obj.Type].Data);
                            }
                    }
                    else
                        this.Monitor.Log("Unable to deserialize custom item, GUID does not exist: " + obj.Type, LogLevel.Error);
                }
            }
        }

        /****
        ** Serializer
        ****/
        private void SetupSerializer()
        {
            this.MainSerializer = new XmlSerializer(typeof(SaveGame), EntoaroxFrameworkMod.SerialiserTypes.Concat(EntoaroxFrameworkMod.SerializerTypes).ToArray());
            this.FarmerSerializer = new XmlSerializer(typeof(Farmer), EntoaroxFrameworkMod.FarmerTypes.Concat(EntoaroxFrameworkMod.SerializerTypes).ToArray());
            this.LocationSerializer = new XmlSerializer(typeof(GameLocation), EntoaroxFrameworkMod.LocationTypes.Concat(EntoaroxFrameworkMod.SerializerTypes).ToArray());
            EntoaroxFrameworkMod.SerializerInjected = true;
            this.EnforceSerializer();
        }

        private void EnforceSerializer()
        {
            SaveGame.serializer = this.MainSerializer;
            SaveGame.farmerSerializer = this.FarmerSerializer;
            SaveGame.locationSerializer = this.LocationSerializer;
        }
    }
}
