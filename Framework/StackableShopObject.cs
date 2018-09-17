using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Object = StardewValley.Object;

namespace Entoarox.Framework
{
    public class StackableShopObject : Object
    {
        /*********
        ** Fields
        *********/
        private readonly Object Item;
        public readonly int StackAmount;
        public int MaxStackSize;


        /*********
        ** Public methods
        *********/
        public StackableShopObject(Object item, int stack)
        {
            this.Item = item;
            this.StackAmount = stack;
            this.ParentSheetIndex = item.ParentSheetIndex;
            this.Price = this.salePrice() * stack;
            this.MaxStackSize = (int)Math.Floor(999d / stack);
            this.name = this.Item.Name;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            this.Item.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, false);
            float _scale = 0.5f + scaleSize;
            Game1.drawWithBorder(this.getStackNumber().ToString(), Color.Black, Color.White, location + new Vector2(Game1.tileSize - Game1.tinyFont.MeasureString(this.getStackNumber().ToString()).X * _scale, Game1.tileSize - (float)((double)Game1.tinyFont.MeasureString(this.getStackNumber().ToString()).Y * 3.0f / 4.0f) * _scale), 0.0f, _scale, 1f, true);
        }

        public override int salePrice()
        {
            return this.Item.salePrice() * this.StackAmount;
        }

        public int getStackNumber()
        {
            return this.Stack * this.StackAmount;
        }

        public override int maximumStackSize()
        {
            return this.MaxStackSize;
        }

        public new bool canStackWith(Item obj)
        {
            return obj.canStackWith(this) && obj is StackableShopObject && this.Stack + obj.Stack < this.maximumStackSize();
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
            return new StackableShopObject(this.Item, this.StackAmount);
        }

        public override Item getOne()
        {
            return this.Clone();
        }

        public Object Revert()
        {
            return new Object(this.Item.ParentSheetIndex, this.StackAmount * this.Stack);
        }
    }
}
