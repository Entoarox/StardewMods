using System;
using System.Collections.Generic;
using System.Linq;
using Entoarox.AdvancedLocationLoader.Configs;
using StardewValley;

namespace Entoarox.AdvancedLocationLoader
{
    internal class TeleportationResolver
    {
        /*********
        ** Fields
        *********/
        private static readonly Dictionary<string, TeleportationResolver> Cache = new Dictionary<string, TeleportationResolver>();
        private readonly Dictionary<string, Response> Destinations;
        private readonly TeleporterList List;


        /*********
        ** Public methods
        *********/
        public static TeleportationResolver Request(string list)
        {
            if (!TeleportationResolver.Cache.ContainsKey(list))
                TeleportationResolver.Cache.Add(list, new TeleportationResolver(list));
            return TeleportationResolver.Cache[list];
        }

        public void Init()
        {
            List<Response> destinations = new List<Response>();
            foreach (KeyValuePair<string, Response> entry in this.Destinations)
                if (entry.Key != Game1.currentLocation.Name)
                    destinations.Add(entry.Value);
            Game1.currentLocation.lastQuestionKey = "SelectTeleportDestination";
            Game1.currentLocation.createQuestionDialogue(ModEntry.Strings.Get("teleporter"), destinations.ToArray(), this.Resolver, null);
        }

        public void Resolver(Farmer who, string answer)
        {
            if (answer == "cancel")
                return;
            int i = Convert.ToInt32(answer);
            TeleporterDestination destination = this.List.Destinations[i];
            Game1.warpFarmer(destination.MapName, destination.TileX, destination.TileY, false);
        }


        /*********
        ** Private methods
        *********/
        private TeleportationResolver(string list)
        {
            this.List = ModEntry.PatchData.Teleporters.FirstOrDefault(e => e.ListName == list);
            this.Destinations = new Dictionary<string, Response> { { "", new Response("cancel", ModEntry.Strings.Get("cancel")) } };
            for (int c = 0; c < this.List.Destinations.Count; c++)
            {
                if (this.List.Destinations[c].MapName != Game1.currentLocation.Name)
                    this.Destinations.Add(this.List.Destinations[c].MapName, new Response(c.ToString(), this.List.Destinations[c].ItemText));
            }
        }
    }
}
