using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.Interface
{
    public abstract class BaseComponent : IComponent
    {
        protected BaseComponent(string name, Rectangle bounds, int layer)
        {
            this.Name = name;
            this.Layer = layer;
            this.OuterBounds = bounds;
        }
        protected Rectangle GetDrawRectangle(Point offset, Rectangle rect) => new Rectangle((offset.X + rect.X) * Game1.pixelZoom, (offset.Y + rect.Y) * Game1.pixelZoom, rect.Width * Game1.pixelZoom, rect.Height * Game1.pixelZoom);
        protected Vector2 GetDrawVector(Point offset, Rectangle rect) => new Vector2((offset.X + rect.X) * Game1.pixelZoom, (offset.Y + rect.Y) * Game1.pixelZoom);
        protected Rectangle GetRealRectangle(Rectangle rect) => new Rectangle(rect.X * Game1.pixelZoom, rect.Y * Game1.pixelZoom, rect.Width * Game1.pixelZoom, rect.Height * Game1.pixelZoom);
        protected Rectangle GetZoomRectangle(Rectangle rect) => new Rectangle((int)Math.Floor(rect.X / Game1.pixelZoom + 0f), (int)Math.Floor(rect.Y / Game1.pixelZoom + 0f), (int)Math.Ceiling(rect.Width / Game1.pixelZoom + 0f), (int)Math.Ceiling(rect.Height / Game1.pixelZoom + 0f));

        public int Layer { get; set; }
        public bool Visible { get; set; }
        public virtual Rectangle OuterBounds { get; set; }

        public string Name { get; private set; }
        private IComponentCollection _Owner;
        public IComponentCollection Owner { get => _Owner ?? throw new InvalidOperationException(Strings.ComponentNotAttached); }

        public bool IsAttached { get; private set; } = false;

        public virtual void Attach(IComponentCollection collection)
        {
            if (this.IsAttached)
                throw new InvalidOperationException(Strings.ComponentAttached);
            this.IsAttached = true;
            this._Owner = collection;
        }

        public virtual void Detach(IComponentCollection collection)
        {
            if (!this.IsAttached)
                throw new ArgumentException(Strings.ComponentNotAttached);
            this.IsAttached = false;
            this._Owner = null;
        }

        public abstract void Draw(Point offset, SpriteBatch batch);

        public virtual void Update(GameTime time)
        {
            
        }
    }
}
