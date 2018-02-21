using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace Entoarox.AdvancedLocationLoader
{
    internal class TeleportationResolver
    {
        private static Dictionary<string, TeleportationResolver> Cache = new Dictionary<string, TeleportationResolver>();
        internal static TeleportationResolver Request(string list)
        {
            if (!TeleportationResolver.Cache.ContainsKey(list))
                TeleportationResolver.Cache.Add(list, new TeleportationResolver(list));
            return TeleportationResolver.Cache[list];
        }
        private Configs.TeleporterList List;
        private Dictionary<string, Response> Destinations;
        private TeleportationResolver(string list)
        {
            this.List = ModEntry.PatchData.Teleporters.FirstOrDefault(e => e.ListName == list);
            this.Destinations = new Dictionary<string, Response>() { { "", new Response("cancel", ModEntry.Strings.Get("cancel")) } };
            for (int c = 0; c < this.List.Destinations.Count; c++)
            {
                if (this.List.Destinations[c].MapName != Game1.currentLocation.name)
                    this.Destinations.Add(this.List.Destinations[c].MapName, new Response(c.ToString(), this.List.Destinations[c].ItemText));
            }
        }
        internal void Init()
        {
            List<Response> destinations = new List<Response>();
            foreach (KeyValuePair<string, Response> entry in this.Destinations)
                if (entry.Key != Game1.currentLocation.name)
                    destinations.Add(entry.Value);
            Game1.currentLocation.lastQuestionKey = "SelectTeleportDestination";
            Game1.currentLocation.createQuestionDialogue(ModEntry.Strings.Get("teleporter"), destinations.ToArray(), this.Resolver, null);
        }
        internal void Resolver(StardewValley.Farmer who, string answer)
        {
            if (answer == "cancel")
                return;
            int i = Convert.ToInt32(answer);
            Configs.TeleporterDestination destination = this.List.Destinations[i];
            Game1.warpFarmer(destination.MapName, destination.TileX, destination.TileY, false);
        }
    }
}
