using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

using StardewValley;

namespace SundropCity
{
    public class SundropCityMod : Mod
    {
        internal static IModHelper SHelper;
        internal static IMonitor SMonitor;
        public override void Entry(IModHelper helper)
        {
            // Define internals
            SHelper = helper;
            SMonitor = this.Monitor;

            // Setup events
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }

        private void OnSaveLoaded(object s, EventArgs e)
        {
            // Add new locations
            this.Helper.Content.Load<xTile.Map>("assets/sundrop_promenade.tbin");
            Game1.locations.Add(new GameLocation(this.Helper.Content.GetActualAssetKey("assets/sundrop_promenade.tbin"), "SundropPromenade"));
        }
    }
}
