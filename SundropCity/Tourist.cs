using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Characters;
using xTile.Dimensions;

namespace SundropCity
{
    class Tourist : NPC
    {
        const int TOURIST_LINE_COUNT = 12;
        protected AnimatedSprite Base;
        protected AnimatedSprite Makeup;
        protected AnimatedSprite Hat;
        protected AnimatedSprite Hair;
        protected AnimatedSprite Shirt;
        protected AnimatedSprite Pants;
        protected AnimatedSprite Shoes;
        protected Color HairColor = Color.Brown;
        protected Color PantsColor = Color.DarkSeaGreen;
        protected Color ShoeColor = Color.SlateGray;
        protected byte Timer = 0;
        protected byte Delay = 0;
        protected Vector2 OldPos;
        public Tourist(Vector2 position)
        {
            this.willDestroyObjectsUnderfoot = false;
            this.Speed = 2;
            this.Position = position;
            this.Name = "SundropTourist" + Guid.NewGuid().ToString();
            this.Sprite = new AnimatedSprite();
            this.Base = new AnimatedSprite(SundropCityMod.SHelper.Content.GetActualAssetKey("assets/Characters/Tourists/body.png"), 0, 16, 32);
            this.Makeup = new AnimatedSprite(SundropCityMod.SHelper.Content.GetActualAssetKey("assets/Characters/Tourists/lashes.png"), 0, 16, 32);
            this.Hair = new AnimatedSprite(SundropCityMod.SHelper.Content.GetActualAssetKey("assets/Characters/Tourists/Hair/f1.png"), 0, 16, 32);
            this.Shirt = new AnimatedSprite(SundropCityMod.SHelper.Content.GetActualAssetKey("assets/Characters/Tourists/Top/g1.png"), 0, 16, 32);
            this.Pants = new AnimatedSprite(SundropCityMod.SHelper.Content.GetActualAssetKey("assets/Characters/Tourists/Bottom/g1.png"), 0, 16, 32);
            this.Shoes = new AnimatedSprite(SundropCityMod.SHelper.Content.GetActualAssetKey("assets/Characters/Tourists/Shoe/g1.png"), 0, 16, 32);
        }
        public override Microsoft.Xna.Framework.Rectangle GetBoundingBox()
        {
            return new Microsoft.Xna.Framework.Rectangle((int)this.Position.X + 16, (int)this.Position.Y + 16, this.Base.SpriteWidth * 4 * 3 / 4, 32);
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
        }
        public override void MovePosition(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation)
        {
            TimeSpan elapsedGameTime;
            if (this.xVelocity != 0f || this.yVelocity != 0f)
            {
                this.applyVelocity(this.currentLocation);
            }
            else if (this.moveUp)
            {
                if (currentLocation == null || !currentLocation.isCollidingPosition(this.nextPosition(0), viewport, false, 0, false, this) || this.isCharging)
                {
                    this.position.Y -= (float)(this.speed + this.addedSpeed);
                    if (!this.ignoreMovementAnimation)
                    {
                        this.AnimateUp(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, currentLocation) ? "Cowboy_Footstep" : "");
                        this.faceDirection(0);
                    }
                }
                else if (!currentLocation.isTilePassable(this.nextPosition(0), viewport) || !this.willDestroyObjectsUnderfoot)
                {
                    this.Halt();
                }
                else if (this.willDestroyObjectsUnderfoot)
                {
                    new Vector2((float)(this.getStandingX() / 64), (float)(this.getStandingY() / 64 - 1));
                    if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(0), true))
                    {
                        this.doEmote(12, true);
                        this.position.Y -= (float)(this.speed + this.addedSpeed);
                    }
                    else
                    {
                        int num = this.blockedInterval;
                        elapsedGameTime = time.ElapsedGameTime;
                        this.blockedInterval = num + elapsedGameTime.Milliseconds;
                    }
                }
            }
            else if (this.moveRight)
            {
                if (currentLocation == null || !currentLocation.isCollidingPosition(this.nextPosition(1), viewport, false, 0, false, this) || this.isCharging)
                {
                    this.position.X += (float)(this.speed + this.addedSpeed);
                    if (!this.ignoreMovementAnimation)
                    {
                        this.AnimateRight(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, currentLocation) ? "Cowboy_Footstep" : "");
                        this.faceDirection(1);
                    }
                }
                else if (!currentLocation.isTilePassable(this.nextPosition(1), viewport) || !this.willDestroyObjectsUnderfoot)
                {
                    this.Halt();
                }
                else if (this.willDestroyObjectsUnderfoot)
                {
                    new Vector2((float)(this.getStandingX() / 64 + 1), (float)(this.getStandingY() / 64));
                    if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(1), true))
                    {
                        this.doEmote(12, true);
                        this.position.X += (float)(this.speed + this.addedSpeed);
                    }
                    else
                    {
                        int num2 = this.blockedInterval;
                        elapsedGameTime = time.ElapsedGameTime;
                        this.blockedInterval = num2 + elapsedGameTime.Milliseconds;
                    }
                }
            }
            else if (this.moveDown)
            {
                if (currentLocation == null || !currentLocation.isCollidingPosition(this.nextPosition(2), viewport, false, 0, false, this) || this.isCharging)
                {
                    this.position.Y += (float)(this.speed + this.addedSpeed);
                    if (!this.ignoreMovementAnimation)
                    {
                        this.AnimateDown(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, currentLocation) ? "Cowboy_Footstep" : "");
                        this.faceDirection(2);
                    }
                }
                else if (!currentLocation.isTilePassable(this.nextPosition(2), viewport) || !this.willDestroyObjectsUnderfoot)
                {
                    this.Halt();
                }
                else if (this.willDestroyObjectsUnderfoot)
                {
                    new Vector2((float)(this.getStandingX() / 64), (float)(this.getStandingY() / 64 + 1));
                    if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(2), true))
                    {
                        this.doEmote(12, true);
                        this.position.Y += (float)(this.speed + this.addedSpeed);
                    }
                    else
                    {
                        int num3 = this.blockedInterval;
                        elapsedGameTime = time.ElapsedGameTime;
                        this.blockedInterval = num3 + elapsedGameTime.Milliseconds;
                    }
                }
            }
            else if (this.moveLeft)
            {
                if (currentLocation == null || !currentLocation.isCollidingPosition(this.nextPosition(3), viewport, false, 0, false, this) || this.isCharging)
                {
                    this.position.X -= (float)(this.speed + this.addedSpeed);
                    if (!this.ignoreMovementAnimation)
                    {
                        this.AnimateLeft(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, currentLocation) ? "Cowboy_Footstep" : "");
                        this.faceDirection(3);
                    }
                }
                else if (!currentLocation.isTilePassable(this.nextPosition(3), viewport) || !this.willDestroyObjectsUnderfoot)
                {
                    this.Halt();
                }
                else if (this.willDestroyObjectsUnderfoot)
                {
                    new Vector2((float)(this.getStandingX() / 64 - 1), (float)(this.getStandingY() / 64));
                    if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(3), true))
                    {
                        this.doEmote(12, true);
                        this.position.X -= (float)(this.speed + this.addedSpeed);
                    }
                    else
                    {
                        int num4 = this.blockedInterval;
                        elapsedGameTime = time.ElapsedGameTime;
                        this.blockedInterval = num4 + elapsedGameTime.Milliseconds;
                    }
                }
            }
            else
            {
                this.AnimateOnce(time);
            }
            if (this.blockedInterval >= 3000 && (float)this.blockedInterval <= 3750f && !Game1.eventUp)
            {
                this.doEmote((Game1.random.NextDouble() < 0.5) ? 8 : 40, true);
                this.blockedInterval = 3750;
            }
            else if (this.blockedInterval >= 5000)
            {
                this.speed = 4;
                this.isCharging = true;
                this.blockedInterval = 0;
            }
        }
        public override void update(GameTime time, GameLocation location)
        {
            // Handle vanilla stuffs
            base.update(time, location);
            // Make sure none of our tourists become void walkers by making sure that if they reach the edge of the map they do a heel-face-turn to get away.
            if(this.position.X<=0)
            {
                this.Halt();
                this.Timer = 25;
                this.SetMovingOnlyRight();
            }
            if (this.position.Y <= 0)
            {
                this.Halt();
                this.Timer = 25;
                this.SetMovingOnlyDown();
            }
            if(this.position.X >= this.currentLocation.map.DisplayWidth)
            {
                this.Halt();
                this.Timer = 25;
                this.SetMovingOnlyLeft();
            }
            if (this.position.Y >= this.currentLocation.map.DisplayHeight)
            {
                this.Halt();
                this.Timer = 25;
                this.SetMovingOnlyUp();
            }
            // Slows down update rate on the "Am I stuck?" check
            if (this.Timer > 0)
                this.Timer--;
            // When the timer is 0 do a stuck check
            else if (this.OldPos == this.Position || Game1.random.NextDouble() <= 0.01)
            {
                // If the stuck check matches (Or the tourist has a random change of mind) set the timer
                this.Timer = 15;
                // And increase delay by 1
                this.Delay++;
            }
            // If not stuck
            else
            {
                // Check if delay is bigger then 0, if so decrease by 3
                if (this.Delay > 0)
                    this.Delay -= 3;
                // Set the timer
                this.Timer = 15;
            }
            // If delay is at 6 or more
            if(this.Delay>6)
            {
                // We set it to 0
                this.Delay = 0;
                // Set the timer
                this.Timer = 25;
                // And make the NPC get ready to move in a random direction
                this.Halt();
                int dir = this.getFacingDirection() + Game1.random.Next(4);
                if (dir > 3)
                    dir -= 4;
                this.faceDirection(dir);
                this.setMovingInFacingDirection();
            }
            // If this is the master game
            if(Game1.IsMasterGame && this.Base.CurrentAnimation == null)
                this.MovePosition(time, Game1.viewport, location);
            // We remember our position for the stuck check
            this.OldPos = this.Position;
        }
        public override void draw(SpriteBatch b, float alpha = 1)
        {
            if (!Utility.isOnScreen(this.Position, 128))
                return;
            b.Draw(this.Base.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((this.Base.SpriteWidth * 4 / 2), (this.GetBoundingBox().Height / 2)) + ((this.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero), this.Base.SourceRect, Color.White * alpha, this.rotation, new Vector2((this.Base.SpriteWidth / 2), this.Base.SpriteHeight * 3f / 4f), Math.Max(0.2f, this.scale.Value) * 4f, (this.flip || (this.Base.CurrentAnimation != null && this.Base.CurrentAnimation[this.Base.currentAnimationIndex].flip)) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, this.drawOnTop ? 0.991f : (this.getStandingY() / 10000f)));
            if(this.Makeup!=null)
                b.Draw(this.Makeup.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((this.Base.SpriteWidth * 4 / 2), (this.GetBoundingBox().Height / 2)) + ((this.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero), this.Base.SourceRect, Color.White * alpha, this.rotation, new Vector2((this.Base.SpriteWidth / 2), this.Base.SpriteHeight * 3f / 4f), Math.Max(0.2f, this.scale.Value) * 4f, (this.flip || (this.Base.CurrentAnimation != null && this.Base.CurrentAnimation[this.Base.currentAnimationIndex].flip)) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, this.drawOnTop ? 0.991f : (this.getStandingY() / 10000f + 0.000001f)));
            if(this.Hair!=null)
                b.Draw(this.Hair.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((this.Base.SpriteWidth * 4 / 2), (this.GetBoundingBox().Height / 2)) + ((this.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero), this.Base.SourceRect, this.HairColor * alpha, this.rotation, new Vector2((this.Base.SpriteWidth / 2), this.Base.SpriteHeight * 3f / 4f), Math.Max(0.2f, this.scale.Value) * 4f, (this.flip || (this.Base.CurrentAnimation != null && this.Base.CurrentAnimation[this.Base.currentAnimationIndex].flip)) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, this.drawOnTop ? 0.991f : (this.getStandingY() / 10000f + 0.000002f)));
            if (this.Hat != null)
                b.Draw(this.Hat.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((this.Base.SpriteWidth * 4 / 2), (this.GetBoundingBox().Height / 2)) + ((this.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero), this.Base.SourceRect, Color.White * alpha, this.rotation, new Vector2((this.Base.SpriteWidth / 2), this.Base.SpriteHeight * 3f / 4f), Math.Max(0.2f, this.scale.Value) * 4f, (this.flip || (this.Base.CurrentAnimation != null && this.Base.CurrentAnimation[this.Base.currentAnimationIndex].flip)) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, this.drawOnTop ? 0.991f : (this.getStandingY() / 10000f +0.000003f)));
            if (this.Shirt != null)
                b.Draw(this.Shirt.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((this.Base.SpriteWidth * 4 / 2), (this.GetBoundingBox().Height / 2)) + ((this.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero), this.Base.SourceRect, Color.White * alpha, this.rotation, new Vector2((this.Base.SpriteWidth / 2), this.Base.SpriteHeight * 3f / 4f), Math.Max(0.2f, this.scale.Value) * 4f, (this.flip || (this.Base.CurrentAnimation != null && this.Base.CurrentAnimation[this.Base.currentAnimationIndex].flip)) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, this.drawOnTop ? 0.991f : (this.getStandingY() / 10000f + 0.000004f)));
            if (this.Pants != null)
                b.Draw(this.Pants.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((this.Base.SpriteWidth * 4 / 2), (this.GetBoundingBox().Height / 2)) + ((this.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero), this.Base.SourceRect, this.PantsColor * alpha, this.rotation, new Vector2((this.Base.SpriteWidth / 2), this.Base.SpriteHeight * 3f / 4f), Math.Max(0.2f, this.scale.Value) * 4f, (this.flip || (this.Base.CurrentAnimation != null && this.Base.CurrentAnimation[this.Base.currentAnimationIndex].flip)) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, this.drawOnTop ? 0.991f : (this.getStandingY() / 10000f + 0.000005f)));
            if (this.Shoes != null)
                b.Draw(this.Shoes.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((this.Base.SpriteWidth * 4 / 2), (this.GetBoundingBox().Height / 2)) + ((this.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero), this.Base.SourceRect, this.ShoeColor * alpha, this.rotation, new Vector2((this.Base.SpriteWidth / 2), this.Base.SpriteHeight * 3f / 4f), Math.Max(0.2f, this.scale.Value) * 4f, (this.flip || (this.Base.CurrentAnimation != null && this.Base.CurrentAnimation[this.Base.currentAnimationIndex].flip)) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, this.drawOnTop ? 0.991f : (this.getStandingY() / 10000f + 0.000006f)));
        }
        public override bool checkAction(Farmer who, GameLocation l)
        {
            if (this.textAboveHeadTimer > 0)
                return false;
            this.showTextAboveHead(SundropCityMod.SHelper.Translation.Get("Tourist.Lines." + Game1.random.Next(TOURIST_LINE_COUNT).ToString()));
            return true;
        }

        private void Animate(AnimatedSprite sprite, GameTime time)
        {
            if (sprite == null)
                return;
            if (sprite.CurrentAnimation != null)
                sprite.animateOnce(time);
            else
            {
                sprite.faceDirection(this.FacingDirection);
                if (this.isMoving())
                {
                    switch (this.FacingDirection)
                    {
                        case 0:
                            sprite.AnimateUp(time, 0, "");
                            break;
                        case 1:
                            sprite.AnimateRight(time, 0, "");
                            break;
                        case 2:
                            sprite.AnimateDown(time, 0, "");
                            break;
                        case 3:
                            sprite.AnimateLeft(time, 0, "");
                            break;
                    }
                }
                else
                    sprite.StopAnimation();
            }
        }
    }
}
