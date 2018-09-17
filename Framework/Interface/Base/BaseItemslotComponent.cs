using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.Framework.Interface
{
    internal class BaseItemslotComponent : BaseDynamicComponent, IItemContainer
    {
        /*********
        ** Accessors
        *********/
        public Item CurrentItem { get; set; }
        public virtual bool IsGhostSlot => false;


        /*********
        ** Public methods
        *********/
        public BaseItemslotComponent(string name, Point origin, Item item = null, int layer = 0)
            : base(name, new Rectangle(origin.X, origin.Y, 16, 16), layer)
        {
            this.CurrentItem = item;
        }

        public virtual bool AcceptsItem(Item item)
        {
            return true;
        }

        public override void LeftClick(Point offset, Point position) { }

        public override void Draw(Point offset, SpriteBatch batch)
        {
            Vector2 drawVector = Utilities.GetDrawVector(offset, this.OuterBounds);
            batch.Draw(Game1.menuTexture, drawVector, Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
            this.CurrentItem?.drawInMenu(batch, drawVector, Game1.pixelZoom, this.IsGhostSlot ? 0.7f : 1, 1);
        }
    }
}
