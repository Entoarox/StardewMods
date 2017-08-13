using System;
using System.Collections.Generic;
using Version = System.Version;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace Entoarox.Framework
{
    public class EntoFramework : Mod
    {
        private static bool CreditsDone = true;
        internal static LoaderTypes LoaderType;
        internal static FrameworkConfig Config;
        public static Version Version { get { return typeof(EntoFramework).Assembly.GetName().Version; } }
        internal enum LoaderTypes
        {
            Unknown,
            SMAPI,
            FarmHand
        }
        /**
         * <summary>Retrieves a pointer to the <see cref="ILocationHelper"/> interface</summary>
         */
#pragma warning disable CS0618 // Type or member is obsolete
        public static ILocationHelper GetLocationHelper()
#pragma warning restore CS0618 // Type or member is obsolete
        {
            return LocationHelper.Singleton;
        }
        /**
         * <summary>Retrieves a pointer to the <see cref="ITypeRegistry"/> interface</summary>
         */
        public static ITypeRegistry GetTypeRegistry()
        {
            return TypeRegistry.Singleton;
        }
        /**
         * <summary>Retrieves a pointer to the <see cref="IContentRegistry"/> interface</summary>
         */
        public static IContentRegistry GetContentRegistry()
        {
            return ContentRegistry.Singleton;
        }
        /**
         * <summary>Retrieves a pointer to the <see cref="IPlayerHelper"/> interface</summary>
         */
        public static IPlayerHelper GetPlayerHelper()
        {
            return PlayerHelper.Singleton;
        }
        /**
         * <summary>Retrieves a pointer to the <see cref="IMessageBox"/> interface</summary>
         */
        public static IMessageBox GetMessageBox()
        {
            return MessageBox.Singleton;
        }
        public static void VersionRequired(string modRequiring, Version requiringVersion)
        {
            if (Version < requiringVersion)
                Logger.ExitGameImmediately($"The `{modRequiring}` mod requires EntoaroxFramework version [{requiringVersion}] or newer to work.");
        }
        internal static IMonitor Logger;
        public override void Entry(IModHelper helper)
        {
            Logger = Monitor;
            Config = helper.ReadConfig<FrameworkConfig>();
            if (Constants.ApiVersion.Build == "Farmhand-Smapi")
            {
                LoaderType = LoaderTypes.FarmHand;
                Logger.Log("The loader has been detected as being the `FarmHand` loader",LogLevel.Trace);
            }
            else
            {
                LoaderType = LoaderTypes.SMAPI;
                Logger.Log("The loader has been detected as being the `SMAPI` loader",LogLevel.Trace);
            }
            Logger.Log("Registering framework events...",LogLevel.Trace);
            helper.ConsoleCommands.Add("ef_bushreset", "Resets bushes in the whole game, use this if you installed a map mod and want to keep using your old save.", Internal.BushReset.Trigger);
            if(Config.TrainerCommands)
            {
                helper.ConsoleCommands
                    .Add("farm_settype", "farm_settype <type> | Enables you to change your farm type to any of the following: " + string.Join(",",Commands.Commands.Farms), Commands.Commands.farm)
                    .Add("farm_clear", "farm_clear | Removes ALL objects from your farm, this cannot be undone!", Commands.Commands.farm)

                    .Add("player_warp","player_warp <location> <x> <y> | Warps the player to the given position in the game.",Commands.Commands.player)
                ;
            }
            GameEvents.UpdateTick += FirstUpdateTick;
            GameEvents.UpdateTick += GameEvents_LoadTick;
            ContentRegistry.Setup();
            TypeRegistry.Setup();
            Events.MoreEvents.Setup();
            if (LoaderType == LoaderTypes.SMAPI)
                GameEvents.UpdateTick += TypeRegistry.Update;
            SaveEvents.AfterReturnToTitle += SaveEvents_AfterReturnToTitle;
            if (Config.SkipCredits)
                GameEvents.UpdateTick += CreditsTick;
            if(LoaderType==LoaderTypes.Unknown)
                Monitor.ExitGameImmediately("Detected that the `FarmHand` loader was used, but was unable to hook into it");
            Logger.Log("Framework has finished!",LogLevel.Info);
            VersionChecker.AddCheck("EntoaroxFramework", Version, "https://raw.githubusercontent.com/Entoarox/StardewMods/master/VersionChecker/EntoaroxFramework.json");
        }
        internal static void FirstUpdateTick(object s, EventArgs e)
        {
            if (LoaderType == LoaderTypes.SMAPI)
            {
                TypeRegistry.Init();
                ContentRegistry.Init();
                GameEvents.UpdateTick += ContentRegistry.Update;
            }
            else
                Events.MoreEvents.FireSmartManagerReady();

            GameEvents.UpdateTick -= FirstUpdateTick;
        }
        public static void SaveEvents_AfterReturnToTitle(object s, EventArgs e)
        {
            GameEvents.UpdateTick -= PlayerHelper.Update;
            LocationEvents.CurrentLocationChanged -= PlayerHelper.LocationEvents_CurrentLocationChanged;
            if (Config.GamePatcher)
            {
                GameEvents.UpdateTick -= GamePatcher.Update;
                TimeEvents.DayOfMonthChanged -= GamePatcher.TimeEvents_DayOfMonthChanged;
                Events.MoreEvents.ActionTriggered -= GamePatcher.MoreEvents_ActionTriggered;
            }
        }
        public static void CreditsTick(object s, EventArgs e)
        {
            if (!(Game1.activeClickableMenu is StardewValley.Menus.TitleMenu) || Game1.activeClickableMenu == null)
                return;
            if (CreditsDone)
            {
                GameEvents.UpdateTick -= CreditsTick;
                return;
            }
            Game1.playSound("bigDeSelect");
            Reflection.ReflectionUtility.SetField(Game1.activeClickableMenu, "logoFadeTimer", 0);
            Reflection.ReflectionUtility.SetField(Game1.activeClickableMenu, "fadeFromWhiteTimer", 0);
            Game1.delayedActions.Clear();
            Reflection.ReflectionUtility.SetField(Game1.activeClickableMenu, "pauseBeforeViewportRiseTimer", 0);
            Reflection.ReflectionUtility.SetField(Game1.activeClickableMenu, "viewportY", -999f);
            Reflection.ReflectionUtility.SetField(Game1.activeClickableMenu, "viewportDY", -0.01f);
            Reflection.ReflectionUtility.GetField<List<TemporaryAnimatedSprite>>(Game1.activeClickableMenu, "birds").Clear();
            Reflection.ReflectionUtility.SetField(Game1.activeClickableMenu, "logoSwipeTimer", 1f);
            Reflection.ReflectionUtility.SetField(Game1.activeClickableMenu, "chuckleFishTimer", 0);
            Game1.changeMusicTrack("MainTheme");
            CreditsDone = true;
        }
        internal static void GameEvents_LoadTick(object s, EventArgs e)
        {
            if (!Game1.hasLoadedGame || Game1.CurrentEvent != null)
                return;
            MessageBox.Setup();
            VersionChecker.DoChecks();
            PlayerHelper.ResetForNewGame();
            GameEvents.UpdateTick -= GameEvents_LoadTick;
            if (Config.GamePatcher)
            {
                GamePatcher.Patch();
                GameEvents.UpdateTick += GamePatcher.Update;
                TimeEvents.DayOfMonthChanged += GamePatcher.TimeEvents_DayOfMonthChanged;
                Events.MoreEvents.ActionTriggered += GamePatcher.MoreEvents_ActionTriggered;
            }
            GameEvents.UpdateTick += PlayerHelper.Update;
            LocationEvents.CurrentLocationChanged += PlayerHelper.LocationEvents_CurrentLocationChanged;
            Events.MoreEvents.FireWorldReady();

            /*
            Logger.Log("DEBUG: HookedLocation handling", LogLevel.Alert);
            GameLocation result = Reflection.HookedLocation.Create(new Shed(Game1.getFarm().map, "TestLoc"));
            Logger.Log("DEBUG OUT:" + string.Join(",", result.GetType().GetMethod("drawWater", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetMethodBody().GetILAsByteArray()), LogLevel.Alert);
            try
            {
                result.drawWater(Game1.spriteBatch);
                Logger.Log("DEBUG OUT: Call succeeded.",LogLevel.Alert);
            }
            catch(Exception err)
            {
                Logger.Log(LogLevel.Alert, "DEBUG ERROR:", err);
            }
            */
        }
    }
}
