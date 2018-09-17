using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Interface
{
    public abstract class BaseComponent : IComponent
    {
        /*********
        ** Fields
        *********/
        private IComponentContainer _Owner;
        private bool _Visible;


        /*********
        ** Accessors
        *********/
        public int Layer { get; set; }

        public bool Visible
        {
            get => this._Visible;
            set
            {
                if (value != this._Visible)
                {
                    this._Visible = value;
                    (this._Owner as IVisibilityObserver)?.VisibilityChanged(this);
                }
            }
        }

        public virtual Rectangle OuterBounds { get; set; }
        public string Name { get; }
        public IComponentContainer Owner => this._Owner ?? throw new InvalidOperationException(Strings.ComponentNotAttached);
        public bool IsAttached { get; private set; }


        /*********
        ** Public methods
        *********/
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

        public virtual void Update(GameTime time) { }


        /*********
        ** Protected methods
        *********/
        protected BaseComponent(string name, Rectangle bounds, int layer)
        {
            this.Name = name;
            this.Layer = layer;
            this.OuterBounds = bounds;
        }
    }
}
