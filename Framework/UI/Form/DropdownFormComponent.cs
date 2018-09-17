using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.UI
{
    public class DropdownFormComponent : BaseFormComponent
    {
        /*********
        ** Fields
        *********/
        protected static readonly Rectangle Background = new Rectangle(433, 451, 3, 3);
        protected static readonly Rectangle Button = new Rectangle(438, 450, 9, 11);
        protected static readonly Rectangle DownScroll = new Rectangle(421, 472, 11, 12);
        protected static readonly Rectangle UpScroll = new Rectangle(421, 459, 11, 12);
        protected string _Value;
        protected List<string> Values;


        /*********
        ** Accessors
        *********/
        public event ValueChanged<string> Handler;

        public string Value
        {
            get => this._Value;
            set
            {
                if (this.Values.Contains(value))
                    this._Value = value;
            }
        }


        /*********
        ** Public methods
        *********/
        public DropdownFormComponent(Point position, List<string> values, ValueChanged<string> handler = null)
            : this(position, 75, values, handler) { }

        public DropdownFormComponent(Point position, int width, List<string> values, ValueChanged<string> handler = null)
        {
            this.SetScaledArea(new Rectangle(position.X, position.Y, width, 11));
            this.Values = values;
            this.Value = this.Values[0];
            if (handler != null)
                this.Handler += handler;
        }

        public override void LeftClick(Point p, Point o)
        {
            if (this.Disabled)
                return;
            new DropdownSelect(new Point(o.X + this.Area.X, o.Y + this.Area.Y + this.Area.Height), this.Area.Width, new Rectangle(o.X + this.Area.X, o.Y + this.Area.Y, this.Area.Width, this.Area.Height), this, this.Parent);
        }

        public override void Draw(SpriteBatch b, Point o)
        {
            if (!this.Visible)
                return;
            // Selected background
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, DropdownFormComponent.Background, o.X + this.Area.X, o.Y + this.Area.Y, this.Area.Width - Game1.pixelZoom * DropdownFormComponent.Button.Width, BaseMenuComponent.Zoom11, Color.White * (this.Disabled ? 0.33f : 1f), Game1.pixelZoom, false);
            // Selected label
            Utility.drawTextWithShadow(b, this.Value, Game1.smallFont, new Vector2(o.X + this.Area.X + BaseMenuComponent.Zoom2, o.Y + this.Area.Y + BaseMenuComponent.Zoom3), Game1.textColor * (this.Disabled ? 0.33f : 1f));
            // Selector button
            b.Draw(Game1.mouseCursors, new Vector2(o.X + this.Area.X + this.Area.Width - Game1.pixelZoom * DropdownFormComponent.Button.Width, o.Y + this.Area.Y), DropdownFormComponent.Button, Color.White * (this.Disabled ? 0.33f : 1f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.88f);
        }

        protected class DropdownSelect : BaseKeyboardFormComponent
        {
            /*********
            ** Fields
            *********/
            protected IComponentContainer Collection;
            protected int HoverOffset;
            protected int KeyboardOffset;
            protected int MaxScroll;
            protected DropdownFormComponent Owner;
            protected int ScrollOffset;
            protected Rectangle Self;
            protected bool ShowHover;
            protected int Size;
            protected string Value;


            /*********
            ** Public methods
            *********/
            public DropdownSelect(Point position, int width, Rectangle self, DropdownFormComponent owner, IComponentContainer collection)
            {
                this.MaxScroll = Math.Max(0, owner.Values.Count - 10);
                this.Size = Math.Min(10, owner.Values.Count);
                this.Value = owner.Value;
                this.Owner = owner;
                this.Collection = collection;
                this.Self = self;
                this.Area = new Rectangle(position.X, position.Y, width, BaseMenuComponent.Zoom2 + Game1.pixelZoom * Math.Min(7 * owner.Values.Count, 70));
                collection.GetAttachedMenu().GiveFocus(this);
                if (this.Area.Y + this.Area.Height > Game1.viewport.Height)
                    this.Area.Y -= this.Area.Height + BaseMenuComponent.Zoom9;
                int index = owner.Values.IndexOf(this.Value);
                this.ScrollOffset = Math.Max(0, index - 9);
                this.KeyboardOffset = index;
            }

            public override bool InBounds(Point p, Point o)
            {
                return base.InBounds(p, new Point(0, 0));
            }

            public override void FocusGained()
            {
                this.Selected = true;
            }

            public override void FocusLost()
            {
                if (this.Value == this.Owner.Value)
                    return;
                this.Owner.Value = this.Value;
                this.Owner.Handler?.Invoke(this.Owner, this.Collection, this.Parent.GetAttachedMenu(), this.Owner.Value);
            }

            public override void LeftClick(Point p, Point o)
            {
                this.Value = this.Owner.Values[this.GetCursorIndex(p, o)];
                this.Parent.ResetFocus();
            }

            public override bool Scroll(int d, Point p, Point o)
            {
                int change = d / 120;
                int oldOffset = this.ScrollOffset;
                this.ScrollOffset = Math.Max(0, Math.Min(this.ScrollOffset - change, this.MaxScroll));
                if (oldOffset != this.ScrollOffset)
                {
                    Game1.playSound("drumkit6");
                    return true;
                }

                return false;
            }

            public override void HoverIn(Point p, Point o)
            {
                this.ShowHover = true;
            }

            public override void HoverOver(Point p, Point o)
            {
                this.HoverOffset = this.GetCursorIndex(p, o);
            }

            public override void HoverOut(Point p, Point o)
            {
                this.ShowHover = false;
            }

            public override void Draw(SpriteBatch b, Point o)
            {
                if (!this.Visible)
                    return;
                o = new Point(0, 0);
                Color col = Color.Black * 0.25f;
                // Background
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, DropdownFormComponent.Background, o.X + this.Area.X, o.Y + this.Area.Y - Game1.pixelZoom, this.Area.Width - BaseMenuComponent.Zoom2, this.Area.Height, Color.White, Game1.pixelZoom, false);
                for (int c = 0; c < this.Size; c++)
                {
                    // Selected
                    if (this.Owner.Values[this.ScrollOffset + c] == this.Value)
                        b.Draw(Game1.staminaRect, new Rectangle(o.X + this.Area.X + Game1.pixelZoom, o.Y + this.Area.Y + BaseMenuComponent.Zoom7 * c, this.Area.Width - BaseMenuComponent.Zoom4, BaseMenuComponent.Zoom7), new Rectangle(0, 0, 1, 1), Color.Wheat * 0.5f);
                    if (this.Selected && this.KeyboardOffset == this.ScrollOffset + c)
                    {
                        // Top
                        b.Draw(Game1.staminaRect, new Rectangle(o.X + this.Area.X + Game1.pixelZoom, o.Y + this.Area.Y + BaseMenuComponent.Zoom7 * c, this.Area.Width - BaseMenuComponent.Zoom4, BaseMenuComponent.Zoom05), new Rectangle(0, 0, 1, 1), col);
                        // Bottom
                        b.Draw(Game1.staminaRect, new Rectangle(o.X + this.Area.X + Game1.pixelZoom, o.Y + this.Area.Y + BaseMenuComponent.Zoom7 * c + BaseMenuComponent.Zoom6 + BaseMenuComponent.Zoom05, this.Area.Width - BaseMenuComponent.Zoom4, BaseMenuComponent.Zoom05), new Rectangle(0, 0, 1, 1), col);
                        // Left
                        b.Draw(Game1.staminaRect, new Rectangle(o.X + this.Area.X + Game1.pixelZoom, o.Y + this.Area.Y + BaseMenuComponent.Zoom7 * c + BaseMenuComponent.Zoom05, BaseMenuComponent.Zoom05, BaseMenuComponent.Zoom6), new Rectangle(0, 0, 1, 1), col);
                        // Right
                        b.Draw(Game1.staminaRect, new Rectangle(o.X + this.Area.X + this.Area.Width - BaseMenuComponent.Zoom3 - BaseMenuComponent.Zoom05, o.Y + this.Area.Y + BaseMenuComponent.Zoom7 * c + BaseMenuComponent.Zoom05, BaseMenuComponent.Zoom05, BaseMenuComponent.Zoom6), new Rectangle(0, 0, 1, 1), col);
                    }

                    // Hover
                    if (this.ShowHover && this.HoverOffset == this.ScrollOffset + c)
                        b.Draw(Game1.staminaRect, new Rectangle(o.X + this.Area.X + Game1.pixelZoom + BaseMenuComponent.Zoom05, o.Y + this.Area.Y + BaseMenuComponent.Zoom05 + BaseMenuComponent.Zoom7 * c, this.Area.Width - BaseMenuComponent.Zoom5, BaseMenuComponent.Zoom6), new Rectangle(0, 0, 1, 1), Color.Wheat * 0.75f);
                    // Text
                    Utility.drawTextWithShadow(b, this.Owner.Values[this.ScrollOffset + c], Game1.smallFont, new Vector2(o.X + this.Area.X + BaseMenuComponent.Zoom2, o.Y + this.Area.Y + BaseMenuComponent.Zoom7 * c + Game1.pixelZoom), Game1.textColor * (this.Disabled ? 0.33f : 1f));
                }

                // ScrollUp
                if (this.ScrollOffset > 0)
                    b.Draw(Game1.mouseCursors, new Rectangle(o.X + this.Area.X + this.Area.Width - BaseMenuComponent.Zoom2, o.Y + this.Area.Y, BaseMenuComponent.Zoom7, BaseMenuComponent.Zoom7), DropdownFormComponent.UpScroll, Color.White);
                // ScrollDown
                if (this.ScrollOffset < this.MaxScroll)
                    b.Draw(Game1.mouseCursors, new Rectangle(o.X + this.Area.X + this.Area.Width - BaseMenuComponent.Zoom2, o.Y + this.Area.Y + this.Area.Height - BaseMenuComponent.Zoom9, BaseMenuComponent.Zoom7, BaseMenuComponent.Zoom7), DropdownFormComponent.DownScroll, Color.White);
            }

            public override void CommandReceived(char cmd)
            {
                switch ((int)cmd)
                {
                    case 13:
                        this.Value = this.Owner.Values[this.KeyboardOffset];
                        this.Parent.ResetFocus();
                        break;
                }
            }

            public override void SpecialReceived(Keys key)
            {
                switch (key)
                {
                    case Keys.Down:
                        if (this.KeyboardOffset < this.Owner.Values.Count - 1)
                            this.KeyboardOffset++;
                        if (this.KeyboardOffset - this.ScrollOffset > 9 && this.ScrollOffset < this.MaxScroll)
                            this.ScrollOffset++;
                        break;
                    case Keys.Up:
                        if (this.KeyboardOffset > 0)
                            this.KeyboardOffset--;
                        if (this.KeyboardOffset - this.ScrollOffset < 0 && this.ScrollOffset > 0)
                            this.ScrollOffset--;
                        break;
                }
            }


            /*********
            ** Protected methods
            *********/
            protected int GetCursorIndex(Point p, Point o)
            {
                int index = (int)Math.Floor((p.Y - this.Area.Y) / Game1.pixelZoom / 7D) + this.ScrollOffset;
                if (index < 0)
                    index = 0;
                if (index >= this.Owner.Values.Count)
                    index = this.Owner.Values.Count - 1;
                return index;
            }
        }
    }
}
