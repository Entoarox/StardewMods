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
    /// <summary>The mod entry class.</summary>
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

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;

            helper.ConsoleCommands.Add("fp_info", "Gives info about the path you are currently standing on", this.CommandInfo);
        }


        /*********
        ** Protected methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.Modifiers = new[]
            {
                new PlayerModifier { WalkSpeedModifier = this.Config.WoodFloorBoost, RunSpeedModifier = this.Config.WoodFloorBoost },
                new PlayerModifier { WalkSpeedModifier = this.Config.StoneFloorBoost, RunSpeedModifier = this.Config.StoneFloorBoost },
                new PlayerModifier { WalkSpeedModifier = this.Config.WeatheredFloorBoost, RunSpeedModifier = this.Config.WeatheredFloorBoost },
                new PlayerModifier { WalkSpeedModifier = this.Config.CrystalFloorBoost, RunSpeedModifier = this.Config.CrystalFloorBoost },
                new PlayerModifier { WalkSpeedModifier = this.Config.StrawFloorBoost, RunSpeedModifier = this.Config.StrawFloorBoost },
                new PlayerModifier { WalkSpeedModifier = this.Config.GravelPathBoost, RunSpeedModifier = this.Config.GravelPathBoost },
                new PlayerModifier { WalkSpeedModifier = this.Config.WoodPathBoost, RunSpeedModifier = this.Config.WoodPathBoost },
                new PlayerModifier { WalkSpeedModifier = this.Config.CrystalPathBoost, RunSpeedModifier = this.Config.CrystalPathBoost },
                new PlayerModifier { WalkSpeedModifier = this.Config.CobblePathBoost, RunSpeedModifier = this.Config.CobblePathBoost },
                new PlayerModifier { WalkSpeedModifier = this.Config.SteppingStoneBoost, RunSpeedModifier = this.Config.SteppingStoneBoost }
            };
            this.Helper.Player().Modifiers.Add(new PlayerModifier { WalkSpeedModifier = this.Config.WalkSpeedBoost, RunSpeedModifier = this.Config.RunSpeedBoost });
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, EventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;
            Vector2 pos = Game1.player.getTileLocation();
            if (Game1.currentLocation.terrainFeatures.TryGetValue(pos, out TerrainFeature terrainFeature) && terrainFeature is Flooring flooring)
            {
                PlayerModifier newBoost = this.Modifiers[flooring.whichFloor.Value];
                if (this.CurrentBoost != null && this.CurrentBoost == newBoost)
                    return;
                if (this.CurrentBoost != null)
                {
                    this.Monitor.Log($"Replacing path boost: {this.CurrentBoost.WalkSpeedModifier}", LogLevel.Trace);
                    this.Helper.Player().Modifiers.Remove(this.CurrentBoost);
                }

                this.Monitor.Log($"Adding path boost: {newBoost.WalkSpeedModifier}", LogLevel.Trace);
                this.Helper.Player().Modifiers.Add(newBoost);
                this.CurrentBoost = newBoost;
            }
            else if (this.CurrentBoost != null)
            {
                this.Monitor.Log($"Removing path boost: {this.CurrentBoost.WalkSpeedModifier}", LogLevel.Trace);
                this.Helper.Player().Modifiers.Remove(this.CurrentBoost);
                this.CurrentBoost = null;
            }
        }

        private void CommandInfo(string name, string[] args)
        {
            if (Context.IsPlayerFree && Game1.currentLocation.terrainFeatures.TryGetValue(Game1.player.getTileLocation(), out TerrainFeature terrainFeature) && terrainFeature is Flooring floor)
                this.Monitor.Log($"Floor ID for current tile's floor: {floor.whichFloor.Value}", LogLevel.Alert);
            else
                this.Monitor.Log("No path in the current tile", LogLevel.Alert);
        }
    }
}
