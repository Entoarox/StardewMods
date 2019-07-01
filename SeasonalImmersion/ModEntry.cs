using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace Entoarox.SeasonalImmersion
{
    /// <summary>The mod entry class.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        private static readonly BlendState BlendAlpha = new BlendState
        {
            ColorWriteChannels = ColorWriteChannels.Alpha,
            AlphaDestinationBlend = Blend.Zero,
            ColorDestinationBlend = Blend.Zero,
            AlphaSourceBlend = Blend.One,
            ColorSourceBlend = Blend.One
        };

        private static readonly BlendState BlendColor = new BlendState
        {
            ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue,
            AlphaDestinationBlend = Blend.Zero,
            ColorDestinationBlend = Blend.Zero,
            AlphaSourceBlend = Blend.SourceAlpha,
            ColorSourceBlend = Blend.SourceAlpha
        };

        private static readonly string[] Seasons = { "spring", "summer", "fall", "winter" };

        private static string FilePath;
        private ModConfig Config;
        private ContentMode Mode = 0;
        private ZipFile Zip;


        /*********
        ** Accessors
        *********/
        internal static Dictionary<string, Dictionary<string, Texture2D>> SeasonTextures;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();
            ModEntry.FilePath = helper.DirectoryPath;

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }


        /*********
        ** Protected methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            try
            {
                this.VerboseLog("Loading Seasonal Immersion ContentPack...");
                this.LoadContent();
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Could not load ContentPack\n{ex.Message}\n{ex.StackTrace}", LogLevel.Error);
            }

            this.Helper.Content.AssetLoaders.Add(new SeasonalTextureLoader());
        }

        private Texture2D PreMultiply(Texture2D texture)
        {
            try
            {
                RenderTarget2D result = new RenderTarget2D(Game1.graphics.GraphicsDevice, texture.Width, texture.Height);
                Game1.graphics.GraphicsDevice.SetRenderTarget(result);
                Game1.graphics.GraphicsDevice.Clear(Color.Black);
                Game1.spriteBatch.Begin(SpriteSortMode.Immediate, ModEntry.BlendColor);
                Game1.spriteBatch.Draw(texture, texture.Bounds, Color.White);
                Game1.spriteBatch.End();
                Game1.spriteBatch.Begin(SpriteSortMode.Immediate, ModEntry.BlendAlpha);
                Game1.spriteBatch.Draw(texture, texture.Bounds, Color.White);
                Game1.spriteBatch.End();
                Game1.graphics.GraphicsDevice.SetRenderTarget(null);
                MemoryStream stream = new MemoryStream();
                result.SaveAsPng(stream, texture.Width, texture.Height);
                return Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
            }
            catch (Exception err)
            {
                this.Monitor.Log($"Failed to PreMultiply texture, alpha will not work properly\n{err.Message}\n{err.StackTrace}", LogLevel.Warn);
                return texture;
            }
        }

        private void LoadContent()
        {
            if (ModEntry.SeasonTextures != null)
                this.Monitor.Log("SeasonalImmersionMod::Entry has already been called previously, this shouldnt be happening!", LogLevel.Warn);
            this.VerboseLog("Attempting to resolve content pack...");
            if (Directory.Exists(Path.Combine(ModEntry.FilePath, "ContentPack")))
                this.Mode = ContentMode.Directory;
            else if (File.Exists(Path.Combine(ModEntry.FilePath, "ContentPack.zip")))
            {
                try
                {
                    this.Zip = new ZipFile(Path.Combine(ModEntry.FilePath, "ContentPack.zip"));
                    this.Mode = ContentMode.Zipped;
                }
                catch (Exception ex)
                {
                    this.Monitor.Log($"Was unable to reference ContentPack.zip file, using internal content pack as a fallback.\n{ex.Message}\n{ex.StackTrace}", LogLevel.Error);
                    this.Mode = ContentMode.Internal;
                }
            }
            else
                this.Mode = ContentMode.Internal;

            Stream stream = this.GetStream("manifest.json");
            if (stream == null)
            {
                switch (this.Mode)
                {
                    case ContentMode.Directory:
                        this.Monitor.Log("Found `ContentPack` directory but the `manifest.json` file is missing, falling back to internal.", LogLevel.Error);
                        this.Mode = ContentMode.Internal;
                        stream = this.GetStream("manifest.json");
                        break;
                    case ContentMode.Zipped:
                        this.Monitor.Log("Found `ContentPack.zip` file but the `manifest.json` file is missing, falling back to internal.", LogLevel.Error);
                        this.Mode = ContentMode.Internal;
                        stream = this.GetStream("manifest.json");
                        break;
                }
            }
            if (stream == null && this.Mode == ContentMode.Internal)
            {
                this.Monitor.Log("Attempted to use internal ContentPack but could not resolve manifest, disabling mod.", LogLevel.Error);
                return;
            }

            this.VerboseLog($"Content pack resolved to mode: {this.Mode}");
            ModEntry.SeasonTextures = new Dictionary<string, Dictionary<string, Texture2D>>();
            ContentPackManifest manifest = JsonConvert.DeserializeObject<ContentPackManifest>(new StreamReader(stream).ReadToEnd(), new VersionConverter());
            this.Monitor.Log($"Using the `{manifest.Name}` content pack, version {manifest.Version} by {manifest.Author}", LogLevel.Info);
            // Resolve content dir cause CA messes all stuffs up...
            List<string> Files;
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources", Game1.content.RootDirectory, "XACT", "FarmerSounds.xgs")))
                Files = new List<string>(Directory.EnumerateFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources", Game1.content.RootDirectory, "Buildings")).Where(a => Path.GetExtension(a).Equals(".xnb")));
            else
                Files = new List<string>(Directory.EnumerateFiles(Path.Combine(Game1.content.RootDirectory, "Buildings")).Where(a => Path.GetExtension(a).Equals(".xnb")));
            Files.AddRange(new[]
            {
                "Flooring.xnb",
                "Craftables.xnb",
                "Craftables_outdoor.xnb",
                "Craftables_indoor.xnb"
            });
            foreach (string file in Files)
            {
                Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
                string name = Path.GetFileNameWithoutExtension(file);
                this.VerboseLog($"Checking if file is seasonal: {name}");
                int count = 0;
                foreach (string season in ModEntry.Seasons)
                {
                    Texture2D tex = this.GetTexture(Path.Combine(season, name + ".png"));
                    if (tex == null)
                        continue;
                    count++;
                    textures.Add(season, tex);
                }

                if (count != 4)
                {
                    if (count > 0)
                        this.Monitor.Log($"Skipping file due to the textures being incomplete: {file}", LogLevel.Warn);
                    else
                        this.VerboseLog($"Skipping file due to there being no textures for it found: {file}");
                    continue;
                }

                this.VerboseLog($"Making file seasonal: {file}");
                ModEntry.SeasonTextures.Add(name, textures);
            }

            this.Helper.Events.Player.Warped += this.OnWarped;
            this.Helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            this.Monitor.Log($"ContentPack processed, found [{ModEntry.SeasonTextures.Count}] seasonal files", LogLevel.Info);
        }

        private Stream GetStream(string file)
        {
            try
            {
                switch (this.Mode)
                {
                    case ContentMode.Directory:
                        if (!File.Exists(Path.Combine(ModEntry.FilePath, "ContentPack", file)))
                        {
                            this.VerboseLog($"Skipping file due to being unable to find it in the ContentPack directory: {file}");
                            return null;
                        }

                        return new FileStream(Path.Combine(ModEntry.FilePath, "ContentPack", file), FileMode.Open);
                    case ContentMode.Zipped:
                        if (!this.Zip.ContainsEntry(file))
                        {
                            this.VerboseLog($"Skipping file due to being unable to find it in the ContentPack.zip: {file}");
                            return null;
                        }

                        MemoryStream memoryStream = new MemoryStream();
                        this.Zip[file].Extract(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        return memoryStream;
                    case ContentMode.Internal:
                        Stream manifestStream = this.GetType().Assembly.GetManifestResourceStream("Entoarox.SeasonalImmersion.ContentPack." + file.Replace(Path.DirectorySeparatorChar, '.'));
                        if (manifestStream == null)
                        {
                            this.VerboseLog($"Skipping file due to being unable to find it in the bundled resource pack: {file}");
                            return null;
                        }

                        return manifestStream;
                    default:
                        this.Monitor.Log($"Skipping file due to the content pack location being unknown: {file}", LogLevel.Error);
                        return null;
                }
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Skipping file due to a unknown error: {file}\n{ex}", LogLevel.Error);
                return null;
            }
        }

        private Texture2D GetTexture(string file)
        {
            Stream stream = this.GetStream(file);
            if (stream == null)
                return null;
            try
            {
                return this.PreMultiply(Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream));
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Skipping texture due to a unknown error: {file}\n{ex}", LogLevel.Warn);
                return null;
            }
        }

        private void UpdateTextures()
        {
            if (ModEntry.SeasonTextures.Count == 0)
                return;
            try
            {
                this.VerboseLog("Attempting to update seasonal textures...");
                // Check houses/greenhouse
                if (ModEntry.SeasonTextures.ContainsKey("houses"))
                    this.Helper.Reflection.GetField<Texture2D>(typeof(Farm), nameof(Farm.houseTextures)).SetValue(ModEntry.SeasonTextures["houses"][Game1.currentSeason]);
                // Check flooring
                if (ModEntry.SeasonTextures.ContainsKey("Flooring"))
                    Flooring.floorsTexture = ModEntry.SeasonTextures["Flooring"][Game1.currentSeason];
                // Check furniture
                if (ModEntry.SeasonTextures.ContainsKey("furniture"))
                    Furniture.furnitureTexture = ModEntry.SeasonTextures["furniture"][Game1.currentSeason];
                // Check outdoor craftables
                if (Game1.currentLocation.IsOutdoors && ModEntry.SeasonTextures.ContainsKey("Craftables_outdoor"))
                {
                    this.Helper.Content.InvalidateCache("TileSheets\\Craftables_outdoor");
                    Game1.bigCraftableSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\Craftables_outdoor");
                }
                // Check indoor craftables
                else if (!Game1.currentLocation.IsOutdoors && ModEntry.SeasonTextures.ContainsKey("Craftables_indoor"))
                {
                    this.Helper.Content.InvalidateCache("TileSheets\\Craftables_indoor");
                    Game1.bigCraftableSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\Craftables_indoor");
                }
                // Reset big craftables
                else
                {
                    this.Helper.Content.InvalidateCache("TileSheets\\Craftables");
                    Game1.bigCraftableSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\Craftables");
                }

                // Loop buildings
                foreach (Building building in Game1.getFarm().buildings)
                    if (ModEntry.SeasonTextures.ContainsKey(building.buildingType.Value))
                        building.texture = new Lazy<Texture2D>(() => ModEntry.SeasonTextures[building.buildingType.Value][Game1.currentSeason]);
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Failed to update seasonal textures\n{ex}", LogLevel.Error);
            }
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.dayOfMonth == 1)
                this.UpdateTextures();
        }

        /// <summary>Raised after a player warps to a new location. NOTE: this event is currently only raised for the current player.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation.Name == "Farm" || (e.OldLocation != null && e.OldLocation.IsOutdoors != e.NewLocation.IsOutdoors))
                this.UpdateTextures();
        }

        private void VerboseLog(string message)
        {
            if (this.Config.VerboseLog)
                this.Monitor.Log(message, LogLevel.Trace);
        }
    }
}
