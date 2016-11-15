using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.Menus
{
    public class DropdownFormComponent : BaseKeyboardFormComponent
    {
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
        protected bool Expanded = false;
        protected List<ClickableTextComponent> Options=new List<ClickableTextComponent>();
        protected int Counter = 0;
        protected int MaxScroll;
        protected int Offset;
        protected int Size;
        protected bool ShowTarget = false;
        protected int HoverOffset = 0;
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
            MaxScroll = Math.Max(0,values.Count - 10);
            Size = Math.Min(10, values.Count);
        }
        public override void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            if (Disabled)
                return;
            if(!Expanded)
            {
                Expanded = true;
                base.FocusGained(c, m);
                Counter = 0;
                Area.Height = Game1.pixelZoom * 13 + Game1.pixelZoom * Math.Min(7 * Values.Count,70);
                return;
            }
            double px = (p.Y - (o.Y+Area.Y))/Game1.pixelZoom;
            if(px>11)
            {
                int item = (int)Math.Floor((px - 11)/7)+Offset;
                if (item < Values.Count && Values[item]!=Value)
                {
                    Value = Values[item];
                    Handler?.Invoke(this, c, m, Value);
                }
            }
            c.ResetFocus();
        }
        public override void HoverOver(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Counter = 0;
        }
        public override void HoverOut(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Counter = 1;
            ShowTarget = false;
        }
        public override void Update(GameTime t, IComponentCollection c, FrameworkMenu m)
        {
            if (Counter < 1)
                return;
            Counter++;
            if (Counter < 50)
                return;
            Counter = 0;
            c.ResetFocus();
        }
        public override void Scroll(int d, Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            if (Disabled || !Expanded || !Visible)
                return;
            int change =d / 120;
            Offset = Math.Max(0, Math.Min(Offset - change, MaxScroll));
        }
        public override void FocusGained(IComponentCollection c, FrameworkMenu m)
        {
            
        }
        public override void FocusLost(IComponentCollection c, FrameworkMenu m)
        {
            Expanded = false;
            HoverOffset = 0;
            Area.Height = Game1.pixelZoom * 11;
            base.FocusLost(c, m);
        }
        public override void HoverIn(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            ShowTarget = true;
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
            if (!Expanded)
                return;
            Point o2 = new Point(o.X, o.Y + 11 * Game1.pixelZoom);
            // Expanded background
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, Background, o2.X + Area.X, o2.Y + Area.Y - Game1.pixelZoom, Area.Width-zoom2, Area.Height-zoom11, Color.White, Game1.pixelZoom, false);
            // Handle Y position math once for use in the loop
            int MouseY = ShowTarget ? Game1.getMouseY() - o.Y - Area.Y - zoom11 : -1;
            // Expanded options
            for (int c=0;c<Size;c++)
            {
                if(Values[Offset+c]==Value)
                    b.Draw(Game1.staminaRect, new Rectangle(o.X + Area.X+Game1.pixelZoom, o.Y+Area.Y+zoom7*c + zoom11,Area.Width-zoom4,zoom7), new Rectangle(0, 0, 1, 1), Color.Wheat, 0.0f, Vector2.Zero, SpriteEffects.None, 0.975f);
                int start = c * zoom7;
                if (Selected && HoverOffset == Offset + c)
                {
                    Color col = Color.Wheat * 0.5f;
                    col.R = (byte)(col.R *2);
                    // Top
                    b.Draw(Game1.staminaRect, new Rectangle(o.X + Area.X + Game1.pixelZoom, o.Y + Area.Y + zoom7 * c + zoom11, Area.Width - zoom4, zoom05), new Rectangle(0, 0, 1, 1), col, 0.0f, Vector2.Zero, SpriteEffects.None, 0.975f);
                    // Bottom
                    b.Draw(Game1.staminaRect, new Rectangle(o.X + Area.X + Game1.pixelZoom, o.Y + Area.Y + zoom7 * c + zoom17 + zoom05, Area.Width - zoom4, zoom05), new Rectangle(0, 0, 1, 1), col, 0.0f, Vector2.Zero, SpriteEffects.None, 0.975f);
                    // Left
                    b.Draw(Game1.staminaRect, new Rectangle(o.X + Area.X + Game1.pixelZoom, o.Y + Area.Y + zoom7 * c + zoom16, zoom05, zoom6), new Rectangle(0, 0, 1, 1), col, 0.0f, Vector2.Zero, SpriteEffects.None, 0.975f);
                    // Right
                    b.Draw(Game1.staminaRect, new Rectangle(o.X + Area.X + Area.Width - zoom3 - zoom05, o.Y + Area.Y + zoom7 * c + zoom16, zoom05, Game1.pixelZoom * 6), new Rectangle(0, 0, 1, 1), col, 0.0f, Vector2.Zero, SpriteEffects.None, 0.975f);
                }
                if (MouseY > start && MouseY < start + 7 * Game1.pixelZoom)
                    b.Draw(Game1.staminaRect, new Rectangle(o.X + Area.X + Game1.pixelZoom + zoom05, o.Y + Area.Y + zoom05 + zoom7 * c + zoom11, Area.Width - zoom5, zoom6), new Rectangle(0, 0, 1, 1), Color.Wheat*0.5f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.975f);
                Utility.drawTextWithShadow(b, Values[Offset+c], Game1.smallFont, new Vector2(o.X + Area.X + zoom2, o.Y + Area.Y + zoom7 * c + zoom12), Game1.textColor * (Disabled ? 0.33f : 1f));
            }
            // Draw the scroll indicators
            if (Offset > 0)
                b.Draw(Game1.mouseCursors, new Rectangle(o.X + Area.X + Area.Width - zoom2, o2.Y + Area.Y, zoom7, zoom7), UpScroll, Color.White);
            if (Offset < MaxScroll)
                b.Draw(Game1.mouseCursors, new Rectangle(o.X + Area.X + Area.Width - zoom2, o2.Y + Area.Y + Area.Height - zoom20, zoom7, zoom7), DownScroll, Color.White);
        }
        public override void CommandReceived(char k, IComponentCollection c, FrameworkMenu m)
        {
            if (Disabled)
                return;
            switch ((int)k)
            {
                case 13:
                    c.ResetFocus();
                    if (Value == Values[HoverOffset])
                        break;
                    Value = Values[HoverOffset];
                    Handler?.Invoke(this, c, m, Value);
                    break;
            }
        }
        public override void SpecialReceived(Keys k, IComponentCollection c, FrameworkMenu m)
        {
            switch (k)
            {
                case Keys.Down:
                    if (HoverOffset < Values.Count-1)
                        HoverOffset++;
                    if (HoverOffset - Offset > 9 && Offset < MaxScroll)
                        Offset++;
                    break;
                case Keys.Up:
                    if (HoverOffset > 0)
                        HoverOffset--;
                    if (HoverOffset - Offset < 0 && Offset > 0)
                        Offset--;
                    break;
            }
        }
    }
}
