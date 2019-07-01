using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;
using xTile.ObjectModel;
using SObject = StardewValley.Object;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using xTileRectangle = xTile.Dimensions.Rectangle;

namespace Entoarox.DynamicDungeons
{
    internal class DynamicDungeon : GameLocation
    {
        /*********
        ** Properties
        *********/
        private double _Difficulty;
        private readonly Texture2D Minimap;
        private static readonly Random Random = new Random();

        private readonly int Seed;
        private int Time;


        /*********
        ** Accessors
        *********/
        public int Floor;
        public Point EntryPoint;
        public bool DrawInfo = false;

        public double Difficulty
        {
            get => this._Difficulty;
            set => this._Difficulty = Math.Max(0, Math.Min(10, value));
        }

        public List<ResourceClump> ResourceClumps = new List<ResourceClump>();


        /*********
        ** Public methods
        *********/
        public DynamicDungeon(double difficulty = 0, int? seed = null)
        {
#pragma warning disable AvoidNetField // Avoid Netcode types when possible
            this.name.Value = "DynamicDungeon";
#pragma warning restore AvoidNetField // Avoid Netcode types when possible
            this.Seed = seed ?? DynamicDungeon.Random.Next();
            this.Difficulty = difficulty;
            DungeonBuilder builder = new DungeonBuilder(difficulty, 1);
            this.Floor = 1;
            this.Minimap = builder.GetMiniMap();
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
            ModEntry.SMonitor.Log("DynamicDungeon#drawAboveAlwaysFrontLayer", StardewModdingAPI.LogLevel.Trace);
            if (this.DrawInfo)
                this.Map.GetLayer("MapInfo").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, Game1.pixelZoom);
            foreach (NPC current in this.characters)
            {
                if (current is Monster monster)
                    monster.drawAboveAllLayers(b);
            }

            base.drawAboveAlwaysFrontLayer(b);
            // Floor
            string floor = this.Floor.ToString();
            Vector2 size = Game1.smallFont.MeasureString(floor);
            size.X += 26;
            if (size.X > 150)
            {
                floor = "?????";
                size = Game1.smallFont.MeasureString(floor);
                size.X += 26;
            }

