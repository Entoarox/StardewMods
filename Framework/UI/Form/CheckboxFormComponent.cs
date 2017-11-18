using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.UI
{
    public class CheckboxFormComponent : BaseFormComponent
    {
        protected readonly static Rectangle sourceRectUnchecked = new Rectangle(227, 425, 9, 9);
        protected readonly static Rectangle sourceRectChecked = new Rectangle(236, 425, 9, 9);
        public event ValueChanged<bool> Handler;
        public bool Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this._Value = value;
            }
        }
        protected bool _Value;
        protected string Label;
        public CheckboxFormComponent(Point offset, string label, ValueChanged<bool> handler=null)
        {
            SetScaledArea(new Rectangle(offset.X, offset.Y, 9 + GetStringWidth(label, Game1.smallFont), 9));
            this.Value = false;
            this.Label = label;
            if(handler!=null)
                Handler += handler;
        }
        public override void LeftClick(Point p, Point o)
        {
            if (this.Disabled)
                return;
            Game1.playSound("drumkit6");
            this.Value = !this.Value;
            Handler?.Invoke(this, this.Parent, this.Parent.GetAttachedMenu(), this.Value);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (!this.Visible)
                return;
            b.Draw(Game1.mouseCursors, new Vector2(o.X + this.Area.X, o.Y + this.Area.Y), this.Value ? sourceRectChecked : sourceRectUnchecked, Color.White * (this.Disabled ? 0.33f : 1f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.4f);
            Utility.drawTextWithShadow(b, this.Label, Game1.smallFont, new Vector2(o.X + this.Area.X + zoom10, o.Y + this.Area.Y+zoom2), Game1.textColor * (this.Disabled ? 0.33f : 1f), 1f, 0.1f, -1, -1, 1f, 3);
        }
    }
}
