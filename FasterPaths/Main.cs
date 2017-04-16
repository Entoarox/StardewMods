using System;

using StardewValley;
using StardewValley.TerrainFeatures;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using Entoarox.Framework;

using Microsoft.Xna.Framework;

namespace Entoarox.FasterPaths
{
    public class FasterPaths : Mod
    {
        private ConfigFP cfg;
        private IPlayerHelper PlayerHelper;
        private FarmerModifier[] Modifiers;
        private Version Version = new Version(1, 3, 0);
        private FarmerModifier CurrentBoost;
        public override void Entry(IModHelper helper)
        {
            VersionChecker.AddCheck("FasterPaths", Version, "https://raw.githubusercontent.com/Entoarox/Stardew-SMAPI-mods/master/Projects/VersionChecker/FasterPaths.json");
            cfg = Helper.ReadConfig<ConfigFP>();
            GameEvents.UpdateTick += UpdateTick;
            Command.RegisterCommand("fp_info", "Gives info about the path you are currently standing on").CommandFired += CommandInfo;
            Modifiers= new FarmerModifier[10] {
                new FarmerModifier() { WalkSpeedModifier = cfg.woodFloorBoost, RunSpeedModifier = cfg.woodFloorBoost },
                new FarmerModifier() { WalkSpeedModifier = cfg.stoneFloorBoost, RunSpeedModifier = cfg.stoneFloorBoost },
                new FarmerModifier() { WalkSpeedModifier = cfg.weatheredFloorBoost, RunSpeedModifier = cfg.weatheredFloorBoost },
                new FarmerModifier() { WalkSpeedModifier = cfg.crystalFloorBoost, RunSpeedModifier = cfg.crystalFloorBoost },
                new FarmerModifier() { WalkSpeedModifier = cfg.strawFloorBoost, RunSpeedModifier = cfg.strawFloorBoost },
                new FarmerModifier() { WalkSpeedModifier = cfg.gravelPathBoost, RunSpeedModifier = cfg.gravelPathBoost },
                new FarmerModifier() { WalkSpeedModifier = cfg.woodPathBoost, RunSpeedModifier = cfg.woodPathBoost },
                new FarmerModifier() { WalkSpeedModifier = cfg.crystalPathBoost, RunSpeedModifier = cfg.crystalPathBoost },
                new FarmerModifier() { WalkSpeedModifier = cfg.cobblePathBoost, RunSpeedModifier = cfg.cobblePathBoost },
                new FarmerModifier() { WalkSpeedModifier = cfg.steppingStoneBoost, RunSpeedModifier = cfg.steppingStoneBoost }
            };
            PlayerHelper = EntoFramework.GetPlayerHelper();
            PlayerHelper.AddModifier(new FarmerModifier() { WalkSpeedModifier = cfg.walkSpeedBoost, RunSpeedModifier = cfg.runSpeedBoost });
        }
        public void UpdateTick(object s, EventArgs e)
        {
            if (!Game1.hasLoadedGame)
                return;
            Vector2 pos = Game1.player.getTileLocation();
            if (Game1.currentLocation.terrainFeatures.ContainsKey(pos) && Game1.currentLocation.terrainFeatures[pos] is Flooring)
            {
                FarmerModifier NewBoost = Modifiers[(Game1.currentLocation.terrainFeatures[pos] as Flooring).whichFloor];
                if (CurrentBoost != null && CurrentBoost == NewBoost)
                    return;
                if (CurrentBoost != null)
                {
                    Monitor.Log("Replacing path boost: " + CurrentBoost.WalkSpeedModifier,LogLevel.Trace);
                    PlayerHelper.RemoveModifier(CurrentBoost);
                }
                Monitor.Log("Adding path boost: "+NewBoost.WalkSpeedModifier,LogLevel.Trace);
                PlayerHelper.AddModifier(NewBoost);
                CurrentBoost = NewBoost;
            }
            else if (CurrentBoost != null)
            {
                Monitor.Log("Removing path boost: " + CurrentBoost.WalkSpeedModifier,LogLevel.Trace);
                PlayerHelper.RemoveModifier(CurrentBoost);
                CurrentBoost = null;
            }
        }
        public void CommandInfo(object s, EventArgsCommand e)
        {
            if(Game1.currentLocation.terrainFeatures.ContainsKey(Game1.player.getTileLocation()) && Game1.currentLocation.terrainFeatures[Game1.player.getTileLocation()] is Flooring)
            {
                Flooring floor = (Flooring)Game1.currentLocation.terrainFeatures[Game1.player.getTileLocation()];
                Monitor.Log("Floor ID for current tile's floor: " + floor.whichFloor,LogLevel.Alert);
            }
            else
                Monitor.Log("No path in the current tile", LogLevel.Alert);
        }
    }
}