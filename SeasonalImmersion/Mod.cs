﻿using System;
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
    public class SeasonalImmersion : Mod
    {
        public override void Entry(IModHelper helper)
        {
            FilePath = helper.DirectoryPath;

            try
            {
                this.Monitor.Log("Loading Seasonal Immersion ContentPack...", LogLevel.Info);
                this.LoadContent();
                LocationEvents.CurrentLocationChanged += this.LocationEvents_CurrentLocationChanged;
                TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
                SeasonalImmersion.ContentReady = true;
                this.Monitor.Log($"ContentPack processed, found [{SeasonTextures.Count}] seasonal files", LogLevel.Info);
            }
            catch (Exception ex)
            {
                this.Monitor.Log("Could not load ContentPack" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, LogLevel.Error);
            }
        }

        private static bool ContentReady = false;
        private static string FilePath;
        private static Dictionary<string, Dictionary<string, Texture2D>> SeasonTextures = new Dictionary<string, Dictionary<string, Texture2D>>();
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
                Monitor.Log("Failed to PreMultiply texture, alpha will not work properly" + Environment.NewLine + err.Message + Environment.NewLine + err.StackTrace, LogLevel.Warn);
                return texture;
            }
        }
        internal int Mode = 0;
        internal ZipFile Zip;
        internal void LoadContent()
        {
            Monitor.Log("Attempting to resolve content pack...", LogLevel.Trace);
            if (Directory.Exists(Path.Combine(FilePath, "ContentPack")))
                Mode = 1;
            else if (File.Exists(Path.Combine(FilePath, "ContentPack.zip")))
            {
                try
                {
                    Zip = new ZipFile(Path.Combine(FilePath, "ContentPack.zip"));
                    Mode = 2;
                }
                catch(Exception ex)
                {
                    Monitor.Log("Was unable to reference ContentPack.zip file, using internal content pack as a fallback." + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, LogLevel.Error);
                    Mode = 3;
                }
            }
            else
                Mode = 3;
            Monitor.Log("Content pack resolved to mode: "+Mode.ToString(), LogLevel.Trace);
            Stream stream = GetStream("manifest.json");
            if (stream == null)
            {
                switch (Mode)
                {
                    case 1:
                        Monitor.Log("Found `ContentPack` directory but the `manifest.json` file is missing!", LogLevel.Error);
                        break;
                    case 2:
                        Monitor.Log("Found `ContentPack.zip` file but the `manifest.json` file is missing!", LogLevel.Error);
                        break;
                    case 3:
                        Monitor.Log("Attempted to use internal ContentPack, but could not resolve manifest!", LogLevel.Error);
                        break;
                }
                return;
            }
            ContentPackManifest manifest = JsonConvert.DeserializeObject<ContentPackManifest>(new StreamReader(stream).ReadToEnd(), new VersionConverter());
            Monitor.Log("Using the `" + manifest.Name + "` content pack, version " + manifest.Version + " by " + manifest.Author, LogLevel.Info);
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
                Monitor.Log("Checking if file is seasonal: " + name, LogLevel.Trace);
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
                        Monitor.Log("Skipping file due to the textures being incomplete: " + file, LogLevel.Warn);
                    else
                        Monitor.Log("Skipping file due to there being no textures for it found: " + file, LogLevel.Trace);
                    continue;
                }
                Monitor.Log("Making file seasonal: " + file, LogLevel.Trace);
                SeasonTextures.Add(name, textures);
            }
        }
        internal Stream GetStream(string file)
        {
            try
            {
                switch (Mode)
                {
                    case 1:
                        if (!File.Exists(Path.Combine(FilePath, "ContentPack", file)))
                        {
                            Monitor.Log("Skipping file due to being unable to find it in the ContentPack directory: " + file, LogLevel.Trace);
                            return null;
                        }
                        return new FileStream(Path.Combine(FilePath, "ContentPack", file), FileMode.Open);
                    case 2:
                        if (!Zip.ContainsEntry(file))
                        {
                            Monitor.Log("Skipping file due to being unable to find it in the ContentPack.zip: " + file, LogLevel.Trace);
                            return null;
                        }
                        MemoryStream memoryStream = new MemoryStream();
                        Zip[file].Extract(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        return memoryStream;
                    case 3:
                        Stream manifestStream=GetType().Assembly.GetManifestResourceStream("Entoarox.SeasonalImmersion.ContentPack." + file.Replace(Path.DirectorySeparatorChar, '.'));
                        if (manifestStream == null)
                        {
                            Monitor.Log("Skipping file due to being unable to find it in the bundled resource pack: " + file, LogLevel.Trace);
                            return null;
                        }
                        return manifestStream;
                    default:
                        Monitor.Log("Skipping file due to the content pack location being unknown: " + file, LogLevel.Error);
                        return null;
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Skipping file due to a unknown error: {file}\n{ex}", LogLevel.Error);
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
                Monitor.Log($"Skipping texture due to a unknown error: {file}\n{ex}", LogLevel.Warn);
                return null;
            }
        }
        internal void UpdateTextures()
        {
            if (SeasonTextures.Count == 0)
                return;
            try
            {
                Monitor.Log("Attempting to update seasonal textures...", LogLevel.Debug);
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
                Game1.bigCraftableSpriteSheet = SeasonalImmersion.SeasonTextures.ContainsKey("Craftables") ? SeasonalImmersion.SeasonTextures["Craftables"][Game1.currentSeason] : Game1.content.Load<Texture2D>("TileSheets\\Craftables");
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
                Monitor.Log($"Failed to update seasonal textures\n{ex}", LogLevel.Error);
            }
        }
        internal void TimeEvents_AfterDayStarted(object s, EventArgs e)
        {
            if (ContentReady && Game1.hasLoadedGame && Game1.dayOfMonth == 1) // new season
                UpdateTextures();
        }
        internal void LocationEvents_CurrentLocationChanged(object s, EventArgsCurrentLocationChanged e)
        {
            if (Game1.hasLoadedGame && Game1.currentLocation != null && ContentReady && e.NewLocation.name == "Farm")
                UpdateTextures();
        }
    }
}
