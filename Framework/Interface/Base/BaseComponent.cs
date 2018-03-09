using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.Interface
{
    public abstract class BaseComponent : IComponent
    {
        public BaseComponent(string name, Rectangle bounds, int layer)
        {
            this.Name = name;
            this.Layer = layer;
            this.OuterBounds = bounds;
        }

        public int Layer { get; set; }

        private bool _Visible;
        public bool Visible
        {
            get => this._Visible;
            set
            {
                if(value!=this._Visible)
                {
                    this._Visible = value;
                    (this._Owner as IVisibilityObserver)?.VisibilityChanged(this);
                }
            }
        }
        public virtual Rectangle OuterBounds { get; set; }

        public string Name { get; private set; }
        private IComponentContainer _Owner;
        public IComponentContainer Owner => this._Owner ?? throw new InvalidOperationException(Strings.ComponentNotAttached);

        public bool IsAttached { get; private set; } = false;

        public virtual void Attach(IComponentContainer collection)
        {
            if (this.IsAttached)
                throw new InvalidOperationException(Strings.ComponentAttached);
            this.IsAttached = true;
            this._Owner = collection;
        }

        public virtual void Detach(IComponentContainer collection)
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
