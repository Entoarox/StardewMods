using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.Menus
{
    public class ClickableTextComponent : BaseInteractiveMenuComponent
    {
        protected SpriteFont Font;
        protected string Text;
        protected IClickHandler Handler;
        protected float Scale;
        protected bool Shadow;
        protected Color @Color;
        protected bool Hovered;
        public ClickableTextComponent(Point position, SpriteFont font, string text, IClickHandler handler, Color? color=null, float scale=1, bool shadow=true)
        {
            if (color == null)
                color = Game1.textColor;
            Vector2 s = (font.MeasureString(text) * scale) / Game1.pixelZoom;
            SetScaledArea(new Rectangle(position.X, position.Y, (int)Math.Ceiling(s.X), (int)Math.Ceiling(s.Y)));
            Font = font;
            Text = text;
            Handler = handler;
            Scale = scale;
            Shadow = shadow;
            @Color = (Color)color;
        }
        public override void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Handler.LeftClick(p, o, c, m);
        }
        public override void RightClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Handler.RightClick(p, o, c, m);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (Shadow)
                Utility.drawTextWithShadow(b, Text, Font, new Vector2(Area.X + o.X, Area.Y + o.Y), Color * (Hovered ? 1 : 0.8f), Scale);
            else
                b.DrawString(Font, Text, new Vector2(Area.X + o.X, Area.Y + o.Y), Color * (Hovered ? 1 : 0.8f), 0, Vector2.Zero, Scale, SpriteEffects.None, 1);
        }
    }
}