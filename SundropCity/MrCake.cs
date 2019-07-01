using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace SundropCity
{
    class MrCake : NPC
    {
        private readonly NPC Target;
        private int Delay = 0;
        public MrCake(Vector2 position, NPC target) : base(new AnimatedSprite(SundropCityMod.SHelper.Content.GetActualAssetKey("assets/Characters/Sprites/MrCake.png"), 0, 32, 32), position * 64f, 2, "Mr. Cake")
        {
            this.Target = target;
            this.Breather = false;
            this.HideShadow = true;
            this.willDestroyObjectsUnderfoot = false;
            this.DefaultMap = "SundropPromenade";
            this.DefaultPosition = position * 64f;
            this.Portrait = SundropCityMod.SHelper.Content.Load<Texture2D>("assets/Characters/Portraits/MrCake.png");
            this.addExtraDialogues("Mr. Cake looks at you with approval.");
        }

        public override Rectangle GetBoundingBox()
        {
            return new Rectangle((int)this.Position.X + 16, (int)this.Position.Y + 16, this.Sprite.SpriteWidth * 4 * 3 / 4, 32);
        }

        public override void update(GameTime time, GameLocation location, long id, bool move)
        {
            if (this.currentLocation != null && this.Target.currentLocation != null && this.currentLocation != this.Target.currentLocation)
                Game1.warpCharacter(this, this.Target.currentLocation, this.Target.Position);
            float distance = Vector2.Distance(new Vector2(this.position.X+32,this.Position.Y), this.Target.Position);
            if (distance > 96)
            {
                this.Delay = 5;
                this.faceGeneralDirection(this.Target.Position);
                if (Game1.IsMasterGame)
                {
                    this.setMovingInFacingDirection();
                    if (this.Sprite.CurrentAnimation == null)
                        this.MovePosition(time, Game1.viewport, location);
                }
            }
            else if(this.Delay>0)
            {
                this.Delay--;
                if (this.Delay == 0)
                {
                    this.Halt();
                    this.faceDirection(this.Target.FacingDirection);
                }
            }
            base.update(time, location, id, move);
        }
    }
}
