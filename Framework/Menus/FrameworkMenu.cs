using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

#pragma warning disable CS0618 // Type or member is obsolete
/// <summary>
/// WIP, do not use!
/// </summary>
namespace Entoarox.Framework.Menus
{
    [Obsolete("Work in progress, do not use")]
    public interface IMenuComponent
    {
        void Update(GameTime t, IComponentCollection collection, FrameworkMenu menu);
        void Draw(SpriteBatch b, Point offset);
    }
    public interface IInteractiveMenuComponent : IMenuComponent
    {
        bool InBounds(Point position, Point offset);
        void LeftClick(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
        void RightClick(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
        void LeftHeld(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
        void LeftUp(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
        void HoverIn(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
        void HoverOut(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
        void HoverOver(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
        void Scroll(int direction, Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
    }
    public interface IComponentCollection
    {
        bool AcceptsComponent(IMenuComponent component);
        void AddComponent(IMenuComponent component);
        void RemoveComponent(IMenuComponent component);
        void RemoveComponents<T>() where T : IMenuComponent;
        void RemoveComponents(Predicate<IMenuComponent> filter);
        void ClearComponents();
    }
    public interface IClickHandler
    {
        void LeftClick(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
        void RightClick(Point position, Point offset, IComponentCollection collection, FrameworkMenu menu);
    }
    [Obsolete("Work in progress, do not use")]
    public class FrameworkMenu : IClickableMenu, IComponentCollection
    {
        protected List<IMenuComponent> StaticComponents = new List<IMenuComponent>();
        protected List<IInteractiveMenuComponent> InteractiveComponents = new List<IInteractiveMenuComponent>();
        protected Rectangle Area;
        protected IInteractiveMenuComponent HoverInElement;
        protected bool DrawChrome;
        protected Vector2 Center;
        public FrameworkMenu(Rectangle area, bool showCloseButton = true, bool drawChrome = true)
        {
            DrawChrome = drawChrome;
            Area = new Rectangle(area.X * Game1.pixelZoom, area.Y * Game1.pixelZoom, area.Width * Game1.pixelZoom, area.Height * Game1.pixelZoom);
            initialize(Area.X, Area.Y, Area.Width, Area.Height, showCloseButton);
        }
        public FrameworkMenu(Point size, bool showCloseButton = true, bool drawChrome = true)
        {
            DrawChrome = drawChrome;
            Center = Utility.getTopLeftPositionForCenteringOnScreen(size.X * Game1.pixelZoom, size.Y * Game1.pixelZoom, 0, 0);
            Area = new Rectangle((int)Center.X, (int)Center.Y, size.X * Game1.pixelZoom, size.Y * Game1.pixelZoom);
            initialize(Area.X, Area.Y, Area.Width, Area.Height, showCloseButton);
        }
        public void AddComponent(IMenuComponent component)
        {
            if (component is IInteractiveMenuComponent)
                InteractiveComponents.Add(component as IInteractiveMenuComponent);
            else
                StaticComponents.Add(component);
        }
        public void RemoveComponent(IMenuComponent component)
        {
            if (component is IInteractiveMenuComponent)
                InteractiveComponents.Remove(component as IInteractiveMenuComponent);
            else
                StaticComponents.Remove(component);
        }
        public void RemoveComponents<T>() where T : IMenuComponent
        {
            InteractiveComponents.RemoveAll((a)=> a.GetType()==typeof(T));
            StaticComponents.RemoveAll((a) => a.GetType() == typeof(T));
        }
        public void RemoveComponents(Predicate<IMenuComponent> filter)
        {
            InteractiveComponents.RemoveAll(filter);
            StaticComponents.RemoveAll(filter);
        }
        public void ClearComponents()
        {
            InteractiveComponents.Clear();
            StaticComponents.Clear();
        }
        public bool AcceptsComponent(IMenuComponent component)
        {
            return true;
        }
        public override void releaseLeftClick(int x, int y)
        {
            Point p = new Point(x, y);
            Point o = new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom);
            foreach (IInteractiveMenuComponent el in InteractiveComponents)
                if (el.InBounds(p, o))
                    el.LeftUp(p, o, this, this);
        }
        public override void leftClickHeld(int x, int y)
        {
            Point p = new Point(x, y);
            Point o = new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom);
            foreach (IInteractiveMenuComponent el in InteractiveComponents)
                if (el.InBounds(p, o))
                    el.LeftHeld(p, o, this, this);
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            Point p = new Point(x, y);
            Point o = new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom);
            foreach (IInteractiveMenuComponent el in InteractiveComponents)
                if (el.InBounds(p,o))
                    el.LeftClick(p,o, this, this);
        }
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            Point p = new Point(x, y);
            Point o = new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom);
            foreach (IInteractiveMenuComponent el in InteractiveComponents)
                if (el.InBounds(p,o))
                    el.RightClick(p, o, this, this);
        }
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (!Area.Contains(x, y))
                return;
            Point p = new Point(x, y);
            Point o = new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom);
            if (HoverInElement != null && !HoverInElement.InBounds(p,o))
            {
                HoverInElement.HoverOut(p,o, this, this);
                HoverInElement = null;
            }
            foreach (IInteractiveMenuComponent el in InteractiveComponents)
                if (el.InBounds(p,o))
                {
                    if(HoverInElement==null)
                    {
                        HoverInElement = el;
                        el.HoverIn(p,o, this, this);
                    }
                    el.HoverOver(p,o, this, this);
                }
        }
        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            Point p = Game1.getMousePosition();
            Point o = new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom);
            foreach (IInteractiveMenuComponent el in InteractiveComponents)
                el.Scroll(direction, p,o, this, this);
        }
        public override void update(GameTime time)
        {
            base.update(time);
            foreach (IInteractiveMenuComponent el in InteractiveComponents)
                el.Update(time, this, this);
        }
        public override void draw(SpriteBatch b)
        {
            if (DrawChrome)
                //Game1.drawDialogueBox(Area.X,Area.Y,Area.Width,Area.Height, false, true);
                drawTextureBox(b, Area.X, Area.Y, Area.Width, Area.Height, Color.White);
            Point o = new Point(Area.X + 5 * Game1.pixelZoom, Area.Y + 5 * Game1.pixelZoom);
            foreach (IMenuComponent el in StaticComponents)
                el.Draw(b, o);
            foreach (IInteractiveMenuComponent el in InteractiveComponents)
                el.Draw(b, o);
            base.draw(b);
            drawMouse(b);
        }
    }
    abstract public class BaseMenuComponent : IMenuComponent
    {
        protected Rectangle Area;
        protected Texture2D Texture;
        protected Rectangle Crop;
        public bool Visible;
        protected void SetScaledArea(Rectangle area)
        {
            Area = new Rectangle(area.X * Game1.pixelZoom, area.Y * Game1.pixelZoom, area.Width * Game1.pixelZoom, area.Height * Game1.pixelZoom);
        }
        protected int GetStringWidth(string text, SpriteFont font, float scale=1f)
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
            Texture = texture;
            Crop = (Rectangle)crop;
            SetScaledArea(area);
        }
        public virtual void Update(GameTime t, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void Draw(SpriteBatch b, Point offset)
        {
            if (Visible)
                b.Draw(Texture, new Vector2(Area.X + offset.X, Area.Y + offset.Y), Crop, Color.White, 0, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);
        }
    }
    abstract public class BaseInteractiveMenuComponent : BaseMenuComponent, IInteractiveMenuComponent
    {
        protected BaseInteractiveMenuComponent()
        {

        }
        public BaseInteractiveMenuComponent(Rectangle area, Texture2D texture, Rectangle? crop = null) : base(area, texture, crop)
        {

        }
        public virtual bool InBounds(Point p, Point o)
        {
            Rectangle Offset = new Rectangle(Area.X + o.X, Area.Y + o.Y, Area.Width, Area.Height);
            return Offset.Contains(p);
        }
        public virtual void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void RightClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void LeftHeld(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void RightHeld(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void LeftUp(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void RightUp(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void HoverIn(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void HoverOut(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void HoverOver(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void Scroll(int d, Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {

        }
    }
    public class TextureComponent : BaseMenuComponent
    {
        public TextureComponent(Rectangle area, Texture2D texture, Rectangle? crop = null) : base(area, texture, crop)
        {

        }
    }
    public class AnimatedComponent : BaseMenuComponent
    {
        protected TemporaryAnimatedSprite Sprite;
        public AnimatedComponent(Rectangle area, TemporaryAnimatedSprite sprite)
        {
            SetScaledArea(area);
            Sprite = sprite;
        }
        public override void Update(GameTime t, IComponentCollection c, FrameworkMenu m)
        {
            Sprite.update(t);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if(Visible)
                Sprite.draw(b, false, o.X, o.Y);
        }
    }
    public class ClickableTextureComponent : BaseInteractiveMenuComponent
    {
        protected IClickHandler Handler;
        protected bool ScaleOnHover;
        public ClickableTextureComponent(Rectangle area, Texture2D texture, IClickHandler handler, Rectangle? crop = null, bool scaleOnHover=true) : base(area, texture, crop)
        {
            Handler = handler;
            ScaleOnHover = scaleOnHover;
        }
        public override void HoverIn(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Area.X -= 2;
            Area.Y -= 2;
            Area.Width += 4;
            Area.Height += 4;
        }
        public override void HoverOut(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Area.X += 2;
            Area.Y += 2;
            Area.Width -= 4;
            Area.Height -= 4;
        }
        public override void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Handler.LeftClick(p, o, c, m);
        }
        public override void RightClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Handler.RightClick(p, o, c, m);
        }
    }
    public class ClickableAnimatedComponent : BaseInteractiveMenuComponent
    {
        protected TemporaryAnimatedSprite Sprite;
        protected IClickHandler Handler;
        protected bool ScaleOnHover;
        public ClickableAnimatedComponent(Rectangle area, TemporaryAnimatedSprite sprite, IClickHandler handler, bool scaleOnHover = true)
        {
            Handler = handler;
            ScaleOnHover = scaleOnHover;
            Sprite = sprite;
            SetScaledArea(area);
        }
        public override void HoverIn(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Area.X -= 2;
            Area.Y -= 2;
            Area.Width += 4;
            Area.Height += 4;
        }
        public override void HoverOut(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Area.X += 2;
            Area.Y += 2;
            Area.Width -= 4;
            Area.Height -= 4;
        }
        public override void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Handler.LeftClick(p, o, c, m);
        }
        public override void RightClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Handler.RightClick(p, o, c, m);
        }
        public override void Update(GameTime t, IComponentCollection c, FrameworkMenu m)
        {
            Sprite.update(t);
        }
        public override void Draw(SpriteBatch b, Point offset)
        {
            if(Visible)
                Sprite.draw(b, false, offset.X, offset.Y);
        }
    }
    abstract public class BaseFormComponent : BaseInteractiveMenuComponent
    {
        public bool Disabled=false;
        /// <summary>
        /// Called whenever a form components value is changed, the type of the value depends on the individual components
        /// </summary>
        /// <typeparam name="T">The type of the value as declared by the form component</typeparam>
        /// <param name="key">The number given in the component constructor as a pseudo-unique ID for this element</param>
        /// <param name="value">The new value that this element now has</param>
        public delegate void ValueChanged<T>(int optionKey, T value);
    }
    public class CheckboxFormComponent : BaseFormComponent
    {
        protected static Rectangle sourceRectUnchecked = new Rectangle(227, 425, 9, 9);
        protected static Rectangle sourceRectChecked = new Rectangle(236, 425, 9, 9);
        public bool Checked;
        protected string Label;
        protected ValueChanged<bool> Handler;
        protected int OptionKey;
        public CheckboxFormComponent(Point offset, string label, int optionKey,  ValueChanged<bool> handler, bool @checked=false)
        {
            SetScaledArea(new Rectangle(offset.X, offset.Y, 9 + GetStringWidth(label, Game1.dialogueFont), 9));
            Checked = @checked;
            Label = label;
            Handler = handler;
            OptionKey = optionKey;
        }
        public override void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Console.WriteLine("Checkbox.LeftClick");
            if (Disabled)
                return;
            Game1.playSound("drumkit6");
            Checked = !Checked;
            Handler(OptionKey, Checked);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            b.Draw(Game1.mouseCursors, new Vector2(o.X + Area.X, o.Y+Area.Y), Checked ? sourceRectChecked : sourceRectUnchecked, Color.White * (Disabled ? 0.33f : 1f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.4f);
            Utility.drawTextWithShadow(b, Label, Game1.dialogueFont, new Vector2(o.X+Area.X + 9*Game1.pixelZoom, o.Y+Area.Y), Game1.textColor * (Disabled ? 0.33f : 1f), 1f, 0.1f, -1, -1, 1f, 3);
        }
    }
    public class PlusMinusFormComponent : BaseFormComponent
    {
        protected static Rectangle PlusButton = new Rectangle(185, 345, 6, 8);
        protected static Rectangle MinusButton = new Rectangle(177, 345, 6, 8);
        protected static Rectangle Background = new Rectangle(227, 425, 9, 9);
        public int Value;
        protected int MinValue;
        protected int MaxValue;
        protected ValueChanged<int> Handler;
        //protected int ButtonOffset;
        protected Rectangle PlusArea;
        protected Rectangle MinusArea;
        protected int Counter = 0;
        protected int Limiter = 10;
        protected int OptionKey;
        public PlusMinusFormComponent(Point position, int minValue, int maxValue, int optionKey, ValueChanged<int> handler)
        {
            int width = Math.Max(GetStringWidth(minValue.ToString(), Game1.smallFont), GetStringWidth(maxValue.ToString(), Game1.smallFont))+2;
            SetScaledArea(new Rectangle(position.X, position.Y, 16+width, 8));
            MinusArea = new Rectangle(Area.X, Area.Y, 7 * Game1.pixelZoom, Area.Height);
            PlusArea = new Rectangle(Area.X+Area.Width-7*Game1.pixelZoom, Area.Y, 7 * Game1.pixelZoom, Area.Height);
            MinValue = minValue;
            MaxValue = maxValue;
            Handler = handler;
            //ButtonOffset = (int)Math.Round((Area.Height - 8) / 2D);
            OptionKey = optionKey;
        }
        private void Resolve(Point p, Point o)
        {
            Rectangle PlusAreaOffset = new Rectangle(PlusArea.X + o.X, PlusArea.Y + o.Y, PlusArea.Height, PlusArea.Width);
            if (PlusAreaOffset.Contains(p) && Value < MaxValue)
            {
                Value++;
                Game1.playSound("drumkit6");
                Handler(OptionKey, Value);
                return;
            }
            Rectangle MinusAreaOffset = new Rectangle(MinusArea.X + o.X, MinusArea.Y + o.Y, MinusArea.Height, MinusArea.Width);
            if (MinusAreaOffset.Contains(p) && Value > MinValue)
            {
                Game1.playSound("drumkit6");
                Value--;
                Handler(OptionKey, Value);
                return;
            }
        }
        public override void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            if (Disabled)
                return;
            LeftUp(p, o, c, m);
            Resolve(p, o);
        }
        public override void LeftHeld(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Counter++;
            if (Disabled || Counter%Limiter!=0)
                return;
            Counter = 0;
            Limiter = Math.Max(3, Limiter - 1);
            Resolve(p, o);
        }
        public override void LeftUp(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Counter = 0;
            Limiter = 10;
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            // Minus button on the left
            b.Draw(Game1.mouseCursors, new Vector2(o.X+Area.X, o.Y+Area.Y), MinusButton, Color.White * (Disabled || Value<=MinValue ? 0.33f : 1f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.4f);
            // Plus button on the right
            b.Draw(Game1.mouseCursors, new Vector2(o.X+Area.X + (Area.Width - Game1.pixelZoom * 6), o.Y+Area.Y), PlusButton, Color.White * (Disabled || Value>=MaxValue ? 0.33f : 1f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.4f);
            // Box in the center
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, Background, o.X + Area.X + 6 * Game1.pixelZoom, o.Y + Area.Y, Area.Width - 12 * Game1.pixelZoom, Area.Height, Color.White, Game1.pixelZoom, false);
            // Text label in the center
            Utility.drawTextWithShadow(b, Value.ToString(), Game1.smallFont, new Vector2(o.X+Area.X + 8 * Game1.pixelZoom, o.Y+Area.Y+Game1.pixelZoom), Game1.textColor * (Disabled ? 0.33f : 1f));
        }
    }
    public class SliderFormComponent<T> : BaseFormComponent
    {
        protected static Rectangle Background = new Rectangle(403, 383, 6, 6);
        protected static Rectangle Button = new Rectangle(420, 441, 10, 6);
        public T Value;
        protected int OldIndex;
        protected int Offset;
        protected int Index;
        protected List<T> Values;
        protected int OptionKey;
        protected ValueChanged<T> Handler;
        public SliderFormComponent(Point position, List<T> values, int optionKey, ValueChanged<T> handler) : this(position, 100, values, optionKey, handler)
        {
        }
        public SliderFormComponent(Point position, int width, List<T> values, int optionKey, ValueChanged<T> handler)
        {
            Offset = (int)Math.Round((width-10) / (values.Count - 1D));
            SetScaledArea(new Rectangle(position.X, position.Y, width, 6));
            Values = values;
            OptionKey = optionKey;
            Handler = handler;
            Value = values[0];
            Index = 0;
            OldIndex = 0;
        }
        public override void LeftHeld(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            if (Disabled)
                return;
            Index = Math.Max(Math.Min((int)Math.Floor((p.X-o.X) / Offset / Game1.pixelZoom *1D),Values.Count-1),0);
            Value = Values[Index];
        }
        public override void LeftUp(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            if (OldIndex == Index)
                return;
            OldIndex = Index;
            Handler(OptionKey, Value);
        }
        public override void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            LeftHeld(p, o, c, m);
        }
        public override void HoverOut(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            LeftUp(p, o, c, m);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, Background, o.X+Area.X, o.Y+Area.Y, Area.Width, Area.Height, Color.White, Game1.pixelZoom, false);
            b.Draw(Game1.mouseCursors, new Vector2(o.X + Area.X + (Index == Values.Count - 1 ? Area.Width - 10 * Game1.pixelZoom : (Index * Offset * Game1.pixelZoom)), o.Y + Area.Y), new Rectangle?(Button), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.9f);
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
