using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.Framework.UI
{
    public class TextboxFormComponent : BaseKeyboardFormComponent
    {
        /*********
        ** Fields
        *********/
        protected string _Value;
        protected static Texture2D Box;
        protected int CaretSize = (int)Game1.smallFont.MeasureString("|").Y;
        protected string OldValue;
        protected Predicate<string> Validator = e => true;


        /*********
        ** Accessors
        *********/
        public string Value
        {
            get => this._Value;
            set => this._Value = value;
        }

        public event ValueChanged<string> Handler;
        public Func<TextboxFormComponent, FrameworkMenu, string, string> TabPressed;
        public Func<TextboxFormComponent, FrameworkMenu, string, string> EnterPressed;


        /*********
        ** Public methods
        *********/
        public TextboxFormComponent(Point position, ValueChanged<string> handler = null)
            : this(position, 75, null, handler) { }

        public TextboxFormComponent(Point position, Predicate<string> validator, ValueChanged<string> handler = null)
            : this(position, 75, validator, handler) { }

        public TextboxFormComponent(Point position, int width, ValueChanged<string> handler = null)
            : this(position, width, null, handler) { }

        public TextboxFormComponent(Point position, int width = 75, Predicate<string> validator = null, ValueChanged<string> handler = null)
        {
            if (TextboxFormComponent.Box == null)
                TextboxFormComponent.Box = Game1.content.Load<Texture2D>("LooseSprites\\textBox");
            this.SetScaledArea(new Rectangle(position.X, position.Y, width, TextboxFormComponent.Box.Height / Game1.pixelZoom));
            if (validator != null)
                this.Validator = validator;
            if (handler != null)
                this.Handler += handler;
            this.Value = "";
            this.OldValue = this.Value;
        }

        public override void FocusLost()
        {
            if (this.Disabled || this.OldValue.Equals(this.Value))
                return;
            this.OldValue = this.Value;
            this.Handler?.Invoke(this, this.Parent, this.Parent.GetAttachedMenu(), this.Value);
        }

        public override void FocusGained()
        {
            if (this.Disabled)
                return;
            this.Selected = true;
        }

        public override void Draw(SpriteBatch b, Point o)
        {
            if (!this.Visible)
                return;
            bool flag = DateTime.Now.Millisecond % 1000 >= 500;
            string text = this.Value;
            b.Draw(TextboxFormComponent.Box, new Rectangle(this.Area.X + o.X, this.Area.Y + o.Y, BaseMenuComponent.Zoom4, this.Area.Height), new Rectangle(Game1.pixelZoom, 0, BaseMenuComponent.Zoom4, this.Area.Height), Color.White * (this.Disabled ? 0.33f : 1));
            b.Draw(TextboxFormComponent.Box, new Rectangle(this.Area.X + o.X + BaseMenuComponent.Zoom4, this.Area.Y + o.Y, this.Area.Width - BaseMenuComponent.Zoom8, this.Area.Height), new Rectangle(BaseMenuComponent.Zoom4, 0, 4, this.Area.Height), Color.White * (this.Disabled ? 0.33f : 1));
            b.Draw(TextboxFormComponent.Box, new Rectangle(this.Area.X + o.X + this.Area.Width - BaseMenuComponent.Zoom4, this.Area.Y + o.Y, BaseMenuComponent.Zoom4, this.Area.Height), new Rectangle(TextboxFormComponent.Box.Bounds.Width - BaseMenuComponent.Zoom4, 0, BaseMenuComponent.Zoom4, this.Area.Height), Color.White * (this.Disabled ? 0.33f : 1));
            Vector2 v;
            for (v = Game1.smallFont.MeasureString(text); v.X > this.Area.Width - Game1.pixelZoom * 5; v = Game1.smallFont.MeasureString(text))
                text = text.Substring(1);
            if (flag && this.Selected)
                b.Draw(Game1.staminaRect, new Rectangle(this.Area.X + o.X + BaseMenuComponent.Zoom3 + BaseMenuComponent.Zoom05 + (int)v.X, this.Area.Y + o.Y + 8, BaseMenuComponent.Zoom05, this.CaretSize), Game1.textColor);
            Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2(this.Area.X + o.X + BaseMenuComponent.Zoom4, this.Area.Y + o.Y + BaseMenuComponent.Zoom3), Game1.textColor * (this.Disabled ? 0.33f : 1));
        }

        public override void TextReceived(char chr)
        {
            if (this.Disabled || !Game1.smallFont.Characters.Contains(chr) || !this.Validator(chr.ToString()))
                return;
            Game1.playSound("cowboy_monsterhit");
            this.Value = this.Value + chr;
        }

        public override void TextReceived(string str)
        {
            foreach (char c in str)
                if (!Game1.smallFont.Characters.Contains(c))
                    return;
            if (this.Disabled || !this.Validator(str))
                return;
            Game1.playSound("coin");
            this.Value = this.Value + str;
        }

        public override void CommandReceived(char cmd)
        {
            if (this.Disabled)
                return;
            switch ((int)cmd)
            {
                case 8:
                    if (this.Value.Length <= 0)
                        return;
                    this.Value = this.Value.Substring(0, this.Value.Length - 1);
                    Game1.playSound("tinyWhip");
                    return;
                case 9:
                    if (this.TabPressed != null)
                    {
                        this.Value = this.TabPressed(this, this.Parent.GetAttachedMenu(), this.Value);
                        return;
                    }

                    if (!(this.Parent is IComponentCollection))
                        return;
                    bool next = false;
                    IInteractiveMenuComponent first = null;
                    foreach (IInteractiveMenuComponent imc in ((IComponentCollection)this.Parent).InteractiveComponents)
                    {
                        if (first == null && imc is TextboxFormComponent)
                            first = imc;
                        if (imc == this)
                        {
                            next = true;
                            continue;
                        }

                        if (next && imc is TextboxFormComponent)
                        {
                            this.Parent.GiveFocus(imc);
                            return;
                        }
                    }

                    this.Parent.GiveFocus(first);
                    return;
                case 13:
                    if (this.EnterPressed != null)
                        this.Value = this.EnterPressed(this, this.Parent.GetAttachedMenu(), this.Value);
                    else
                        this.Parent.ResetFocus();
                    return;
            }
        }
    }
}
