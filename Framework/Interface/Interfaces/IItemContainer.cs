using StardewValley;

namespace Entoarox.Framework.Interface
{
    internal interface IItemContainer
    {
        /*********
        ** Accessors
        *********/
        Item CurrentItem { get; set; }
        bool IsGhostSlot { get; }

        /*********
        ** Methods
        *********/
        bool AcceptsItem(Item item);
    }
}
