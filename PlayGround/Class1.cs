using System;
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
        private const string LeylineProperty = "AlchemyLeyline";
        // List of locations currently involved in a distance check
        private static List<string> _Active = new List<string>();
        public static double GetPathDistance(GameLocation startLocation)
        {
            double RecursionMethod(GameLocation location, double dist)
            {
                Console.WriteLine(location.Name+" ~ Validating check...");
                // We check if this location is already actively being pathed through in the active query, and return double.MaxValue if that is the case
                if (_Active.Contains(startLocation.Name))
                {
                    Console.WriteLine(location.Name + " ~ Check invalid, preventing infinite loop...");
                    return double.MaxValue;
                }
                Console.WriteLine(location.Name + " ~ Check valid, starting...");
                // We set ourselves as active to prevent infinite recursion
                _Active.Add(location.Name);
                // Check if this is a leveled location
                if (location is MineShaft)
                {
                    Console.WriteLine(location.Name + " ~ Leveled location, overriding cache...");
                    var shaft = location as MineShaft;
                    _Active.Remove(location.Name);
                    if (shaft.mineLevel > 120) // SkullCave
                        return RecursionMethod(Game1.getLocationFromName("SkullCave"), dist + DistancePenalty + (shaft.mineLevel * SkullDepthPenalty));
                    else // Mines
                        return RecursionMethod(Game1.getLocationFromName("Mine"), dist + DistancePenalty + (shaft.mineLevel * MineDepthPenalty));
                }
                Console.WriteLine(location.Name + " ~ Assuming max distance...");
                // We assume to begin with that we are insanely far away (No real situation should ever have -this- high a value, so it also makes it possible to detect a location that is not connected at all)
                double mdist = double.MaxValue;
                Console.WriteLine(location.Name + " ~ Checking for map property...");
                // AlchemyOffset, used to create path distance end points that can have a default penalty or have it as 0 for no default penalty
                if (location.map.Properties.ContainsKey(LeylineProperty))
                    mdist = Convert.ToDouble((string)location.map.Properties[LeylineProperty]);
                else // The hard offset of a alchemyOffset point overrides any distance based cost
                {
                    Console.WriteLine(location.Name + " ~ Map property not found, looking for source node...");
                    // Check through all warps in the location
                    foreach (Warp warp in location.warps)
                    {
                        // We get the path distance for the found warp, if it hasnt gotten one calculated yet then we will also be doing so
                        double vdist0 = RecursionMethod(Game1.getLocationFromName(warp.TargetName), dist + DistancePenalty);
                        // We check if the path distance for this location is less then the one we currently have, and if so, hold onto it
                        if (vdist0 < mdist)
                            mdist = vdist0;
                    }
                    Console.WriteLine(location.Name + " ~ Warps checked, current distance value: "+mdist);
                    // We loop through all Buildings tiles on the map to look for certain tile actions
                    for (int x = 0; x < location.map.Layers[0].TileSize.Width; x++)
                        for (int y = 0; y < location.map.Layers[0].TileSize.Height; y++)
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
                                    // We get the path distance for the found Action warp, if it hasnt gotten one calculated yet then we will also be doing so
                                    double vdist1 = RecursionMethod(Game1.getLocationFromName(targetName), dist + DistancePenalty);
                                    // We check if the path distance for this location is less then the one we currently have, and if so, hold onto it
                                    if (vdist1 < mdist)
                                        mdist = vdist1;
                                    break;
                                default:
                                    // Locations that are normal or locked-door warps are handled here
                                    var props = prop.Split(' ');
                                    if ((props[0].Equals("Warp") || props[0].Equals("LockedDoorWarp")) && Game1.getLocationFromName(props[3]) != null)
                                    {
                                        // We get the path distance for the found Action warp, if it hasnt gotten one calculated yet then we will also be doing so
                                        double vdist2 = RecursionMethod(Game1.getLocationFromName(props[3]), dist + DistancePenalty);
                                        // We check if the path distance for this location is less then the one we currently have, and if so, hold onto it
                                        if (vdist2 < mdist)
                                            mdist = vdist2;
                                    }
                                    break;
                            }

                        }
                }
                Console.WriteLine(location.Name + " ~ Actions checked, current distance value: " + mdist);
                // We remove ourselves from the active list so future queries will work properly again
                _Active.Remove(location.Name);
                // We add the result for this location to the cache only if its parent distance is 0 (This is the location being checked)
                if (dist == 0)
                    _Cache.Add(location.Name, mdist);
                return mdist;
            }
            // We only calculate path distance if we havent done so already for this location (Unless it is leveled, then we always recalculate)
            if (!_Cache.ContainsKey(startLocation.Name))
                return RecursionMethod(startLocation, 0);
            // We return the offset (Distance of the parent) to our own, and return it
            return _Cache[startLocation.Name];
        }
    }
}
