using Entoarox.Utilities.UI.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Utilities.UI.Abstract
{
    public abstract class AbstractComponent : IComponent
    {
        public virtual IComponentContainer Container { get; set; }

        public string Id { get; set; }

        protected int _Layer = 0;
        public int Layer
        {
            get => this._Layer;
            set
            {
                this._Layer = value;
                this.Container?.MarkDirty();
            }
        }

        protected bool _Visible = true;
        public bool Visible
        {
            get => this._Visible;
            set
            {
                this._Visible = value;
                this.Container?.MarkDirty();
            }
        }

        public virtual Rectangle DisplayRegion { get; set; }

        public abstract void Draw(Rectangle drawRect, SpriteBatch batch);
    }
}
