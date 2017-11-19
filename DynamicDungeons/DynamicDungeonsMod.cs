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
        private DungeonBuilder _Builder;
        private Texture2D _BuilderMinimap;
        private GameLocation _BuilderLocation;
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
            if (this._BuilderLocation != null)
                Game1.locations.Remove(this._BuilderLocation);
            if (seed == null)
                this._Builder = new DungeonBuilder(10.0,0);
            else
                this._Builder = new DungeonBuilder(10.0,0,(int)seed);
            this._BuilderMinimap = this._Builder.GetMiniMap();
            this._BuilderLocation = new GameLocation(this._Builder.GetMap(), "DynamicDungeon");
            Game1.locations.Add(this._BuilderLocation);
            watch.Stop();
            this.Monitor.Log("Generation completed in ["+watch.ElapsedMilliseconds+"] miliseconds", LogLevel.Alert);
        }
        private void CheckForAction()
        {
            if (Game1.activeClickableMenu == null && !Game1.player.UsingTool && !Game1.pickingTool && !Game1.menuUp && (!Game1.eventUp || Game1.currentLocation.currentEvent.playerControlSequence) && !Game1.nameSelectUp && Game1.numberOfSelectedItems == -1 && !Game1.fadeToBlack)
            {
                this._DoAction = false;
                Vector2 grabTile = new Vector2((Game1.getOldMouseX() + Game1.viewport.X), (Game1.getOldMouseY() + Game1.viewport.Y)) / Game1.tileSize;
                if (!Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
                    grabTile = Game1.player.GetGrabTile();
                Tile tile = Game1.currentLocation.map.GetLayer("Buildings").PickTile(new xTile.Dimensions.Location((int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize), Game1.viewport.Size);
                PropertyValue propertyValue = null;
                if (tile != null)
                    tile.Properties.TryGetValue("Action", out propertyValue);
                if (propertyValue != null)
                {
                    string[] split = ((string)propertyValue).Split(' ');
                    string[] args = new string[split.Length - 1];
                    Array.Copy(split, 1, args, 0, args.Length);
                    this._ActionInfo = (Game1.player, split[0], args, grabTile);
                    this._DoAction = true;
                }
            }
        }
        private void HandleAction()
        {
            this.Monitor.Log("Action detected: `" + this._ActionInfo.action + '`');
            /**
             * Loot tables:
             * General  - Default loot table, no specific type requirements [LootChest]
             * Gems     - Gems only [GemRock]
             * Supplies - Things useful to continue mining longer [CavedSkeleton]
             */
            if (this._ActionInfo.action.Equals("DDLoot")) // DDLoot <string:table> <int:dropcount> [bool:deleteTile] [<float:enemySpawnChance> <string:spawnedEnemy>]
                Game1.drawObjectDialogue($"TODO: Trigger the \"{this._ActionInfo.arguments[0]}\" loot table and drop ({this._ActionInfo.arguments[1]}) items");
            else if (this._ActionInfo.action.Equals("DDShop")) // DDShop <vendorID> [Currently only DwarfVendor]
                Game1.drawObjectDialogue($"TODO: Open the \"{this._ActionInfo.arguments[0]}\" shop menu here");
            else if (this._ActionInfo.action.Equals("DDLooted")) // DDLooted
                Game1.drawObjectDialogue("TODO: Localize `You search thoroughly but cant find anything of value.`");
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
                GraphicsEvents.OnPreRenderHudEvent += this.GraphicsEvents_OnPreRenderHudEvent;
                InputEvents.ButtonReleased += this.InputEvents_ButtonReleased;
                ControlEvents.ControllerButtonPressed += this.ControlEvents_ControllerButtonPressed;
                ControlEvents.ControllerButtonReleased += this.ControlEvents_ControllerButtonReleased;
                ControlEvents.MouseChanged += this.ControlEvents_MouseChanged;
            }
        }
        private void GraphicsEvents_OnPreRenderHudEvent(object s, EventArgs e)
        {
            if (Game1.currentLocation != null && this._BuilderLocation != null && Game1.currentLocation.Name.Equals(this._BuilderLocation.Name))
            {
                IClickableMenu.drawTextureBox(Game1.spriteBatch, 158, 4, 125, 40, Color.White);
                IClickableMenu.drawTextureBox(Game1.spriteBatch, 158, 48, 80, 52, Color.White);
                IClickableMenu.drawTextureBox(Game1.spriteBatch, 4, 4, 150, 150, Color.White);
                Game1.spriteBatch.Draw(this._BuilderMinimap, new Vector2(20, 20), Color.White);
                Point p = Game1.player.getTileLocationPoint();
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 19, p.Y + 19, 1, 1), Color.Red * 0.33f);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 19, p.Y + 21, 1, 1), Color.Red * 0.33f);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 21, p.Y + 19, 1, 1), Color.Red * 0.33f);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 21, p.Y + 21, 1, 1), Color.Red * 0.33f);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 20, p.Y + 19, 1, 1), Color.Red * 0.66f);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 19, p.Y + 20, 1, 1), Color.Red * 0.66f);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 21, p.Y + 20, 1, 1), Color.Red * 0.66f);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 20, p.Y + 21, 1, 1), Color.Red * 0.66f);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 20, p.Y + 20, 1, 1), Color.Red * 0.99f);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13, 17, 9, 14), Color.Green);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 10, 17, 9, 14), Color.Green);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 20, 17, 9, 14), Color.Green);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 30, 17, 9, 14), Color.Green);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 40, 17, 9, 14), Color.DarkOrange);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 50, 17, 9, 14), Color.DarkOrange);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 60, 17, 9, 14), Color.DarkOrange);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 70, 17, 9, 14), Color.Red);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 80, 17, 9, 14), Color.Red);
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 90, 17, 9, 14), Color.DarkRed);
                Utility.drawTextWithShadow(Game1.spriteBatch, "9999", Game1.smallFont, new Vector2(158 + 12, 48 + 12), Game1.textColor);
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
                Point p = (Game1.getLocationFromName("FarmHouse") as StardewValley.Locations.FarmHouse).getEntryLocation();
                Game1.warpFarmer("FarmHouse", p.X, p.Y, false);
                this.Monitor.Log("Teleportation completed!", LogLevel.Alert);
            }
            else if (Game1.currentLocation.Name.Equals("DynamicDungeon") || e.Button == SButton.F6)
            {
                if (this._Builder == null)
                    GenerateDungeon();
                this.Monitor.Log("Teleporting player to dungeon...", LogLevel.Alert);
                Point p = this._Builder.GetFloorPoint();
                Game1.warpFarmer(this._BuilderLocation.Name, p.X, p.Y, false);
                this.Monitor.Log("Teleportation completed!", LogLevel.Alert);
            }
        }
        private void ControlEvents_ControllerButtonPressed(object sender, EventArgsControllerButtonPressed e)
        {
            if (e.ButtonPressed == Buttons.A)
                CheckForAction();
        }
        private void ControlEvents_ControllerButtonReleased(object sender, EventArgsControllerButtonReleased e)
        {
            if (this._DoAction && e.ButtonReleased == Buttons.A)
            {
                this.HandleAction();
                this._DoAction = false;
            }
        }
        private void ControlEvents_MouseChanged(object sender, EventArgsMouseStateChanged e)
        {
            if (e.NewState.RightButton == ButtonState.Pressed && e.PriorState.RightButton != ButtonState.Pressed)
                CheckForAction();
            if (this._DoAction && e.NewState.RightButton == ButtonState.Released)
            {
                this.HandleAction();
                this._DoAction = false;
            }
        }
        #endregion
    }
}
