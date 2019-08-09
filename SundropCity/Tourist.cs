using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using StardewValley;

namespace SundropCity
{
    internal class Tourist : NPC
    {
        public const int TOURIST_LINE_COUNT = 60;
        public const int WALLY_LINE_COUNT = 16;

        public const int TILE_SPAWN = 0;
        public const int TILE_BLOCK = 1;
        public const int TILE_KEEPMOVING = 2;
        public const int TILE_BROWSE = 4;
        public const int TILE_ARROW_DOWN = 5;
        public const int TILE_ARROW_RIGHT = 6;
        public const int TILE_ARROW_UP = 7;
        public const int TILE_ARROW_LEFT = 8;
        public const int TILE_WARP_DOWN = 10;
        public const int TILE_WARP_RIGHT = 11;
        public const int TILE_WARP_UP = 12;
        public const int TILE_WARP_LEFT = 13;

        protected AnimatedSprite Base;
        protected AnimatedSprite Makeup;
        protected AnimatedSprite Hat;
        protected AnimatedSprite Hair;
        protected AnimatedSprite Shirt;
        protected AnimatedSprite Pants;
        protected AnimatedSprite Shoes;
        protected AnimatedSprite Accessory;
        protected AnimatedSprite FaceHair;

        protected Color HairColor;
        protected Color PantsColor;
        protected Color ShoeColor;

        protected byte Timer;
        protected byte Delay;
        protected byte Cooldown;
        protected byte Stuck;
        internal bool IsWally;
        protected Vector2 OldPos;

        internal static Tourist Wally;
        internal static int WallyAge;

        internal static Dictionary<string, List<Vector2>> WarpCache = new Dictionary<string, List<Vector2>>();
        internal static List<string> BaseColors = new List<string>();
        internal static List<string> MaleHair = new List<string>();
        internal static List<string> FemaleHair = new List<string>();
        internal static List<string> MaleShirt = new List<string>();
        internal static List<string> FemaleShirt = new List<string>();
        internal static List<string> MalePants = new List<string>();
        internal static List<string> FemalePants = new List<string>();
        internal static List<string> MaleShoes = new List<string>();
        internal static List<string> FemaleShoes = new List<string>();
        internal static List<string> MaleAccessory = new List<string>();
        internal static List<string> FemaleAccessory = new List<string>();
        internal static List<string> MaleHat = new List<string>();
        internal static List<string> FemaleHat = new List<string>();
        internal static List<string> FaceHairs = new List<string>();

        internal static Color[] HairColors =
        {
            new Color(246, 255, 94),
            new Color(215, 209, 219),
            new Color(193, 197, 219),
            new Color(169, 170, 161),
            new Color(224, 198, 243),
            new Color(200, 195, 143),
            new Color(219, 192, 127),
            new Color(198, 198, 90),
            new Color(146, 86, 26),
            new Color(54, 37, 109),
            new Color(127, 134, 158),
            new Color(60, 52, 85),
            new Color(77, 40, 103),
            new Color(79, 40, 40),
            new Color(255, 64, 0),
            new Color(231, 137, 21),
            new Color(194, 70, 2),
            new Color(146, 22, 16),
            new Color(42, 45, 249),
            new Color(42, 249, 108),
            new Color(255, 0, 0),
            new Color(255, 103, 0),
            new Color(190, 4, 255),
            new Color(255, 0, 219),
        };

        internal static Color[] ClothingColors =
        {
            new Color(255, 134, 73),
            new Color(255, 73, 94),
            new Color(353, 88, 78),
            new Color(240, 197, 0),
            new Color(199, 240, 0),
            new Color(114, 204, 47),
            new Color(67, 204, 130),
            new Color(67, 203, 204),
            new Color(67, 134, 204),
            new Color(45, 60, 153),
            new Color(89, 45, 153),
            new Color(165, 16, 164),
            new Color(84, 133, 140),
            new Color(114, 147, 152),
            new Color(72, 115, 49),
            new Color(122, 170, 97),
            new Color(244, 255, 73),
            new Color(182, 130, 64),
            new Color(115, 50, 22),
            new Color(97, 60, 43),
            new Color(255, 66, 45),
            new Color(182, 24, 24),
            new Color(50, 12, 61),
            new Color(43, 26, 85),
        };

