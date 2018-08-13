using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.UI
{
    public abstract class BaseMenuComponent : IMenuComponent
    {
        protected static readonly int zoom05 = Game1.pixelZoom / 2;
        protected static readonly int zoom2 = 2 * Game1.pixelZoom;
        protected static readonly int zoom3 = 3 * Game1.pixelZoom;
        protected static readonly int zoom4 = 4 * Game1.pixelZoom;
        protected static readonly int zoom5 = 5 * Game1.pixelZoom;
        protected static readonly int zoom6 = 6 * Game1.pixelZoom;
        protected static readonly int zoom7 = 7 * Game1.pixelZoom;
        protected static readonly int zoom8 = 8 * Game1.pixelZoom;
        protected static readonly int zoom9 = 9 * Game1.pixelZoom;
        protected static readonly int zoom10 = 10 * Game1.pixelZoom;
        protected static readonly int zoom11 = 11 * Game1.pixelZoom;
        protected static readonly int zoom12 = 12 * Game1.pixelZoom;
        protected static readonly int zoom13 = 13 * Game1.pixelZoom;
        protected static readonly int zoom14 = 14 * Game1.pixelZoom;
        protected static readonly int zoom15 = 15 * Game1.pixelZoom;
        protected static readonly int zoom16 = 16 * Game1.pixelZoom;
        protected static readonly int zoom17 = 17 * Game1.pixelZoom;
        protected static readonly int zoom20 = 20 * Game1.pixelZoom;
        protected static readonly int zoom22 = 22 * Game1.pixelZoom;
        protected static readonly int zoom28 = 28 * Game1.pixelZoom;
        protected Rectangle Area;
        protected Texture2D Texture;
        protected Rectangle Crop;
        public IComponentContainer Parent
        {
            get
            {
                if (this._Parent == null)
                    throw new NullReferenceException("Component attempted to reference its parent while not attached");
                return this._Parent;
            }
            private set
            {
                this._Parent = value;
            }
        }
        private IComponentContainer _Parent=null;
        public bool Visible { get; set; } = true;
        public int Layer { get; set; } = 0;
        protected void SetScaledArea(Rectangle area)
        {
            this.Area = new Rectangle(area.X * Game1.pixelZoom, area.Y * Game1.pixelZoom, area.Width * Game1.pixelZoom, area.Height * Game1.pixelZoom);
        }
        protected int GetStringWidth(string text, SpriteFont font, float scale = 1f)
        {
            return (int)Math.Ceiling(font.MeasureString(text).X / Game1.pixelZoom * scale);
        }
        protected BaseMenuComponent()
        {

        }
        public BaseMenuComponent(Rectangle area, Texture2D texture, Rectangle? crop = null)
        {
            if (crop == null)
                crop = new Rectangle(0, 0, texture.Width, texture.Height);
            this.Texture = texture;
            this.Crop = (Rectangle)crop;
            SetScaledArea(area);
        }
        public void Attach(IComponentContainer collection)
        {
            if (this._Parent !=null)
                throw new Exception("Component is already attached and must be detached first before it can be attached again");
            OnAttach(collection);
            this._Parent = collection;
        }
        public void Detach(IComponentContainer collection)
        {
            if (this._Parent ==null)
                throw new Exception("Component is not attached and must be attached first before it can be detached");
            OnDetach(this.Parent);
            this._Parent = null;
        }
        public virtual void OnAttach(IComponentContainer parent)
        {

        }
        public virtual void OnDetach(IComponentContainer parent)
        {

        }
        public virtual Point GetPosition()
        {
            return new Point(this.Area.X, this.Area.Y);
        }
        public virtual Rectangle GetRegion()
        {
            return this.Area;
        }
        public virtual void Update(GameTime t)
        {

        }
        public virtual void Draw(SpriteBatch b, Point o)
        {
            if (this.Visible)
                b.Draw(this.Texture, new Rectangle(this.Area.X + o.X, this.Area.Y + o.Y, this.Area.Width, this.Area.Height), this.Crop, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1f);
        }
    }
}
