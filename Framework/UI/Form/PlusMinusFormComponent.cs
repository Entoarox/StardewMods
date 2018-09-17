using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.UI
{
    public class PlusMinusFormComponent : BaseKeyboardFormComponent
    {
        /*********
        ** Fields
        *********/
        protected static readonly Rectangle Background = new Rectangle(227, 425, 9, 9);
        protected static readonly Rectangle MinusButton = new Rectangle(177, 345, 6, 8);
        protected static readonly Rectangle PlusButton = new Rectangle(185, 345, 6, 8);
        protected int _Value;
        protected int Counter;
        protected int Limiter = 10;
        protected int MaxValue;
        protected Rectangle MinusArea;
        protected int MinValue;
        protected int OldValue;
        protected int OptionKey;
        protected Rectangle PlusArea;
        protected string SelectedValue;


        /*********
        ** Accessors
        *********/
        public event ValueChanged<int> Handler;

        public int Value
        {
            get => this._Value;
            set
            {
                this._Value = value;
                this.SelectedValue = this._Value.ToString();
            }
        }


        /*********
        ** Public methods
        *********/
        public PlusMinusFormComponent(Point position, int minValue, int maxValue, ValueChanged<int> handler = null)
        {
            int width = Math.Max(this.GetStringWidth(minValue.ToString(), Game1.smallFont), this.GetStringWidth(maxValue.ToString(), Game1.smallFont)) + 2;
            this.SetScaledArea(new Rectangle(position.X, position.Y, 16 + width, 8));
            this.MinusArea = new Rectangle(this.Area.X, this.Area.Y, BaseMenuComponent.Zoom7, this.Area.Height);
            this.PlusArea = new Rectangle(this.Area.X + this.Area.Width - BaseMenuComponent.Zoom7, this.Area.Y, BaseMenuComponent.Zoom7, this.Area.Height);
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.Value = this.MinValue;
            this.OldValue = this.Value;
            if (handler != null)
                this.Handler += handler;
        }


        public override void LeftClick(Point p, Point o)
        {
            if (this.Disabled)
                return;
            this.Counter = 0;
            this.Limiter = 10;
            Rectangle boxArea = new Rectangle(this.Area.X + o.X + BaseMenuComponent.Zoom7, this.Area.Y + o.Y, this.Area.Width - BaseMenuComponent.Zoom14, this.Area.Height);
            if (boxArea.Contains(p))
            {
                this.Selected = true;
                this.SelectedValue = this.Value.ToString();
                return;
            }

            if (this.Selected)
                this.FocusLost();
            this.Resolve(p, o);
        }

        public override void LeftHeld(Point p, Point o)
        {
            this.Counter++;
            if (this.Disabled || this.Counter % this.Limiter != 0)
                return;
            this.Counter = 0;
            this.Limiter = Math.Max(1, this.Limiter - 1);
            this.Resolve(p, o);
        }

        public override void LeftUp(Point p, Point o)
        {
            this.Counter = 0;
            this.Limiter = 15;
            if (this.OldValue == this.Value)
                return;
            this.OldValue = this.Value;
            this.Handler?.Invoke(this, this.Parent, this.Parent.GetAttachedMenu(), this.Value);
        }

        public override void FocusLost()
        {
            this.Selected = false;
            if (this.OldValue == this.Value)
                return;
            this.OldValue = this.Value;
            this.Handler?.Invoke(this, this.Parent, this.Parent.GetAttachedMenu(), this.Value);
        }

        public override void Draw(SpriteBatch b, Point o)
        {
            if (!this.Visible)
                return;
            // Minus button on the left
            b.Draw(Game1.mouseCursors, new Vector2(o.X + this.Area.X, o.Y + this.Area.Y), PlusMinusFormComponent.MinusButton, Color.White * (this.Disabled || this.Value <= this.MinValue ? 0.33f : 1f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.4f);
            // Plus button on the right
            b.Draw(Game1.mouseCursors, new Vector2(o.X + this.Area.X + this.Area.Width - BaseMenuComponent.Zoom6, o.Y + this.Area.Y), PlusMinusFormComponent.PlusButton, Color.White * (this.Disabled || this.Value >= this.MaxValue ? 0.33f : 1f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.4f);
            // Box in the center
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, PlusMinusFormComponent.Background, o.X + this.Area.X + BaseMenuComponent.Zoom6, o.Y + this.Area.Y, this.Area.Width - BaseMenuComponent.Zoom12, this.Area.Height, Color.White * (this.Disabled ? 0.33f : 1), Game1.pixelZoom, false);
            if (!this.Selected)
            {
                // Text label in the center (Non-selected)
                Utility.drawTextWithShadow(b, this.Value.ToString(), Game1.smallFont, new Vector2(o.X + this.Area.X + BaseMenuComponent.Zoom8, o.Y + this.Area.Y + Game1.pixelZoom), Game1.textColor * (this.Disabled ? 0.33f : 1f));
                return;
            }
            // Drawing code used when the textbox is selected

            // Draw the caret (Text cursor)
            if (DateTime.Now.Millisecond % 1000 >= 500)
                b.Draw(Game1.staminaRect, new Rectangle(this.Area.X + o.X + BaseMenuComponent.Zoom05 + BaseMenuComponent.Zoom8 + (int)Game1.smallFont.MeasureString(this.SelectedValue).X, this.Area.Y + o.Y + (int)(Game1.pixelZoom * 1.5), BaseMenuComponent.Zoom05, this.Area.Height - BaseMenuComponent.Zoom3), Game1.textColor);
            // Draw the actual text
            Utility.drawTextWithShadow(b, this.SelectedValue, Game1.smallFont, new Vector2(o.X + this.Area.X + BaseMenuComponent.Zoom8, o.Y + this.Area.Y + Game1.pixelZoom), Game1.textColor);
        }

        public override void TextReceived(char chr)
        {
            Game1.playSound("cowboy_monsterhit");
            this.TextReceived(chr.ToString());
        }

        public override void TextReceived(string str)
        {
            if (this.Disabled || !int.TryParse(str, out _))
                return;
            this.Value = int.Parse(this.SelectedValue + str);
            this.Value = Math.Max(this.MinValue, Math.Min(this.MaxValue, this.Value));
            this.SelectedValue = this.Value.ToString();
        }

        public override void CommandReceived(char cmd)
        {
            switch ((int)cmd)
            {
                case 8:
                    if (this.SelectedValue.Length <= 0)
                        return;
                    this.SelectedValue = this.SelectedValue.Substring(0, this.SelectedValue.Length - 1);
                    if (this.SelectedValue == "" || !int.TryParse(this.SelectedValue, out _))
                    {
                        this.SelectedValue = "0";
                        this.Value = 0;
                        return;
                    }

                    Game1.playSound("tinyWhip");
                    this.Value = int.Parse(this.SelectedValue);
                    this.Value = Math.Max(this.MinValue, Math.Min(this.MaxValue, this.Value));
                    this.SelectedValue = this.Value.ToString();
                    return;
                case 13:
                    this.Parent.ResetFocus();
                    return;
            }
        }


        /*********
        ** Protected methods
        *********/
        private void Resolve(Point p, Point o)
        {
            Rectangle plusAreaOffset = new Rectangle(this.PlusArea.X + o.X, this.PlusArea.Y + o.Y, this.PlusArea.Height, this.PlusArea.Width);
            if (plusAreaOffset.Contains(p) && this.Value < this.MaxValue)
            {
                this.Value++;
                Game1.playSound("drumkit6");
                return;
            }

            Rectangle minusAreaOffset = new Rectangle(this.MinusArea.X + o.X, this.MinusArea.Y + o.Y, this.MinusArea.Height, this.MinusArea.Width);
            if (minusAreaOffset.Contains(p) && this.Value > this.MinValue)
            {
                Game1.playSound("drumkit6");
                this.Value--;
            }
        }
    }
}