        internal static string FemaleMakeup = SundropCityMod.SHelper.Content.GetActualAssetKey("assets/Characters/Tourists/lashes.png");
        internal static string WallyTexture = SundropCityMod.SHelper.Content.GetActualAssetKey("assets/Characters/Tourists/wally.png");
        internal static AnimatedSprite BlankSprite = new AnimatedSprite();

        internal static List<Vector2> GetSpawnPoints(GameLocation location, HashSet<int> validTiles)
        {
            var layer = location.map.GetLayer("Tourists");
            if (layer == null)
                return new List<Vector2>();
            List<Vector2> validPoints = new List<Vector2>();
            for (int x = 0; x < layer.LayerWidth; x++)
            for (int y = 0; y < layer.LayerHeight; y++)
            {
                if (!location.isTileLocationTotallyClearAndPlaceable(x, y))
                    continue;
                int index = Tourist.GetTileIndex(location, x, y);
                if (index == -1)
                    continue;
                if (validTiles.Contains(index))
                    validPoints.Add(new Vector2(x, y));
            }

            return validPoints;
        }

        internal static int GetTileIndex(GameLocation loc, int x, int y)
        {
            var layer = loc.map.GetLayer("Tourists");
            if (layer == null)
                return -1;
            return layer.Tiles[x, y]?.TileIndex ?? -1;
        }

        public Tourist(Vector2 position)
        {
            this.Speed = 2;
            this.Position = position;
            this.Name = "SundropTourist" + Guid.NewGuid();
            // ReSharper disable twice VirtualMemberCallInConstructor
            this.faceDirection(Game1.random.Next(4));
            this.Sprite = Tourist.BlankSprite;
            this.RandomizeLook();
        }

        internal void RandomizeLook()
        {
            if (Tourist.Wally == null && Game1.random.NextDouble() < 0.001)
            {
                Tourist.Wally = this;
                this.IsWally = true;
                this.Base = new AnimatedSprite(Tourist.WallyTexture, 0, 20, 34);
                Tourist.WallyAge = short.MaxValue;
                Game1.showGlobalMessage("Where's wally...");
            }
            else
            {
                if (Tourist.Wally == this)
                {
                    if (Tourist.WallyAge != 0)
                        return;
                    Tourist.Wally = null;
                    this.IsWally = false;
                    Game1.showGlobalMessage("Wally has gone home for now...");
                }

                this.Base = new AnimatedSprite(BaseColors[Game1.random.Next(BaseColors.Count)], 0, 20, 34);
                this.HairColor = HairColors[Game1.random.Next(HairColors.Length)];
                this.PantsColor = ClothingColors[Game1.random.Next(ClothingColors.Length)];
                this.ShoeColor = ClothingColors[Game1.random.Next(ClothingColors.Length)];
                bool isFemale = Game1.random.NextDouble() < 0.5;
                if (isFemale)
                {
                    this.Makeup = new AnimatedSprite(FemaleMakeup, 0, 20, 34);
                    this.Shirt = new AnimatedSprite(FemaleShirt[Game1.random.Next(FemaleShirt.Count)], 0, 20, 34);
                    this.Pants = new AnimatedSprite(FemalePants[Game1.random.Next(FemalePants.Count)], 0, 20, 34);
                    this.Shoes = new AnimatedSprite(FemaleShoes[Game1.random.Next(FemaleShoes.Count)], 0, 20, 34);
                    int hat = Game1.random.Next(FemaleHat.Count * 3);
                    if (hat < FemaleHat.Count)
                    {
                        string hatKey = FemaleHat[hat];
                        this.Hat = new AnimatedSprite(hatKey, 0, 20, 34);
                        string hatName = Path.GetFileNameWithoutExtension(hatKey) ?? "0";
                        if (hatName[hatName.Length - 1] != 't')
                        {
                            string path = FemaleHair[Game1.random.Next(FemaleHair.Count)];
                            string ext = Path.GetExtension(path);
                            string file = path.Substring(0, path.Length - ext?.Length ?? 0) + 'h';
                            this.Hair = new AnimatedSprite(file + ext, 0, 20, 34);
                        }
                    }

                    if (this.Hair == null)
                        this.Hair = new AnimatedSprite(FemaleHair[Game1.random.Next(FemaleHair.Count)], 0, 20, 34);
                    int accessory = Game1.random.Next(Tourist.FemaleAccessory.Count * 2);
                    if (accessory < Tourist.FemaleAccessory.Count)
                        this.Accessory = new AnimatedSprite(FemaleAccessory[accessory], 0, 20, 34);
                }
                else
                {
                    this.Shirt = new AnimatedSprite(MaleShirt[Game1.random.Next(MaleShirt.Count)], 0, 20, 34);
                    this.Pants = new AnimatedSprite(MalePants[Game1.random.Next(MalePants.Count)], 0, 20, 34);
                    this.Shoes = new AnimatedSprite(MaleShoes[Game1.random.Next(MaleShoes.Count)], 0, 20, 34);
                    int beard = Game1.random.Next(FaceHairs.Count * 2);
                    if (beard < FaceHairs.Count)
                        this.FaceHair = new AnimatedSprite(FaceHairs[beard], 0, 20, 34);
                    int hat = Game1.random.Next(MaleHat.Count * 3);
                    if (hat < MaleHat.Count)
                    {
                        string hatKey = MaleHat[hat];
                        this.Hat = new AnimatedSprite(hatKey, 0, 20, 34);
                        string hatName = Path.GetFileNameWithoutExtension(hatKey) ?? "0";
                        if (hatName[hatName.Length - 1] != 't')
                        {
                            string path = MaleHair[Game1.random.Next(MaleHair.Count)];
                            string ext = Path.GetExtension(path);
                            string file = path.Substring(0, path.Length - ext?.Length ?? 0) + 'h';
                            this.Hair = new AnimatedSprite(file + ext, 0, 20, 34);
                        }
                    }

                    if (this.Hair == null)
                        this.Hair = new AnimatedSprite(MaleHair[Game1.random.Next(MaleHair.Count)], 0, 20, 34);
                    int accessory = Game1.random.Next(Tourist.MaleAccessory.Count * 2);
                    if (accessory < Tourist.MaleAccessory.Count)
                        this.Accessory = new AnimatedSprite(MaleAccessory[accessory], 0, 20, 34);
                }
            }
        }

