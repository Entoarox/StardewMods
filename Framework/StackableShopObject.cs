using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework
{
    public class StackableShopObject : StardewValley.Object
    {
        public readonly int stackAmount;
        private StardewValley.Object Item;
        public int MaxStackSize;
        public StackableShopObject(StardewValley.Object item, int stack)
        {
            this.Item = item;
            this.stackAmount = stack;
            this.parentSheetIndex = item.parentSheetIndex;
            this.price = salePrice() * stack;
            this.MaxStackSize = (int)Math.Floor(999d / stack);
            this.name = this.Item.Name;
        }
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            this.Item.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, false);
            var _scale = 0.5f + scaleSize;
            Game1.drawWithBorder(getStackNumber().ToString(), Color.Black, Color.White, location + new Vector2(Game1.tileSize - Game1.tinyFont.MeasureString(getStackNumber().ToString()).X * _scale, Game1.tileSize - (float)((double)Game1.tinyFont.MeasureString(getStackNumber().ToString()).Y * 3.0f / 4.0f) * _scale), 0.0f, _scale, 1f, true);
        }
        public override int salePrice()
        {
            return this.Item.salePrice() * this.stackAmount;
        }
        public int getStackNumber()
        {
            return (this.stack * this.stackAmount);
        }
        public override int maximumStackSize()
        {
            return this.MaxStackSize;
        }
        public new bool canStackWith(Item obj)
        {
            return obj.canStackWith(this) && obj is StackableShopObject && (this.Stack + obj.Stack) < maximumStackSize();
        }
        public override string getDescription()
        {
            return this.Item.getDescription();
        }
        public override Color getCategoryColor()
        {
            return this.Item.getCategoryColor();
        }
        public override string getCategoryName()
        {
            return this.Item.getCategoryName();
        }
        public override bool isPlaceable()
        {
            return false;
        }
        public override bool isPassable()
        {
            return false;
        }
        public StackableShopObject Clone()
        {
            return new StackableShopObject(this.Item, this.stackAmount);
        }
        public override Item getOne()
        {
            return Clone();
        }
        public StardewValley.Object Revert()
        {
            return new StardewValley.Object(this.Item.parentSheetIndex, this.stackAmount * this.stack);
        }
    }
}
