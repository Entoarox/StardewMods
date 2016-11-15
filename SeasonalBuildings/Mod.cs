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

namespace Entoarox.SeasonalBuildings
{
    internal class ContentPackManifest
    {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value null
        public string Name;
        public string Author;
        public Version Version;
        public string Description;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value null
    }
    internal class SeasonalBuildingsMod : StardewModdingAPI.Mod
    {
        public override void Entry(params object[] objects)
        {
            StardewModdingAPI.Events.GameEvents.UpdateTick += GameEvents_UpdateTick;
            FilePath = PathOnDisk;
            string mode = "unknown";
            switch(Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    mode = "xna";
                    break;
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    mode = "mono";
                    break;
                default:
                    mode = "other";
                    break;
            }
            StardewModdingAPI.Log.SyncColour("[SeasonalBuildings] Version "+GetType().Assembly.GetName().Version+'/'+mode+" by Entoarox, do not redistribute", ConsoleColor.Cyan);
        }
        private static string FilePath;
        private static Dictionary<string, Dictionary<string, Texture2D>> SeasonTextures = new Dictionary<string, Dictionary<string, Texture2D>>();
        private static string[] seasons = new string[] { "spring", "summer", "fall", "winter" };
        private static SpriteBatch spriteBatch;
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
        private static Texture2D PreMultiply(Texture2D texture)
        {
            try
            {
                RenderTarget2D result = new RenderTarget2D(Game1.graphics.GraphicsDevice, texture.Width, texture.Height);
                Game1.graphics.GraphicsDevice.SetRenderTarget(result);
                Game1.graphics.GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin(SpriteSortMode.Immediate, blendColor);
                spriteBatch.Draw(texture, texture.Bounds, Color.White);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, blendAlpha);
                spriteBatch.Draw(texture, texture.Bounds, Color.White);
                spriteBatch.End();
                Game1.graphics.GraphicsDevice.SetRenderTarget(null);
                return result as Texture2D;
            }
            catch(Exception err)
            {
                StardewModdingAPI.Log.SyncColour("Failed to PreMultiply texture, alpha will not work properly" + Environment.NewLine + err.Message + Environment.NewLine + err.StackTrace, ConsoleColor.Red);
                return texture;
            }
        }
        internal static void LoadContent()
        {
            StardewModdingAPI.Log.SyncColour("[SeasonalBuildings] Attempting to load content pack...", ConsoleColor.Cyan);
            if (spriteBatch == null)
                spriteBatch = new SpriteBatch(Game1.graphics.GraphicsDevice);
            string path = Path.Combine(FilePath, "ContentPack.zip");
            ZipArchive zip;
            if (File.Exists(path))
                zip = new ZipArchive(new FileStream(path, FileMode.Open), ZipArchiveMode.Read);
            else
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Entoarox.SeasonalBuildings.ContentPack.zip");
                if (stream == null)
                {
                    StardewModdingAPI.Log.SyncColour("[SeasonalBuildings] Could not resolve the ContentPack.zip file, did you forget to download one?", ConsoleColor.Red);
                    return;
                }
                StardewModdingAPI.Log.SyncColour("[SeasonalBuildings] No ContentPack.zip file found, using the embedded content pack", ConsoleColor.Yellow);
                zip = new ZipArchive(stream,ZipArchiveMode.Read);
            }
            ZipArchiveEntry manifestData = zip.GetEntry("manifest.json");
            ContentPackManifest manifest = JsonConvert.DeserializeObject<ContentPackManifest>(new StreamReader(manifestData.Open()).ReadToEnd(), new VersionConverter());
            StardewModdingAPI.Log.SyncColour("[SeasonalBuildings] Using the `" + manifest.Name + "` content pack, version " + manifest.Version + " by " + manifest.Author, ConsoleColor.Cyan);
            foreach (string file in Directory.EnumerateFiles(Path.Combine(Game1.content.RootDirectory, "Buildings")))
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
        internal static void GameEvents_UpdateTick(object s, EventArgs e)
        {
            if (!Game1.hasLoadedGame || Game1.getFarm() == null)
                return;
            LoadContent();
            StardewModdingAPI.Events.LocationEvents.CurrentLocationChanged += LocationEvents_CurrentLocationChanged;
            StardewModdingAPI.Events.GameEvents.UpdateTick -= GameEvents_UpdateTick;
        }
        internal static void LocationEvents_CurrentLocationChanged(object s, EventArgs e)
        {
            if (Game1.currentLocation.name != "Farm")
                return;
            if (SeasonTextures.ContainsKey("houses"))
                Game1.getFarm().houseTextures = SeasonTextures["houses"][Game1.currentSeason];
            foreach (Building building in Game1.getFarm().buildings)
                if (SeasonTextures.ContainsKey(building.buildingType))
                    building.texture = SeasonTextures[building.buildingType][Game1.currentSeason];
        }
    }
}