        private void DoWarp()
        {
            var validWarps = WarpCache[this.currentLocation.Name];
            var target = validWarps[Game1.random.Next(validWarps.Count)];
            int dir = Tourist.GetTileIndex(this.currentLocation, (int)target.X, (int)target.Y) - TILE_WARP_DOWN;
            this.RandomizeLook();
            this.Position = target * 64f;
            this.faceDirection(dir);
            this.setMovingInFacingDirection();
            this.Timer = 10;
            this.Delay = 0;
            this.Stuck = 0;
            this.Cooldown = byte.MaxValue;
        }
        public override bool isColliding(GameLocation l, Vector2 tile)
        {
            return Tourist.GetTileIndex(l, (int)tile.X, (int)tile.Y) == TILE_BLOCK;
        }
        public override void update(GameTime time, GameLocation location)
        {
            // Handle vanilla stuffs
            base.update(time, location);
            // Set some default vars
            var point = this.getTileLocationPoint();
            var backLayer = this.currentLocation.map.GetLayer("Back");
            int index = Tourist.GetTileIndex(this.currentLocation, point.X, point.Y);
            // Check if we are in a keep moving region
            if (index == TILE_KEEPMOVING)
            {
                if (this.Timer % 2 == 1)
                    this.Timer--;
                else
                    this.Timer += 2;
                if (!this.isMoving())
                    this.Delay++;
                else
                    this.Delay = 0;
                this.setMovingInFacingDirection();
                if (this.Delay > 3)
                    if (!Utility.isOnScreen(this.Position, 8))
                        this.DoWarp();
            }
            // If warp cooldown is active, decrement
            if (this.Cooldown > 0)
                this.Cooldown--;
            // Slows down update rate on the "Am I stuck?" check
            if (this.Timer > 0)
                this.Timer--;
            // When the timer is 0 do a stuck check
            else if (!this.isMoving() || Game1.random.NextDouble() <= 0.05)
            {
                // If the stuck check matches (Or the tourist has a random change of mind) set the timer
                this.Timer = 5;
                // And increase delay by 1
                this.Delay++;
                // If stuck, we add an extra bit of delay
                if (!this.isMoving())
                    this.Delay += 2;
            }
            // When within reach of a arrow tile prefer to follow it
            else if (index >= TILE_ARROW_DOWN && index <= TILE_ARROW_LEFT && Game1.random.NextDouble() < 0.1)
            {
                if (this.FacingDirection != index - TILE_ARROW_DOWN || Game1.random.NextDouble() < 0.25)
                {
                    this.Halt();
                    this.faceDirection(index - TILE_ARROW_DOWN);
                    this.setMovingInFacingDirection();
                    this.Timer = 10;
                    this.Delay = 0;
                }
            }
            // When within reach of a warp tile, see if we should warp
            else if(index >= TILE_WARP_DOWN && index <= TILE_WARP_LEFT && this.Cooldown == 0 || this.position.X < 0 || this.position.Y < 0 || this.position.Y > backLayer.DisplayHeight || this.position.X > backLayer.DisplayWidth)
                this.DoWarp();
            // If on a browse tile, maybe start browsing for a while?
            else if(index == TILE_BROWSE && Game1.random.NextDouble() < 0.25)
            {
                this.Halt();
                this.Timer = 120;
                this.Delay = 0;
            }
            // If nothing else needs to be done
            else
            {
                // Check if delay is bigger then 0, if so decrease by 3
                if (this.Delay > 0)
                    this.Delay -= 3;
                // Reset stuck counter
                this.Stuck = 0;
                // Set the timer
                this.Timer = 10;
            }
            // If delay is at 6 or more
            if (this.Delay > 6)
            {
                // Increment stuck counter
                this.Stuck++;
                // We set it to 0
                this.Delay = 0;
                // Set the timer
                this.Timer = 25;
                // And make the NPC get ready to move in a random direction
                this.Halt();
                int cur = this.FacingDirection;
                if (index >= TILE_ARROW_DOWN && index <= TILE_ARROW_LEFT && cur != index - TILE_ARROW_DOWN)
                    cur = index - TILE_ARROW_DOWN;
                else if (index >= TILE_WARP_DOWN && index <= TILE_WARP_LEFT && cur != index - TILE_WARP_DOWN)
                    cur = index - TILE_WARP_DOWN;
                else
                    cur = Game1.random.Next(4);
                this.faceDirection(cur);
                this.setMovingInFacingDirection();
            }
            // Check if stuck counter has reached 5 & this NPC is off screen
            if(this.Stuck>5 && !Utility.isOnScreen(this.Position, 8))
                this.DoWarp();
            // If this is the master game
            if (Game1.IsMasterGame && this.Base.CurrentAnimation == null)
                this.MovePosition(time, Game1.viewport, location);
            // We remember our position for the stuck check
            this.OldPos = this.Position;
            if (this.IsWally && Tourist.WallyAge > 0)
                Tourist.WallyAge--;
        }
        public override bool hasSpecialCollisionRules()
        {
            return true;
        }
        public override Rectangle GetBoundingBox()
        {
            return new Rectangle((int)this.Position.X + 16, (int)this.Position.Y + 16, 32, 32);
        }

