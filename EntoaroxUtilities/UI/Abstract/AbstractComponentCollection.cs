using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entoarox.Utilities.UI.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Entoarox.Utilities.UI.Abstract
{
    public abstract class AbstractComponentCollection : IComponentCollection
    {
        protected bool Dirty = false;
        protected Dictionary<string, IComponent> ComponentMap = new Dictionary<string, IComponent>();

        protected IComponent[] DrawComponents = new IComponent[0];
        protected IUpdatingComponent[] UpdateComponents = new IUpdatingComponent[0];
        protected IInteractiveComponent[] InteractiveComponents = new IInteractiveComponent[0];

        public IComponent this[string componentId] => this.ComponentMap[componentId];
        public IComponentContainer Components => this;
        public IComponentMenu Menu => this.Container?.Menu ?? throw new InvalidOperationException("Cannot access the Menu of a IComponentCollection before it has been attached to one.");

        public virtual Rectangle ComponentRegion => this.DisplayRegion;
        public virtual Rectangle FocusRegion => this.DisplayRegion;
        public virtual IComponentContainer Container { get; set; }
        public virtual Rectangle DisplayRegion { get; set; }
        public virtual bool Enabled { get; set; }

        public virtual IInteractiveComponent NextUp { get; set; }
        public virtual IInteractiveComponent NextDown { get; set; }
        public virtual IInteractiveComponent NextLeft { get; set; }
        public virtual IInteractiveComponent NextRight { get; set; }
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

        public void MarkDirty()
        {
            this.Dirty = true;
        }

        public virtual void Add(IComponent component)
        {
            this.ComponentMap.Add(component.Id, component);
            this.MarkDirty();
        }

        public virtual bool Contains(IComponent component)
        {
            return this.ComponentMap.ContainsKey(component.Id) && this.ComponentMap[component.Id] == component;
        }

        public virtual bool Contains(string componentId)
        {
            return this.ComponentMap.ContainsKey(componentId);
        }

        public virtual IEnumerator<IComponent> GetEnumerator()
        {
            return this.ComponentMap.Values.GetEnumerator();
        }

        public virtual void Remove(IComponent component)
        {
            if (this.Contains(component))
            {
                this.ComponentMap.Remove(component.Id);
                this.MarkDirty();
            }
        }

        public virtual void Remove(string componentId)
        {
            if (this.ComponentMap.ContainsKey(componentId))
            {
                this.ComponentMap.Remove(componentId);
                this.MarkDirty();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public virtual void Draw(Rectangle screenRect, Rectangle drawRect, SpriteBatch batch)
        {
            this.Draw(drawRect, batch);
        }

        public virtual void Draw(Rectangle drawRect, SpriteBatch batch)
        {
            int x = drawRect.X + this.ComponentRegion.X;
            int y = drawRect.Y + this.ComponentRegion.Y;
            int l = this.DrawComponents.Length;
            for (int c=0;c < l;c++)
            {
                var o = this.DrawComponents[c];
                o.Draw(new Rectangle(o.DisplayRegion.X + x, o.DisplayRegion.Y + y, o.DisplayRegion.Width, o.DisplayRegion.Height), batch);
            }
        }

        public virtual void Update(GameTime time)
        {
            if(this.Dirty)
            {
                this.UpdateComponentCache();
                this.Dirty = false;
            }
            int l = this.UpdateComponents.Length;
            for (int c=0;c<l;c++)
            {
                this.UpdateComponents[c].Update(time);
            }
        }

        protected virtual void UpdateComponentCache()
        {
            this.DrawComponents = this.Where(_ => _.Visible).OrderBy(_ => _.Layer).ToArray();
            this.UpdateComponents = this.Where(_ => _.Visible && _ is IUpdatingComponent).OrderBy(_ => _.Layer).Cast<IUpdatingComponent>().ToArray();
            this.InteractiveComponents = this.Where(_ => _.Visible && _ is IInteractiveComponent c && c.Enabled).OrderBy(_ => _.Layer).Cast<IInteractiveComponent>().ToArray();
        }

        public virtual void FocusGained()
        {
            throw new NotImplementedException("Collections should never have focus and thus never receive this event!");
        }
        public virtual void FocusLost()
        {
            throw new NotImplementedException("Collections should never have focus and thus never receive this event!");
        }

        public virtual void KeyDown(Keys key)
        {
            throw new NotImplementedException("Collections should never have focus and thus never receive this event!");
        }
        public virtual void KeyHeld(Keys key)
        {
            throw new NotImplementedException("Collections should never have focus and thus never receive this event!");
        }
        public virtual void KeyUp(Keys key)
        {
            throw new NotImplementedException("Collections should never have focus and thus never receive this event!");
        }

        public virtual void LeftDown(Point position)
        {

        }
        public virtual void LeftHeld(Point position)
        {

        }
        public virtual void LeftUp(Point position)
        {

        }

        public virtual void MouseIn(Point position)
        {

        }
        public virtual void MouseMove(Point position)
        {

        }
        public virtual void MouseOut(Point position)
        {

        }

        public virtual void RightDown(Point position)
        {

        }
        public virtual void RightHeld(Point position)
        {

        }
        public virtual void RightUp(Point position)
        {

        }

        public int Scroll(int scrollAmount)
        {
            return scrollAmount;
        }
    }
}
