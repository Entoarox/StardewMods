using System;
using System.Linq;
using System.Collections.Generic;

using StardewValley;
using StardewValley.Locations;

namespace PlayGround
{
    // Algorithm for calculating "distance"
    public static class DistanceCalculator
    {
        // We remember the calculations for all static maps, since they *shouldnt* change during gameplay
        private static Dictionary<string, double> _Cache = new Dictionary<string, double>();
        // The extra "distance" added for every floor down into the skull cave (Always 121 or more floors)
        private const double SkullDepthPenalty = 0.2;
        // The extra "distance" added for every floor down into mines (Never more then 120 floors)
        private const double MineDepthPenalty = 0.1;
        // The extra "distance" added for every warp that has to be used
        private const double DistancePenalty = 1;
        // The name of the "leyline" map property to pay attention to
        public const string LeylineProperty = "AlchemyLeyline";
        // List of locations currently involved in a distance check
        private static List<string> _Active = new List<string>();
        public static double GetPathDistance(GameLocation startLocation)
        {
            double RecursionMethod(GameLocation location, double dist)
            {
                Console.WriteLine(location.Name + " ~ Trying to find source...");
                // We set ourselves as active to prevent infinite recursion
                _Active.Add(location.Name);
                // Check if this is a leveled location
                if (location is MineShaft)
                {
                    Console.WriteLine(location.Name + " ~ Leveled location, overriding cache...");
                    var shaft = location as MineShaft;
                    _Active.Remove(location.Name);
                    if (shaft.mineLevel > 120) // SkullCave
                        return GetPathDistance(Game1.getLocationFromName("SkullCave")) + DistancePenalty + (shaft.mineLevel * SkullDepthPenalty);
                    else // Mines
                        return GetPathDistance(Game1.getLocationFromName("Mine")) + DistancePenalty + (shaft.mineLevel * MineDepthPenalty);
                }
                Console.WriteLine(location.Name + " ~ Assuming max distance...");
                // We assume to begin with that we are insanely far away (No real situation should ever have -this- high a value, so it also makes it possible to detect a location that is not connected at all)
                double mdist = double.MaxValue;
                Console.WriteLine(location.Name + " ~ Checking for map property...");
                // AlchemyOffset, used to create path distance end points that can have a default penalty or have it as 0 for no default penalty
                if (location.map.Properties.ContainsKey(LeylineProperty))
                    mdist = Convert.ToDouble((string)location.map.Properties[LeylineProperty]) - DistancePenalty;
                else // The hard offset of a alchemyOffset point overrides any distance based cost
                {
                    Console.WriteLine(location.Name + " ~ Map property not found, looking for source node...");
                    // List if maps this map is connected to
                    List<string> maps = new List<string>();
                    // Check through all warps in the location
                    foreach (Warp warp in location.warps)
                    {
                        if (!maps.Contains(warp.TargetName) && !_Active.Contains(warp.TargetName))
                            maps.Add(warp.TargetName);
                    }
                    Console.WriteLine(location.Name + " ~ Warps checked, current distance value: "+mdist);
                    // We loop through all Buildings tiles on the map to look for certain tile actions
                    for (int x = 0; x < location.map.Layers[0].LayerWidth; x++)
                        for (int y = 0; y < location.map.Layers[0].LayerHeight; y++)
                        {
                            // We check if it has a Action property, otherwise we can just ignore it
                            string prop = location.doesTileHaveProperty(x, y, "Action", "Buildings");
                            if (prop == null)
                                continue;
                            // We check if the property is a certain type
                            switch (prop)
                            {
                                // Locations with special warps are handled here
                                case "WarpCommunityCenter":
                                case "WarpGreenhouse":
                                    string targetName = prop.Substring(4);
                                    if (!maps.Contains(targetName) && !_Active.Contains(targetName))
                                        maps.Add(targetName);
                                    break;
                                default:
                                    // Locations that are normal or locked-door warps are handled here
                                    var props = prop.Split(' ');
                                    if ((props[0].Equals("Warp") || props[0].Equals("LockedDoorWarp")) && Game1.getLocationFromName(props[3]) != null)
                                    {
                                        if (!maps.Contains(props[3]) && !_Active.Contains(props[3]))
                                            maps.Add(props[3]);
                                    }
                                    break;
                            }

                        }
                    _Active.AddRange(maps);
                    foreach(string map in maps)
                    {
                        double vdist = RecursionMethod(Game1.getLocationFromName(map), dist + DistancePenalty);
                        if (vdist < mdist)
                            mdist = vdist;
                    }
                }
                Console.WriteLine(location.Name + " ~ Actions checked, current distance value: " + mdist);
                // We remove ourselves from the active list so future queries will work properly again
                // We add the result for this location to the cache only if its parent distance is 0 (This is the location being checked)
                if (dist == 0)
                {
                    _Active.Clear();
                    _Cache.Add(location.Name, mdist + DistancePenalty);
                }
                return mdist + DistancePenalty;
            }
            // We only calculate path distance if we havent done so already for this location (Unless it is leveled, then we always recalculate)
            if (!_Cache.ContainsKey(startLocation.Name))
                return RecursionMethod(startLocation, 0);
            // We return the offset (Distance of the parent) to our own, and return it
            return _Cache[startLocation.Name];
        }
    }
}
