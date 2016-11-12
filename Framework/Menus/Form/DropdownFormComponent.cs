using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

// WIP
namespace Entoarox.Framework.Menus
{
    public class DropdownFormComponent : BaseFormComponent
    {
        class ClickHandler : IClickHandler
        {
            protected string Value;
            protected DropdownFormComponent Parent;
            public ClickHandler(DropdownFormComponent parent, string value)
            {
                Parent = parent;
                Value = value;
            }
            public void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
            {
                Parent.Value = Value;
            }
            public void RightClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
            {

            }
        }
        protected static Rectangle Background = new Rectangle(433, 451, 3, 3);
        protected static Rectangle Button = new Rectangle(437, 450, 10, 11);
        protected static Rectangle UpScroll = new Rectangle(421, 459, 11, 12);
        protected static Rectangle DownScroll = new Rectangle(421, 472, 11, 12);
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
            Expanded = false;
            Area.Height = Game1.pixelZoom * 11;
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
            Expanded = false;
            Area.Height = Game1.pixelZoom * 11;
        }
        public override void Scroll(int d, Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            if (Disabled || !Expanded)
                return;
            int change =d / 120;
            Offset = Math.Max(0, Math.Min(Offset - change, MaxScroll));
        }
        public override void FocusLost(IComponentCollection c, FrameworkMenu m)
        {
            Expanded = false;
            Area.Height = Game1.pixelZoom * 11;
        }
        public override void HoverIn(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            ShowTarget = true;
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            // Selected background
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, Background, o.X+Area.X, o.Y+Area.Y, Area.Width-Game1.pixelZoom*(Button.Width-1), Game1.pixelZoom*11, Color.White * (Disabled ? 0.33f : 1f), Game1.pixelZoom, false);
            // Selected label
            Utility.drawTextWithShadow(b, Value, Game1.dialogueFont, new Vector2(o.X + Area.X + Game1.pixelZoom*2, o.Y + Area.Y + Game1.pixelZoom), Game1.textColor * (Disabled ? 0.33f : 1f));
            // Selector button
            b.Draw(Game1.mouseCursors, new Vector2(o.X+Area.X + Area.Width - Game1.pixelZoom * Button.Width, o.Y + Area.Y), Button, Color.White * (Disabled ? 0.33f : 1f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.88f);
            if (!Expanded)
                return;
            Point o2 = new Point(o.X, o.Y + 11 * Game1.pixelZoom);
            // Expanded background
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, Background, o2.X + Area.X, o2.Y + Area.Y - Game1.pixelZoom, Area.Width-Game1.pixelZoom*2, Area.Height-Game1.pixelZoom*11, Color.White, Game1.pixelZoom, false);
            // Handle Y position math once for use in the loop
            int MouseY = ShowTarget ? Game1.getMouseY() - o.Y - Area.Y - 11 * Game1.pixelZoom : -1;
            // Expanded options
            for (int c=0;c<Size;c++)
            {
                if(Values[Offset+c]==Value)
                    b.Draw(Game1.staminaRect, new Rectangle(o.X + Area.X+Game1.pixelZoom, o.Y+Area.Y+Game1.pixelZoom*7*c + 11*Game1.pixelZoom,Area.Width-Game1.pixelZoom*4,7*Game1.pixelZoom), new Rectangle(0, 0, 1, 1), Color.Wheat, 0.0f, Vector2.Zero, SpriteEffects.None, 0.975f);
                int start = c * 7 * Game1.pixelZoom;
                if (MouseY > start && MouseY < start + 7 * Game1.pixelZoom)
                    b.Draw(Game1.staminaRect, new Rectangle(o.X + Area.X + Game1.pixelZoom, o.Y + Area.Y + Game1.pixelZoom * 7 * c + 11 * Game1.pixelZoom, Area.Width - Game1.pixelZoom * 4, 7 * Game1.pixelZoom), new Rectangle(0, 0, 1, 1), Color.Wheat*0.5f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.975f);
                Utility.drawTextWithShadow(b, Values[Offset+c], Game1.smallFont, new Vector2(o.X + Area.X + Game1.pixelZoom*2, o.Y + Area.Y + Game1.pixelZoom + Game1.pixelZoom * 7 * c + 11*Game1.pixelZoom), Game1.textColor * (Disabled ? 0.33f : 1f));
            }
            // Draw the scroll indicators
            if (Offset > 0)
                b.Draw(Game1.mouseCursors, new Rectangle(o.X + Area.X + Area.Width - 2 * Game1.pixelZoom, o2.Y + Area.Y, 7 * Game1.pixelZoom, 7 * Game1.pixelZoom), UpScroll, Color.White);
            if (Offset < MaxScroll)
                b.Draw(Game1.mouseCursors, new Rectangle(o.X + Area.X + Area.Width - 2 * Game1.pixelZoom, o2.Y + Area.Y + Area.Height - 20 * Game1.pixelZoom, 7 * Game1.pixelZoom, 7 * Game1.pixelZoom), DownScroll, Color.White);
        }
    }
}
