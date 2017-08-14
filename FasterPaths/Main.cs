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
        private PlayerModifier[] Modifiers;
        private Version Version = new Version(1, 3, 1);
        private PlayerModifier CurrentBoost;
        public override void Entry(IModHelper helper)
        {
            Helper.RequestUpdateCheck("https://raw.githubusercontent.com/Entoarox/Stardew-SMAPI-mods/master/Projects/FasterPaths/update.json");
            cfg = Helper.ReadConfig<ConfigFP>();
            GameEvents.UpdateTick += UpdateTick;
            helper.ConsoleCommands.Add("fp_info", "Gives info about the path you are currently standing on", this.CommandInfo);
            Modifiers= new PlayerModifier[10] {
                new PlayerModifier() { WalkSpeedModifier = cfg.woodFloorBoost, RunSpeedModifier = cfg.woodFloorBoost },
                new PlayerModifier() { WalkSpeedModifier = cfg.stoneFloorBoost, RunSpeedModifier = cfg.stoneFloorBoost },
                new PlayerModifier() { WalkSpeedModifier = cfg.weatheredFloorBoost, RunSpeedModifier = cfg.weatheredFloorBoost },
                new PlayerModifier() { WalkSpeedModifier = cfg.crystalFloorBoost, RunSpeedModifier = cfg.crystalFloorBoost },
                new PlayerModifier() { WalkSpeedModifier = cfg.strawFloorBoost, RunSpeedModifier = cfg.strawFloorBoost },
                new PlayerModifier() { WalkSpeedModifier = cfg.gravelPathBoost, RunSpeedModifier = cfg.gravelPathBoost },
                new PlayerModifier() { WalkSpeedModifier = cfg.woodPathBoost, RunSpeedModifier = cfg.woodPathBoost },
                new PlayerModifier() { WalkSpeedModifier = cfg.crystalPathBoost, RunSpeedModifier = cfg.crystalPathBoost },
                new PlayerModifier() { WalkSpeedModifier = cfg.cobblePathBoost, RunSpeedModifier = cfg.cobblePathBoost },
                new PlayerModifier() { WalkSpeedModifier = cfg.steppingStoneBoost, RunSpeedModifier = cfg.steppingStoneBoost }
            };
            Helper.Player().Modifiers.Add(new PlayerModifier() { WalkSpeedModifier = cfg.walkSpeedBoost, RunSpeedModifier = cfg.runSpeedBoost });
        }
        public void UpdateTick(object s, EventArgs e)
        {
            if (!Game1.hasLoadedGame)
                return;
            Vector2 pos = Game1.player.getTileLocation();
            if (Game1.currentLocation.terrainFeatures.ContainsKey(pos) && Game1.currentLocation.terrainFeatures[pos] is Flooring)
            {
                PlayerModifier NewBoost = Modifiers[(Game1.currentLocation.terrainFeatures[pos] as Flooring).whichFloor];
                if (CurrentBoost != null && CurrentBoost == NewBoost)
                    return;
                if (CurrentBoost != null)
                {
                    Monitor.Log("Replacing path boost: " + CurrentBoost.WalkSpeedModifier,LogLevel.Trace);
                    Helper.Player().Modifiers.Remove(CurrentBoost);
                }
                Monitor.Log("Adding path boost: "+NewBoost.WalkSpeedModifier,LogLevel.Trace);
                Helper.Player().Modifiers.Add(NewBoost);
                CurrentBoost = NewBoost;
            }
            else if (CurrentBoost != null)
            {
                Monitor.Log("Removing path boost: " + CurrentBoost.WalkSpeedModifier,LogLevel.Trace);
                Helper.Player().Modifiers.Remove(CurrentBoost);
                CurrentBoost = null;
            }
        }
        public void CommandInfo(string name, string[] args)
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