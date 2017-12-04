using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using StardewValley.Locations;
using SFarmer = StardewValley.Farmer;

using xTile.ObjectModel;
using xTile.Tiles;
using Location = xTile.Dimensions.Location;
using xTileRectangle = xTile.Dimensions.Rectangle;

namespace Entoarox.DynamicDungeons
{
    class DynamicDungeon : GameLocation
    {
        private static Random _Random = new Random();

        private int _Seed;
        private Texture2D _Minimap;
        private int _Time;

        public int Floor;
        public Point EntryPoint;
        private double _Difficulty;
        public double Difficulty
        {
            get => _Difficulty;
            set => _Difficulty = Math.Max(0, Math.Min(10, value));
        }
        public List<ResourceClump> ResourceClumps = new List<ResourceClump>();
        public DynamicDungeon(double difficulty=0, int? seed = null)
        {
            this.name = "DynamicDungeon";
            this._Seed = seed ?? _Random.Next();
            this.Difficulty = difficulty;
            var builder = new DungeonBuilder(difficulty, 1);
            this.Floor = 1;
            this._Minimap = builder.GetMiniMap();
            this.map = builder.GetMap();
            this.EntryPoint = builder.GetFloorPoint();
            this.forceViewportPlayerFollow = true;
            // Embed StardewValley.GameLocation.ctor()
            this.waterTiles = new bool[this.map.Layers[0].LayerWidth, this.map.Layers[0].LayerHeight];
            bool flag = false;
            for (int i = 0; i < this.map.Layers[0].LayerWidth; i++)
            {
                for (int j = 0; j < this.map.Layers[0].LayerHeight; j++)
                {
                    if (this.doesTileHaveProperty(i, j, "Water", "Back") != null)
                    {
                        flag = true;
                        this.waterTiles[i, j] = true;
                    }
                }
            }
            if (!flag)
                this.waterTiles = null;
        }
        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {
            foreach (NPC current in this.characters)
            {
                if (current is Monster)
                {
                    (current as Monster).drawAboveAllLayers(b);
                }
            }
            base.drawAboveAlwaysFrontLayer(b);
            string floor = this.Floor.ToString();
            Vector2 size = Game1.smallFont.MeasureString(floor);
            size.X += 26;
            if (size.X > 150)
            {
                floor = "?????";
                size = Game1.smallFont.MeasureString(floor);
                size.X += 26;
            }
            // Background boxes
            IClickableMenu.drawTextureBox(Game1.spriteBatch, 158, 4, 125, 40, Color.White);
            IClickableMenu.drawTextureBox(Game1.spriteBatch, 158, 48, (int)Math.Ceiling(size.X), 52, Color.White);
            IClickableMenu.drawTextureBox(Game1.spriteBatch, 4, 4, 150, 150, Color.White);
            // Minimap image
            Game1.spriteBatch.Draw(this._Minimap, new Vector2(20, 20), Color.White);
            // Player dot
            Point p = Game1.player.getTileLocationPoint();
            Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 19, p.Y + 19, 1, 1), Color.Red * 0.33f);
            Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 19, p.Y + 21, 1, 1), Color.Red * 0.33f);
            Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 21, p.Y + 19, 1, 1), Color.Red * 0.33f);
            Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 21, p.Y + 21, 1, 1), Color.Red * 0.33f);
            Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 20, p.Y + 19, 1, 1), Color.Red * 0.66f);
            Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 19, p.Y + 20, 1, 1), Color.Red * 0.66f);
            Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 21, p.Y + 20, 1, 1), Color.Red * 0.66f);
            Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 20, p.Y + 21, 1, 1), Color.Red * 0.66f);
            Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(p.X + 20, p.Y + 20, 1, 1), Color.Red * 0.99f);
            // Depth level
            Utility.drawTextWithShadow(Game1.spriteBatch, this.Floor.ToString(), Game1.smallFont, new Vector2(158 + 12, 48 + 12), Game1.textColor);
            if (this.Difficulty >= 1)
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13, 17, 9, 14), Color.Green);
            if (this.Difficulty >= 2)
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 10, 17, 9, 14), Color.Green);
            if (this.Difficulty >= 3)
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 20, 17, 9, 14), Color.Green);
            if (this.Difficulty >= 4)
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 30, 17, 9, 14), Color.Green);
            if (this.Difficulty >= 5)
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 40, 17, 9, 14), Color.DarkOrange);
            if (this.Difficulty >= 6)
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 50, 17, 9, 14), Color.DarkOrange);
            if (this.Difficulty >= 7)
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 60, 17, 9, 14), Color.DarkOrange);
            if (this.Difficulty >= 8)
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 70, 17, 9, 14), Color.Red);
            if (this.Difficulty >= 9)
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 80, 17, 9, 14), Color.Red);
            if (this.Difficulty == 10)
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(158 + 13 + 90, 17, 9, 14), Color.DarkRed);
        }
        public override void resetForPlayerEntry()
        {
            this._Time = Game1.timeOfDay;
            base.resetForPlayerEntry();
            this.forceViewportPlayerFollow = true;
        }
        public override void checkForMusic(GameTime time)
        {
            if(Game1.currentSong==null || !Game1.currentSong.IsPlaying)
                Game1.changeMusicTrack("Upper_Ambient");
        }
        public override void UpdateWhenCurrentLocation(GameTime time)
        {
            Game1.timeOfDay = this._Time;
            foreach (ResourceClump current in this.ResourceClumps)
                current.tickUpdate(time, current.tile);
            base.UpdateWhenCurrentLocation(time);
        }
        public override StardewValley.Object getFish(float millisecondsAfterNibble, int bait, int waterDepth, SFarmer who, double baitPotency)
        {
            return LootHandler.LootTables["Fishing"].GetDrop(this._Seed, (this.Difficulty + baitPotency) / (30 - waterDepth));
        }
        public override string checkForBuriedItem(int xLocation, int yLocation, bool explosion, bool detectOnly)
        {
            if(Game1.random.NextDouble() < 0.15)
            Game1.createItemDebris(LootHandler.LootTables["Digging"].GetDrop(this._Seed, (this.Difficulty) / (30)), new Vector2(xLocation, yLocation), 1);
            return "";
        }
        public override void monsterDrop(Monster monster, int x, int y)
        {
            //TODO: Consider implementing custom monster loot?
            base.monsterDrop(monster, x, y);
        }
        public override void draw(SpriteBatch b)
        {
            foreach (ResourceClump current in this.ResourceClumps)
                current.draw(b, current.tile);
            base.draw(b);
        }
        public override bool isCollidingPosition(XnaRectangle position, xTileRectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character)
        {
            foreach (ResourceClump current in this.ResourceClumps)
                if (!glider && current.getBoundingBox(current.tile).Intersects(position))
                    return true;
            return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character);
        }
        public override bool isTileOccupied(Vector2 tileLocation, string characterToIgnore = "")
        {
            foreach (var current in this.ResourceClumps)
                if (current.occupiesTile((int)tileLocation.X, (int)tileLocation.Y))
                    return true;
            return this.EntryPoint.Equals(new Point((int)tileLocation.X,(int)tileLocation.Y)) || base.isTileOccupied(tileLocation, characterToIgnore);
        }

        public override bool checkAction(Location tileLocation, xTileRectangle viewport, SFarmer who)
        {
            Tile tile = Game1.currentLocation.map.GetLayer("Buildings").Tiles[tileLocation.X, tileLocation.Y];
            if (tile != null && tile.Properties.TryGetValue("Action", out var propertyValue))
            {
                string[] split = ((string)propertyValue).Split(' ');
                string[] args = new string[split.Length - 1];
                Array.Copy(split, 1, args, 0, args.Length);
                string action = split[0];
                if (action.Equals("DDLoot")) // DDLoot <string:table> <int:dropcount> [bool:deleteTile] [<float:enemySpawnChance> <string:spawnedEnemy>]
                {
                    Game1.drawObjectDialogue($"TODO: Trigger the \"{args[0]}\" loot table and drop ({args[1]}) items");
                    return true;
                }
                else if (action.Equals("DDShop")) // DDShop <vendorID> [Currently only DwarfVendor]
                {
                    Game1.drawObjectDialogue($"TODO: Open the \"{args[0]}\" shop menu here");
                    return true;
                }
                else if (action.Equals("DDLooted")) // DDLooted
                {
                    Game1.drawObjectDialogue("TODO: Localize `You search thoroughly but cant find anything of value.`");
                    return true;
                }
            }
            return false;
        }
    }
}
