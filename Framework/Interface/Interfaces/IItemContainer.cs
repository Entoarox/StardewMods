using StardewValley;

namespace Entoarox.Framework.Interface
{
    interface IItemContainer
    {
        bool AcceptsItem(Item item);
        Item CurrentItem { get; set; }
        bool IsGhostSlot { get; }
    }
}
