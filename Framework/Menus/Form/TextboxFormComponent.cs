using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.Menus
{
    public class TextboxFormComponent : BaseFormComponent, IKeyboardSubscriber
    {
        protected static Texture2D Box;
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }
        public bool Selected { get; set; }
        public event ValueChanged<string> Handler;
        public Func<TextboxFormComponent, FrameworkMenu, string, string> TabPressed;
        public Func<TextboxFormComponent, FrameworkMenu, string, string> EnterPressed;
        protected string _Value;
        protected bool OnlyNumbers;
        protected string OldValue;
        protected FrameworkMenu Menu;
        public TextboxFormComponent(Point position, ValueChanged<string> handler = null) : this(position, 75, false, handler)
        {

        }
        public TextboxFormComponent(Point position, bool onlyNumbers, ValueChanged<string> handler = null) : this(position, 75, onlyNumbers, handler)
        {

        }
        public TextboxFormComponent(Point position, int width, ValueChanged<string> handler=null) : this(position, width, false, handler)
        {

        }
        public TextboxFormComponent(Point position, int width, bool onlyNumbers, ValueChanged<string> handler=null)
        {
            if(Box==null)
                Box=Game1.content.Load<Texture2D>("LooseSprites\\textBox");
            SetScaledArea(new Rectangle(position.X, position.Y, width, Box.Height/Game1.pixelZoom));
            OnlyNumbers = onlyNumbers;
            if (handler != null)
                Handler += handler;
            Value = "";
            OldValue = Value;
        }
        public override void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Selected = true;
            m.TextboxActive = true;
            Menu = m;
            Game1.keyboardDispatcher.Subscriber = this;
        }
        public override void RightClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Selected = true;
            m.TextboxActive = true;
            Game1.keyboardDispatcher.Subscriber = this;
        }
        public override void FocusLost(IComponentCollection c, FrameworkMenu m)
        {
            Selected = false;
            m.TextboxActive = false;
            if (OldValue.Equals(Value))
                return;
            OldValue = Value;
            Handler?.Invoke(this, c, m, Value);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            bool flag = DateTime.Now.Millisecond % 1000 >= 500;
            string text = Value;
            b.Draw(Box, new Rectangle(Area.X+o.X, Area.Y+o.Y, Game1.tileSize / 4, Area.Height), new Rectangle(0, 0, Game1.tileSize / 4, Area.Height), Color.White);
            b.Draw(Box, new Rectangle(Area.X+o.X + Game1.tileSize / 4, Area.Y+o.Y, Area.Width - Game1.tileSize / 2, Area.Height), new Rectangle(Game1.tileSize / 4, 0, 4, Area.Height), Color.White);
            b.Draw(Box, new Rectangle(Area.X+o.X + Area.Width - Game1.tileSize / 4, Area.Y+o.Y, Game1.tileSize / 4, Area.Height), new Rectangle(Box.Bounds.Width - Game1.tileSize / 4, 0, Game1.tileSize / 4, Area.Height), Color.White);
            Vector2 v;
            for (v = Game1.smallFont.MeasureString(text); v.X > Area.Width - Game1.pixelZoom*5; v = Game1.smallFont.MeasureString(text))
                text = text.Substring(1);
            if (flag && Selected)
                b.Draw(Game1.staminaRect, new Rectangle(Area.X+o.X + (int)(Game1.pixelZoom*3.5) + (int)v.X, Area.Y+o.Y + 8, Game1.pixelZoom/2, (int)Game1.smallFont.MeasureString("|").Y), Game1.textColor);
            Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2(Area.X+o.X + Game1.pixelZoom*4, Area.Y+o.Y+Game1.pixelZoom*3), Game1.textColor);
        }
        public void RecieveTextInput(char inputChar)
        {
            Game1.playSound("cowboy_monsterhit");
            RecieveTextInput(inputChar.ToString());
        }
        public void RecieveTextInput(string text)
        {
            int t = -1;
            if (Disabled || (OnlyNumbers && !int.TryParse(text, out t)))
                return;
            Value = Value + text;
        }
        public void RecieveCommandInput(char c)
        {
            if (Disabled)
                return;
            switch((int)c)
            {
                case 8:
                    if (Value.Length <= 0)
                        return;
                    Value = Value.Substring(0, Value.Length - 1);
                    Game1.playSound("tinyWhip");
                    return;
                case 9:
                    if (TabPressed != null)
                        Value = TabPressed(this, Menu, Value);
                    return;
                case 13:
                    if (EnterPressed != null)
                        Value = EnterPressed(this, Menu, Value);
                    else
                        Menu.ResetFocus();
                    return;
            }
        }
        public void RecieveSpecialInput(Keys k)
        {

        }
    }
}
