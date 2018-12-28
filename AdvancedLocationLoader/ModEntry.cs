using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entoarox.AdvancedLocationLoader.Configs;
using Entoarox.AdvancedLocationLoader.Locations;
using Entoarox.AdvancedLocationLoader.Processing;
using Entoarox.Framework;
using Entoarox.Framework.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using xTile;
using Warp = Entoarox.AdvancedLocationLoader.Configs.Warp;

namespace Entoarox.AdvancedLocationLoader
{
    internal class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        internal static ITranslationHelper Strings;
        internal static IMonitor Logger;
        internal static IModHelper SHelper;
        internal static Compound PatchData;
        private Patcher Patcher;

        /// <summary>Whether a player save has been loaded.</summary>
        internal bool IsSaveLoaded => Game1.hasLoadedGame && !string.IsNullOrEmpty(Game1.player.Name) && !SaveGame.IsProcessing;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // init
            ModEntry.Logger = this.Monitor;
            ModEntry.SHelper = helper;
            ModEntry.Strings = helper.Translation;

            MoreEvents.ActionTriggered += this.OnActionTriggered;
            helper.Events.Specialised.UnvalidatedUpdateTicked += this.OnUnvalidatedUpdateTick;
            helper.Events.Player.Warped += this.OnWarped;

            this.Helper.Content.RegisterSerializerType<Greenhouse>();
            this.Helper.Content.RegisterSerializerType<Sewer>();
            this.Helper.Content.RegisterSerializerType<Desert>();
            this.Helper.Content.RegisterSerializerType<DecoratableLocation>();

