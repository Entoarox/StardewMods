using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.Framework.UI
{
    public class AnimatedComponent : BaseMenuComponent
    {
        /*********
        ** Fields
        *********/
        protected TemporaryAnimatedSprite Sprite;


        /*********
        ** Public methods
        *********/
        public AnimatedComponent(Point position, TemporaryAnimatedSprite sprite)
        {
            this.SetScaledArea(new Rectangle(position.X, position.Y, sprite.sourceRect.Width, sprite.sourceRect.Height));
            this.Sprite = sprite;
        }

        public override void Update(GameTime t)
        {
            this.Sprite.update(t);
        }

        public override void Draw(SpriteBatch b, Point o)
        {
            if (this.Visible)
                this.Sprite.draw(b, false, o.X + this.Area.X, o.Y + this.Area.Y);
        }
    }
}
