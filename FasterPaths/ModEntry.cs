using System;
using System.Diagnostics.CodeAnalysis;
using Entoarox.Framework;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Entoarox.FasterPaths
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by SMAPI.")]
    internal class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        private ModConfig Config;
        private PlayerModifier CurrentBoost;
        private PlayerModifier[] Modifiers;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            GameEvents.UpdateTick += this.UpdateTick;
            helper.ConsoleCommands.Add("fp_info", "Gives info about the path you are currently standing on", this.CommandInfo);
            this.Modifiers = new PlayerModifier[10]
            {
                new PlayerModifier {WalkSpeedModifier = this.Config.WoodFloorBoost, RunSpeedModifier = this.Config.WoodFloorBoost},
                new PlayerModifier {WalkSpeedModifier = this.Config.StoneFloorBoost, RunSpeedModifier = this.Config.StoneFloorBoost},
                new PlayerModifier {WalkSpeedModifier = this.Config.WeatheredFloorBoost, RunSpeedModifier = this.Config.WeatheredFloorBoost},
                new PlayerModifier {WalkSpeedModifier = this.Config.CrystalFloorBoost, RunSpeedModifier = this.Config.CrystalFloorBoost},
                new PlayerModifier {WalkSpeedModifier = this.Config.StrawFloorBoost, RunSpeedModifier = this.Config.StrawFloorBoost},
                new PlayerModifier {WalkSpeedModifier = this.Config.GravelPathBoost, RunSpeedModifier = this.Config.GravelPathBoost},
                new PlayerModifier {WalkSpeedModifier = this.Config.WoodPathBoost, RunSpeedModifier = this.Config.WoodPathBoost},
                new PlayerModifier {WalkSpeedModifier = this.Config.CrystalPathBoost, RunSpeedModifier = this.Config.CrystalPathBoost},
                new PlayerModifier {WalkSpeedModifier = this.Config.CobblePathBoost, RunSpeedModifier = this.Config.CobblePathBoost},
                new PlayerModifier {WalkSpeedModifier = this.Config.SteppingStoneBoost, RunSpeedModifier = this.Config.SteppingStoneBoost}
            };
            this.Helper.Player().Modifiers.Add(new PlayerModifier { WalkSpeedModifier = this.Config.WalkSpeedBoost, RunSpeedModifier = this.Config.RunSpeedBoost });
        }


        /*********
        ** Protected methods
        *********/
        private void UpdateTick(object s, EventArgs e)
        {
            if (!Game1.hasLoadedGame)
                return;
            Vector2 pos = Game1.player.getTileLocation();
            if (Game1.currentLocation.terrainFeatures.ContainsKey(pos) && Game1.currentLocation.terrainFeatures[pos] is Flooring)
            {
                PlayerModifier newBoost = this.Modifiers[(Game1.currentLocation.terrainFeatures[pos] as Flooring).whichFloor.Value];
                if (this.CurrentBoost != null && this.CurrentBoost == newBoost)
                    return;
                if (this.CurrentBoost != null)
                {
                    this.Monitor.Log("Replacing path boost: " + this.CurrentBoost.WalkSpeedModifier, LogLevel.Trace);
                    this.Helper.Player().Modifiers.Remove(this.CurrentBoost);
                }

                this.Monitor.Log("Adding path boost: " + newBoost.WalkSpeedModifier, LogLevel.Trace);
                this.Helper.Player().Modifiers.Add(newBoost);
                this.CurrentBoost = newBoost;
            }
            else if (this.CurrentBoost != null)
            {
                this.Monitor.Log("Removing path boost: " + this.CurrentBoost.WalkSpeedModifier, LogLevel.Trace);
                this.Helper.Player().Modifiers.Remove(this.CurrentBoost);
                this.CurrentBoost = null;
            }
        }

        private void CommandInfo(string name, string[] args)
        {
            if (Game1.currentLocation.terrainFeatures.ContainsKey(Game1.player.getTileLocation()) && Game1.currentLocation.terrainFeatures[Game1.player.getTileLocation()] is Flooring)
            {
                Flooring floor = (Flooring)Game1.currentLocation.terrainFeatures[Game1.player.getTileLocation()];
                this.Monitor.Log("Floor ID for current tile's floor: " + floor.whichFloor.Value, LogLevel.Alert);
            }
            else
                this.Monitor.Log("No path in the current tile", LogLevel.Alert);
        }
    }
}
