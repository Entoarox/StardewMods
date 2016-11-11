using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

// WIP
namespace Entoarox.Framework.Menus
{
    class DropdownFormComponent : BaseFormComponent
    {
        public static Rectangle dropDownBGSource = new Rectangle(433, 451, 3, 3);
        public static Rectangle dropDownButtonSource = new Rectangle(437, 450, 10, 11);
        public List<string> dropDownOptions = new List<string>();
        public const int pixelsHigh = 11;
        public int selectedOption;
        public int recentSlotY;
        public int startingSelected;
        private bool clicked;
        private Rectangle dropDownBounds;


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
        protected string _Value;
        protected List<string> Values;
        protected ValueChanged<string> Handler;

        public DropdownFormComponent(Point position, List<string> values, ValueChanged<string> handler) : this(position, 75, values, handler)
        {

        }
        public DropdownFormComponent(Point position, int width, List<string> values, ValueChanged<string> handler)
        {
            SetScaledArea(new Rectangle(position.X, position.Y, width, 11));
            Values = values;
            Handler = handler;
            Value = values[0];
        }
        public override void LeftHeld(Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            if (Disabled)
                return;
        }

        public OptionsDropDown(string label, int whichOption, int x = -1, int y = -1)
      : base(label, x, y, (int) Game1.smallFont.MeasureString("Windowed Borderless ").X + Game1.pixelZoom * 12, 11 * Game1.pixelZoom, whichOption)
    {
            Game1.options.setDropDownToProperValue(this);
            this.dropDownBounds = new Rectangle(this.bounds.X, this.bounds.Y, this.bounds.Width - Game1.pixelZoom * 12, this.bounds.Height * this.dropDownOptions.Count);
        }

        public override void leftClickHeld(int x, int y)
        {
            if (this.greyedOut)
                return;
            base.leftClickHeld(x, y);
            this.clicked = true;
            this.dropDownBounds.Y = Math.Min(this.dropDownBounds.Y, Game1.viewport.Height - this.dropDownBounds.Height - this.recentSlotY);
            this.selectedOption = (int)Math.Max(Math.Min((float)(y - this.dropDownBounds.Y) / (float)this.bounds.Height, (float)(this.dropDownOptions.Count - 1)), 0.0f);
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (this.greyedOut)
                return;
            base.receiveLeftClick(x, y);
            this.startingSelected = this.selectedOption;
            this.leftClickHeld(x, y);
            Game1.playSound("shwip");
            OptionsDropDown.selected = this;
        }

        public override void leftClickReleased(int x, int y)
        {
            if (this.greyedOut || this.dropDownOptions.Count <= 0)
                return;
            base.leftClickReleased(x, y);
            this.clicked = false;
            if (this.dropDownBounds.Contains(x, y))
                Game1.options.changeDropDownOption(this.whichOption, this.selectedOption, this.dropDownOptions);
            else
                this.selectedOption = this.startingSelected;
            OptionsDropDown.selected = (OptionsDropDown)null;
        }

        public override void draw(SpriteBatch b, int slotX, int slotY)
        {
            this.recentSlotY = slotY;
            base.draw(b, slotX, slotY);
            float num = this.greyedOut ? 0.33f : 1f;
            if (this.clicked)
            {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, OptionsDropDown.dropDownBGSource, slotX + this.dropDownBounds.X, slotY + this.dropDownBounds.Y, this.dropDownBounds.Width, this.dropDownBounds.Height, Color.White * num, (float)Game1.pixelZoom, false);
                for (int index = 0; index < this.dropDownOptions.Count; ++index)
                {
                    if (index == this.selectedOption)
                        b.Draw(Game1.staminaRect, new Rectangle(slotX + this.dropDownBounds.X, slotY + this.dropDownBounds.Y + index * this.bounds.Height, this.dropDownBounds.Width, this.bounds.Height), new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Wheat, 0.0f, Vector2.Zero, SpriteEffects.None, 0.975f);
                    b.DrawString(Game1.smallFont, this.dropDownOptions[index], new Vector2((float)(slotX + this.dropDownBounds.X + Game1.pixelZoom), (float)(slotY + this.dropDownBounds.Y + Game1.pixelZoom * 2 + this.bounds.Height * index)), Game1.textColor * num, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.98f);
                }
                b.Draw(Game1.mouseCursors, new Vector2((float)(slotX + this.bounds.X + this.bounds.Width - Game1.pixelZoom * 12), (float)(slotY + this.bounds.Y)), new Rectangle?(OptionsDropDown.dropDownButtonSource), Color.Wheat * num, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.981f);
            }
            else
            {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, OptionsDropDown.dropDownBGSource, slotX + this.bounds.X, slotY + this.bounds.Y, this.bounds.Width - Game1.pixelZoom * 12, this.bounds.Height, Color.White * num, (float)Game1.pixelZoom, false);
                if (OptionsDropDown.selected == null || OptionsDropDown.selected.Equals((object)this))
                    b.DrawString(Game1.smallFont, this.selectedOption >= this.dropDownOptions.Count || this.selectedOption < 0 ? "" : this.dropDownOptions[this.selectedOption], new Vector2((float)(slotX + this.bounds.X + Game1.pixelZoom), (float)(slotY + this.bounds.Y + Game1.pixelZoom * 2)), Game1.textColor * num, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.88f);
                b.Draw(Game1.mouseCursors, new Vector2((float)(slotX + this.bounds.X + this.bounds.Width - Game1.pixelZoom * 12), (float)(slotY + this.bounds.Y)), new Rectangle?(OptionsDropDown.dropDownButtonSource), Color.White * num, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.88f);
            }
        }
    }
}
