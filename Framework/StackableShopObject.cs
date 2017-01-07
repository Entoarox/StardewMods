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
            Item = item;
            stackAmount = stack;
            parentSheetIndex = item.parentSheetIndex;
            price = salePrice() * stack;
            MaxStackSize = (int)Math.Floor(999d / stack);
        }
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            Item.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, false);
            var _scale = 0.5f + scaleSize;
            Game1.drawWithBorder(getStackNumber().ToString(), Color.Black, Color.White, location + new Vector2(Game1.tileSize - Game1.tinyFont.MeasureString(getStackNumber().ToString()).X * _scale, Game1.tileSize - (float)((double)Game1.tinyFont.MeasureString(getStackNumber().ToString()).Y * 3.0f / 4.0f) * _scale), 0.0f, _scale, 1f, true);
        }
        public override int salePrice()
        {
            return Item.salePrice() * stackAmount;
        }
        public override string Name
        {
            get { return Item.name; }
            set { Item.name = value; }
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
        public override string getDescription()
        {
            return Item.getDescription();
        }
        public override Color getCategoryColor()
        {
            return Item.getCategoryColor();
        }
        public override string getCategoryName()
        {
            return Item.getCategoryName();
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
            return new StackableShopObject(Item, stackAmount);
        }
        public override Item getOne()
        {
            return Clone();
        }
        public StardewValley.Object Revert()
        {
            return new StardewValley.Object(Item.parentSheetIndex, stackAmount * stack);
        }
    }
}
