using System;
using System.Collections.Generic;
using Version = System.Version;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace Entoarox.Framework
{
    internal class FrameworkConfig
    {
        public bool GamePatcher=true;
        public bool SkipCredits=false;
        public bool DebugMode=false;
    }
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
        public static ILocationHelper GetLocationHelper()
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
        internal static IMonitor Logger;
        public override void Entry(IModHelper helper)
        {
            Logger = Monitor;
            Config = helper.ReadConfig<FrameworkConfig>();
            if (Constants.Version.Build == "Farmhand-Smapi")
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
            GameEvents.UpdateTick += GameEvents_LoadTick;
            ContentRegistry.Setup();
            TypeRegistry.Setup();
            Events.MoreEvents.Setup();
            if (LoaderType == LoaderTypes.SMAPI)
            {
                GameEvents.FirstUpdateTick += GameEvents_FirstUpdateTick;
                GameEvents.UpdateTick += TypeRegistry.Update;
                GameEvents.Initialize += TypeRegistry.Init;
            }
            if(Config.SkipCredits)
                GameEvents.UpdateTick += CreditsTick;
            if(LoaderType==LoaderTypes.Unknown)
                Monitor.ExitGameImmediately("Detected that the `FarmHand` loader was used, but was unable to hook into it");
            Logger.Log("Framework has finished!",LogLevel.Info);
            VersionChecker.AddCheck("EntoaroxFramework", Version, "https://raw.githubusercontent.com/Entoarox/StardewMods/master/VersionChecker/EntoaroxFramework.json");
        }
        internal static void GameEvents_FirstUpdateTick(object s, EventArgs e)
        {
            ContentRegistry.Init();
            GameEvents.UpdateTick += ContentRegistry.Update;
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
        }
    }
}
