using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Entoarox.CustomBooks.Menu
{
    internal class BookMenu : IClickableMenu
    {
        /*********
        ** Fields
        *********/
        private Rectangle ArrowHotspotLeft => new Rectangle((int)(this.Origin.X + 30), (int)(this.Origin.Y + this.Size.Y - 60), BookMenu.RightArrow.Width * 4, BookMenu.RightArrow.Height * 4);
        private Rectangle ArrowHotspotRight => new Rectangle((int)(this.Origin.X + this.Size.X - 66), (int)(this.Origin.Y + this.Size.Y - 60), BookMenu.RightArrow.Width * 4, BookMenu.RightArrow.Height * 4);
        private Rectangle CloseHotspot => new Rectangle((int)(this.Origin.X + this.Size.X - 30), (int)(this.Origin.Y + 30), BookMenu.CloseButton.Width * 3, BookMenu.CloseButton.Height * 3);
        private Rectangle LeftPage => new Rectangle((int)(this.Origin.X + 100), (int)(this.Origin.Y + 100), 320, 520);
        private Vector2 Origin => Utility.getTopLeftPositionForCenteringOnScreen(this.Size.X, this.Size.Y);
        private Rectangle RightPage => new Rectangle((int)(this.Origin.X + 500), (int)(this.Origin.Y + 100), 320, 520);
        private Point Size => new Point(this.Binder.Width * 4, this.Binder.Height * 4);
        private readonly Texture2D Binder;
        private static readonly Rectangle CloseButton = new Rectangle(337, 494, 12, 12);
        private readonly Texture2D Content;
        private int Hover;
        private static readonly Rectangle LeftArrow = new Rectangle(352, 495, 12, 11);
        private int Offset;
        private static readonly Rectangle RightArrow = new Rectangle(365, 495, 12, 11);


        /*********
        ** Accessors
        *********/
        public List<Page> Pages;
        public string Id;


        /*********
        ** Public methods
        *********/
        public BookMenu(string id)
        {
            this.Id = id;
            this.Pages = ModEntry.Shelf.Books[id].GetPages();
            this.Pages.Insert(0, new ConfigPage(this));
            this.Pages.Add(new InsertPage(this));
            this.Binder = ModEntry.SHelper.Content.Load<Texture2D>("assets/binding.png");
            this.Content = ModEntry.SHelper.Content.Load<Texture2D>("assets/pages.png");
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void performHoverAction(int x, int y)
        {
            if (this.Offset > 0 && this.ArrowHotspotLeft.Contains(x, y))
                this.Hover = 2;
            else if (this.Offset < this.Pages.Count - 2 && this.ArrowHotspotRight.Contains(x, y))
                this.Hover = 3;
            else if (this.CloseHotspot.Contains(x, y))
                this.Hover = 1;
            else
            {
                this.Hover = 0;
                if (this.LeftPage.Contains(x, y))
                    this.Pages[this.Offset].Hover(this.LeftPage, x, y);
                if (this.Offset + 1 < this.Pages.Count && this.RightPage.Contains(x, y))
                    this.Pages[this.Offset + 1].Hover(this.RightPage, x, y);
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (this.LeftPage.Contains(x, y))
                this.Pages[this.Offset].Release(this.LeftPage, x, y);
            if (this.Offset + 1 < this.Pages.Count && this.RightPage.Contains(x, y))
                this.Pages[this.Offset + 1].Release(this.RightPage, x, y);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            switch (this.Hover)
            {
                case 1:
                    this.exitThisMenu(true);
                    break;
                case 2:
                    this.Offset -= 2;
                    Game1.playSound("shwip");
                    break;
                case 3:
                    this.Offset += 2;
                    Game1.playSound("shwip");
                    break;
            }

            if (this.LeftPage.Contains(x, y))
                this.Pages[this.Offset].Click(this.LeftPage, x, y);
            if (this.Offset + 1 < this.Pages.Count && this.RightPage.Contains(x, y))
                this.Pages[this.Offset + 1].Click(this.RightPage, x, y);
        }

        public override void draw(SpriteBatch b)
        {
            /*
            var p = Utility.getTopLeftPositionForCenteringOnScreen(this.Background.Width * 2, this.Background.Height * 2);
            var origin = new Point((int)p.X, (int)p.Y);
            b.Draw(this.Background, p, null, Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
            */
            Vector2 p = this.Origin;
            b.Draw(this.Binder, p, null, ModEntry.Shelf.Books[this.Id].Color, 0, Vector2.Zero, 4f, SpriteEffects.None, 0);
            b.Draw(this.Content, p, null, Color.White, 0, Vector2.Zero, 4f, SpriteEffects.None, 0);

            b.Draw(Game1.mouseCursors, new Vector2(this.CloseHotspot.X - (this.Hover == 1 ? BookMenu.CloseButton.Height * 0.25f : 0), this.CloseHotspot.Y - (this.Hover == 1 ? BookMenu.CloseButton.Height * 0.25f : 0)), BookMenu.CloseButton, Color.White, 0, Vector2.Zero, this.Hover == 1 ? 3.5f : 3f, SpriteEffects.None, 0);
            if (this.Offset > 0)
                b.Draw(Game1.mouseCursors, new Vector2(this.ArrowHotspotLeft.X - (this.Hover == 2 ? BookMenu.RightArrow.Width * 0.25f : 0), this.ArrowHotspotLeft.Y - (this.Hover == 2 ? BookMenu.RightArrow.Height * 0.25f : 0)), BookMenu.LeftArrow, Color.White, 0, Vector2.Zero, this.Hover == 2 ? 4.5f : 4, SpriteEffects.None, 0);
            if (this.Offset < this.Pages.Count - 2)
                b.Draw(Game1.mouseCursors, new Vector2(this.ArrowHotspotRight.X - (this.Hover == 3 ? BookMenu.RightArrow.Width * 0.25f : 0), this.ArrowHotspotRight.Y - (this.Hover == 3 ? BookMenu.RightArrow.Height * 0.25f : 0)), BookMenu.RightArrow, Color.White, 0, Vector2.Zero, this.Hover == 3 ? 4.5f : 4, SpriteEffects.None, 0);
            this.Pages[this.Offset].Draw(b, this.LeftPage);
            if (this.Offset + 1 < this.Pages.Count)
                this.Pages[this.Offset + 1].Draw(b, this.RightPage);
            this.drawMouse(b);
        }
    }
}
