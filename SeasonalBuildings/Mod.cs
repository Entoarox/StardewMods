using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Buildings;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Entoarox.SeasonalBuildings
{
    internal class ContentPackManifest
    {
        public string Name;
        public string Author;
        public Version Version;
        public string Description;
    }
    internal class SeasonalBuildingsMod : StardewModdingAPI.Mod
    {
        public override void Entry(params object[] objects)
        {
            StardewModdingAPI.Events.GameEvents.LoadContent += GameEvents_LoadContent;
            FilePath = PathOnDisk;
            string mode = "xna";
            if(Type.GetType("Microsoft.Xna.Framework.Input.Joystick")!=null)
                mode = "mono";
            StardewModdingAPI.Log.SyncColour("[SeasonalBuildings] Version "+GetType().Assembly.GetName().Version+'/'+mode+" by Entoarox, do not redistribute", ConsoleColor.Cyan);
        }
        private static string FilePath;
        private static Dictionary<string, Dictionary<string, Texture2D>> SeasonTextures = new Dictionary<string, Dictionary<string, Texture2D>>();
        private static string[] seasons = new string[] { "spring", "summer", "fall", "winter" };
        internal static void GameEvents_LoadContent(object s, EventArgs e)
        {
            string path = Path.Combine(FilePath, "ContentPack.zip");
            ZipArchive zip;
            if (File.Exists(path))
                zip = new ZipArchive(new FileStream(path, FileMode.Open), ZipArchiveMode.Update);
            else
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Entoarox.SeasonalBuildings.ContentPack.zip");
                if (stream == null)
                {
                    StardewModdingAPI.Log.SyncColour("[SeasonalBuildings] Could not resolve the ContentPack.zip file, did you forget to download one?", ConsoleColor.Red);
                    return;
                }
                StardewModdingAPI.Log.SyncColour("[SeasonalBuildings] No ContentPack.zip file found, using the embedded content pack", ConsoleColor.Yellow);
                zip = new ZipArchive(stream,ZipArchiveMode.Update);
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
                    textures.Add(season, Texture2D.FromStream(Game1.graphics.GraphicsDevice, zipFile.Open()));
                }
                SeasonTextures.Add(name, textures);
            skipFile:
                continue;
            }
            StardewModdingAPI.Events.GameEvents.UpdateTick += GameEvents_UpdateTick;
        }
        internal static void GameEvents_UpdateTick(object s, EventArgs e)
        {
            if (!Game1.hasLoadedGame || Game1.getFarm() == null)
                return;
            TimeEvents_SeasonOfYearChanged(s, e);
            StardewModdingAPI.Events.TimeEvents.SeasonOfYearChanged += TimeEvents_SeasonOfYearChanged;
            StardewModdingAPI.Events.LocationEvents.CurrentLocationChanged += LocationEvents_CurrentLocationChanged;
            StardewModdingAPI.Events.GameEvents.UpdateTick -= GameEvents_UpdateTick;
        }
        internal static void TimeEvents_SeasonOfYearChanged(object s, EventArgs e)
        {
            if (SeasonTextures.ContainsKey("houses"))
                Game1.getFarm().houseTextures = SeasonTextures["houses"][Game1.currentSeason];
            foreach (Building building in Game1.getFarm().buildings)
                if (SeasonTextures.ContainsKey(building.buildingType))
                    building.texture = SeasonTextures[building.buildingType][Game1.currentSeason];
        }
        internal static void LocationEvents_CurrentLocationChanged(object s, EventArgs e)
        {
            if (Game1.currentLocation.name != "Farm")
                return;
            StardewModdingAPI.Events.LocationEvents.CurrentLocationChanged -= LocationEvents_CurrentLocationChanged;
            TimeEvents_SeasonOfYearChanged(s, e);
        }
    }
}
