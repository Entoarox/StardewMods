using System;
using System.Reflection;
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Entoarox.Framework.Core
{
    using Core.Utilities;
    using Extensions;
    using Events;
    using xTile.Tiles;
    using xTile.ObjectModel;

    internal class ModEntry : Mod
    {
        #region References
        internal static PropertyInfo Loaders;
        internal static PropertyInfo Injectors;
        internal static FrameworkConfig Config;
        internal static IMonitor Logger;
        internal static IModHelper SHelper;
        internal static bool SkipCredits = false;
        #endregion
        #region Mod
        public override void Entry(IModHelper helper)
        {
            SHelper = helper;
            Logger = Monitor;
            Config = Helper.ReadConfig<FrameworkConfig>();
            Helper.ConsoleCommands.Add("world_bushreset", "Resets bushes in the whole game, use this if you installed a map mod and want to keep using your old save.", Commands);
            if (Config.TrainerCommands)
                helper.ConsoleCommands
                    .Add("farm_settype", "farm_settype <type> | Enables you to change your farm type to any of the following: " + string.Join(",", Farms), Commands)
                    .Add("farm_clear", "farm_clear | Removes ALL objects from your farm, this cannot be undone!", Commands)

                    .Add("player_warp", "player_warp <location> <x> <y> | Warps the player to the given position in the game.", Commands)
                ;
            GameEvents.UpdateTick += GameEvents_FirstUpdateTick;
            Helper.RequestUpdateCheck("https://raw.githubusercontent.com/Entoarox/StardewMods/master/Framework/About/update.json");
            // Setup content manager hooks
            Type t = Game1.content.GetType();
            Loaders = t.GetProperty("Loaders", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            Injectors = t.GetProperty("Injectors", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }
        #endregion
        #region Events

        private static List<string> Farms = new List<string>() { "standard", "river", "forest", "hilltop", "wilderniss" };
        private static string Verify;
        private static bool CreditsDone = false;
        private EventArgsActionTriggered ActionInfo;
        private Item prevItem = null;
        private void ControlEvents_ControllerButtonPressed(object sender, EventArgsControllerButtonPressed e)
        {
            if (e.ButtonPressed == Buttons.A)
                CheckForAction();
        }
        private void ControlEvents_ControllerButtonReleased(object sender, EventArgsControllerButtonReleased e)
        {
            if (ActionInfo != null && e.ButtonReleased == Buttons.A)
            {
                MoreEvents.FireActionTriggered(ActionInfo);
                ActionInfo = null;
            }
        }
        private void ControlEvents_MouseChanged(object sender, EventArgsMouseStateChanged e)
        {
            if (e.NewState.RightButton == ButtonState.Pressed && e.PriorState.RightButton != ButtonState.Pressed)
                CheckForAction();
            if (ActionInfo != null && e.NewState.RightButton == ButtonState.Released)
            {
                MoreEvents.FireActionTriggered(ActionInfo);
                ActionInfo = null;
            }
        }
        private void CheckForAction()
        {
            if (Game1.activeClickableMenu == null && !Game1.player.UsingTool && !Game1.pickingTool && !Game1.menuUp && (!Game1.eventUp || Game1.currentLocation.currentEvent.playerControlSequence) && !Game1.nameSelectUp && Game1.numberOfSelectedItems == -1 && !Game1.fadeToBlack)
            {
                ActionInfo = null;
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
                    ActionInfo = new EventArgsActionTriggered(Game1.player, split[0], args, grabTile);
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
                        Monitor.Log("Please provide the type you wish to change your farm to.", LogLevel.Error);
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
                        Monitor.Log("Player warped.", LogLevel.Alert);
                    }
                    catch (Exception err)
                    {
                        Monitor.Log("A error occured trying to warp: ", LogLevel.Error, err);
                    }
                    break;
            }
        }
        private void GameEvents_FirstUpdateTick(object s, EventArgs e)
        {
            GameEvents.UpdateTick -= GameEvents_FirstUpdateTick;
            if (Config.SkipCredits || SkipCredits)
            {
                if (CreditsDone || !(Game1.activeClickableMenu is StardewValley.Menus.TitleMenu) || Game1.activeClickableMenu == null)
                    return;
                Game1.playSound("bigDeSelect");
                Helper.Reflection.GetPrivateField<int>(Game1.activeClickableMenu, "logoFadeTimer").SetValue(0);
                Helper.Reflection.GetPrivateField<int>(Game1.activeClickableMenu, "fadeFromWhiteTimer").SetValue(0);
                Game1.delayedActions.Clear();
                Helper.Reflection.GetPrivateField<int>(Game1.activeClickableMenu, "pauseBeforeViewportRiseTimer").SetValue(0);
                Helper.Reflection.GetPrivateField<float>(Game1.activeClickableMenu, "viewportY").SetValue(-999);
                Helper.Reflection.GetPrivateField<float>(Game1.activeClickableMenu, "viewportDY").SetValue(-0.01f);
                Helper.Reflection.GetPrivateField<List<TemporaryAnimatedSprite>>(Game1.activeClickableMenu, "birds").GetValue().Clear();
                Helper.Reflection.GetPrivateField<float>(Game1.activeClickableMenu, "logoSwipeTimer").SetValue(-1);
                Helper.Reflection.GetPrivateField<int>(Game1.activeClickableMenu, "chuckleFishTimer").SetValue(0);
                Game1.changeMusicTrack("MainTheme");
                CreditsDone = true;
            }
            SetupSerializer();
            Core.UpdateHandler.DoUpdateChecks();
            GameEvents.UpdateTick += GameEvents_UpdateTick;
        }
        private void GameEvents_UpdateTick(object s, EventArgs e)
        {
            EnforceSerializer();
            if ((Game1.player.CurrentItem == null && prevItem != null) || (Game1.player.CurrentItem != null && !Game1.player.CurrentItem.Equals(prevItem)))
            {
                MoreEvents.FireActiveItemChanged(new EventArgsActiveItemChanged(prevItem, Game1.player.CurrentItem));
                prevItem = Game1.player.CurrentItem;
            }
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
            MainSerializer = new XmlSerializer(typeof(SaveGame), _serialiserTypes.Concat(SerializerTypes).ToArray());
            FarmerSerializer = new XmlSerializer(typeof(StardewValley.Farmer), _farmerTypes.Concat(SerializerTypes).ToArray());
            LocationSerializer = new XmlSerializer(typeof(GameLocation), _locationTypes.Concat(SerializerTypes).ToArray());
            SerializerInjected = true;
            EnforceSerializer();
        }
        private void EnforceSerializer()
        {
            SaveGame.serializer = MainSerializer;
            SaveGame.farmerSerializer = FarmerSerializer;
            SaveGame.locationSerializer = LocationSerializer;
        }
        #endregion
        #region Misc
        private void PrepareCustomEvents()
        {
            ControlEvents.ControllerButtonPressed += ControlEvents_ControllerButtonPressed;
            ControlEvents.ControllerButtonReleased += ControlEvents_ControllerButtonReleased;
            ControlEvents.MouseChanged += ControlEvents_MouseChanged;
            GameEvents.UpdateTick += GameEvents_UpdateTick;
        }
        #endregion
    }
}
