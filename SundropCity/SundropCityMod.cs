using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using StardewModdingAPI;

using StardewValley;

namespace SundropCity
{
    public class SundropCityMod : Mod
    {
        internal static IModHelper SHelper;
        internal static IMonitor SMonitor;

        private List<string> Maps;

        public override void Entry(IModHelper helper)
        {
            // Define internals
            SHelper = helper;
            SMonitor = this.Monitor;

            // Add custom loader to make custom tree work
            helper.Content.AssetLoaders.Add(new SundropLoader());

            // Load maps
            List<string> maps = new List<string>();
            foreach(string file in Directory.EnumerateFiles(Path.Combine(helper.DirectoryPath,"assets","Maps")))
            {
                string ext = Path.GetExtension(file);
                this.Monitor.Log($"Checking file: {file} (ext: {ext})", LogLevel.Trace);
                if (!ext.Equals(".tbin"))
                    continue;
                string map = Path.GetFileName(file);
                this.Monitor.Log("Found sundrop location: " + map, LogLevel.Trace);
                maps.Add(map);
                helper.Content.Load<xTile.Map>(Path.Combine("assets", "Maps", map));
            }
            this.Maps = maps;

            // Setup events
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saved += this.OnSaved;
            helper.Events.GameLoop.Saving += this.OnSaving;
        }

        private void Setup()
        {
            // Add new locations
            foreach (string map in this.Maps)
            {
                this.Monitor.Log("Adding sundrop location: "+ Path.GetFileNameWithoutExtension(map), LogLevel.Trace);
                Game1.locations.Add(new SundropLocation(this.Helper.Content.GetActualAssetKey(Path.Combine("assets", "Maps", map)), Path.GetFileNameWithoutExtension(map)));
            }
        }

        private void OnSaveLoaded(object s, EventArgs e)
        {
            this.Setup();
        }
        private void OnSaved(object s, EventArgs e)
        {
            this.Setup();
        }
        private void OnSaving(object s, EventArgs e)
        {
            Game1.locations = Game1.locations.Where(a => !(a is SundropLocation)).ToList();
        }
    }
}
