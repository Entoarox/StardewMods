using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.UI
{
    public class SliderFormComponent<T> : BaseFormComponent
    {
        /*********
        ** Fields
        *********/
        protected static readonly Rectangle Background = new Rectangle(403, 383, 6, 6);
        protected static readonly Rectangle Button = new Rectangle(420, 441, 10, 6);
        protected T _Value;
        protected ValueChanged<T> Handler;
        protected int Index;
        protected int Offset;
        protected int OldIndex;
        protected int OptionKey;
        protected List<T> Values;


        /*********
        ** Accessors
        *********/
        public T Value
        {
            get => this._Value;
            set
            {
                int i = this.Values.IndexOf(value);
                if (i == -1)
                    return;
                this.Index = i;
                this._Value = value;
            }
        }


        /*********
        ** Public methods
        *********/
        public SliderFormComponent(Point position, List<T> values, ValueChanged<T> handler = null)
            : this(position, 100, values, handler) { }

        public SliderFormComponent(Point position, int width, List<T> values, ValueChanged<T> handler = null)
        {
            this.Offset = (int)Math.Round((width - 10) * Game1.pixelZoom / (values.Count - 1D));
            this.SetScaledArea(new Rectangle(position.X, position.Y, width, 6));
            this.Values = values;
            this.Value = values[0];
            this.Index = 0;
            this.OldIndex = 0;
            if (handler != null)
                this.Handler += handler;
        }

        public override void LeftHeld(Point p, Point o)
        {
            if (this.Disabled)
                return;
            this.Index = Math.Max(Math.Min((int)Math.Floor((double)(p.X - (o.X + this.Area.X)) / this.Offset), this.Values.Count - 1), 0);
            this.Value = this.Values[this.Index];
        }

        public override void LeftUp(Point p, Point o)
        {
            if (this.OldIndex == this.Index)
                return;
            this.OldIndex = this.Index;
            this.Handler?.Invoke(this, this.Parent, this.Parent.GetAttachedMenu(), this.Value);
        }

        public override void LeftClick(Point p, Point o)
        {
            this.LeftHeld(p, o);
        }

        public override void Draw(SpriteBatch b, Point o)
        {
            if (!this.Visible)
                return;
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, SliderFormComponent<T>.Background, o.X + this.Area.X, o.Y + this.Area.Y, this.Area.Width, this.Area.Height, Color.White * (this.Disabled ? 0.33f : 1), Game1.pixelZoom, false);
            b.Draw(Game1.mouseCursors, new Vector2(o.X + this.Area.X + (this.Index == this.Values.Count - 1 ? this.Area.Width - BaseMenuComponent.Zoom10 : this.Index * this.Offset), o.Y + this.Area.Y), SliderFormComponent<T>.Button, Color.White * (this.Disabled ? 0.33f : 1), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.9f);
        }
    }

    public class SliderFormComponent : SliderFormComponent<int>
    {
        /*********
        ** Public methods
        *********/
        public SliderFormComponent(Point position, int steps, ValueChanged<int> handler = null)
            : this(position, 100, steps, handler) { }

        public SliderFormComponent(Point position, int width, int steps, ValueChanged<int> handler = null)
            : base(position, width, Enumerable.Range(0, steps).ToList(), handler) { }
    }
}
