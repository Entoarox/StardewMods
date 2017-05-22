using System;
using System.IO;
using System.Collections.Generic;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace Entoarox.Framework
{
    public class EntoFramework : Mod
    {
        private static bool CreditsDone = true;
        internal static FrameworkConfig Config;
        private static Version _Version;
        public static Version Version { get => _Version; }
        private static string _PlatformContentDir;
        public static string PlatformContentDir
        {
            get
            {
                if (_PlatformContentDir == null)
                    _PlatformContentDir = File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources", Game1.content.RootDirectory, "XACT", "FarmerSounds.xgs")) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources", Game1.content.RootDirectory) : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content");
                return _PlatformContentDir;
            }
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
        internal static bool Repair = false;
        public override void Entry(IModHelper helper)
        {
            _Version = new Version(ModManifest.Version.MajorVersion, ModManifest.Version.MinorVersion, ModManifest.Version.PatchVersion);
            Logger = Monitor;
            Config = helper.ReadConfig<FrameworkConfig>();
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
            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            Events.MoreEvents.Setup();
            ContentRegistry.Init();
            ContentRegistry.Singleton.ReloadStaticReferences();
            GameEvents.UpdateTick += ContentRegistry.Update;
            GameEvents.UpdateTick += TypeRegistry.Update;
            TypeRegistry.Init();
            SaveEvents.AfterReturnToTitle += SaveEvents_AfterReturnToTitle;
            if (Config.SkipCredits)
                GameEvents.UpdateTick += CreditsTick;
            Logger.Log("Framework has finished!",LogLevel.Info);
            VersionChecker.AddCheck("EntoaroxFramework", Version, "https://raw.githubusercontent.com/Entoarox/StardewMods/master/VersionChecker/EntoaroxFramework.json");
        }
        public static void SaveEvents_AfterReturnToTitle(object s, EventArgs e)
        {
            GameEvents.UpdateTick -= PlayerHelper.Update;
            LocationEvents.CurrentLocationChanged -= PlayerHelper.LocationEvents_CurrentLocationChanged;
            GameEvents.UpdateTick -= ContentRegistry.Update;
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
        internal static void SaveEvents_AfterLoad(object s, EventArgs e)
        {
            if (Repair)
            {
                ContentRegistry.Init();
                ContentRegistry.Singleton.ReloadStaticReferences();
                GameEvents.UpdateTick += ContentRegistry.Update;
            }
            else
                Repair = true;
            MessageBox.Setup();
            VersionChecker.DoChecks();
            PlayerHelper.ResetForNewGame();
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