        public override void Halt()
        {
                this.moveUp = false;
                this.moveDown = false;
                this.moveRight = false;
                this.moveLeft = false;
                this.Speed = 2;
                this.addedSpeed = 0;
                this.Base?.StopAnimation();
                this.Makeup?.StopAnimation();
                this.Hat?.StopAnimation();
                this.Hair?.StopAnimation();
                this.Shirt?.StopAnimation();
                this.Pants?.StopAnimation();
                this.Shoes?.StopAnimation();
                this.Accessory?.StopAnimation();
        }

        public override void faceDirection(int direction)
        {
            if (direction == -3)
                return;
            this.FacingDirection = direction;
            this.Base?.faceDirection(direction);
                this.Makeup?.faceDirection(direction);
                this.Hat?.faceDirection(direction);
                this.Hair?.faceDirection(direction);
                this.Shirt?.faceDirection(direction);
                this.Pants?.faceDirection(direction);
                this.Shoes?.faceDirection(direction);
                this.Accessory?.faceDirection(direction);
        }
        protected void AnimateDown(GameTime time, int interval, string sound)
        {
            this.Base?.AnimateDown(time, interval, sound);
                this.Makeup?.AnimateDown(time, interval, sound);
                this.Hat?.AnimateDown(time, interval, sound);
                this.Hair?.AnimateDown(time, interval, sound);
                this.Shirt?.AnimateDown(time, interval, sound);
                this.Pants?.AnimateDown(time, interval, sound);
                this.Shoes?.AnimateDown(time, interval, sound);
                this.Accessory?.AnimateDown(time, interval, sound);
            }
        protected void AnimateUp(GameTime time, int interval, string sound)
        {
            this.Base?.AnimateUp(time, interval, sound);
                this.Makeup?.AnimateUp(time, interval, sound);
                this.Hat?.AnimateUp(time, interval, sound);
                this.Hair?.AnimateUp(time, interval, sound);
                this.Shirt?.AnimateUp(time, interval, sound);
                this.Pants?.AnimateUp(time, interval, sound);
                this.Shoes?.AnimateUp(time, interval, sound);
                this.Accessory?.AnimateUp(time, interval, sound);
            }
        protected void AnimateLeft(GameTime time, int interval, string sound)
        {
            this.Base?.AnimateLeft(time, interval, sound);
                this.Makeup?.AnimateLeft(time, interval, sound);
                this.Hat?.AnimateLeft(time, interval, sound);
                this.Hair?.AnimateLeft(time, interval, sound);
                this.Shirt?.AnimateLeft(time, interval, sound);
                this.Pants?.AnimateLeft(time, interval, sound);
                this.Shoes?.AnimateLeft(time, interval, sound);
                this.Accessory?.AnimateLeft(time, interval, sound);
            }

