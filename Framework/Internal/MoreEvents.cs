using System;

using StardewValley;
using StardewValley.Menus;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using StardewModdingAPI.Events;

using xTile.Tiles;
using xTile.ObjectModel;

namespace Entoarox.Framework.Events
{
    public static partial class MoreEvents
    {
        internal static Item prevItem=null;
        internal static void ControlEvents_ControllerButtonPressed(object sender, EventArgsControllerButtonPressed e)
        {
            if (e.ButtonPressed == Buttons.A)
                CheckForAction();
        }
        internal static void ControlEvents_MouseChanged(object sender, EventArgsMouseStateChanged e)
        {
            if (e.NewState.RightButton == ButtonState.Pressed && e.PriorState.RightButton != ButtonState.Pressed)
                CheckForAction();
        }
        private static void CheckForAction()
        {
            if (Game1.activeClickableMenu==null && !Game1.player.UsingTool && !Game1.pickingTool && !Game1.menuUp && (!Game1.eventUp || Game1.currentLocation.currentEvent.playerControlSequence) && !Game1.nameSelectUp && Game1.numberOfSelectedItems == -1 && !Game1.fadeToBlack)
            {
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
                    ActionTriggered(null, new EventArgsActionTriggered(Game1.player, split[0], args, grabTile));
                }
            }
        }
        internal static void FireSmartManagerReady()
        {
            SmartManagerReady?.Invoke(null, null);
        }
        internal static void FireWorldReady()
        {
            WorldReady?.Invoke(null, EventArgs.Empty);
        }
        internal static void MenuEvents_MenuChanged(object s, EventArgsClickableMenuChanged e)
        {
            if (BeforeSaving != null && (e.NewMenu is SaveGameMenu || e.NewMenu is ShippingMenu))
            {
                MenuEvents.MenuClosed += MenuEvents_MenuClosed;
                BeforeSaving(null, EventArgs.Empty);
            }
        }
        internal static void MenuEvents_MenuClosed(object s, EventArgsClickableMenuClosed e)
        {
            AfterSaving(null, EventArgs.Empty);
            MenuEvents.MenuClosed -= MenuEvents_MenuClosed;
        }
        internal static void GameEvents_UpdateTick(object s, EventArgs e)
        {
            if((Game1.player.CurrentItem==null && prevItem!=null) || (Game1.player.CurrentItem!=null && !Game1.player.CurrentItem.Equals(prevItem)))
            {
                ActiveItemChanged(null, new EventArgsActiveItemChanged(prevItem, Game1.player.CurrentItem));
                prevItem = Game1.player.CurrentItem;
            }
        }
        internal static void Setup()
        {
            WorldReady += (s, e) =>
             {
                 MenuEvents.MenuChanged += MenuEvents_MenuChanged;
                 ControlEvents.ControllerButtonPressed += ControlEvents_ControllerButtonPressed;
                 ControlEvents.MouseChanged += ControlEvents_MouseChanged;
                 GameEvents.UpdateTick += GameEvents_UpdateTick;
             };
            ActionTriggered += EventSink;
            WorldReady += EventSink;
            BeforeSaving += EventSink;
            AfterSaving += EventSink;
            ActiveItemChanged += EventSink;
        }
        internal static void EventSink(object s, EventArgs e)
        {

        }
    }
}
