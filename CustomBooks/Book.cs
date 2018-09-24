using System;
using Entoarox.CustomBooks.Menu;
using Entoarox.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using SObject = StardewValley.Object;

namespace Entoarox.CustomBooks
{
    internal class Book : SObject, ICustomItem
    {
        /*********
        ** Fields
        *********/
        private bool Broken => !ModEntry.Shelf.Books.ContainsKey(this.Id);
        private Color Color => this.Broken ? new Color(139, 69, 19) : ModEntry.Shelf.Books[this.Id].Color;
        private static readonly Color CategoryColor = new Color(0, 100, 200);
        private static Texture2D Icon;


        /*********
        ** Accessors
        *********/
        public string Id;

        public override string DisplayName
        {
            get => this.Broken ? "Destroyed Book" : this.name;
            set => this.displayName = value;
        }


        /*********
        ** Public methods
        *********/
        public Book()
        {
            if (Book.Icon == null)
                Book.Icon = ModEntry.SHelper.Content.Load<Texture2D>("assets/icon.png");
        }

        public Book(string id)
        {
            if (Book.Icon == null)
                Book.Icon = ModEntry.SHelper.Content.Load<Texture2D>("assets/icon.png");
            this.Id = id;
            this.name = ModEntry.Shelf.Books[id].Name;
            this.bigCraftable.Value = true;
        }

        public void Activate()
        {
            if (this.Broken)
                Game1.drawObjectDialogue("The book has been damaged beyond repair...");
            else
                Game1.activeClickableMenu = new BookMenu(this.Id);
        }

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
            return this.Broken;
        }

        public override string getDescription()
        {
            return this.Broken ? "The damaged remains of a book, pure trash now..." : "Contains blank pages\nready to be filled\nwith words and images.";
        }

        public override string getCategoryName()
        {
            return this.Broken ? "Junk" : "Custom Book";
        }

        public override Color getCategoryColor()
        {
            return this.Broken ? Color.DarkGray : Book.CategoryColor;
        }

        public override Item getOne()
        {
            return new Book(this.Id);
        }

        public override int getStack()
        {
            return 1;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            float halfTilesize = Game1.tileSize / 2;
            spriteBatch.Draw(Game1.shadowTexture, location + new Vector2(halfTilesize, Game1.tileSize * 3 / 4), Game1.shadowTexture.Bounds, Color.White * 0.5f, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 3f, SpriteEffects.None, layerDepth - 0.0001f);
            spriteBatch.Draw(Book.Icon, location + new Vector2((int)(halfTilesize * scaleSize), (int)(halfTilesize * scaleSize)), new Rectangle(0, 0, 16, 16), Color.White * transparency, 0f, new Vector2(8f, 8f) * scaleSize, Game1.pixelZoom * scaleSize, SpriteEffects.None, layerDepth);
            spriteBatch.Draw(Book.Icon, location + new Vector2((int)(halfTilesize * scaleSize), (int)(halfTilesize * scaleSize)), new Rectangle(0, 16, 16, 16), this.Color * transparency, 0f, new Vector2(8f, 8f) * scaleSize, Game1.pixelZoom * scaleSize, SpriteEffects.None, layerDepth);
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            Vector2 vector = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize + Game1.tileSize / 2 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), y * Game1.tileSize + Game1.tileSize / 2 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)));
            spriteBatch.Draw(Book.Icon, vector, new Rectangle(0, 0, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0);
            spriteBatch.Draw(Book.Icon, vector, new Rectangle(0, 16, 16, 16), this.Color, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0);
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            objectPosition.Y += Game1.tileSize;
            spriteBatch.Draw(Book.Icon, objectPosition, new Rectangle(0, 0, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (f.getStandingY() + 2) / 10000f));
            spriteBatch.Draw(Book.Icon, objectPosition, new Rectangle(0, 16, 16, 16), this.Color, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (f.getStandingY() + 3) / 10000f));
        }
    }
}