        protected void AnimateRight(GameTime time, int interval, string sound)
        {
            this.Base?.AnimateRight(time, interval, sound);
                this.Makeup?.AnimateRight(time, interval, sound);
                this.Hat?.AnimateRight(time, interval, sound);
                this.Hair?.AnimateRight(time, interval, sound);
                this.Shirt?.AnimateRight(time, interval, sound);
                this.Pants?.AnimateRight(time, interval, sound);
                this.Shoes?.AnimateRight(time, interval, sound);
                this.Accessory?.AnimateRight(time, interval, sound);
            }

        protected void AnimateOnce(GameTime time)
        {
            this.Base?.animateOnce(time);
                this.Makeup?.animateOnce(time);
                this.Hat?.animateOnce(time);
                this.Hair?.animateOnce(time);
                this.Shirt?.animateOnce(time);
                this.Pants?.animateOnce(time);
                this.Shoes?.animateOnce(time);
                this.Accessory?.animateOnce(time);
            }

        public override void MovePosition(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation curLocation)
        {
            // ReSharper disable twice CompareOfFloatsByEqualityOperator
            if (this.xVelocity != 0f || this.yVelocity != 0f)
                this.applyVelocity(this.currentLocation);
            else if (this.moveUp)
            {
                if (curLocation == null || !curLocation.isCollidingPosition(this.nextPosition(0), viewport, false, 0, false, this) || this.isCharging)
                {
                    this.position.Y -= this.speed + this.addedSpeed;
                    if (!this.ignoreMovementAnimation)
                    {
                        this.AnimateUp(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, curLocation) ? "Cowboy_Footstep" : "");
                        this.faceDirection(0);
                    }
                }
                else if (!curLocation.isTilePassable(this.nextPosition(0), viewport))
                    this.Halt();
            }
            else if (this.moveRight)
            {
                if (curLocation == null || !curLocation.isCollidingPosition(this.nextPosition(1), viewport, false, 0, false, this) || this.isCharging)
                {
                    this.position.X += (this.speed + this.addedSpeed);
                    if (!this.ignoreMovementAnimation)
                    {
                        this.AnimateRight(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, curLocation) ? "Cowboy_Footstep" : "");
                        this.faceDirection(1);
                    }
                }
                else if (!curLocation.isTilePassable(this.nextPosition(1), viewport))
                    this.Halt();
            }
            else if (this.moveDown)
            {
                if (curLocation == null || !curLocation.isCollidingPosition(this.nextPosition(2), viewport, false, 0, false, this) || this.isCharging)
                {
                    this.position.Y += (this.speed + this.addedSpeed);
                    if (!this.ignoreMovementAnimation)
                    {
                        this.AnimateDown(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, curLocation) ? "Cowboy_Footstep" : "");
                        this.faceDirection(2);
                    }
                }
                else if (!curLocation.isTilePassable(this.nextPosition(2), viewport))
                    this.Halt();
            }
            else if (this.moveLeft)
            {
                if (curLocation == null || !curLocation.isCollidingPosition(this.nextPosition(3), viewport, false, 0, false, this) || this.isCharging)
                {
                    this.position.X -= (this.speed + this.addedSpeed);
                    if (!this.ignoreMovementAnimation)
                    {
                        this.AnimateLeft(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, curLocation) ? "Cowboy_Footstep" : "");
                        this.faceDirection(3);
                    }
                }
                else if (!curLocation.isTilePassable(this.nextPosition(3), viewport))
                    this.Halt();
            }
            else
                this.AnimateOnce(time);
            if (this.blockedInterval >= 3000 && this.blockedInterval <= 3750f && !Game1.eventUp)
            {
                this.doEmote(Game1.random.NextDouble() < 0.5 ? 8 : 40);
                this.blockedInterval = 3750;
            }
            else if (this.blockedInterval >= 5000)
            {
                this.speed = 4;
                this.isCharging = true;
                this.blockedInterval = 0;
            }
        }

        public override void draw(SpriteBatch b, float alpha = 1)
        {
            if (!Utility.isOnScreen(this.Position, 128))
                return;
            float depth = Math.Max(0f, this.drawOnTop ? 0.991f : this.getStandingY() / 10000f);
            var positionVector = this.getLocalPosition(Game1.viewport) + new Vector2(this.Base.SpriteWidth * 2, this.GetBoundingBox().Height / 2f) + ((this.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero);
            var originVector = new Vector2(this.Base.SpriteWidth / 2f + 2, this.Base.SpriteHeight * 0.75f);
            float scaleFloat = Math.Max(0.2f, this.Scale) * 4f;
            var spriteEffects = this.flip || this.Base.CurrentAnimation != null && this.Base.CurrentAnimation[this.Base.currentAnimationIndex].flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            b.Draw(this.Base.Texture, positionVector, this.Base.SourceRect, Color.White * alpha, this.rotation, originVector, scaleFloat, spriteEffects, depth);
            if (this.IsWally)
                return;
            if (this.Makeup != null)
                b.Draw(this.Makeup.Texture, positionVector, this.Base.SourceRect, Color.White * alpha, this.rotation, originVector, scaleFloat, spriteEffects, depth + 0.00001f);
            b.Draw(this.Shirt.Texture, positionVector, this.Base.SourceRect, Color.White * alpha, this.rotation, originVector, scaleFloat, spriteEffects, depth + 0.00002f);
            b.Draw(this.Pants.Texture, positionVector, this.Base.SourceRect, this.PantsColor * alpha, this.rotation, originVector, scaleFloat, spriteEffects, depth + 0.00003f);
            b.Draw(this.Shoes.Texture, positionVector, this.Base.SourceRect, this.ShoeColor * alpha, this.rotation, originVector, scaleFloat, spriteEffects, depth + 0.00004f);
            if (this.FaceHair != null)
                b.Draw(this.FaceHair.Texture, positionVector, this.Base.SourceRect, this.HairColor * alpha, this.rotation, originVector, scaleFloat, spriteEffects, depth + 0.00005f);
            if (this.Accessory != null)
                b.Draw(this.Accessory.Texture, positionVector, this.Base.SourceRect, Color.White * alpha, this.rotation, originVector, scaleFloat, spriteEffects, depth + 0.00006f);
            b.Draw(this.Hair.Texture, positionVector, this.Base.SourceRect, this.HairColor * alpha, this.rotation, originVector, scaleFloat, spriteEffects, depth + 0.00007f);
            if (this.Hat != null)
                b.Draw(this.Hat.Texture, positionVector, this.Base.SourceRect, Color.White * alpha, this.rotation, originVector, scaleFloat, spriteEffects, depth + 0.00008f);
        }

        public override bool checkAction(Farmer who, GameLocation l)
        {
            if (this.textAboveHeadTimer > 0)
                return false;
            this.showTextAboveHead(this.IsWally ? SundropCityMod.SHelper.Translation.Get("Tourist.Wally." + Game1.random.Next(Tourist.WALLY_LINE_COUNT)) : SundropCityMod.SHelper.Translation.Get("Tourist.Lines." + Game1.random.Next(Tourist.TOURIST_LINE_COUNT)));
            return true;
        }
    }
}