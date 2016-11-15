using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.Menus
{
    public class SliderFormComponent<T> : BaseFormComponent
    {
        protected static Rectangle Background = new Rectangle(403, 383, 6, 6);
        protected static Rectangle Button = new Rectangle(420, 441, 10, 6);
        public T Value
        {
            get
            {
                return _Value;
            }
            set
            {
                int i = Values.IndexOf(value);
                if (i == -1)
                    return;
                Index = i;
                _Value = value;
            }
        }
        protected T _Value;
        protected int OldIndex;
        protected int Offset;
        protected int Index;
        protected List<T> Values;
        protected int OptionKey;
        protected ValueChanged<T> Handler;
        public SliderFormComponent(Point position, List<T> values, ValueChanged<T> handler=null) : this(position, 100, values, handler)
        {
        }
        public SliderFormComponent(Point position, int width, List<T> values, ValueChanged<T> handler=null)
        {
            Offset = (int)Math.Round((width - 10) / (values.Count - 1D));
            SetScaledArea(new Rectangle(position.X, position.Y, width, 6));
            Values = values;
            Value = values[0];
            Index = 0;
            OldIndex = 0;
            if(handler!=null)
                Handler += handler;
        }
        public override void LeftHeld(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            if (Disabled)
                return;
            Index = Math.Max(Math.Min((int)Math.Floor((p.X - o.X) / Offset / Game1.pixelZoom * 1D), Values.Count - 1), 0);
            Value = Values[Index];
        }
        public override void LeftUp(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            if (OldIndex == Index)
                return;
            OldIndex = Index;
            Handler?.Invoke(this, c, m, Value);
        }
        public override void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            LeftHeld(p, o, c, m);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (!Visible)
                return;
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, Background, o.X + Area.X, o.Y + Area.Y, Area.Width, Area.Height, Color.White, Game1.pixelZoom, false);
            b.Draw(Game1.mouseCursors, new Vector2(o.X + Area.X + (Index == Values.Count - 1 ? Area.Width - 10 * Game1.pixelZoom : (Index * Offset * Game1.pixelZoom)), o.Y + Area.Y), new Rectangle?(Button), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.9f);
        }
    }
    public class SliderFormComponent : SliderFormComponent<int>
    {
        public SliderFormComponent(Point position, int steps, ValueChanged<int> handler=null) : this(position, 100, steps, handler)
        {
        }
        public SliderFormComponent(Point position, int width, int steps, ValueChanged<int> handler=null) : base(position, width, Enumerable.Range(0, steps).ToList(), handler)
        {
        }
    }
}
