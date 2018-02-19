using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using StardewValley;

namespace Entoarox.CustomBooks
{
    using Framework;
    class Book : StardewValley.Object, ICustomItem
    {
        private static Texture2D Icon;
        static Book()
        {
            Icon = CustomBooksMod.SHelper.Content.Load<Texture2D>("icon.png");
        }
        public string Id;
        public Book()
        {
        }
        public Book(string id)
        {
            this.Id = id;
            this.name = CustomBooksMod.Shelf.Books[id].Name;
            this.bigCraftable = true;
        }

        public void Activate()
        {
            Game1.activeClickableMenu = new BookMenu(this.Id);
        }

        public string Sleep()
        {
            return this.Id;
        }

        public void Wakeup(string data)
        {
            this.Id = data;
        }

        public override string DisplayName { get => this.name; set => this.displayName = value; }
        public override bool canBeDropped()
        {
            return false;
        }
        public override bool canBeGivenAsGift()
        {
            return false;
        }
        public override bool canBeTrashed()
        {
            return false;
        }
        public override string getDescription()
        {
            return "Contains blank pages\nready to be filled\nwith words and images.";
        }
        public override string getCategoryName()
        {
            return "Custom Book";
        }
        public override Color getCategoryColor()
        {
            return new Color(0, 100, 200);
        }
        public override Item getOne()
        {
            return new Book(this.Id);
        }
        public override int getStack()
        {
            return 1;
        }
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            float halfTilesize = Game1.tileSize / 2;
            spriteBatch.Draw(Game1.shadowTexture, location + new Vector2(halfTilesize, (Game1.tileSize * 3 / 4)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * 0.5f, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 3f, SpriteEffects.None, layerDepth - 0.0001f);
            spriteBatch.Draw(Icon, location + new Vector2(((int)(halfTilesize * scaleSize)), ((int)(halfTilesize * scaleSize))), new Rectangle(0, 0, 16, 16), Color.White * transparency, 0f, new Vector2(8f, 8f) * scaleSize, Game1.pixelZoom * scaleSize, SpriteEffects.None, layerDepth);
            spriteBatch.Draw(Icon, location + new Vector2(((int)(halfTilesize * scaleSize)), ((int)(halfTilesize * scaleSize))), new Rectangle(0, 16, 16, 16), CustomBooksMod.Shelf.Books[this.Id].Color * transparency, 0f, new Vector2(8f, 8f) * scaleSize, Game1.pixelZoom * scaleSize, SpriteEffects.None, layerDepth);
            /**/
        }
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            var vector= Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize + Game1.tileSize / 2 + ((this.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0)), (float)(y * Game1.tileSize + Game1.tileSize / 2 + ((this.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0))));
            spriteBatch.Draw(Icon, vector, new Rectangle(0, 0, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0);
            spriteBatch.Draw(Icon, vector, new Rectangle(0, 16, 16, 16), CustomBooksMod.Shelf.Books[this.Id].Color, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0);
        }
        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, StardewValley.Farmer f)
        {
            objectPosition.Y += Game1.tileSize;
            spriteBatch.Draw(Icon, objectPosition, new Rectangle(0, 0, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (f.getStandingY() + 2) / 10000f));
            spriteBatch.Draw(Icon, objectPosition, new Rectangle(0,16,16,16), CustomBooksMod.Shelf.Books[this.Id].Color, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (f.getStandingY() + 3) / 10000f));
        }
    }
}
