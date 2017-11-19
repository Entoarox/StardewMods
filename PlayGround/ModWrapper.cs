using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace PlayGround
{
    class ModWrapper : Mod
    {
        public override void Entry(IModHelper helper)
        {
            this.Helper.ConsoleCommands.Add("epg_findley", "Debug command, should not be used by players", (a, b) =>
             {
                 GameLocation loc = Game1.getLocationFromName("WizardHouse");
                 if (!loc.map.Properties.ContainsKey(DistanceCalculator.LeylineProperty))
                     loc.map.Properties.Add(DistanceCalculator.LeylineProperty, 0);
                 this.Monitor.Log("Path distance for current location: " + DistanceCalculator.GetPathDistance(Game1.currentLocation));
             });
        }
    }
}
