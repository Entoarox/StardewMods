using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using StardewValley;

namespace Entoarox.Framework
{
    abstract class CustomObject : StardewValley.Object, ICustomItem
    {
        protected Texture2D ItemIcon;
        protected string ItemName;
        protected string ItemDescription = null;
        protected string ItemCategory;
        protected int ItemStack = 1;
        protected Color? CategoryColor = null;
        protected bool IsDroppable = false;
        protected bool IsGiftable = false;
        protected bool IsTrashable = false;

        protected CustomObject()
        {

        }
        protected CustomObject(string itemName, string itemDescription, Texture2D itemIcon)
        {
            this.ItemName = itemName;
            this.ItemIcon = itemIcon;
        }

        public override string DisplayName { get => this.name ?? this.ItemName; set => this.name = value; }
        public override bool canBeDropped()
        {
            return this.IsDroppable;
        }
        public override bool canBeGivenAsGift()
        {
            return this.IsGiftable;
        }
        public override bool canBeTrashed()
        {
            return this.IsTrashable;
        }
        public override string getDescription()
        {
            return this.ItemDescription ?? "";
        }
        public override string getCategoryName()
        {
            return this.ItemCategory ?? "General";
        }
        public override Color getCategoryColor()
        {
            return this.CategoryColor ?? Color.Black;
        }
        protected abstract Item Copy();
        public override Item getOne()
        {
            return this.Copy();
        }
        public override int getStack()
        {
            return this.ItemStack;
        }
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            float halfTilesize = Game1.tileSize / 2;
            spriteBatch.Draw(Game1.shadowTexture, location + new Vector2(halfTilesize, (Game1.tileSize * 3 / 4)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * 0.5f, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 3f, SpriteEffects.None, layerDepth - 0.0001f);
            spriteBatch.Draw(this.ItemIcon, location + new Vector2(((int)(halfTilesize * scaleSize)), ((int)(halfTilesize * scaleSize))), this.ItemIcon.Bounds, Color.White * transparency, 0f, new Vector2(8f, 8f) * scaleSize, Game1.pixelZoom * scaleSize, SpriteEffects.None, layerDepth);
        }
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            var vector = Game1.GlobalToLocal(Game1.viewport, new Vector2((x * Game1.tileSize + Game1.tileSize / 2 + ((this.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0)), (y * Game1.tileSize + Game1.tileSize / 2 + ((this.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0))));
            spriteBatch.Draw(this.ItemIcon, vector, this.ItemIcon.Bounds, Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0);
        }
        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, StardewValley.Farmer f)
        {
            spriteBatch.Draw(this.ItemIcon, objectPosition, this.ItemIcon.Bounds, Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (f.getStandingY() + 2) / 10000f));
        }
    }
}
