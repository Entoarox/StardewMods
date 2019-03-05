using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Characters;

namespace MagicJunimoPet
{
    class MagicJunimo : Pet
    {
        private static readonly Color[] Colors = new Color[100];

        private int Timer;
        private Color Color1;
        private Color Color2;
        private Color Color3;

        static MagicJunimo()
        {
            for(float h=0;h<=1;h+=0.01f)
            {
                if(h*100<Colors.Length)
                    Colors[(int)Math.Floor(h * 100)] = ColorScale.ColorFromHSL(h, .7f, .7f);
            }
        }
        public MagicJunimo()
        {
            this.Name = "MagicJunimo";
            this.displayName = this.Name;
            this.Sprite = new AnimatedSprite(MJPModEntry.TexturePath, 0, 32, 32);
            this.Position = new Vector2(52.5f, 8f) * 64f;
            this.Breather = false;
            this.willDestroyObjectsUnderfoot = false;
            this.Timer = new Random().Next(3000);
            this.SetColor();
        }
        private void SetColor()
        {
            this.Color1 = Colors[(int)Math.Floor((this.Timer % 3000) / 30d)];
            this.Color2 = Colors[Math.Max(0, (int)Math.Floor((this.Timer % 3000) / 30d) - 1)];
            this.Color3 = Colors[Math.Max(0, (int)Math.Floor((this.Timer % 3000) / 30d) - 2)];
        }
        public override void update(GameTime time, GameLocation location)
        {
            this.Timer += time.ElapsedGameTime.Milliseconds;
            this.SetColor();
            if (Game1.random.NextDouble() < 0.001)
                this.playContentSound();
            base.update(time, location);
            if (this.currentLocation == null)
            {
                this.currentLocation = location;
            }
            if (!Game1.eventUp && !Game1.IsClient)
            {
                if (Game1.timeOfDay > 2000 && this.Sprite.CurrentAnimation == null && this.xVelocity == 0f && this.yVelocity == 0f)
                {
                    this.CurrentBehavior = 1;
                }
                switch (this.CurrentBehavior)
                {
                    case 1:
                        if (Game1.timeOfDay < 2000 && Game1.random.NextDouble() < 0.001)
                        {
                            this.CurrentBehavior = 0;
                        }
                        else if (Game1.random.NextDouble() < 0.002)
                        {
                            doEmote(24, true);
                        }
                        return;
                    case 2:
                        if (this.Sprite.currentFrame != 18 && this.Sprite.CurrentAnimation == null)
                        {
                            initiateCurrentBehavior();
                        }
                        else if (this.Sprite.currentFrame == 18 && Game1.random.NextDouble() < 0.01)
                        {
                            switch (Game1.random.Next(10))
                            {
                                case 0:
                                    this.CurrentBehavior = 0;
                                    Halt();
                                    faceDirection(2);
                                    this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                    {
                        new FarmerSprite.AnimationFrame(17, 200),
                        new FarmerSprite.AnimationFrame(16, 200),
                        new FarmerSprite.AnimationFrame(0, 200)
                    });
                                    this.Sprite.loop = false;
                                    break;
                                case 1:
                                case 2:
                                case 3:
                                    {
                                        List<FarmerSprite.AnimationFrame> licks = new List<FarmerSprite.AnimationFrame>
                    {
                        new FarmerSprite.AnimationFrame(19, 300),
                        new FarmerSprite.AnimationFrame(20, 200),
                        new FarmerSprite.AnimationFrame(21, 200),
                        new FarmerSprite.AnimationFrame(22, 200, false, false, this.LickSound, false),
                        new FarmerSprite.AnimationFrame(23, 200)
                    };
                                        int extraLicks = Game1.random.Next(1, 6);
                                        for (int i = 0; i < extraLicks; i++)
                                        {
                                            licks.Add(new FarmerSprite.AnimationFrame(21, 150));
                                            licks.Add(new FarmerSprite.AnimationFrame(22, 150, false, false, this.LickSound, false));
                                            licks.Add(new FarmerSprite.AnimationFrame(23, 150));
                                        }
                                        licks.Add(new FarmerSprite.AnimationFrame(18, 1, false, false, this.hold, false));
                                        this.Sprite.loop = false;
                                        this.Sprite.setCurrentAnimation(licks);
                                        break;
                                    }
                                default:
                                    {
                                        bool blink = Game1.random.NextDouble() < 0.45;
                                        this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                    {
                        new FarmerSprite.AnimationFrame(19, blink ? 200 : Game1.random.Next(1000, 9000)),
                        new FarmerSprite.AnimationFrame(18, 1, false, false, this.hold, false)
                    });
                                        this.Sprite.loop = false;
                                        if (blink && Game1.random.NextDouble() < 0.2)
                                        {
                                            playContentSound();
                                            shake(200);
                                        }
                                        break;
                                    }
                            }
                        }
                        break;
                    case 0:
                        if (this.Sprite.CurrentAnimation == null && Game1.random.NextDouble() < 0.01)
                        {
                            switch (Game1.random.Next(4))
                            {
                                case 0:
                                case 1:
                                case 2:
                                    initiateCurrentBehavior();
                                    break;
                                case 3:
                                    switch (this.FacingDirection)
                                    {
                                        case 0:
                                        case 2:
                                            Halt();
                                            faceDirection(2);
                                            this.Sprite.loop = false;
                                            this.CurrentBehavior = 2;
                                            break;
                                        case 1:
                                            if (Game1.random.NextDouble() < 0.85)
                                            {
                                                this.Sprite.loop = false;
                                                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                            {
                                new FarmerSprite.AnimationFrame(24, 100),
                                new FarmerSprite.AnimationFrame(25, 100),
                                new FarmerSprite.AnimationFrame(26, 100),
                                new FarmerSprite.AnimationFrame(27, Game1.random.Next(8000, 30000), false, false, this.FlopSound, false)
                            });
                                            }
                                            else
                                            {
                                                this.Sprite.loop = false;
                                                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                            {
                                new FarmerSprite.AnimationFrame(30, 300),
                                new FarmerSprite.AnimationFrame(31, 300),
                                new FarmerSprite.AnimationFrame(30, 300),
                                new FarmerSprite.AnimationFrame(31, 300),
                                new FarmerSprite.AnimationFrame(30, 300),
                                new FarmerSprite.AnimationFrame(31, 500),
                                new FarmerSprite.AnimationFrame(24, 800, false, false, this.Leap, false),
                                new FarmerSprite.AnimationFrame(4, 1)
                            });
                                            }
                                            break;
                                        case 3:
                                            if (Game1.random.NextDouble() < 0.85)
                                            {
                                                this.Sprite.loop = false;
                                                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                            {
                                new FarmerSprite.AnimationFrame(24, 100, false, true, null, false),
                                new FarmerSprite.AnimationFrame(25, 100, false, true, null, false),
                                new FarmerSprite.AnimationFrame(26, 100, false, true, null, false),
                                new FarmerSprite.AnimationFrame(27, Game1.random.Next(8000, 30000), false, true, this.FlopSound, false),
                                new FarmerSprite.AnimationFrame(12, 1)
                            });
                                            }
                                            else
                                            {
                                                this.Sprite.loop = false;
                                                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                            {
                                new FarmerSprite.AnimationFrame(30, 300, false, true, null, false),
                                new FarmerSprite.AnimationFrame(31, 300, false, true, null, false),
                                new FarmerSprite.AnimationFrame(30, 300, false, true, null, false),
                                new FarmerSprite.AnimationFrame(31, 300, false, true, null, false),
                                new FarmerSprite.AnimationFrame(30, 300, false, true, null, false),
                                new FarmerSprite.AnimationFrame(31, 500, false, true, null, false),
                                new FarmerSprite.AnimationFrame(24, 800, false, true, this.Leap, false),
                                new FarmerSprite.AnimationFrame(12, 1)
                            });
                                            }
                                            break;
                                    }
                                    break;
                            }
                        }
                        break;
                }
                if (this.Sprite.CurrentAnimation != null)
                {
                    this.Sprite.loop = false;
                }
                if (this.Sprite.CurrentAnimation == null)
                {
                    MovePosition(time, Game1.viewport, location);
                }
                else if (this.xVelocity != 0f || this.yVelocity != 0f)
                {
                    Rectangle nextPosition = GetBoundingBox();
                    nextPosition.X += (int)this.xVelocity;
                    nextPosition.Y -= (int)this.yVelocity;
                    if (this.currentLocation == null || !this.currentLocation.isCollidingPosition(nextPosition, Game1.viewport, false, 0, false, this))
                    {
                        this.position.X += (int)this.xVelocity;
                        this.position.Y -= (int)this.yVelocity;
                    }
                    this.xVelocity = (int)(this.xVelocity - this.xVelocity / 4f);
                    this.yVelocity = (int)(this.yVelocity - this.yVelocity / 4f);
                }
            }
        }
        public void Leap(Farmer who)
        {
            if (this.currentLocation.Equals(Game1.currentLocation))
            {
                jump();
            }
            if (this.FacingDirection == 1)
            {
                this.xVelocity = 8f;
            }
            else if (this.FacingDirection == 3)
            {
                this.xVelocity = -8f;
            }
        }
        public void LickSound(Farmer who)
        {
            if (Utility.isOnScreen(getTileLocationPoint(), 128, this.currentLocation))
            {
                Game1.playSound("junimoMeep1");
            }
        }
        public void FlopSound(Farmer who)
        {
            if (Utility.isOnScreen(getTileLocationPoint(), 128, this.currentLocation))
            {
                Game1.playSound("thudStep");
            }
        }
        public override void playContentSound()
        {
            if (Utility.isOnScreen(getTileLocationPoint(), 128, this.currentLocation))
            {
                Game1.playSound("junimoMeep1");
            }
        }
        public override void draw(SpriteBatch b)
        {
            if (this.Sprite.Texture != null)
            {
                Vector2 pos = Game1.GlobalToLocal(Game1.viewport, this.Position);
                pos.Y -= 64f;
                b.Draw(this.Sprite.Texture, pos, this.Sprite.sourceRect, this.Color3, 0f, Vector2.Zero, 4f, (this.Sprite.CurrentAnimation != null && this.Sprite.CurrentAnimation[this.Sprite.currentAnimationIndex].flip) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, GetBoundingBox().Center.Y / 10000f - 0.000002f);
                b.Draw(this.Sprite.Texture, pos, this.Sprite.sourceRect, this.Color2, 0f, Vector2.Zero, 4f, (this.Sprite.CurrentAnimation != null && this.Sprite.CurrentAnimation[this.Sprite.currentAnimationIndex].flip) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, GetBoundingBox().Center.Y / 10000f - 0.000001f);
                b.Draw(this.Sprite.Texture, pos, this.Sprite.sourceRect, this.Color1, 0f, Vector2.Zero, 4f, (this.Sprite.CurrentAnimation != null && this.Sprite.CurrentAnimation[this.Sprite.currentAnimationIndex].flip) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, GetBoundingBox().Center.Y / 10000f);
            }
            if (this.IsEmoting)
            {
                Vector2 emotePosition = getLocalPosition(Game1.viewport);
                emotePosition.X += 32f;
                emotePosition.Y -= 64f;
                b.Draw(Game1.emoteSpriteSheet, emotePosition, new Rectangle(this.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, this.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, getStandingY() / 10000f + 0.0001f);
            }
        }
    }
}
