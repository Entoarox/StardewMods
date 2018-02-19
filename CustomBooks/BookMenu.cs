using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace Entoarox.CustomBooks
{
    class BookMenu : IClickableMenu
    {
        private int Offset = 0;
        private int HoveredArrow = 0;
        private bool HoveredClose = false;
        private Texture2D Background;
        private Texture2D Binder;
        private Texture2D Content;
        public List<Page> Pages;
        public string Id;
        //private static Rectangle LeftPage = new Rectangle(60 * 2, 45 * 2, 140 * 2, 245 * 2);
        //private static Rectangle RightPage = new Rectangle(250 * 2, 45 * 2, 140 * 2, 245 * 2);
        private static Rectangle LeftArrow = new Rectangle(352, 495, 12, 11);
        private static Rectangle RightArrow = new Rectangle(365, 495, 12, 11);
        private static Rectangle CloseButton = new Rectangle(337, 494, 12, 12);
        private Point Size => new Point(this.Binder.Width * 4, this.Binder.Height * 4);
        private Vector2 Origin => Utility.getTopLeftPositionForCenteringOnScreen(this.Size.X,this.Size.Y);
        private Rectangle CloseHotspot => new Rectangle((int)(this.Origin.X + this.Size.X - 30), (int)(this.Origin.Y + 30), CloseButton.Width * 3, CloseButton.Height * 3);
        private Rectangle ArrowHotspotLeft => new Rectangle((int)(this.Origin.X + 30), (int)(this.Origin.Y + this.Size.Y - 60), RightArrow.Width * 4, RightArrow.Height * 4);
        private Rectangle ArrowHotspotRight => new Rectangle((int)(this.Origin.X + this.Size.X - 66), (int)(this.Origin.Y + this.Size.Y - 60), RightArrow.Width * 4, RightArrow.Height * 4);
        private Rectangle LeftPage => new Rectangle((int)(this.Origin.X + 100), (int)(this.Origin.Y + 100), 320,520);
        private Rectangle RightPage => new Rectangle((int)(this.Origin.X + 500), (int)(this.Origin.Y + 100), 320, 520);
        public BookMenu(string id)
        {
            this.Id = id;
            this.Pages = CustomBooksMod.Shelf.Books[id].GetPages();
            this.Pages.Insert(0, new ConfigPage());
            this.Pages.Add(new ConfigPage(true));
            this.Background = CustomBooksMod.SHelper.Content.Load<Texture2D>("book.png");
            this.Binder = CustomBooksMod.SHelper.Content.Load<Texture2D>("binding.png");
            this.Content = CustomBooksMod.SHelper.Content.Load<Texture2D>("pages.png");
        }
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            
        }
        public override void performHoverAction(int x, int y)
        {
            this.HoveredArrow = this.ArrowHotspotLeft.Contains(x, y) ? 1 : this.ArrowHotspotRight.Contains(x, y) ? 2 : 0;
            this.HoveredClose = this.CloseHotspot.Contains(x, y);
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.Offset > 0 && this.ArrowHotspotLeft.Contains(x, y))
            {
                this.Offset-=2;
                Game1.playSound("shwip");
            }
            if (this.Offset < this.Pages.Count-2 && this.ArrowHotspotRight.Contains(x, y))
            {
                this.Offset+=2;
                Game1.playSound("shwip");
            }
            else if(this.CloseHotspot.Contains(x,y))
            {
                this.exitThisMenu(true);
            }
        }
        public override void draw(SpriteBatch b)
        {
            /*
            var p = Utility.getTopLeftPositionForCenteringOnScreen(this.Background.Width * 2, this.Background.Height * 2);
            var origin = new Point((int)p.X, (int)p.Y);
            b.Draw(this.Background, p, null, Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
            */
            var p = this.Origin;
            var origin = new Point((int)p.X, (int)p.Y);
            b.Draw(this.Binder, p, null, CustomBooksMod.Shelf.Books[this.Id].Color, 0, Vector2.Zero, 4f, SpriteEffects.None, 0);
            b.Draw(this.Content, p, null, Color.White, 0, Vector2.Zero, 4f, SpriteEffects.None, 0);

            b.Draw(Game1.mouseCursors, new Vector2(this.CloseHotspot.X - (this.HoveredClose ? CloseButton.Height * 0.25f : 0), this.CloseHotspot.Y - (this.HoveredClose ? CloseButton.Height * 0.25f : 0)), CloseButton, Color.White, 0, Vector2.Zero, this.HoveredClose ? 3.5f : 3f, SpriteEffects.None, 0);
            if (this.Offset > 0)
                b.Draw(Game1.mouseCursors, new Vector2(this.ArrowHotspotLeft.X - (this.HoveredArrow == 1 ? RightArrow.Width * 0.25f : 0), this.ArrowHotspotLeft.Y - (this.HoveredArrow == 1 ? RightArrow.Height * 0.25f : 0)), LeftArrow, Color.White, 0, Vector2.Zero, this.HoveredArrow == 1 ? 4.5f : 4, SpriteEffects.None, 0);
            if (this.Offset < this.Pages.Count-2)
                b.Draw(Game1.mouseCursors, new Vector2(this.ArrowHotspotRight.X - (this.HoveredArrow == 2 ? RightArrow.Width * 0.25f : 0), this.ArrowHotspotRight.Y - (this.HoveredArrow == 2 ? RightArrow.Height * 0.25f : 0)), RightArrow, Color.White, 0, Vector2.Zero, this.HoveredArrow == 2 ? 4.5f : 4, SpriteEffects.None, 0);
            this.Pages[this.Offset].Draw(b, this.LeftPage);
            if(this.Offset+1 < this.Pages.Count)
                this.Pages[this.Offset+1].Draw(b, this.RightPage);
            this.drawMouse(b);
        }
    }
}
