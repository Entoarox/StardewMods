using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.Framework.UI
{
    public class LabelComponent : BaseMenuComponent
    {
        /*********
        ** Fields
        *********/
        protected static readonly Rectangle Center = new Rectangle(262, 267, 1, 16);
        protected static readonly Rectangle Left = new Rectangle(256, 267, 6, 16);
        protected static readonly Rectangle Right = new Rectangle(263, 267, 6, 16);
        protected string _Label;


        /*********
        ** Accessors
        *********/
        public string Label
        {
            get => this._Label;
            set
            {
                this._Label = value;
                this.Area.Width = (this.GetStringWidth(value, Game1.smallFont) + 12) * Game1.pixelZoom;
            }
        }


        /*********
        ** Public methods
        *********/
        public LabelComponent(Point position, string label)
        {
            this.SetScaledArea(new Rectangle(position.X, position.Y, this.GetStringWidth(label, Game1.smallFont) + 12, 16));
            this._Label = label;
        }

        public override void Draw(SpriteBatch b, Point o)
        {
            // Left
            b.Draw(Game1.mouseCursors, new Rectangle(o.X + this.Area.X, o.Y + this.Area.Y, BaseMenuComponent.Zoom6, this.Area.Height), LabelComponent.Left, Color.White);
            // Right
            b.Draw(Game1.mouseCursors, new Rectangle(o.X + this.Area.X + this.Area.Width - BaseMenuComponent.Zoom6, o.Y + this.Area.Y, BaseMenuComponent.Zoom6, this.Area.Height), LabelComponent.Right, Color.White);
            // Center
            b.Draw(Game1.mouseCursors, new Rectangle(o.X + this.Area.X + BaseMenuComponent.Zoom6, o.Y + this.Area.Y, this.Area.Width - BaseMenuComponent.Zoom12, this.Area.Height), LabelComponent.Center, Color.White);
            // Label
            Utility.drawTextWithShadow(b, this.Label, Game1.smallFont, new Vector2(o.X + this.Area.X + BaseMenuComponent.Zoom6, o.Y + this.Area.Y + BaseMenuComponent.Zoom5), Game1.textColor);
        }
    }
}
