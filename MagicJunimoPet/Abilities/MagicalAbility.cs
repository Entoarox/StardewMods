using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace MagicJunimoPet.Abilities
{
    class MagicalAbility : Ability, ITickingAbility, ISkinAbility
    {
        private static readonly Texture2D Icon;
        private static Rectangle Region = new Rectangle(0, 0, 16, 16);
        private static Color[] ColorMap = new Color[100];

        static MagicalAbility()
        {
            Icon = MJPModEntry.SHelper.Content.Load<Texture2D>("assets/icons.png");
            for (float h = 0; h <= 1; h += 0.01f)
            {
                if (h * 100 < ColorMap.Length)
                    ColorMap[(int)Math.Floor(h * 100)] = ColorScale.ColorFromHSL(h, .7f, .7f);
            }
        }

        private int Timer;
        private Color[] Colors = new Color[3];

        public override string Id => "Magical";

        public override void DrawIcon(Rectangle region, SpriteBatch b)
        {
            b.Draw(Icon, region, Region, Color.White);
        }
        public void DrawPet(SpriteBatch b)
        {
            if (this.Pet.Sprite.Texture != null)
            {
                Vector2 pos = Game1.GlobalToLocal(Game1.viewport, this.Pet.Position);
                pos.Y -= 64f;
                b.Draw(this.Pet.Sprite.Texture, pos, this.Pet.Sprite.sourceRect, this.Colors[2], 0f, Vector2.Zero, 4f, (this.Pet.Sprite.CurrentAnimation != null && this.Pet.Sprite.CurrentAnimation[this.Pet.Sprite.currentAnimationIndex].flip) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, this.Pet.GetBoundingBox().Center.Y / 10000f - 0.000002f);
                b.Draw(this.Pet.Sprite.Texture, pos, this.Pet.Sprite.sourceRect, this.Colors[1], 0f, Vector2.Zero, 4f, (this.Pet.Sprite.CurrentAnimation != null && this.Pet.Sprite.CurrentAnimation[this.Pet.Sprite.currentAnimationIndex].flip) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, this.Pet.GetBoundingBox().Center.Y / 10000f - 0.000001f);
                b.Draw(this.Pet.Sprite.Texture, pos, this.Pet.Sprite.sourceRect, this.Colors[0], 0f, Vector2.Zero, 4f, (this.Pet.Sprite.CurrentAnimation != null && this.Pet.Sprite.CurrentAnimation[this.Pet.Sprite.currentAnimationIndex].flip) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, this.Pet.GetBoundingBox().Center.Y / 10000f);
            }
            if (this.Pet.IsEmoting)
            {
                Vector2 emotePosition = this.Pet.getLocalPosition(Game1.viewport);
                emotePosition.X += 32f;
                emotePosition.Y -= 64f;
                b.Draw(Game1.emoteSpriteSheet, emotePosition, new Rectangle(this.Pet.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, this.Pet.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, this.Pet.getStandingY() / 10000f + 0.0001f);
            }
        }

        public void OnUpdate(GameTime time)
        {
            this.Timer += time.ElapsedGameTime.Milliseconds;
            this.Colors[0] = ColorMap[(int)Math.Floor((this.Timer % 3000) / 30d)];
            this.Colors[1] = ColorMap[Math.Max(0, (int)Math.Floor((this.Timer % 3000) / 30d) - 1)];
            this.Colors[2] = ColorMap[Math.Max(0, (int)Math.Floor((this.Timer % 3000) / 30d) - 2)];
        }
    }
}
