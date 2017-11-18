using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Buildings;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using Ionic.Zip;

namespace Entoarox.SeasonalImmersion
{
    public class SeasonalImmersionMod : Mod
    {
        public override void Entry(IModHelper helper)
        {
            FilePath = helper.DirectoryPath;

            try
            {
                this.Monitor.Log("Loading Seasonal Immersion ContentPack...", LogLevel.Info);
                LoadContent();
            }
            catch (Exception ex)
            {
                this.Monitor.Log("Could not load ContentPack" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, LogLevel.Error);
            }
        }
        
        private static string FilePath;
        private static Dictionary<string, Dictionary<string, Texture2D>> SeasonTextures;
        private static string[] seasons = new string[] { "spring", "summer", "fall", "winter" };
        private static BlendState blendColor = new BlendState
        {
            ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue,
            AlphaDestinationBlend = Blend.Zero,
            ColorDestinationBlend = Blend.Zero,
            AlphaSourceBlend = Blend.SourceAlpha,
            ColorSourceBlend = Blend.SourceAlpha
        };
        private static BlendState blendAlpha = new BlendState
        {
            ColorWriteChannels = ColorWriteChannels.Alpha,
            AlphaDestinationBlend = Blend.Zero,
            ColorDestinationBlend = Blend.Zero,
            AlphaSourceBlend = Blend.One,
            ColorSourceBlend = Blend.One
        };
        private Texture2D PreMultiply(Texture2D texture)
        {
            try
            {
                RenderTarget2D result = new RenderTarget2D(Game1.graphics.GraphicsDevice, texture.Width, texture.Height);
                Game1.graphics.GraphicsDevice.SetRenderTarget(result);
                Game1.graphics.GraphicsDevice.Clear(Color.Black);
                Game1.spriteBatch.Begin(SpriteSortMode.Immediate, blendColor);
                Game1.spriteBatch.Draw(texture, texture.Bounds, Color.White);
                Game1.spriteBatch.End();
                Game1.spriteBatch.Begin(SpriteSortMode.Immediate, blendAlpha);
                Game1.spriteBatch.Draw(texture, texture.Bounds, Color.White);
                Game1.spriteBatch.End();
                Game1.graphics.GraphicsDevice.SetRenderTarget(null);
                MemoryStream stream = new MemoryStream();
                (result as Texture2D).SaveAsPng(stream,texture.Width,texture.Height);
                return Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
            }
            catch(Exception err)
            {
                this.Monitor.Log("Failed to PreMultiply texture, alpha will not work properly" + Environment.NewLine + err.Message + Environment.NewLine + err.StackTrace, LogLevel.Warn);
                return texture;
            }
        }
        internal ContentMode Mode = 0;
        internal ZipFile Zip;
        internal void LoadContent()
        {
            if(SeasonTextures!=null)
                this.Monitor.Log("SeasonalImmersionMod::Entry has already been called previously, this shouldnt be happening!", LogLevel.Warn);
            this.Monitor.Log("Attempting to resolve content pack...", LogLevel.Trace);
            if (Directory.Exists(Path.Combine(FilePath, "ContentPack")))
                this.Mode = ContentMode.Directory;
            else if (File.Exists(Path.Combine(FilePath, "ContentPack.zip")))
            {
                try
                {
                    this.Zip = new ZipFile(Path.Combine(FilePath, "ContentPack.zip"));
                    this.Mode = ContentMode.Zipped;
                }
                catch (Exception ex)
                {
                    this.Monitor.Log("Was unable to reference ContentPack.zip file, using internal content pack as a fallback." + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, LogLevel.Error);
                    this.Mode = ContentMode.Internal;
                }
            }
            else
                this.Mode = ContentMode.Internal;
            Stream stream = GetStream("manifest.json");
            if (stream == null)
            {
                switch (this.Mode)
                {
                    case ContentMode.Directory:
                        this.Monitor.Log("Found `ContentPack` directory but the `manifest.json` file is missing, falling back to internal.", LogLevel.Error);
                        this.Mode = ContentMode.Internal;
                        stream = GetStream("manifest.json");
                        break;
                    case ContentMode.Zipped:
                        this.Monitor.Log("Found `ContentPack.zip` file but the `manifest.json` file is missing, falling back to internal.", LogLevel.Error);
                        this.Mode = ContentMode.Internal;
                        stream = GetStream("manifest.json");
                        break;
                }
            }
            if (stream == null && this.Mode ==ContentMode.Internal)
            {
                this.Monitor.Log("Attempted to use internal ContentPack but could not resolve manifest, disabling mod.", LogLevel.Error);
                return;
            }
            this.Monitor.Log("Content pack resolved to mode: " + Enum.GetName(typeof(ContentMode), this.Mode), LogLevel.Trace);
            SeasonTextures = new Dictionary<string, Dictionary<string, Texture2D>>();
            ContentPackManifest manifest = JsonConvert.DeserializeObject<ContentPackManifest>(new StreamReader(stream).ReadToEnd(), new VersionConverter());
            this.Monitor.Log("Using the `" + manifest.Name + "` content pack, version " + manifest.Version + " by " + manifest.Author, LogLevel.Info);
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
                this.Monitor.Log("Checking if file is seasonal: " + name, LogLevel.Trace);
                int count = 0;
                foreach (string season in seasons)
                {
                    Texture2D tex = GetTexture(Path.Combine(season, name + ".png"));
                    if (tex == null)
                        continue;
                    count++;
                    textures.Add(season, tex);
                }
                if (count != 4)
                {
                    if (count > 0)
                        this.Monitor.Log("Skipping file due to the textures being incomplete: " + file, LogLevel.Warn);
                    else
                        this.Monitor.Log("Skipping file due to there being no textures for it found: " + file, LogLevel.Trace);
                    continue;
                }
                this.Monitor.Log("Making file seasonal: " + file, LogLevel.Trace);
                SeasonTextures.Add(name, textures);
            }
            LocationEvents.CurrentLocationChanged += this.LocationEvents_CurrentLocationChanged;
            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
            this.Monitor.Log("ContentPack processed, found [" + SeasonTextures.Count + "] seasonal files", LogLevel.Info);
        }
        internal Stream GetStream(string file)
        {
            try
            {
                switch (this.Mode)
                {
                    case ContentMode.Directory:
                        if (!File.Exists(Path.Combine(FilePath, "ContentPack", file)))
                        {
                            this.Monitor.Log("Skipping file due to being unable to find it in the ContentPack directory: " + file, LogLevel.Trace);
                            return null;
                        }
                        return new FileStream(Path.Combine(FilePath, "ContentPack", file), FileMode.Open);
                    case ContentMode.Zipped:
                        if (!this.Zip.ContainsEntry(file))
                        {
                            this.Monitor.Log("Skipping file due to being unable to find it in the ContentPack.zip: " + file, LogLevel.Trace);
                            return null;
                        }
                        MemoryStream memoryStream = new MemoryStream();
                        this.Zip[file].Extract(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        return memoryStream;
                    case ContentMode.Internal:
                        Stream manifestStream=GetType().Assembly.GetManifestResourceStream("Entoarox.SeasonalImmersion.ContentPack." + file.Replace(Path.DirectorySeparatorChar, '.'));
                        if (manifestStream == null)
                        {
                            this.Monitor.Log("Skipping file due to being unable to find it in the bundled resource pack: " + file, LogLevel.Trace);
                            return null;
                        }
                        return manifestStream;
                    default:
                        this.Monitor.Log("Skipping file due to the content pack location being unknown: " + file, LogLevel.Error);
                        return null;
                }
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Skipping file due to a unknown error: {file}\n{ex}", LogLevel.Error);
                return null;
            }
        }
        internal Texture2D GetTexture(string file)
        {
            Stream stream = GetStream(file);
            if (stream == null)
                return null;
            try
            {
                return PreMultiply(Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream));
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Skipping texture due to a unknown error: {file}\n{ex}", LogLevel.Warn);
                return null;
            }
        }
        internal void UpdateTextures()
        {
            if (SeasonTextures.Count == 0)
                return;
            try
            {
                this.Monitor.Log("Attempting to update seasonal textures...", LogLevel.Debug);
                // Check houses/greenhouse
                if (SeasonTextures.ContainsKey("houses"))
                    Game1.getFarm().houseTextures = SeasonTextures["houses"][Game1.currentSeason];
                // Check flooring
                if (SeasonTextures.ContainsKey("Flooring"))
                    StardewValley.TerrainFeatures.Flooring.floorsTexture = SeasonTextures["Flooring"][Game1.currentSeason];
                // Check furniture
                if (SeasonTextures.ContainsKey("furniture"))
                    StardewValley.Objects.Furniture.furnitureTexture = SeasonTextures["furniture"][Game1.currentSeason];
                // Reset big craftables
                if (!SeasonTextures.ContainsKey("Craftables"))
                    Game1.bigCraftableSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\Craftables");
                else
                    Game1.bigCraftableSpriteSheet = SeasonTextures["Craftables"][Game1.currentSeason];
                // Check outdoor craftables
                if (Game1.currentLocation.IsOutdoors && SeasonTextures.ContainsKey("Craftables_outdoor"))
                    Game1.bigCraftableSpriteSheet = SeasonTextures["Craftables_outdoor"][Game1.currentSeason];
                // Check indoor craftables
                if (!Game1.currentLocation.IsOutdoors && SeasonTextures.ContainsKey("Craftables_indoor"))
                    Game1.bigCraftableSpriteSheet = SeasonTextures["Craftables_indoor"][Game1.currentSeason];
                // Loop buildings
                foreach (Building building in Game1.getFarm().buildings)
                    if (SeasonTextures.ContainsKey(building.buildingType))
                        building.texture = SeasonTextures[building.buildingType][Game1.currentSeason];
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Failed to update seasonal textures\n{ex}", LogLevel.Error);
            }
        }
        internal void TimeEvents_AfterDayStarted(object s, EventArgs e)
        {
            if (Game1.dayOfMonth == 1)
                UpdateTextures();
        }
        internal void LocationEvents_CurrentLocationChanged(object s, EventArgsCurrentLocationChanged e)
        {
            if (e.NewLocation.name=="Farm" || (e.PriorLocation!=null && e.PriorLocation.isOutdoors!=e.NewLocation.isOutdoors))
                UpdateTextures();
        }
    }
}
