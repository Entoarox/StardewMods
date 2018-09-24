using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.Framework.UI
{
    public class CheckboxFormComponent : BaseFormComponent
    {
        /*********
        ** Fields
        *********/
        protected static readonly Rectangle SourceRectChecked = new Rectangle(236, 425, 9, 9);
        protected static readonly Rectangle SourceRectUnchecked = new Rectangle(227, 425, 9, 9);
        protected bool _Value;
        protected string Label;


        /*********
        ** Accessors
        *********/
        public event ValueChanged<bool> Handler;

        public bool Value
        {
            get => this._Value;
            set => this._Value = value;
        }


        /*********
        ** Public methods
        *********/
        public CheckboxFormComponent(Point offset, string label, ValueChanged<bool> handler = null)
        {
            this.SetScaledArea(new Rectangle(offset.X, offset.Y, 9 + this.GetStringWidth(label, Game1.smallFont), 9));
            this.Value = false;
            this.Label = label;
            if (handler != null)
                this.Handler += handler;
        }

        public override void LeftClick(Point p, Point o)
        {
            if (this.Disabled)
                return;
            Game1.playSound("drumkit6");
            this.Value = !this.Value;
            this.Handler?.Invoke(this, this.Parent, this.Parent.GetAttachedMenu(), this.Value);
        }

        public override void Draw(SpriteBatch b, Point o)
        {
            if (!this.Visible)
                return;
            b.Draw(Game1.mouseCursors, new Vector2(o.X + this.Area.X, o.Y + this.Area.Y), this.Value ? CheckboxFormComponent.SourceRectChecked : CheckboxFormComponent.SourceRectUnchecked, Color.White * (this.Disabled ? 0.33f : 1f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.4f);
            Utility.drawTextWithShadow(b, this.Label, Game1.smallFont, new Vector2(o.X + this.Area.X + BaseMenuComponent.Zoom10, o.Y + this.Area.Y + BaseMenuComponent.Zoom2), Game1.textColor * (this.Disabled ? 0.33f : 1f), 1f, 0.1f, -1, -1, 1f, 3);
        }
    }
}
