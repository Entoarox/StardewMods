using System;
using System.Collections.Generic;
using Version = System.Version;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

#pragma warning disable CS0618 // Type or member is obsolete
namespace Entoarox.Framework
{
    internal class FrameworkConfig : Config
    {
        public override T GenerateDefaultConfig<T>()
        {
            GamePatcher = true;
            SkipCredits = false;
            return this as T;
        }
        public bool GamePatcher;
        public bool SkipCredits;
    }
    public class EntoFramework : Mod
    {
        internal static DataLogger Logger;
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
         * <summary>Retrieves a pointer to the <see cref="IPlayerHelper"/> interface</summary>
         */
        public static IMessageBox GetMessageBox()
        {
            return MessageBox.Singleton;
        }
        public override void Entry(params object[] objects)
        {
            Config = new FrameworkConfig().InitializeConfig(BaseConfigPath);
            Logger = new DataLogger("EntoaroxFramework",4);
            Logger.LogModInfo("Entoarox", GetType().Assembly.GetName().Version);
            Logger.Debug("Attempting to resolve loader...");
            if (Constants.Version.Build == "Farmhand-Smapi")
            {
                LoaderType = LoaderTypes.FarmHand;
                Logger.Trace("The loader has been detected as being the `FarmHand` loader");
            }
            else
            {
                LoaderType = LoaderTypes.SMAPI;
                Logger.Trace("The loader has been detected as being the `SMAPI` loader");
            }
            Logger.Trace("Registering framework events...");
            GameEvents.UpdateTick += GameEvents_LoadTick;
            ContentRegistry.Setup();
            TypeRegistry.Setup();
            if (LoaderType == LoaderTypes.SMAPI)
            {
                GameEvents.FirstUpdateTick += GameEvents_FirstUpdateTick;
                GameEvents.UpdateTick += TypeRegistry.Update;
                GameEvents.Initialize += TypeRegistry.Init;
            }
            if(Config.SkipCredits)
                GameEvents.UpdateTick += CreditsTick;
            Logger.Debug("Preparation complete!");
            if(LoaderType==LoaderTypes.Unknown)
                Logger.Error("Was unable to hook into the `FarmHand` loader, has it updated since the last framework release?");
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
            Game1.playSound("bigDeSelect");
            Reflection.FieldHelper.SetField(Game1.activeClickableMenu, "logoFadeTimer", 0);
            Reflection.FieldHelper.SetField(Game1.activeClickableMenu, "fadeFromWhiteTimer", 0);
            Game1.delayedActions.Clear();
            Reflection.FieldHelper.SetField(Game1.activeClickableMenu, "pauseBeforeViewportRiseTimer", 0);
            Reflection.FieldHelper.SetField(Game1.activeClickableMenu, "viewportY", -999f);
            Reflection.FieldHelper.SetField(Game1.activeClickableMenu, "viewportDY", -0.01f);
            Reflection.FieldHelper.GetField<List<TemporaryAnimatedSprite>>(Game1.activeClickableMenu, "birds").Clear();
            Reflection.FieldHelper.SetField(Game1.activeClickableMenu, "logoSwipeTimer", 1f);
            Reflection.FieldHelper.SetField(Game1.activeClickableMenu, "chuckleFishTimer", 0);
            Game1.changeMusicTrack("MainTheme");
            GameEvents.UpdateTick -= CreditsTick;
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
#pragma warning restore CS0618 // Type or member is obsolete
