using Entoarox.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.CustomPaths
{
    internal class CustomPathObject : Object, ICustomItem, IDeserializationHandler
    {
        /*********
        ** Accessors
        *********/
        public string Id;

        public override string DisplayName
        {
            get => CustomPathsMod.Map[this.Id].Name;
            set => base.DisplayName = value;
        }


        /*********
        ** Public methods
        *********/
        public CustomPathObject() { }

        public CustomPathObject(string id)
        {
            this.Id = id;
            this.name = "Custom Path";
            this.Price = CustomPathsMod.Map[this.Id].Price * 2;
            this.CanBeSetDown = true;
        }

        public bool ShouldDelete()
        {
            return !CustomPathsMod.Map.ContainsKey(this.Id);
        }

        public override string getCategoryName()
        {
            return "Path";
        }

        public override string getDescription()
        {
            if (CustomPathsMod.Map[this.Id].Speed > 0)
                return "+" + CustomPathsMod.Map[this.Id].Speed + " speed when walking on this path.";
            return "Decorative path for you to populate stardew with.";
        }

        public override Color getCategoryColor()
        {
            return new Color(148, 61, 40);
        }

        public override int maximumStackSize()
        {
            return 999;
        }

        public override bool canBeGivenAsGift()
        {
            return false;
        }

        public override bool canBeDropped()
        {
            return false;
        }

        public override bool canBeTrashed()
        {
            return true;
        }

        public override int salePrice()
        {
            return CustomPathsMod.Map[this.Id].Price;
        }

        public override Item getOne()
        {
            return new CustomPathObject(this.Id);
        }

        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            return !l.terrainFeatures.ContainsKey(tile) && !l.isTileOccupiedForPlacement(tile);
        }

        public override bool isPlaceable()
        {
            return true;
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            Vector2 vector = new Vector2(x / Game1.tileSize, y / Game1.tileSize);
            if (location.terrainFeatures.ContainsKey(vector))
                return false;
            CustomPath path = new CustomPath(this.Id);
            location.terrainFeatures.Add(vector, path);
            path.MakeConnection(vector);
            path.UpdateNeighbors(vector, false);
            Game1.playSound("thudStep");
            return true;
        }

        public override bool isPassable()
        {
            return true;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            Texture2D texture = CustomPathsMod.Map[this.Id].GetTexture();
            spriteBatch.Draw(texture, location + new Vector2((int)(Game1.tileSize / 2 * scaleSize), (int)(Game1.tileSize / 2 * scaleSize)), new Rectangle(0, 0, 16, 16), Color.White * transparency, 0f, new Vector2(8f, 8f) * scaleSize, Game1.pixelZoom * scaleSize, SpriteEffects.None, layerDepth);
            if (drawStackNumber && this.maximumStackSize() > 1 && scaleSize > 0.3 && this.Stack != 2147483647 && this.Stack > 1)
                Utility.drawTinyDigits(this.Stack, spriteBatch, location + new Vector2((Game1.tileSize - Utility.getWidthOfTinyDigitString(this.Stack, 3f * scaleSize)) + 3f * scaleSize, Game1.tileSize - 18f * scaleSize + 2f), 3f * scaleSize, 1f, Color.White);
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            Texture2D texture = CustomPathsMod.Map[this.Id].GetTexture();
            Vector2 vector = Game1.GlobalToLocal(Game1.viewport, new Vector2((x * Game1.tileSize + Game1.tileSize / 2 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (y * Game1.tileSize + Game1.tileSize / 2 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))));
            spriteBatch.Draw(texture, vector, new Rectangle(0, 0, 16, 16), Color.White * alpha, 0f, new Vector2(8f, 8f), Game1.pixelZoom, SpriteEffects.None, 0);
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            Texture2D texture = CustomPathsMod.Map[this.Id].GetTexture();
            spriteBatch.Draw(texture, objectPosition, new Rectangle(0, 0, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1);
        }
    }
}
