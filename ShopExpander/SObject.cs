using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.ShopExpander
{
    public class SObject : StardewValley.Object
    {
        public string targetedShop;
        public int stackAmount;
        private StardewValley.Object Item;
        public int MaxStackSize;
        public string requirements;
        public SObject(StardewValley.Object item, int stack)
        {
            this.Item = item;
            this.stackAmount = stack; 
            this.ParentSheetIndex = item.ParentSheetIndex;
            this.Price = this.salePrice() * stack;
            this.MaxStackSize = (int)Math.Floor(999d / stack);
            this.name = this.stackAmount.ToString() + ' ' + this.Item.Name;
        }
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
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
            color = color * 0.5f;
            double num5 = 0.0;
            Vector2 origin = new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y);
            double num6 = 3.0;
            int num7 = 0;
            double num8 = layerDepth - 9.99999974737875E-05;
            if(drawShadow)
                spriteBatch1.Draw(texture, position, sourceRectangle, color, (float)num5, origin, (float)num6, (SpriteEffects)num7, (float)num8);
            spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2((int)((Game1.tileSize / 2) * (double)scaleSize), (int)((Game1.tileSize / 2) * scaleSize)), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.ParentSheetIndex, 16, 16)), Color.White * transparency, 0.0f, new Vector2(8f, 8f) * scaleSize, Game1.pixelZoom * scaleSize, SpriteEffects.None, layerDepth);
            float _scale = 0.5f + scaleSize;
            Game1.drawWithBorder(this.getStackNumber().ToString(), Color.Black, Color.White, location + new Vector2(Game1.tileSize - Game1.tinyFont.MeasureString(this.getStackNumber().ToString()).X * _scale, Game1.tileSize - (float)((double)Game1.tinyFont.MeasureString(this.getStackNumber().ToString()).Y * 3.0f / 4.0f) * _scale), 0.0f, _scale, 1f, true);
        }
        public override int salePrice()
        {
            return this.Item.salePrice() * this.stackAmount;
        }
        public int getStackNumber()
        {
            return (this.Stack * this.stackAmount);
        }
        public override int maximumStackSize()
        {
            return this.MaxStackSize;
        }
        public new bool canStackWith(Item obj)
        {
            return obj.canStackWith(this) && obj is SObject && (this.Stack + obj.Stack) < this.maximumStackSize();
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
        public SObject Clone()
        {
            var obj = new SObject(this.Item, this.stackAmount);
            obj.targetedShop = this.targetedShop;
            obj.requirements = this.requirements;

            return obj;
        }
        public override Item getOne()
        {
            return this.Clone();
        }
        public StardewValley.Object Revert()
        {
            return new StardewValley.Object(this.Item.ParentSheetIndex, this.stackAmount * this.Stack);
        }
    }
}