            IClickableMenu.drawTextureBox(Game1.spriteBatch, 158, 48, (int)Math.Ceiling(size.X), 52, Color.White);
            Utility.drawTextWithShadow(Game1.spriteBatch, floor, Game1.smallFont, new Vector2(158 + 12, 48 + 12), Game1.textColor);
            // Difficulty
            IClickableMenu.drawTextureBox(Game1.spriteBatch, 158, 4, 125, 40, Color.White);
            if (this.Difficulty >= 1)
                Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(158 + 13, 17, 9, 14), Color.Green);
            if (this.Difficulty >= 2)
                Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(158 + 13 + 10, 17, 9, 14), Color.Green);
            if (this.Difficulty >= 3)
                Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(158 + 13 + 20, 17, 9, 14), Color.Green);
            if (this.Difficulty >= 4)
                Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(158 + 13 + 30, 17, 9, 14), Color.Green);
            if (this.Difficulty >= 5)
                Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(158 + 13 + 40, 17, 9, 14), Color.DarkOrange);
            if (this.Difficulty >= 6)
                Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(158 + 13 + 50, 17, 9, 14), Color.DarkOrange);
            if (this.Difficulty >= 7)
                Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(158 + 13 + 60, 17, 9, 14), Color.DarkOrange);
            if (this.Difficulty >= 8)
                Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(158 + 13 + 70, 17, 9, 14), Color.Red);
            if (this.Difficulty >= 9)
                Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(158 + 13 + 80, 17, 9, 14), Color.Red);
            if (this.Difficulty == 10)
                Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(158 + 13 + 90, 17, 9, 14), Color.DarkRed);
            // Minimap
            IClickableMenu.drawTextureBox(Game1.spriteBatch, 4, 4, 150, 150, Color.White);
            Game1.spriteBatch.Draw(this.Minimap, new Vector2(20, 20), Color.White);
            Point p = Game1.player.getTileLocationPoint();
            Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(p.X + 19, p.Y + 19, 1, 1), Color.Red * 0.33f);
            Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(p.X + 19, p.Y + 21, 1, 1), Color.Red * 0.33f);
            Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(p.X + 21, p.Y + 19, 1, 1), Color.Red * 0.33f);
            Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(p.X + 21, p.Y + 21, 1, 1), Color.Red * 0.33f);
            Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(p.X + 20, p.Y + 19, 1, 1), Color.Red * 0.66f);
            Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(p.X + 19, p.Y + 20, 1, 1), Color.Red * 0.66f);
            Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(p.X + 21, p.Y + 20, 1, 1), Color.Red * 0.66f);
            Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(p.X + 20, p.Y + 21, 1, 1), Color.Red * 0.66f);
            Game1.spriteBatch.Draw(Game1.staminaRect, new XnaRectangle(p.X + 20, p.Y + 20, 1, 1), Color.Red * 0.99f);
        }

        public override void checkForMusic(GameTime time)
        {
            ModEntry.SMonitor.Log("DynamicDungeon#checkForMusic", StardewModdingAPI.LogLevel.Trace);
            if (Game1.currentSong == null || !Game1.currentSong.IsPlaying)
                Game1.changeMusicTrack("Upper_Ambient");
        }

        public override void UpdateWhenCurrentLocation(GameTime time)
        {
            ModEntry.SMonitor.Log("DynamicDungeon#UpdateWhenCurrentLocation", StardewModdingAPI.LogLevel.Trace);
            Game1.timeOfDay = this.Time;
            foreach (ResourceClump current in this.ResourceClumps)
                current.tickUpdate(time, current.tile.Value, this);
            base.UpdateWhenCurrentLocation(time);
        }

        public override SObject getFish(float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency)
        {
            return LootHandler.LootTables["Fishing"].GetDrop(this.Seed, (this.Difficulty + baitPotency) / (30 - waterDepth));
        }

        public override string checkForBuriedItem(int xLocation, int yLocation, bool explosion, bool detectOnly)
        {
            if (Game1.random.NextDouble() < 0.15)
                Game1.createItemDebris(LootHandler.LootTables["Digging"].GetDrop(this.Seed, this.Difficulty / 30), new Vector2(xLocation, yLocation), 1);
            return "";
        }

        public override void monsterDrop(Monster monster, int x, int y)
        {
            //TODO: Consider implementing custom monster loot?
            base.monsterDrop(monster, x, y);
        }

        public override void draw(SpriteBatch b)
        {
            ModEntry.SMonitor.Log("DynamicDungeon#draw", StardewModdingAPI.LogLevel.Trace);
            foreach (ResourceClump current in this.ResourceClumps)
                current.draw(b, current.tile.Value);
            base.draw(b);
        }

        public override bool isCollidingPosition(XnaRectangle position, xTileRectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character)
        {
            foreach (ResourceClump current in this.ResourceClumps)
                if (!glider && current.getBoundingBox(current.tile.Value).Intersects(position))
                    return true;
            return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character);
        }

        public override bool isTileOccupied(Vector2 tileLocation, string characterToIgnore = "")
        {
            foreach (ResourceClump current in this.ResourceClumps)
                if (current.occupiesTile((int)tileLocation.X, (int)tileLocation.Y))
                    return true;
            return this.EntryPoint.Equals(new Point((int)tileLocation.X, (int)tileLocation.Y)) || base.isTileOccupied(tileLocation, characterToIgnore);
        }

        public new bool performAction(string propertyValue, Farmer who, Location tileLocation)
        {
            string[] split = propertyValue.Split(' ');
            string[] args = new string[split.Length - 1];
            Array.Copy(split, 1, args, 0, args.Length);
            string action = string.IsInterned(split[0]) ?? split[0];

            switch (action)
            {
                case "DDLoot": // DDLoot <string:table> <int:dropcount> [bool:deleteTile] [<float:enemySpawnChance> <string:spawnedEnemy>]
                    Game1.drawObjectDialogue($"TODO: Trigger the \"{args[0]}\" loot table and drop ({args[1]}) items");
                    return true;

                case "DDShop": // DDShop <vendorID> [Currently only DwarfVendor]
                    Game1.drawObjectDialogue($"TODO: Open the \"{args[0]}\" shop menu here");
                    return true;

                case "DDLooted": // DDLooted
                    Game1.drawObjectDialogue(ModEntry.SHelper.Translation.Get("AlreadyLooted"));
                    return true;

                case "DDExit":
                    this.createQuestionDialogue(ModEntry.SHelper.Translation.Get("LeaveDungeon"), new[]
                    {
                        new Response("yes", ModEntry.SHelper.Translation.Get("LeaveDungeon_Yes")),
                        new Response("no", ModEntry.SHelper.Translation.Get("LeaveDungeon_No"))
                    }, this.ExitResolver);
                    return false;

                default:
                    return false;
            }
        }

        public override bool checkAction(Location tileLocation, xTileRectangle viewport, Farmer who)
        {
            Vector2 vector = new Vector2(tileLocation.X, tileLocation.Y);
            xTile.Tiles.Tile tile = this.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * Game1.tileSize, tileLocation.Y * Game1.tileSize), viewport.Size) ?? this.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * Game1.tileSize, (tileLocation.Y + 1) * Game1.tileSize), viewport.Size);
            if (tile != null && tile.Properties.TryGetValue("Action", out PropertyValue propertyValue) && propertyValue != null)
                return (this.currentEvent != null || this.isCharacterAtTile(vector + new Vector2(0f, 1f)) == null) && this.performAction(propertyValue, who, tileLocation);
            return base.checkAction(tileLocation, viewport, who);
        }


        /*********
        ** Protected methods
        *********/
        /*
        protected override void resetLocalState()
        {
            ModEntry.SMonitor.Log("DynamicDungeon#resetLocalState", StardewModdingAPI.LogLevel.Trace);
            this.Time = Game1.timeOfDay;
            base.resetLocalState();
            this.resetForPlayerEntry();
            this.forceViewportPlayerFollow = true;
        }
        */

        private void ExitResolver(Farmer player, string answer)
        {
            answer = string.IsInterned(answer) ?? answer;
            if (answer == "yes")
                Game1.warpFarmer("DynamicDungeonEntrance", 5, 7, false);
        }
    }
}
