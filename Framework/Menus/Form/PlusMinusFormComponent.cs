using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.Menus
{
    public class PlusMinusFormComponent : BaseFormComponent, IKeyboardSubscriber
    {
        protected static Rectangle PlusButton = new Rectangle(185, 345, 6, 8);
        protected static Rectangle MinusButton = new Rectangle(177, 345, 6, 8);
        protected static Rectangle Background = new Rectangle(227, 425, 9, 9);
        public event ValueChanged<int> Handler;
        public int Value {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }
        protected int _Value;
        protected int MinValue;
        protected int MaxValue;
        protected Rectangle PlusArea;
        protected Rectangle MinusArea;
        protected int Counter = 0;
        protected int Limiter = 10;
        protected int OptionKey;
        protected int OldValue;
        public bool Selected { get; set; }
        protected string SelectedValue;
        protected FrameworkMenu Menu;
        public PlusMinusFormComponent(Point position, int minValue, int maxValue, ValueChanged<int> handler=null)
        {
            int width = Math.Max(GetStringWidth(minValue.ToString(), Game1.smallFont), GetStringWidth(maxValue.ToString(), Game1.smallFont)) + 2;
            SetScaledArea(new Rectangle(position.X, position.Y, 16 + width, 8));
            MinusArea = new Rectangle(Area.X, Area.Y, 7 * Game1.pixelZoom, Area.Height);
            PlusArea = new Rectangle(Area.X + Area.Width - 7 * Game1.pixelZoom, Area.Y, 7 * Game1.pixelZoom, Area.Height);
            MinValue = minValue;
            MaxValue = maxValue;
            Value = MinValue;
            OldValue = Value;
            if(handler!=null)
                Handler += handler;
        }
        private void Resolve(Point p, Point o)
        {
            Rectangle PlusAreaOffset = new Rectangle(PlusArea.X + o.X, PlusArea.Y + o.Y, PlusArea.Height, PlusArea.Width);
            if (PlusAreaOffset.Contains(p) && Value < MaxValue)
            {
                Value++;
                Game1.playSound("drumkit6");
                return;
            }
            Rectangle MinusAreaOffset = new Rectangle(MinusArea.X + o.X, MinusArea.Y + o.Y, MinusArea.Height, MinusArea.Width);
            if (MinusAreaOffset.Contains(p) && Value > MinValue)
            {
                Game1.playSound("drumkit6");
                Value--;
                return;
            }
        }
        public override void LeftClick(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            if (Disabled)
                return;
            Counter = 0;
            Limiter = 10;
            Rectangle BoxArea = new Rectangle(Area.X + o.X + 7 * Game1.pixelZoom, Area.Y + o.Y, Area.Width - 14 * Game1.pixelZoom, Area.Height);
            if (BoxArea.Contains(p))
            {
                Selected = true;
                Game1.keyboardDispatcher.Subscriber = this;
                m.TextboxActive = true;
                Menu = m;
                SelectedValue = Value.ToString();
                return;
            }
            if (Selected)
                FocusLost(c, m);
            Resolve(p, o);
        }
        public override void LeftHeld(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Counter++;
            if (Disabled || Counter % Limiter !=0)
                return;
            Counter = 0;
            Limiter = Math.Max(1, Limiter - 1);
            Resolve(p, o);
        }
        public override void LeftUp(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            Counter = 0;
            Limiter = 15;
            if (OldValue == Value)
                return;
            OldValue = Value;
            Handler?.Invoke(this, c, m, Value);
        }
        public override void FocusLost(IComponentCollection c, FrameworkMenu m)
        {
            m.TextboxActive = false;
            Selected = false;
            Game1.keyboardDispatcher.Subscriber = null;
            if (OldValue == Value)
                return;
            OldValue = Value;
            Handler?.Invoke(this, c, m, Value);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (!Visible)
                return;
            // Minus button on the left
            b.Draw(Game1.mouseCursors, new Vector2(o.X + Area.X, o.Y + Area.Y), MinusButton, Color.White * (Disabled || Value <= MinValue ? 0.33f : 1f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.4f);
            // Plus button on the right
            b.Draw(Game1.mouseCursors, new Vector2(o.X + Area.X + (Area.Width - Game1.pixelZoom * 6), o.Y + Area.Y), PlusButton, Color.White * (Disabled || Value >= MaxValue ? 0.33f : 1f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.4f);
            // Box in the center
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, Background, o.X + Area.X + 6 * Game1.pixelZoom, o.Y + Area.Y, Area.Width - 12 * Game1.pixelZoom, Area.Height, Color.White, Game1.pixelZoom, false);
            if (!Selected)
            {
                // Text label in the center (Non-selected)
                Utility.drawTextWithShadow(b, Value.ToString(), Game1.smallFont, new Vector2(o.X + Area.X + 8 * Game1.pixelZoom, o.Y + Area.Y + Game1.pixelZoom), Game1.textColor * (Disabled ? 0.33f : 1f));
                return;
            }
            // Drawing code used when the textbox is selected
            string text = SelectedValue;
            Vector2 v;
            // Limit the draw length
            for (v = Game1.smallFont.MeasureString(text); v.X > Area.Width - Game1.pixelZoom * 5; v = Game1.smallFont.MeasureString(text))
                text = text.Substring(1);
            // Draw the caret (Text cursor)
            if (DateTime.Now.Millisecond % 1000 >= 500)
                b.Draw(Game1.staminaRect, new Rectangle(Area.X + o.X + (int)(8.5 * Game1.pixelZoom + v.X), Area.Y + o.Y + (int)(Game1.pixelZoom*1.5), Game1.pixelZoom / 2, Area.Height-3*Game1.pixelZoom), Game1.textColor);
            // Draw the actual text
            Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2(o.X + Area.X + 8 * Game1.pixelZoom, o.Y + Area.Y + Game1.pixelZoom), Game1.textColor);
        }
        public void RecieveTextInput(char inputChar)
        {
            Game1.playSound("cowboy_monsterhit");
            RecieveTextInput(inputChar.ToString());
        }
        public void RecieveTextInput(string text)
        {
            int t = -1;
            if (Disabled || !int.TryParse(text, out t))
                return;
            Value = int.Parse(SelectedValue + text);
            Value = Math.Max(MinValue, Math.Min(MaxValue, Value));
            SelectedValue = Value.ToString();
        }
        public void RecieveCommandInput(char c)
        {
            if (Disabled)
                return;
            switch ((int)c)
            {
                case 8:
                    if (SelectedValue.Length <= 0)
                        return;
                    SelectedValue = SelectedValue.Substring(0, SelectedValue.Length - 1);
                    int t = -1;
                    if (SelectedValue == "" || !int.TryParse(SelectedValue, out t))
                    {
                        SelectedValue = "0";
                        Value = 0;
                        return;
                    }
                    Game1.playSound("tinyWhip");
                    Value = int.Parse(SelectedValue);
                    Value = Math.Max(MinValue, Math.Min(MaxValue, Value));
                    SelectedValue = Value.ToString();
                    return;
                case 13:
                        Menu.ResetFocus();
                    return;
            }
        }
        public void RecieveSpecialInput(Keys k)
        {

        }
    }
}
