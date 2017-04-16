using System;
using StardewValley;

namespace Entoarox.Framework.Events
{
    public class EventArgsActiveItemChanged : EventArgs
    {
        public readonly Item OldItem;
        public readonly Item NewItem;

        public EventArgsActiveItemChanged(Item oldItem, Item newItem)
        {
            this.OldItem = oldItem;
            this.NewItem = newItem;
        }
    }
}
