using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.CustomBooks.Menu
{
    internal class ConfigPage : SystemPage
    {
        /*********
        ** Fields
        *********/
        private static readonly Texture2D Brush;
        private static readonly Texture2D Delete;
        private bool Deleting;
        private static readonly Texture2D Edit;
        private readonly BookMenu Menu;
        private static readonly Texture2D Quil;


        /*********
        ** Public methods
        *********/
        public ConfigPage(BookMenu menu)
        {
            this.Menu = menu;
        }

        static ConfigPage()
        {
            ConfigPage.Quil = ModEntry.SHelper.Content.Load<Texture2D>("assets/quil.png");
            ConfigPage.Brush = ModEntry.SHelper.Content.Load<Texture2D>("assets/brush.png");
            ConfigPage.Edit = ModEntry.SHelper.Content.Load<Texture2D>("assets/edit.png");
            ConfigPage.Delete = ModEntry.SHelper.Content.Load<Texture2D>("assets/delete.png");
        }

        public override void Click(Rectangle region, int x, int y)
        {
            if (new Rectangle(region.X + 25, region.Y + 320, region.Width - 50, 44).Contains(x, y))
            {
                Game1.playSound("bigDeSelect");
                foreach (Page page in this.Menu.Pages)
                    page.Editable = !page.Editable;
            }
            else if (new Rectangle(region.X + 25, region.Y + 370, region.Width - 50, 44).Contains(x, y))
            {
                if (this.Deleting)
                {
#pragma warning disable AvoidNetField // Avoid Netcode types when possible
                    Game1.player.items.Filter(a => (a as Book)?.Id.Equals(this.Menu.Id) != true);
#pragma warning restore AvoidNetField // Avoid Netcode types when possible
                    ModEntry.Shelf.Books.Remove(this.Menu.Id);
                    Game1.playSound("trashcan");
                    this.Menu.exitThisMenu(false);
                }
                else
                {
                    Game1.playSound("bigDeSelect");
                    this.Deleting = true;
                }
            }
        }

        public override void Hover(Rectangle region, int x, int y)
        {
            if (this.Deleting && !new Rectangle(region.X + 25, region.Y + 370, region.Width - 50, 44).Contains(x, y))
                this.Deleting = false;
        }

        public override void Draw(SpriteBatch batch, Rectangle region)
        {
            Utility.drawTextWithShadow(batch, "Customise Book", Game1.dialogueFont, new Vector2(region.X, region.Y + 16), Game1.textColor, 1);
            Utility.drawTextWithShadow(batch, "Name:", Game1.smallFont, new Vector2(region.X, region.Y + 100), Game1.textColor, 1);
            this.DrawTextBox(batch, ConfigPage.Quil, region.X + 100, region.Y + 100, region.Width - 100);
            Utility.drawTextWithShadow(batch, "Color:", Game1.smallFont, new Vector2(region.X, region.Y + 140), Game1.textColor, 1);
            this.DrawTextBox(batch, ConfigPage.Brush, region.X + 100, region.Y + 140, region.Width - 100);
            Utility.drawTextWithShadow(batch, "Manage Book", Game1.dialogueFont, new Vector2(region.X, region.Y + 250), Game1.textColor, 1);
            this.DrawButton(batch, ConfigPage.Edit, region.X + 25, region.Y + 320, region.Width - 50, "Mode: " + (this.Editable ? "Editing" : "Reading"));
            this.DrawButton(batch, ConfigPage.Delete, region.X + 25, region.Y + 370, region.Width - 50, this.Deleting ? "Click to Confirm" : "Destroy Book");
        }

        public override Bookshelf.Book.Page Serialize()
        {
            return null;
        }
    }
}
