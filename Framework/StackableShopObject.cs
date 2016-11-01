using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Events;

using StardewValley;

namespace Entoarox.Framework
{

#pragma warning disable CS0618 // Type or member is obsolete
    public class StackableShopObject : StardewModdingAPI.Inheritance.SObject
#pragma warning restore CS0618 // Type or member is obsolete
    {
        public string targetedShop;
        public int stackAmount;
        public new int MaxStackSize;
        public string requirements;
        private static bool needHook = true;
        public StackableShopObject(StardewValley.Object item, int stack)
        {
            name = item.Name;
            stackAmount = stack;
            parentSheetIndex = item.parentSheetIndex;
            price = salePrice() * stack;
            if (needHook)
            {
                MenuEvents.MenuClosed += MenuEvents_MenuClosed;
                PlayerEvents.InventoryChanged += PlayerEvents_InventoryChanged;
                needHook = true;
            }
        }
        private StackableShopObject()
        {
            if (needHook)
            {
                MenuEvents.MenuClosed += MenuEvents_MenuClosed;
                PlayerEvents.InventoryChanged += PlayerEvents_InventoryChanged;
                needHook = true;
            }
        }
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            SpriteBatch spriteBatch1 = spriteBatch;
            Texture2D texture = Game1.shadowTexture;
            Vector2 vector2_1 = location;
            double num1 = (Game1.tileSize / 2);
            int num2 = Game1.tileSize * 3 / 4;
            int num3 = Game1.pixelZoom;
            double num4 = num2;
            Vector2 vector2_2 = new Vector2((float)num1, (float)num4);
            Vector2 position = vector2_1 + vector2_2;
            Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
            Color color = Color.White * 0.5f;
            double num5 = 0.0;
            Vector2 origin = new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y);
            double num6 = 3.0;
            int num7 = 0;
            double num8 = layerDepth - 9.99999974737875E-05;
            spriteBatch1.Draw(texture, position, sourceRectangle, color, (float)num5, origin, (float)num6, (SpriteEffects)num7, (float)num8);
            spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2((int)((Game1.tileSize / 2) * (double)scaleSize), (int)((Game1.tileSize / 2) * scaleSize)), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, parentSheetIndex, 16, 16)), Color.White * transparency, 0.0f, new Vector2(8f, 8f) * scaleSize, Game1.pixelZoom * scaleSize, SpriteEffects.None, layerDepth);
            var _scale = 0.5f + scaleSize;
            Game1.drawWithBorder(getStackNumber().ToString(), Color.Black, Color.White, location + new Vector2(Game1.tileSize - Game1.tinyFont.MeasureString(getStackNumber().ToString()).X * _scale, Game1.tileSize - (float)((double)Game1.tinyFont.MeasureString(getStackNumber().ToString()).Y * 3.0f / 4.0f) * _scale), 0.0f, _scale, 1f, true);
        }
        public int getStackNumber()
        {
            return (stack * stackAmount);
        }
        public override int maximumStackSize()
        {
            return MaxStackSize;
        }
        public new bool canStackWith(Item obj)
        {
            return obj.canStackWith(this) && obj is StackableShopObject && (Stack + obj.Stack) < maximumStackSize();
        }
        public new StackableShopObject Clone()
        {
            var obj = new StackableShopObject();

            obj.Name = Name;
            obj.CategoryName = CategoryName;
            obj.Description = Description;
            obj.Texture = Texture;
            obj.IsPassable = IsPassable;
            obj.IsPlaceable = IsPlaceable;
            obj.quality = quality;
            obj.scale = scale;
            obj.isSpawnedObject = isSpawnedObject;
            obj.isRecipe = isRecipe;
            obj.questItem = questItem;
            obj.stack = 1;
            obj.parentSheetIndex = parentSheetIndex;
            obj.MaxStackSize = maximumStackSize();

            obj.stackAmount = stackAmount;
            obj.targetedShop = targetedShop;
            obj.requirements = requirements;

            return obj;
        }
        public override Item getOne()
        {
            return Clone();
        }
        // If the inventory changes while this even is hooked, we need to check if any SObject instances are in it, so we can replace them
        static void PlayerEvents_InventoryChanged(object send, EventArgs e)
        {
            for (int c = 0; c < Game1.player.Items.Count; c++)
            {
                if (Game1.player.Items[c] is StackableShopObject)
                {
                    Item item = new StardewValley.Object(Game1.player.Items[c].parentSheetIndex, (Game1.player.Items[c] as StackableShopObject).getStackNumber());
                    Game1.player.Items[c] = item;
                }
            }
        }
        // When the menu closes, remove the hook for the inventory changed event
        static void MenuEvents_MenuClosed(object send, EventArgs e)
        {
            PlayerEvents.InventoryChanged -= PlayerEvents_InventoryChanged;
            MenuEvents.MenuClosed += MenuEvents_MenuClosed;
            needHook = true;
        }
    }
}
