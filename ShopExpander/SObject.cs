using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using SDVObject = StardewValley.Object;

namespace Entoarox.ShopExpander
{
    internal class SObject : SDVObject
    {
        /*********
        ** Fields
        *********/
        private readonly SDVObject Item;


        /*********
        ** Accessors
        *********/
        public string targetedShop;
        public int stackAmount;
        public int MaxStackSize;
        public string requirements;


        /*********
        ** Public methods
        *********/
        public SObject(SDVObject item, int stack)
        {
            this.Item = item;
            this.stackAmount = stack;
            this.ParentSheetIndex = item.ParentSheetIndex;
            this.Price = this.salePrice() * stack;
            this.MaxStackSize = (int)Math.Floor(999d / stack);
            this.name = this.stackAmount.ToString() + ' ' + this.Item.Name;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            SpriteBatch spriteBatch1 = spriteBatch;
            Texture2D texture = Game1.shadowTexture;
            Vector2 vector2_1 = location;
            double num1 = Game1.tileSize / 2;
            int num2 = Game1.tileSize * 3 / 4;
            double num4 = num2;
            Vector2 vector2_2 = new Vector2((float)num1, (float)num4);
            Vector2 position = vector2_1 + vector2_2;
            Rectangle? sourceRectangle = Game1.shadowTexture.Bounds;
            color = color * 0.5f;
            double num5 = 0.0;
            Vector2 origin = new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y);
            double num6 = 3.0;
            int num7 = 0;
            double num8 = layerDepth - 9.99999974737875E-05;
            if (drawShadow)
                spriteBatch1.Draw(texture, position, sourceRectangle, color, (float)num5, origin, (float)num6, (SpriteEffects)num7, (float)num8);
            spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2((int)((Game1.tileSize / 2) * (double)scaleSize), (int)((Game1.tileSize / 2) * scaleSize)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.ParentSheetIndex, 16, 16), Color.White * transparency, 0.0f, new Vector2(8f, 8f) * scaleSize, Game1.pixelZoom * scaleSize, SpriteEffects.None, layerDepth);
            float _scale = 0.5f + scaleSize;
            Game1.drawWithBorder(this.getStackNumber().ToString(), Color.Black, Color.White, location + new Vector2(Game1.tileSize - Game1.tinyFont.MeasureString(this.getStackNumber().ToString()).X * _scale, Game1.tileSize - (float)((double)Game1.tinyFont.MeasureString(this.getStackNumber().ToString()).Y * 3.0f / 4.0f) * _scale), 0.0f, _scale, 1f, true);
        }
        
        public override int salePrice()
        {
            return this.Item.salePrice() * this.stackAmount;
        }
        
        public int getStackNumber()
        {
            return this.Stack * this.stackAmount;
        }

        public override int maximumStackSize()
        {
            return this.MaxStackSize;
        }

        public override bool canStackWith(ISalable other)
        {
            return other is SObject obj && obj.ParentSheetIndex == this.ParentSheetIndex && this.Stack + other.Stack <= this.maximumStackSize();
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
            return new SObject(this.Item, this.stackAmount)
            {
                targetedShop = this.targetedShop,
                requirements = this.requirements
            };
        }

        public override Item getOne()
        {
            return this.Clone();
        }

        public SDVObject Revert()
        {
            return new SDVObject(this.Item.ParentSheetIndex, this.stackAmount * this.Stack);
        }
    }
}
