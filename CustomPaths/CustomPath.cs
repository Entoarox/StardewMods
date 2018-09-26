using System;
using System.Collections.Generic;
using Entoarox.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace Entoarox.CustomPaths
{
    internal class CustomPath : TerrainFeature, ICustomItem, IDeserializationHandler
    {
        /*********
        ** Fields
        *********/
        private static readonly Dictionary<byte, int> DrawGuide = new Dictionary<byte, int>
        {
            [0] = 0,
            [2] = 1,
            [8] = 2,
            [10] = 3,
            [11] = 4,
            [16] = 5,
            [18] = 6,
            [22] = 7,
            [24] = 8,
            [26] = 9,
            [27] = 10,
            [30] = 11,
            [31] = 12,
            [64] = 13,
            [66] = 14,
            [72] = 15,
            [74] = 16,
            [75] = 17,
            [80] = 18,
            [82] = 19,
            [86] = 20,
            [88] = 21,
            [90] = 22,
            [91] = 23,
            [94] = 24,
            [95] = 25,
            [104] = 26,
            [106] = 27,
            [107] = 28,
            [120] = 29,
            [122] = 30,
            [123] = 31,
            [126] = 32,
            [127] = 33,
            [208] = 34,
            [210] = 35,
            [214] = 36,
            [216] = 37,
            [218] = 38,
            [219] = 39,
            [222] = 40,
            [223] = 41,
            [248] = 42,
            [250] = 43,
            [251] = 44,
            [254] = 45,
            [255] = 46
        };
        private int Connection;
        private Action<SpriteBatch, Vector2, Vector2, float, float> DrawHandler;
        private Texture2D Texture;


        /*********
        ** Accessors
        *********/
        public string Id;


        /*********
        ** Public methods
        *********/
        public CustomPath()
            : base(needsTick: false)
        {
            this.DrawHandler = this.DrawHandlerSetup;
        }

        public CustomPath(string id)
            : this()
        {
            this.Id = id;
        }

        public bool ShouldDelete()
        {
            return !CustomPathsMod.Map.ContainsKey(this.Id);
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return new Rectangle((int)(tileLocation.X * Game1.tileSize), (int)(tileLocation.Y * Game1.tileSize), Game1.tileSize, Game1.tileSize);
        }

        public override bool isPassable(Character c = null)
        {
            return true;
        }

        public override bool performToolAction(Tool t, int damage, Vector2 tileLocation, GameLocation location = null)
        {
            if (location == null) location = Game1.currentLocation;
            if ((t != null || damage > 0) && (damage > 0 || t.GetType() == typeof(Pickaxe) || t.GetType() == typeof(Axe)))
            {
                Game1.createRadialDebris(location, 14, (int)tileLocation.X, (int)tileLocation.Y, 4, false, -1, false, -1);
                Game1.playSound("hammer");
                location.debris.Add(new Debris(new CustomPathObject(this.Id), tileLocation * Game1.tileSize + new Vector2(Game1.tileSize / 2, Game1.tileSize / 2)));
                this.UpdateNeighbors(tileLocation);
                return true;
            }

            return false;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 positionOnScreen, Vector2 tileLocation, float scale, float layerDepth)
        {
            this.DrawHandler(spriteBatch, positionOnScreen, tileLocation, scale, layerDepth);
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            this.DrawHandler(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * Game1.tileSize, tileLocation.Y * Game1.tileSize)), tileLocation, 1, 1E-09f);
        }

        public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
        {
        }

        public override bool seasonUpdate(bool onLoad)
        {
            this.Texture = CustomPathsMod.Map[this.Id].GetTexture();
            return false;
        }

        internal void MakeConnection(Vector2 position, Vector2? ignore = null)
        {
            this.Connection = this.GetTileIndex(position, ignore);
            this.Texture = CustomPathsMod.Map[this.Id].GetTexture();
            this.DrawHandler = this.DrawHandlerReal;
        }

        internal void UpdateNeighbors(Vector2 position, bool ignore = true)
        {
            for (int y = -1; y < 2; y++)
                for (int x = -1; x < 2; x++)
                {
                    if (x == 0 && y == 0)
                        continue;
                    Vector2 vector = new Vector2(position.X + x, position.Y + y);
                    if (Game1.currentLocation.terrainFeatures.TryGetValue(vector, out TerrainFeature obj) && obj is CustomPath path && path.Id.Equals(this.Id))
                        if (ignore)
                            path.MakeConnection(vector, position);
                        else
                            path.MakeConnection(vector);
                }
        }


        /*********
        ** Protected methods
        *********/
        private void DrawHandlerSetup(SpriteBatch spriteBatch, Vector2 positionOnScreen, Vector2 tileLocation, float scale, float layerDepth)
        {
            this.MakeConnection(tileLocation);
            this.DrawHandlerReal(spriteBatch, positionOnScreen, tileLocation, scale, layerDepth);
        }

        private void DrawHandlerReal(SpriteBatch spriteBatch, Vector2 positionOnScreen, Vector2 tileLocation, float scale, float layerDepth)
        {
            spriteBatch.Draw(this.Texture, positionOnScreen, Game1.getSourceRectForStandardTileSheet(this.Texture, this.Connection, 16, 16), Color.White, 0f, Vector2.Zero, scale * Game1.pixelZoom, SpriteEffects.None, layerDepth == 1E-09f ? layerDepth : layerDepth + positionOnScreen.Y / 20000f);
        }

        private bool HasPath(float x, float y, Vector2? ignore)
        {
            return Game1.currentLocation.terrainFeatures.TryGetValue(new Vector2(x, y), out TerrainFeature obj) && obj is CustomPath path && path.Id.Equals(this.Id) && !(ignore is Vector2 skip && x == skip.X && y == skip.Y);
        }

        private int GetTileIndex(Vector2 position, Vector2? ignore = null)
        {
            byte tiles = 0;
            byte mult = 0;
            for (int y = -1; y < 2; y++)
                for (int x = -1; x < 2; x++)
                {
                    if (x == 0 && y == 0)
                        continue;
                    if (mult == 0)
                        mult = 1;
                    else
                        mult *= 2;
                    if (!this.HasPath(position.X + x, position.Y + y, ignore))
                        continue;
                    if (Math.Abs(x) == 1 && Math.Abs(y) == 1)
                    {
                        bool first, second;
                        if (x == -1)
                            first = this.HasPath(position.X + x + 1, position.Y + y, ignore);
                        else
                            first = this.HasPath(position.X + x - 1, position.Y + y, ignore);
                        if (y == -1)
                            second = this.HasPath(position.X + x, position.Y + y + 1, ignore);
                        else
                            second = this.HasPath(position.X + x, position.Y + y - 1, ignore);
                        if (first && second)
                            tiles += mult;
                    }
                    else
                        tiles += mult;
                }

            int index = CustomPath.DrawGuide.ContainsKey(tiles) ? CustomPath.DrawGuide[tiles] : 0;
            if (index == 46 && CustomPathsMod.Map[this.Id].Alternates > 0)
                index += new Random((int)(position.X * position.Y)).Next(0, CustomPathsMod.Map[this.Id].Alternates + 1);
            return index;
        }
    }
}
