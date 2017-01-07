using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Buildings;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Entoarox.SeasonalImmersion
{
    public class SeasonalImmersion : Mod
    {
        public override void Entry(IModHelper helper)
        {
            FilePath = helper.DirectoryPath;
            GameEvents.LoadContent += GameEvents_LoadContent;
        }
        private static bool ContentReady = false;
        private static string FilePath;
        private static Dictionary<string, Dictionary<string, Texture2D>> SeasonTextures = new Dictionary<string, Dictionary<string, Texture2D>>();
        private static string[] seasons = new string[] { "spring", "summer", "fall", "winter" };
        private static Texture2D DefaultBigCraftables;
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
                Monitor.Log("Failed to PreMultiply texture, alpha will not work properly" + Environment.NewLine + err.Message + Environment.NewLine + err.StackTrace, LogLevel.Warn);
                return texture;
            }
        }
        internal void LoadContent()
        {
            Monitor.Log("Attempting to resolve content pack...", LogLevel.Trace);
            string path = Path.Combine(FilePath, "ContentPack.zip");
            ZipArchive zip;
            if (File.Exists(path))
                zip = new ZipArchive(new FileStream(path, FileMode.Open), ZipArchiveMode.Read);
            else
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Entoarox.SeasonalImmersion.ContentPack.zip");
                if (stream == null)
                {
                    Monitor.Log("No ContentPack.zip file found, and did not find a embedded pack either", LogLevel.Error);
                    return;
                }
                Monitor.Log("No ContentPack.zip file found, using the embedded content pack", LogLevel.Warn);
                zip = new ZipArchive(stream,ZipArchiveMode.Read);
            }
            ZipArchiveEntry manifestData = zip.GetEntry("manifest.json");
            ContentPackManifest manifest = JsonConvert.DeserializeObject<ContentPackManifest>(new StreamReader(manifestData.Open()).ReadToEnd(), new VersionConverter());
            Monitor.Log("Using the `" + manifest.Name + "` content pack, version " + manifest.Version + " by " + manifest.Author, LogLevel.Info);
            // Resolve content dir cause CA messes all stuffs up...
            List<string> Files;
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources", Game1.content.RootDirectory, "XACT", "FarmerSounds.xgs")))
                Files = new List<string>(Directory.EnumerateFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources", Game1.content.RootDirectory, "Buildings")));
            else
                Files = new List<string>(Directory.EnumerateFiles(Path.Combine(Game1.content.RootDirectory, "Buildings")));
            Files.Add("Flooring.xnb");
            Files.Add("Craftables_outdoor.xnb");
            Files.Add("Craftables_indoor.xnb");
            foreach (string file in Files)
            {
                Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
                string name = Path.GetFileNameWithoutExtension(file);
                foreach (string season in seasons)
                {
                    ZipArchiveEntry zipFile = zip.GetEntry(season + '/' + name + ".png");
                    if (zipFile == null)
                        goto skipFile;
                    MemoryStream mst = new MemoryStream();
                    zipFile.Open().CopyTo(mst);
                    mst.Position = 0;
                    textures.Add(season, PreMultiply(Texture2D.FromStream(Game1.graphics.GraphicsDevice, mst)));
                }
                SeasonTextures.Add(name, textures);
            skipFile:
                continue;
            }
        }
        internal void GameEvents_LoadContent(object s, EventArgs e)
        {
            try
            {
                Monitor.Log("Loading Seasonal Immersion ContentPack...", LogLevel.Info);
                LoadContent();
                LocationEvents.CurrentLocationChanged += LocationEvents_CurrentLocationChanged;
                GameEvents.LoadContent -= GameEvents_LoadContent;
                TimeEvents.SeasonOfYearChanged += TimeEvents_SeasonOfYearChanged;
                DefaultBigCraftables = Game1.bigCraftableSpriteSheet;
                ContentReady = true;
            }
            catch (Exception err)
            {
                Monitor.Log("Could not load ContentPack" + Environment.NewLine + err.Message + Environment.NewLine + err.StackTrace, LogLevel.Error);
            }
        }
        internal void UpdateTextures()
        {
            try
            {
                Monitor.Log("Attempting to update seasonal textures...", LogLevel.Debug);
                // Check houses/greenhouse
                if (SeasonTextures.ContainsKey("houses"))
                    Game1.getFarm().houseTextures = SeasonTextures["houses"][Game1.currentSeason];
                // Check flooring
                if (SeasonTextures.ContainsKey("Flooring"))
                    StardewValley.TerrainFeatures.Flooring.floorsTexture = SeasonTextures["Flooring"][Game1.currentSeason];
                // Reset big craftables
                Game1.bigCraftableSpriteSheet = DefaultBigCraftables;
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
            catch (Exception err)
            {
                Monitor.Log("Failed to update seasonal textures" + Environment.NewLine + err.Message + Environment.NewLine + err.StackTrace, LogLevel.Error);
            }
        }
        internal void TimeEvents_SeasonOfYearChanged(object s, EventArgs e)
        {
            if (ContentReady && Game1.hasLoadedGame)
                UpdateTextures();
        }
        internal void LocationEvents_CurrentLocationChanged(object s, EventArgs e)
        {
            if (Game1.hasLoadedGame && Game1.currentLocation != null && ContentReady)
                UpdateTextures();
        }
    }
    internal class ContentPackManifest
    {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value null
        public string Name;
        public string Author;
        public System.Version Version;
        public string Description;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value null
    }
}
