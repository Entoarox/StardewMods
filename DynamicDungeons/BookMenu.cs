using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Entoarox.DynamicDungeons
{
    internal class BookMenu : IClickableMenu
    {
        /*********
        ** Fields
        *********/
        private readonly Texture2D Background;
        private static readonly Rectangle CloseButton = new Rectangle(337, 494, 12, 12);
        private int HoveredArrow;
        private bool HoveredClose;
        private static Rectangle ItemBox = new Rectangle(293, 360, 24, 24);
        private static readonly Rectangle LeftArrow = new Rectangle(352, 495, 12, 11);
        private static readonly Rectangle LeftPage = new Rectangle(60 * 2, 15 * 2, 140 * 2, 245 * 2);
        private int Offset;
        private static readonly Rectangle RightArrow = new Rectangle(365, 495, 12, 11);
        private static readonly Rectangle RightPage = new Rectangle(250 * 2, 15 * 2, 140 * 2, 245 * 2);
        public List<Page> Pages;

        private Rectangle ArrowHotspotLeft
        {
            get
            {
                Vector2 origin = Utility.getTopLeftPositionForCenteringOnScreen(this.Background.Width * 2, this.Background.Height * 2);
                return new Rectangle((int)(origin.X + BookMenu.LeftPage.X - BookMenu.LeftArrow.Width * 4), (int)(origin.Y + BookMenu.LeftPage.Y + BookMenu.LeftPage.Height), BookMenu.RightArrow.Width * 4, BookMenu.RightArrow.Height * 4);
            }
        }

        private Rectangle ArrowHotspotRight
        {
            get
            {
                Vector2 origin = Utility.getTopLeftPositionForCenteringOnScreen(this.Background.Width * 2, this.Background.Height * 2);
                return new Rectangle((int)(origin.X + BookMenu.RightPage.X + BookMenu.RightPage.Width), (int)(origin.Y + BookMenu.RightPage.Y + BookMenu.RightPage.Height), BookMenu.RightArrow.Width * 4, BookMenu.RightArrow.Height * 4);
            }
        }

        private Rectangle CloseHotspot
        {
            get
            {
                Vector2 origin = Utility.getTopLeftPositionForCenteringOnScreen(this.Background.Width * 2, this.Background.Height * 2);
                return new Rectangle((int)(origin.X + BookMenu.RightPage.X + BookMenu.RightPage.Width + 54), (int)(origin.Y + BookMenu.RightPage.Y), BookMenu.CloseButton.Width * 3, BookMenu.CloseButton.Height * 3);
            }
        }


        /*********
        ** Public methods
        *********/
        public BookMenu(List<Page> pages)
        {
            this.Pages = pages;
            this.Background = ModEntry.SHelper.Content.Load<Texture2D>("assets/book.png");
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) { }

        public override void performHoverAction(int x, int y)
        {
            this.HoveredArrow = this.ArrowHotspotLeft.Contains(x, y) ? 1 : this.ArrowHotspotRight.Contains(x, y) ? 2 : 0;
            this.HoveredClose = this.CloseHotspot.Contains(x, y);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.Offset > 0 && this.ArrowHotspotLeft.Contains(x, y))
            {
                this.Offset -= 2;
                Game1.playSound("shwip");
            }

            if (this.Offset < this.Pages.Count - 2 && this.ArrowHotspotRight.Contains(x, y))
            {
                this.Offset += 2;
                Game1.playSound("shwip");
            }
            else if (this.CloseHotspot.Contains(x, y))
                this.exitThisMenu(true);
        }

        public override void draw(SpriteBatch b)
        {
            Vector2 p = Utility.getTopLeftPositionForCenteringOnScreen(this.Background.Width * 2, this.Background.Height * 2);
            Point origin = new Point((int)p.X, (int)p.Y);
            b.Draw(this.Background, p, null, Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
            b.Draw(Game1.mouseCursors, new Vector2(this.CloseHotspot.X - (this.HoveredClose ? BookMenu.CloseButton.Height * 0.25f : 0), this.CloseHotspot.Y - (this.HoveredClose ? BookMenu.CloseButton.Height * 0.25f : 0)), BookMenu.CloseButton, Color.White, 0, Vector2.Zero, this.HoveredClose ? 3.5f : 3f, SpriteEffects.None, 0);
            if (this.Offset > 0)
                b.Draw(Game1.mouseCursors, new Vector2(this.ArrowHotspotLeft.X - (this.HoveredArrow == 1 ? BookMenu.RightArrow.Width * 0.25f : 0), this.ArrowHotspotLeft.Y - (this.HoveredArrow == 1 ? BookMenu.RightArrow.Height * 0.25f : 0)), BookMenu.LeftArrow, Color.White, 0, Vector2.Zero, this.HoveredArrow == 1 ? 4.5f : 4, SpriteEffects.None, 0);
            if (this.Offset < this.Pages.Count - 2)
                b.Draw(Game1.mouseCursors, new Vector2(this.ArrowHotspotRight.X - (this.HoveredArrow == 2 ? BookMenu.RightArrow.Width * 0.25f : 0), this.ArrowHotspotRight.Y - (this.HoveredArrow == 2 ? BookMenu.RightArrow.Height * 0.25f : 0)), BookMenu.RightArrow, Color.White, 0, Vector2.Zero, this.HoveredArrow == 2 ? 4.5f : 4, SpriteEffects.None, 0);
            this.Pages[this.Offset].Draw(b, new Rectangle(origin.X + BookMenu.LeftPage.X, origin.Y + BookMenu.LeftPage.Y, BookMenu.LeftPage.Width, BookMenu.LeftPage.Height));
            if (this.Offset + 1 < this.Pages.Count)
                this.Pages[this.Offset + 1].Draw(b, new Rectangle(origin.X + BookMenu.RightPage.X, origin.Y + BookMenu.RightPage.Y, BookMenu.RightPage.Width, BookMenu.RightPage.Height));
            this.drawMouse(b);
        }
    }
}
