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
        private PlayerModifier CurrentBoost;
        public override void Entry(IModHelper helper)
        {
            this.cfg = this.Helper.ReadConfig<ConfigFP>();
            GameEvents.UpdateTick += this.UpdateTick;
            helper.ConsoleCommands.Add("fp_info", "Gives info about the path you are currently standing on", this.CommandInfo);
            this.Modifiers = new PlayerModifier[10] {
                new PlayerModifier() { WalkSpeedModifier = this.cfg.woodFloorBoost, RunSpeedModifier = this.cfg.woodFloorBoost },
                new PlayerModifier() { WalkSpeedModifier = this.cfg.stoneFloorBoost, RunSpeedModifier = this.cfg.stoneFloorBoost },
                new PlayerModifier() { WalkSpeedModifier = this.cfg.weatheredFloorBoost, RunSpeedModifier = this.cfg.weatheredFloorBoost },
                new PlayerModifier() { WalkSpeedModifier = this.cfg.crystalFloorBoost, RunSpeedModifier = this.cfg.crystalFloorBoost },
                new PlayerModifier() { WalkSpeedModifier = this.cfg.strawFloorBoost, RunSpeedModifier = this.cfg.strawFloorBoost },
                new PlayerModifier() { WalkSpeedModifier = this.cfg.gravelPathBoost, RunSpeedModifier = this.cfg.gravelPathBoost },
                new PlayerModifier() { WalkSpeedModifier = this.cfg.woodPathBoost, RunSpeedModifier = this.cfg.woodPathBoost },
                new PlayerModifier() { WalkSpeedModifier = this.cfg.crystalPathBoost, RunSpeedModifier = this.cfg.crystalPathBoost },
                new PlayerModifier() { WalkSpeedModifier = this.cfg.cobblePathBoost, RunSpeedModifier = this.cfg.cobblePathBoost },
                new PlayerModifier() { WalkSpeedModifier = this.cfg.steppingStoneBoost, RunSpeedModifier = this.cfg.steppingStoneBoost }
            };
            this.Helper.Player().Modifiers.Add(new PlayerModifier() { WalkSpeedModifier = this.cfg.walkSpeedBoost, RunSpeedModifier = this.cfg.runSpeedBoost });
        }
        public void UpdateTick(object s, EventArgs e)
        {
            if (!Game1.hasLoadedGame)
                return;
            Vector2 pos = Game1.player.getTileLocation();
            if (Game1.currentLocation.terrainFeatures.ContainsKey(pos) && Game1.currentLocation.terrainFeatures[pos] is Flooring)
            {
                PlayerModifier NewBoost = this.Modifiers[(Game1.currentLocation.terrainFeatures[pos] as Flooring).whichFloor.Value];
                if (this.CurrentBoost != null && this.CurrentBoost == NewBoost)
                    return;
                if (this.CurrentBoost != null)
                {
                    this.Monitor.Log("Replacing path boost: " + this.CurrentBoost.WalkSpeedModifier,LogLevel.Trace);
                    this.Helper.Player().Modifiers.Remove(this.CurrentBoost);
                }
                this.Monitor.Log("Adding path boost: "+NewBoost.WalkSpeedModifier,LogLevel.Trace);
                this.Helper.Player().Modifiers.Add(NewBoost);
                this.CurrentBoost = NewBoost;
            }
            else if (this.CurrentBoost != null)
            {
                this.Monitor.Log("Removing path boost: " + this.CurrentBoost.WalkSpeedModifier,LogLevel.Trace);
                this.Helper.Player().Modifiers.Remove(this.CurrentBoost);
                this.CurrentBoost = null;
            }
        }
        public void CommandInfo(string name, string[] args)
        {
            if(Game1.currentLocation.terrainFeatures.ContainsKey(Game1.player.getTileLocation()) && Game1.currentLocation.terrainFeatures[Game1.player.getTileLocation()] is Flooring)
            {
                Flooring floor = (Flooring)Game1.currentLocation.terrainFeatures[Game1.player.getTileLocation()];
                this.Monitor.Log("Floor ID for current tile's floor: " + floor.whichFloor.Value,LogLevel.Alert);
            }
            else
                this.Monitor.Log("No path in the current tile", LogLevel.Alert);
        }
    }
}
