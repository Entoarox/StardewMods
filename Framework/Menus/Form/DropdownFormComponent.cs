using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.Menus
{
    public class DropdownFormComponent : BaseFormComponent
    {
        protected class DropdownSelect : BaseKeyboardFormComponent
        {
            protected int MaxScroll;
            protected int HoverOffset=0;
            protected int KeyboardOffset=0;
            protected int ScrollOffset=0;
            protected int Size;
            protected bool ShowHover = false;
            protected string Value;
            protected DropdownFormComponent Owner;
            protected IComponentCollection Collection;
            protected Rectangle Self;
            protected int GetCursorIndex(Point p, Point o)
            {
                int index = (int)Math.Floor(((p.Y - Area.Y) / Game1.pixelZoom) / 7D) + ScrollOffset;
                if (index < 0)
                    index = 0;
                if (index >= Owner.Values.Count)
                    index = Owner.Values.Count - 1;
                return index;
            }
            public DropdownSelect(Point position, int width, Rectangle self, DropdownFormComponent owner, IComponentCollection collection)
            {
                MaxScroll = Math.Max(0, owner.Values.Count - 10);
                Size = Math.Min(10, owner.Values.Count);
                Value = owner.Value;
                Owner = owner;
                Collection = collection;
                Self = self;
                Area=new Rectangle(position.X, position.Y, width, zoom2 + Game1.pixelZoom * Math.Min(7 * owner.Values.Count,70));
                collection.GetAttachedMenu().GiveFocus(this);
                if (!collection.GetAttachedMenu().EventRegion.Contains(Area.X, Area.Y + Area.Height))
                    Area.Y -= Area.Height + zoom9;
                int index = owner.Values.IndexOf(Value);
                ScrollOffset = Math.Max(0,index - 9);
                KeyboardOffset = index;
            }
            public override bool InBounds(Point p, Point o)
            {
                return true;
            }
            public override void FocusLost(IComponentCollection c, FrameworkMenu m)
            {
                if (Value == Owner.Value)
                    return;
                Owner.Value = Value;
                Owner.Handler?.Invoke(Owner, Collection, m, Owner.Value);
            }
            public override void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
            {
                if (base.InBounds(p, new Point(0, 0)))
                {
                    Value = Owner.Values[GetCursorIndex(p, o)];
                    m.ResetFocus();
                }
                else if (Self.Contains(p))
                    m.ResetFocus();
                else
                {
                    m.ResetFocus();
                    m.receiveLeftClick(p.X, p.Y, false);
                }
            }
            public override void RightClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
            {
                if (!base.InBounds(p, new Point(0, 0)))
                {
                    m.ResetFocus();
                    m.receiveRightClick(p.X, p.Y, false);
                }
            }
            public override void Scroll(int d, Point p, Point o, IComponentCollection c, FrameworkMenu m)
            {
                int change = d / 120;
                ScrollOffset = Math.Max(0, Math.Min(ScrollOffset - change, MaxScroll));
            }
            public override void HoverOver(Point p, Point o, IComponentCollection c, FrameworkMenu m)
            {
                ShowHover = base.InBounds(p, new Point(0, 0));
                HoverOffset = GetCursorIndex(p, o);
            }
            public override void Draw(SpriteBatch b, Point o)
            {
                o = new Point(0, 0);
                Color col = Color.Black * 0.25f;
                // Background
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, Background, o.X + Area.X, o.Y + Area.Y - Game1.pixelZoom, Area.Width - zoom2, Area.Height, Color.White, Game1.pixelZoom, false);
                for (int c = 0; c < Size; c++)
                {
                    // Selected
                    if (Owner.Values[ScrollOffset + c] == Value)
                        b.Draw(Game1.staminaRect, new Rectangle(o.X + Area.X + Game1.pixelZoom, o.Y + Area.Y + zoom7 * c, Area.Width - zoom4, zoom7), new Rectangle(0, 0, 1, 1), Color.Wheat*0.5f);
                    if (Selected && KeyboardOffset == ScrollOffset + c)
                    {
                        // Top
                        b.Draw(Game1.staminaRect, new Rectangle(o.X + Area.X + Game1.pixelZoom, o.Y + Area.Y + zoom7 * c, Area.Width - zoom4, zoom05), new Rectangle(0, 0, 1, 1), col);
                        // Bottom
                        b.Draw(Game1.staminaRect, new Rectangle(o.X + Area.X + Game1.pixelZoom, o.Y + Area.Y + zoom7 * c + zoom6 + zoom05, Area.Width - zoom4, zoom05), new Rectangle(0, 0, 1, 1), col);
                        // Left
                        b.Draw(Game1.staminaRect, new Rectangle(o.X + Area.X + Game1.pixelZoom, o.Y + Area.Y + zoom7 * c + zoom05, zoom05, zoom6), new Rectangle(0, 0, 1, 1), col);
                        // Right
                        b.Draw(Game1.staminaRect, new Rectangle(o.X + Area.X + Area.Width - zoom3 - zoom05, o.Y + Area.Y + zoom7 * c + zoom05, zoom05, zoom6), new Rectangle(0, 0, 1, 1), col);
                    }
                    // Hover
                    if (ShowHover && HoverOffset==ScrollOffset+c)
                        b.Draw(Game1.staminaRect, new Rectangle(o.X + Area.X + Game1.pixelZoom + zoom05, o.Y + Area.Y + zoom05 + zoom7 * c, Area.Width - zoom5, zoom6), new Rectangle(0, 0, 1, 1), Color.Wheat*0.75f);
                    // Text
                    Utility.drawTextWithShadow(b, Owner.Values[ScrollOffset + c], Game1.smallFont, new Vector2(o.X + Area.X + zoom2, o.Y + Area.Y + zoom7 * c + Game1.pixelZoom), Game1.textColor * (Disabled ? 0.33f : 1f));
                }
                // ScrollUp
                if (ScrollOffset > 0)
                    b.Draw(Game1.mouseCursors, new Rectangle(o.X + Area.X + Area.Width - zoom2, o.Y + Area.Y, zoom7, zoom7), UpScroll, Color.White);
                // ScrollDown
                if (ScrollOffset < MaxScroll)
                    b.Draw(Game1.mouseCursors, new Rectangle(o.X + Area.X + Area.Width - zoom2, o.Y + Area.Y + Area.Height - zoom9, zoom7, zoom7), DownScroll, Color.White);
            }
            public override void CommandReceived(char k, IComponentCollection c, FrameworkMenu m)
            {
                switch ((int)k)
                {
                    case 13:
                        Value = Owner.Values[KeyboardOffset];
                        c.ResetFocus();
                        break;
                }
            }
            public override void SpecialReceived(Keys k, IComponentCollection c, FrameworkMenu m)
            {
                switch (k)
                {
                    case Keys.Down:
                        if (KeyboardOffset < Owner.Values.Count - 1)
                            KeyboardOffset++;
                        if (KeyboardOffset - ScrollOffset > 9 && ScrollOffset < MaxScroll)
                            ScrollOffset++;
                        break;
                    case Keys.Up:
                        if (KeyboardOffset > 0)
                            KeyboardOffset--;
                        if (KeyboardOffset - ScrollOffset < 0 && ScrollOffset > 0)
                            ScrollOffset--;
                        break;
                }
            }
        }
        protected readonly static Rectangle Background = new Rectangle(433, 451, 3, 3);
        protected readonly static Rectangle Button = new Rectangle(437, 450, 10, 11);
        protected readonly static Rectangle UpScroll = new Rectangle(421, 459, 11, 12);
        protected readonly static Rectangle DownScroll = new Rectangle(421, 472, 11, 12);
        public event ValueChanged<string> Handler;
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if(Values.Contains(value))
                    _Value = value;
            }
        }
        protected string _Value;
        protected List<string> Values;
        public DropdownFormComponent(Point position, List<string> values, ValueChanged<string> handler=null) : this(position, 75, values, handler)
        {

        }
        public DropdownFormComponent(Point position, int width, List<string> values, ValueChanged<string> handler=null)
        {
            SetScaledArea(new Rectangle(position.X, position.Y, width, 11));
            Values = values;
            Value = Values[0];
            if(handler!=null)
                Handler += handler;
        }
        public override void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            if (Disabled)
                return;
            new DropdownSelect(new Point(o.X + Area.X, o.Y + Area.Y + Area.Height), Area.Width,new Rectangle(o.X+Area.X,o.Y+Area.Y,Area.Width,Area.Height), this, c);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (!Visible)
                return;
            // Selected background
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, Background, o.X+Area.X, o.Y+Area.Y, Area.Width-Game1.pixelZoom*(Button.Width-1), zoom11, Color.White * (Disabled ? 0.33f : 1f), Game1.pixelZoom, false);
            // Selected label
            Utility.drawTextWithShadow(b, Value, Game1.smallFont, new Vector2(o.X + Area.X + zoom2, o.Y + Area.Y + zoom3), Game1.textColor * (Disabled ? 0.33f : 1f));
            // Selector button
            b.Draw(Game1.mouseCursors, new Vector2(o.X+Area.X + Area.Width - Game1.pixelZoom * Button.Width, o.Y + Area.Y), Button, Color.White * (Disabled ? 0.33f : 1f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.88f);
        }
    }
}
