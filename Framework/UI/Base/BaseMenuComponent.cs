using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.Framework.UI
{
    public abstract class BaseMenuComponent : IMenuComponent
    {
        /*********
        ** Fields
        *********/
        protected static readonly int Zoom05 = Game1.pixelZoom / 2;
        protected static readonly int Zoom2 = 2 * Game1.pixelZoom;
        protected static readonly int Zoom3 = 3 * Game1.pixelZoom;
        protected static readonly int Zoom4 = 4 * Game1.pixelZoom;
        protected static readonly int Zoom5 = 5 * Game1.pixelZoom;
        protected static readonly int Zoom6 = 6 * Game1.pixelZoom;
        protected static readonly int Zoom7 = 7 * Game1.pixelZoom;
        protected static readonly int Zoom8 = 8 * Game1.pixelZoom;
        protected static readonly int Zoom9 = 9 * Game1.pixelZoom;
        protected static readonly int Zoom10 = 10 * Game1.pixelZoom;
        protected static readonly int Zoom11 = 11 * Game1.pixelZoom;
        protected static readonly int Zoom12 = 12 * Game1.pixelZoom;
        protected static readonly int Zoom13 = 13 * Game1.pixelZoom;
        protected static readonly int Zoom14 = 14 * Game1.pixelZoom;
        protected static readonly int Zoom15 = 15 * Game1.pixelZoom;
        protected static readonly int Zoom16 = 16 * Game1.pixelZoom;
        protected static readonly int Zoom17 = 17 * Game1.pixelZoom;
        protected static readonly int Zoom20 = 20 * Game1.pixelZoom;
        protected static readonly int Zoom22 = 22 * Game1.pixelZoom;
        protected static readonly int Zoom28 = 28 * Game1.pixelZoom;
        protected Rectangle Area;
        protected Rectangle Crop;
        protected Texture2D Texture;
        private IComponentContainer _Parent;


        /*********
        ** Accessors
        *********/
        public IComponentContainer Parent
        {
            get
            {
                if (this._Parent == null)
                    throw new NullReferenceException("Component attempted to reference its parent while not attached");
                return this._Parent;
            }
            private set => this._Parent = value;
        }

        public bool Visible { get; set; } = true;
        public int Layer { get; set; } = 0;


        /*********
        ** Public methods
        *********/
        public void Attach(IComponentContainer collection)
        {
            if (this._Parent != null)
                throw new Exception("Component is already attached and must be detached first before it can be attached again");
            this.OnAttach(collection);
            this._Parent = collection;
        }

        public void Detach(IComponentContainer collection)
        {
            if (this._Parent == null)
                throw new Exception("Component is not attached and must be attached first before it can be detached");
            this.OnDetach(this.Parent);
            this._Parent = null;
        }

        public virtual void OnAttach(IComponentContainer parent) { }

        public virtual void OnDetach(IComponentContainer parent) { }

        public virtual Point GetPosition()
        {
            return new Point(this.Area.X, this.Area.Y);
        }

        public virtual Rectangle GetRegion()
        {
            return this.Area;
        }

        public virtual void Update(GameTime t) { }

        public virtual void Draw(SpriteBatch b, Point o)
        {
            if (this.Visible)
                b.Draw(this.Texture, new Rectangle(this.Area.X + o.X, this.Area.Y + o.Y, this.Area.Width, this.Area.Height), this.Crop, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1f);
        }


        /*********
        ** Protected methods
        *********/
        protected BaseMenuComponent() { }

        protected BaseMenuComponent(Rectangle area, Texture2D texture, Rectangle? crop = null)
        {
            if (crop == null)
                crop = new Rectangle(0, 0, texture.Width, texture.Height);
            this.Texture = texture;
            this.Crop = (Rectangle)crop;
            this.SetScaledArea(area);
        }

        protected void SetScaledArea(Rectangle area)
        {
            this.Area = new Rectangle(area.X * Game1.pixelZoom, area.Y * Game1.pixelZoom, area.Width * Game1.pixelZoom, area.Height * Game1.pixelZoom);
        }

        protected int GetStringWidth(string text, SpriteFont font, float scale = 1f)
        {
            return (int)Math.Ceiling(font.MeasureString(text).X / Game1.pixelZoom * scale);
        }
    }
}
