using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Characters;
using StardewValley.Monsters;
using StardewValley.Quests;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using xTile.Tiles;
using xTile.ObjectModel;

namespace Entoarox.Framework.Core
{
    using Events;
    using Core.AssetHandlers;
    using Extensions;

    internal class EntoaroxFrameworkMod : Mod
    {
        #region References
        internal static FrameworkConfig Config;
        internal static IMonitor Logger;
        internal static IModHelper SHelper;
        internal static bool SkipCredits = false;
        internal static List<(string Mod, Keys key, Delegate Handler)> Keybinds = new List<(string Mod, Keys key, Delegate Handler)>();

        private static Dictionary<Keys, (string Mod, Delegate Handler)> _Keybinds = new Dictionary<Keys, (string Mod, Delegate Handler)>();
        private static List<string> Farms = new List<string>() { "standard", "river", "forest", "hilltop", "wilderniss" };
        private static string Verify;
        private static bool CreditsDone = false;
        private EventArgsActionTriggered ActionInfo;
        private Item prevItem = null;
        private static Vector2? LastTouchAction = null;
        #endregion
        #region Mod
        public override void Entry(IModHelper helper)
        {
            var TypeHandler = new DeferredAssetHandler();
            helper.Content.AssetEditors.Add(TypeHandler);
            helper.Content.AssetEditors.Add(TypeHandler);
            helper.Content.AssetEditors.Add(new DeferredTypeHandler());
            helper.Content.AssetLoaders.Add(new XnbLoader());
            helper.Content.AssetEditors.Add(new TextureInjector());
            helper.Content.AssetEditors.Add(new DictionaryInjector());
            SHelper = this.Helper;
            Logger = this.Monitor;
            Config = this.Helper.ReadConfig<FrameworkConfig>();
            this.PrepareCustomEvents();
            this.Helper.ConsoleCommands.Add("world_bushreset", "Resets bushes in the whole game, use this if you installed a map mod and want to keep using your old save.", this.Commands);
            if (Config.TrainerCommands)
                helper.ConsoleCommands
                    .Add("farm_settype", "farm_settype <type> | Enables you to change your farm type to any of the following: " + string.Join(",", Farms), this.Commands)
                    .Add("farm_clear", "farm_clear | Removes ALL objects from your farm, this cannot be undone!", this.Commands)

                    .Add("player_warp", "player_warp <location> <x> <y> | Warps the player to the given position in the game.", this.Commands)
                ;
            GameEvents.UpdateTick += this.GameEvents_FirstUpdateTick;
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
            SaveEvents.AfterLoad += this.SaveEvents_AfterSave;
            this.Helper.RequestUpdateCheck("https://raw.githubusercontent.com/Entoarox/StardewMods/master/Framework/About/update.json");
        }
        #endregion
        #region Events
        private void ControlEvents_KeyboardChanged(object sender, EventArgsKeyboardStateChanged e)
        {
           foreach(Keys key in e.PriorState.GetPressedKeys())
                if(e.NewState.IsKeyUp(key) && _Keybinds.ContainsKey(key))
                    try
                    {
                        _Keybinds[key].Handler.DynamicInvoke();
                    }
                    catch (Exception ex)
                    {
                        string[] parts = _Keybinds[key].Mod.Split('\n');
                        Logger.Log($"The {parts[0]} mod failed handling the `{parts[1]}` keybind being triggered:\n{ex}", LogLevel.Error);
                    }
        }
        private void ControlEvents_ControllerButtonPressed(object sender, EventArgsControllerButtonPressed e)
        {
            if (e.ButtonPressed == Buttons.A)
                CheckForAction();
        }
        private void ControlEvents_ControllerButtonReleased(object sender, EventArgsControllerButtonReleased e)
        {
            if (this.ActionInfo != null && e.ButtonReleased == Buttons.A)
            {
                MoreEvents.FireActionTriggered(this.ActionInfo);
                this.ActionInfo = null;
            }
        }
        private void ControlEvents_MouseChanged(object sender, EventArgsMouseStateChanged e)
        {
            if (e.NewState.RightButton == ButtonState.Pressed && e.PriorState.RightButton != ButtonState.Pressed)
                CheckForAction();
            if (this.ActionInfo != null && e.NewState.RightButton == ButtonState.Released)
            {
                MoreEvents.FireActionTriggered(this.ActionInfo);
                this.ActionInfo = null;
            }
        }
        private void CheckForAction()
        {
            if (Game1.activeClickableMenu == null && !Game1.player.UsingTool && !Game1.pickingTool && !Game1.menuUp && (!Game1.eventUp || Game1.currentLocation.currentEvent.playerControlSequence) && !Game1.nameSelectUp && Game1.numberOfSelectedItems == -1 && !Game1.fadeToBlack)
            {
                this.ActionInfo = null;
                Vector2 grabTile = new Vector2((Game1.getOldMouseX() + Game1.viewport.X), (Game1.getOldMouseY() + Game1.viewport.Y)) / Game1.tileSize;
                if (!Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
                    grabTile = Game1.player.GetGrabTile();
                Tile tile = Game1.currentLocation.map.GetLayer("Buildings").PickTile(new xTile.Dimensions.Location((int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize), Game1.viewport.Size);
                PropertyValue propertyValue = null;
                if (tile != null)
                    tile.Properties.TryGetValue("Action", out propertyValue);
                if (propertyValue != null)
                {
                    string[] split = ((string)propertyValue).Split(' ');
                    string[] args = new string[split.Length - 1];
                    Array.Copy(split, 1, args, 0, args.Length);
                    this.ActionInfo = new EventArgsActionTriggered(Game1.player, split[0], args, grabTile);
                }
            }
        }
        private void Commands(string command, string[] args)
        {
            if (!Game1.hasLoadedGame)
            {
                Logger.Log("You need to load a game before you can use this command.", LogLevel.Error);
                return;
            }
            switch (command)
            {
                case "world_bushreset":
                    foreach (GameLocation loc in Game1.locations)
                    {
                        loc.largeTerrainFeatures = loc.largeTerrainFeatures.FindAll(a => !(a is Bush));
                        if ((loc.isOutdoors || loc.name.Equals("BathHouse_Entry") || loc.treatAsOutdoors) && loc.map.GetLayer("Paths") != null)
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
                    else if (Farms.Contains(args[0]))
                    {
                        Game1.whichFarm = Farms.IndexOf(args[0]);
                        Logger.Log($"Changed farm type to `{args[0]}`, please sleep in a bed then quit&restart to finalize this change.", LogLevel.Alert);
                    }
                    else
                        Logger.Log("Unknown farm type: " + args[0], LogLevel.Error);
                    break;
                case "farm_clear":
                    if (Verify == null && args.Length == 0)
                    {
                        Verify = new Random().Next().ToString("XXX");
                        Logger.Log($"This will remove all objects, natural and user-made from your farm, use `farm_clear {Verify}` to verify that you actually want to do this.", LogLevel.Alert);
                        return;
                    }
                    else if (!args[0].Equals(Verify))
                    {
                        Verify = null;
                        Logger.Log($"Verification failed, attempt to remove objects has been cancelled.", LogLevel.Error);
                        return;
                    }
                    else
                    {
                        Verify = null;
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
        private void GameEvents_FirstUpdateTick(object s, EventArgs e)
        {
            GameEvents.UpdateTick -= this.GameEvents_FirstUpdateTick;
            if (Config.SkipCredits || SkipCredits)
            {
                if (CreditsDone || !(Game1.activeClickableMenu is StardewValley.Menus.TitleMenu) || Game1.activeClickableMenu == null)
                    return;
                Game1.playSound("bigDeSelect");
                this.Helper.Reflection.GetField<int>(Game1.activeClickableMenu, "logoFadeTimer").SetValue(0);
                this.Helper.Reflection.GetField<int>(Game1.activeClickableMenu, "fadeFromWhiteTimer").SetValue(0);
                Game1.delayedActions.Clear();
                this.Helper.Reflection.GetField<int>(Game1.activeClickableMenu, "pauseBeforeViewportRiseTimer").SetValue(0);
                this.Helper.Reflection.GetField<float>(Game1.activeClickableMenu, "viewportY").SetValue(-999);
                this.Helper.Reflection.GetField<float>(Game1.activeClickableMenu, "viewportDY").SetValue(-0.01f);
                this.Helper.Reflection.GetField<List<TemporaryAnimatedSprite>>(Game1.activeClickableMenu, "birds").GetValue().Clear();
                this.Helper.Reflection.GetField<float>(Game1.activeClickableMenu, "logoSwipeTimer").SetValue(-1);
                this.Helper.Reflection.GetField<int>(Game1.activeClickableMenu, "chuckleFishTimer").SetValue(0);
                Game1.changeMusicTrack("MainTheme");
                CreditsDone = true;
            }
            SetupSerializer();
            Core.UpdateHandler.DoUpdateChecks();
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
        }
        private void GameEvents_UpdateTick(object s, EventArgs e)
        {
            EnforceSerializer();
            if (!Context.IsWorldReady)
                return;
            if ((Game1.player.CurrentItem == null && this.prevItem != null) || (Game1.player.CurrentItem != null && !Game1.player.CurrentItem.Equals(this.prevItem)))
            {
                MoreEvents.FireActiveItemChanged(new EventArgsActiveItemChanged(this.prevItem, Game1.player.CurrentItem));
                this.prevItem = Game1.player.CurrentItem;
            }
            PlayerModifierHelper._UpdateModifiers();
            Vector2 playerPos = new Vector2(Game1.player.getStandingX() / Game1.tileSize, Game1.player.getStandingY() / Game1.tileSize);
            if (LastTouchAction!=playerPos)
            {
                string text = Game1.currentLocation.doesTileHaveProperty((int)playerPos.X, (int)playerPos.Y, "TouchAction", "Back");
                LastTouchAction = playerPos;
                if (text != null)
                {
                    string[] split = (text).Split(' ');
                    string[] args = new string[split.Length - 1];
                    Array.Copy(split, 1, args, 0, args.Length);
                    this.ActionInfo = new EventArgsActionTriggered(Game1.player, split[0], args, playerPos);
                    MoreEvents.FireTouchActionTriggered(this.ActionInfo);
                }
            }
        }
        private void SaveEvents_BeforeSave(object s, EventArgs e)
        {
            ItemEvents.FireBeforeSerialize();
            foreach(GameLocation loc in Game1.locations)
            {
                foreach(Chest chest in loc.objects.Where(a => a.Value is Chest).Select(a => (Chest)a.Value))
                {
                    chest.items = Serialize(chest.items);
                }
            }
            ItemEvents.FireAfterSerialize();
        }
        private void SaveEvents_AfterSave(object s, EventArgs e)
        {
            ItemEvents.FireBeforeDeserialize();
            foreach (GameLocation loc in Game1.locations)
            {
                foreach (Chest chest in loc.objects.Where(a => a.Value is Chest).Select(a => (Chest)a.Value))
                {
                    chest.items = Deserialize(chest.items);
                }
            }
            ItemEvents.FireAfterDeserialize();
        }
        #endregion
        #region Functions
        private List<Item> Serialize(List<Item> items)
        {
            List<Item> output = new List<Item>();
            foreach(SObject item in items.Where(a => a is SObject))
            {
                if (item is ICustomItem)
                {
                    SObject obj = new SObject
                    {
                        parentSheetIndex = 0,
                        type = $"{(item.GetType().AssemblyQualifiedName.Base64Encode())}@{(item as ICustomItem).Sleep().Base64Encode()}",
                        name = "[Entoarox.Framework.ICustomItem]",
                        price = item.price
                    };
                    output.Add(obj);
                    output.Add(obj);
                }
                else
                    output.Add(item);
            }
            return output;
        }
        private List<Item> Deserialize(List<Item> items)
        {
            List<Item> output = new List<Item>();
            foreach (SObject item in items.Where(a => a is SObject))
            {
                if (item.name.Equals("[Entoarox.Framework.ICustomItem]"))
                {
                    SObject obj;
                    string[] split = item.type.Split('@');
                    if (split.Length != 2)
                        obj = item;
                    else
                    {
                        string cls = split[0].Base64Decode();
                        string data = split[1].Base64Decode();
                        try
                        {
                            Type type = Type.GetType(cls, true);
                            if (!typeof(SObject).IsAssignableFrom(type))
                            {
                                this.Monitor.Log("Unable to deserialize custom item, item class is not a Stardew Object: " + cls, LogLevel.Error);
                                obj = item;
                            }
                            else if(!type.GetInterfaces().Contains(typeof(ICustomItem)))
                            {
                                this.Monitor.Log("Unable to deserialize custom item, item class does not implement the ICustomItem interface: " + cls, LogLevel.Error);
                                obj = item;
                            }
                            else
                            {
                                obj = (SObject)Activator.CreateInstance(type);
                                (obj as ICustomItem).Wakeup(data);
                            }
                        }
                        catch(TypeLoadException)
                        {
                            this.Monitor.Log("Unable to deserialize custom item, item class was not found: " + cls, LogLevel.Error);
                            obj = item;
                        }
                        catch(Exception err)
                        {
                            this.Monitor.Log("Unable to deserialize custom item of type " + cls + ", unknown error:\n" + err.ToString(), LogLevel.Error);
                            obj = item;
                        }
                    }
                    output.Add(obj);
                }
                else
                    output.Add(item);
            }
            return output;
        }
        #endregion
        #region Serializer
        public static bool SerializerInjected = false;
        public static List<Type> SerializerTypes = new List<Type>();
        private XmlSerializer MainSerializer;
        private XmlSerializer FarmerSerializer;
        private XmlSerializer LocationSerializer;
        private static Type[] _serialiserTypes = new Type[27]
        {
            typeof (Tool), typeof (GameLocation), typeof (Crow), typeof (Duggy), typeof (Bug), typeof (BigSlime),
            typeof (Fireball), typeof (Ghost), typeof (Child), typeof (Pet), typeof (Dog),
            typeof (StardewValley.Characters.Cat),
            typeof (Horse), typeof (GreenSlime), typeof (LavaCrab), typeof (RockCrab), typeof (ShadowGuy),
            typeof (SkeletonMage),
            typeof (SquidKid), typeof (Grub), typeof (Fly), typeof (DustSpirit), typeof (Quest), typeof (MetalHead),
            typeof (ShadowGirl),
            typeof (Monster), typeof (TerrainFeature)
        };

        private static Type[] _farmerTypes = new Type[1] {
            typeof (Tool)
        };

        private static Type[] _locationTypes = new Type[26]
        {
            typeof (Tool), typeof (Crow), typeof (Duggy), typeof (Fireball), typeof (Ghost),
            typeof (GreenSlime), typeof (LavaCrab), typeof (RockCrab), typeof (ShadowGuy), typeof (SkeletonWarrior),
            typeof (Child), typeof (Pet), typeof (Dog), typeof (StardewValley.Characters.Cat), typeof (Horse),
            typeof (SquidKid),
            typeof (Grub), typeof (Fly), typeof (DustSpirit), typeof (Bug), typeof (BigSlime),
            typeof (BreakableContainer),
            typeof (MetalHead), typeof (ShadowGirl), typeof (Monster), typeof (TerrainFeature)
        };
        private void SetupSerializer()
        {
            this.MainSerializer = new XmlSerializer(typeof(SaveGame), _serialiserTypes.Concat(SerializerTypes).ToArray());
            this.FarmerSerializer = new XmlSerializer(typeof(StardewValley.Farmer), _farmerTypes.Concat(SerializerTypes).ToArray());
            this.LocationSerializer = new XmlSerializer(typeof(GameLocation), _locationTypes.Concat(SerializerTypes).ToArray());
            SerializerInjected = true;
            EnforceSerializer();
        }
        private void EnforceSerializer()
        {
            SaveGame.serializer = this.MainSerializer;
            SaveGame.farmerSerializer = this.FarmerSerializer;
            SaveGame.locationSerializer = this.LocationSerializer;
        }
        #endregion
        #region Misc
        private void PrepareCustomEvents()
        {
            ControlEvents.ControllerButtonPressed += this.ControlEvents_ControllerButtonPressed;
            ControlEvents.ControllerButtonReleased += this.ControlEvents_ControllerButtonReleased;
            ControlEvents.MouseChanged += this.ControlEvents_MouseChanged;
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
        }
        private void SetupHotkeys()
        {
            List<Keys> letters = new List<Keys>()
            {
                Keys.A,
                Keys.B,
                Keys.C,
                Keys.D,
                Keys.E,
                Keys.F,
                Keys.G,
                Keys.H,
                Keys.I,
                Keys.J,
                Keys.K,
                Keys.L,
                Keys.M,
                Keys.N,
                Keys.O,
                Keys.P,
                Keys.Q,
                Keys.R,
                Keys.S,
                Keys.T,
                Keys.U,
                Keys.V,
                Keys.W,
                Keys.X,
                Keys.Y,
                Keys.Z,
                Keys.D0,
                Keys.D1,
                Keys.D2,
                Keys.D3,
                Keys.D4,
                Keys.D5,
                Keys.D6,
                Keys.D7,
                Keys.D8,
                Keys.D9,
                Keys.OemPlus,
                Keys.OemMinus
            };
            letters.RemoveAll(a => Game1.options.isKeyInUse(a));
            _Keybinds = new Dictionary<Keys, (string Mod, Delegate Handler)>();
            var keybinds = new List<(string mod, Keys key, Delegate handler)>(Keybinds);
            Dictionary<string, Keys> memory = this.Helper.ReadJsonFile<Dictionary<string, Keys>>("keybinds.json");
            foreach ((string mod, Keys key, Delegate handler) in new List<(string mod, Keys key, Delegate handler)>(keybinds))
                if (memory.ContainsKey(mod) && letters.Contains(key))
                {
                    _Keybinds.Add(key, (mod, handler));
                    keybinds.Remove((mod, key, handler));
                    letters.Remove(key);
                }
            foreach ((string mod, Keys key, Delegate handler) in new List<(string mod, Keys key, Delegate handler)>(keybinds))
                if (!_Keybinds.ContainsKey(key) && letters.Contains(key))
                {
                    _Keybinds.Add(key, (mod, handler));
                    keybinds.Remove((mod, key, handler));
                    letters.Remove(key);
                }
            foreach ((string mod, Keys key, Delegate handler) in keybinds)
            {
                if (letters.Count > 0)
                {
                    Keys letter = letters[0];
                    letters.Remove(letter);
                    _Keybinds.Add(letter, (mod, handler));
                    Keybinds.Remove((mod, key, handler));
                    keybinds.Add((mod, letter, handler));
                }
                else
                {
                    Keybinds.Remove((mod, key, handler));
                    keybinds.Add((mod, Keys.None, handler));
                    var split = mod.Split('\n');
                    this.Monitor.Log($"Could not automatically provide a hotkey for the `{split[0]}` mod's `{split[1]}` key, you will need to manually assign one!", LogLevel.Error);
                }
            }
            ControlEvents.KeyboardChanged += this.ControlEvents_KeyboardChanged;
        }
        #endregion
    }
}