            // load content packs
            ContentPackData[] contentPacks = this.LoadContentPackData().ToArray();
            this.Patcher = new Patcher(this.Monitor, this.Helper.Content, contentPacks);
        }

        internal static void UpdateConditionalEdits()
        {
            foreach (Tile t in ModEntry.PatchData.DynamicTiles)
                Processors.ApplyTile(t);
            foreach (Warp t in ModEntry.PatchData.DynamicWarps)
                Processors.ApplyWarp(t);
            foreach (Property t in ModEntry.PatchData.DynamicProperties)
                Processors.ApplyProperty(t);
        }

        internal static void UpdateTilesheets()
        {
            ModEntry.Logger.Log("Month changed, updating custom seasonal tilesheets...", LogLevel.Trace);
            List<string> locations = new List<string>();
            foreach (KeyValuePair<IContentPack, Tilesheet[]> pair in ModEntry.PatchData.SeasonalTilesheets)
            {
                foreach (Tilesheet tilesheet in pair.Value)
                {
                    GameLocation location = Game1.getLocationFromName(tilesheet.MapName);
                    Processors.ApplyTilesheet(ModEntry.SHelper.Content, pair.Key, tilesheet, location.map);
                    if (!locations.Contains(tilesheet.MapName))
                        locations.Add(tilesheet.MapName);
                }
            }

            foreach (string map in locations)
            {
                Map location = Game1.getLocationFromName(map).map;
                location.DisposeTileSheets(Game1.mapDisplayDevice);
                location.LoadTileSheets(Game1.mapDisplayDevice);
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Load the data from each available content pack.</summary>
        private IEnumerable<ContentPackData> LoadContentPackData()
        {
            ConfigReader configReader = new ConfigReader(this.Monitor);
            foreach (IContentPack contentPack in this.GetAllContentPacks())
            {
                this.Monitor.Log($"Loading content pack '{contentPack.Manifest.Name}'...", LogLevel.Debug);
                ContentPackData data = configReader.Load(contentPack);
                if (data != null)
                    yield return data;
            }
        }

        /// <summary>Raised after the game state is updated (≈60 times per second), regardless of normal SMAPI validation. This event is not thread-safe and may be invoked while game logic is running asynchronously. Changes to game state in this method may crash the game or corrupt an in-progress save. Do not use this event unless you're fully aware of the context in which your code will be run. Mods using this event will trigger a stability warning in the SMAPI console.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUnvalidatedUpdateTick(object sender, UnvalidatedUpdateTickedEventArgs e)
        {
            // wait until game loaded
            if (!this.IsSaveLoaded)
                return;
            this.Helper.Events.Specialised.UnvalidatedUpdateTicked -= this.OnUnvalidatedUpdateTick;

            // apply patcher
            this.Patcher.ApplyPatches(out Compound compoundData);
            ModEntry.PatchData = compoundData;

            // start handling dynamic stuff
            if (compoundData.DynamicTiles.Any() || compoundData.DynamicProperties.Any() || compoundData.DynamicWarps.Any() || compoundData.SeasonalTilesheets.Any())
            {
                this.Monitor.Log("Dynamic content detected, preparing dynamic update logic...", LogLevel.Info);
                this.Helper.Events.GameLoop.DayStarted += ModEntry.OnDayStarted;
            }
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            ModEntry.UpdateConditionalEdits();
            if (Game1.dayOfMonth == 1)
                ModEntry.UpdateTilesheets();
        }

        /// <summary>Raised after a map action is triggered.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnActionTriggered(object sender, EventArgsActionTriggered e)
        {
            try
            {
                switch (e.Action)
                {
                    case "ALLMessage":
                        Actions.Message(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLRawMessage":
                        Actions.RawMessage(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLShift":
                        Actions.Shift(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLReact":
                        Actions.React(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLRandomMessage":
                        Actions.RandomMessage(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLMinecart":
                    case "ALLTeleporter":
                        Actions.Teleporter(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLCondition":
                    case "ALLConditional":
                        Actions.Conditional(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLShop":
                        Actions.Shop(e.Who, e.Arguments, e.Position);
                        break;
                    default:
                        return;
                }

                ModEntry.Logger.Log($"ActionTriggered({e.Action})", LogLevel.Trace);
            }
            catch (Exception err)
            {
                ModEntry.Logger.ExitGameImmediately("Could not fire appropriate action response, a unexpected error happened", err);
            }
        }

        /// <summary>Raised after a player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer)
            {
                this.Helper.Events.Player.Warped -= this.OnWarped;
                ModEntry.UpdateConditionalEdits();
            }
        }

        private IEnumerable<IContentPack> GetAllContentPacks()
        {
            // read SMAPI content packs
            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
                yield return contentPack;

            // read legacy content packs
            string baseDir = Path.Combine(this.Helper.DirectoryPath, "locations");
            Directory.CreateDirectory(baseDir);
            foreach (string dir in Directory.EnumerateDirectories(baseDir))
            {
                IContentPack contentPack = null;
                try
                {
                    // skip SMAPI content pack (shouldn't be installed here)
                    if (File.Exists(Path.Combine(dir, "locations.json")))
                    {
                        ModEntry.Logger.Log($"The folder at path '{dir}' looks like a SMAPI content pack. Those should be installed directly in your Mods folder. This content pack won't be loaded.", LogLevel.Warn);
                        continue;
                    }

                    // read manifest
                    string file = Path.Combine(dir, "manifest.json");
                    if (!File.Exists(file))
                    {
                        ModEntry.Logger.Log($"Can't find a manifest.json in the '{dir}' folder. This content pack won't be loaded.", LogLevel.Warn);
                        continue;
                    }

                    JObject config;
                    try
                    {
                        config = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(file));
                    }
                    catch (Exception ex)
                    {
                        ModEntry.Logger.Log($"Can't read manifest.json in the '{dir}' folder. This content pack won't be loaded.", LogLevel.Error, ex);
                        continue;
                    }

                    // get 'about' field
                    JObject about = config.GetValue("About", StringComparison.InvariantCultureIgnoreCase) as JObject;
                    if (about == null)
                    {
                        ModEntry.Logger.Log($"Can't read content pack 'about' info from the manifest.json in the '{dir}' folder. This content pack won't be loaded.", LogLevel.Error);
                        continue;
                    }

                    // prepare basic data
                    string id = about.GetValue("ModID", StringComparison.InvariantCultureIgnoreCase)?.Value<string>() ?? Guid.NewGuid().ToString("N");
                    string name = about.GetValue("ModName", StringComparison.InvariantCultureIgnoreCase)?.Value<string>() ?? Path.GetDirectoryName(dir);
                    string author = about.GetValue("Author", StringComparison.InvariantCultureIgnoreCase)?.Value<string>();
                    string description = about.GetValue("Description", StringComparison.InvariantCultureIgnoreCase)?.Value<string>();
                    string version = about.GetValue("Version", StringComparison.InvariantCultureIgnoreCase)?.Value<string>() ?? "1.0.0";

                    // create content pack
                    contentPack = this.Helper.ContentPacks.CreateTemporary(dir, id, name, description, author, new SemanticVersion(version));
                }
                catch (Exception ex)
                {
                    ModEntry.Logger.Log($"Could not parse location mod at path '{dir}'. This content pack won't be loaded.", LogLevel.Error, ex);
                }

                if (contentPack != null)
                    yield return contentPack;
            }
        }
    }
}
