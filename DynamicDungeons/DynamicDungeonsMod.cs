using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using xTile.ObjectModel;
using xTile.Tiles;
using SFarmer = StardewValley.Farmer;

namespace Entoarox.DynamicDungeons
{
    public class DynamicDungeonsMod : Mod
    {
        #region fields
        internal static IMonitor SMonitor;
        internal static IModHelper SHelper;
        internal static DynamicDungeon Location;
        private (SFarmer player, string action, string[] arguments, Vector2 position) _ActionInfo;
        private bool _DoAction;
        #endregion
        #region mod
        public override void Entry(IModHelper helper)
        {
            SMonitor = this.Monitor;
            SHelper = this.Helper;
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
            helper.ConsoleCommands.Add("dd_fromseed", "dd_fromseed <seed> | Generate a dungeon from a specific seed", this.Command_Fromseed);
        }
        #endregion
        #region methods
        private void GenerateDungeon(int? seed = null)
        {
            var watch = new Stopwatch();
            this.Monitor.Log("Generating dungeon...", LogLevel.Alert);
            watch.Start();
            Location = new DynamicDungeon(10, seed);
            watch.Stop();
            this.Monitor.Log("Generation completed in ["+watch.ElapsedMilliseconds+"] miliseconds", LogLevel.Alert);
        }
        #endregion
        #region events
        private void Command_Fromseed(string command, string[] arguments)
        {
            try
            {
                GenerateDungeon(Convert.ToInt32(arguments[0], 16));
            }
            catch
            {
                this.Monitor.Log("Input is not a valid seed!", LogLevel.Error);
            }
        }
        private void GameEvents_UpdateTick(object s, EventArgs e)
        {
            if (Context.IsWorldReady)
            {
                GameEvents.UpdateTick -= this.GameEvents_UpdateTick;
                InputEvents.ButtonReleased += this.InputEvents_ButtonReleased;
            }
        }
        private void InputEvents_ButtonReleased(object s, EventArgsInput e)
        {
            if (!Context.IsWorldReady || (e.Button != SButton.F5 && e.Button != SButton.F6 && e.Button != SButton.F7))
                return;
            if (!Game1.currentLocation.Name.Equals("DynamicDungeon") && e.Button == SButton.F5)
            {
                GenerateDungeon();
            }
            else if (Game1.currentLocation.Name.Equals("DynamicDungeon") && e.Button == SButton.F5)
            {
                this.Monitor.Log("Teleporting player to home...", LogLevel.Alert);
                Game1.locations.Remove(Location);
                Point p = (Game1.getLocationFromName("FarmHouse") as StardewValley.Locations.FarmHouse).getEntryLocation();
                Game1.warpFarmer("FarmHouse", p.X, p.Y, false);
                this.Monitor.Log("Teleportation completed!", LogLevel.Alert);
            }
            else if (Game1.currentLocation.Name.Equals("DynamicDungeon") || e.Button == SButton.F6)
            {
                if (Location == null)
                    GenerateDungeon();
                if(!Game1.currentLocation.Name.Equals("DynamicDungeon"))
                    Game1.locations.Add(Location);
                this.Monitor.Log("Teleporting player to dungeon...", LogLevel.Alert);
                Point p = Location.EntryPoint;
                Game1.warpFarmer(Location, p.X, p.Y, 0, false);
                this.Monitor.Log("Teleportation completed!", LogLevel.Alert);
            }
        }
        #endregion
    }
}
