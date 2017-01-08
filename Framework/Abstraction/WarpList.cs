using System.Collections.Generic;

namespace Entoarox.Framework.Abstraction
{
    public class WarpList : WrappedList<Warp, StardewValley.Warp>
    {
        internal WarpList(StardewValley.GameLocation location) : base(location.warps,Warp.Wrap, Warp.Unwrap)
        {
        }
        public Warp this[int x, int y]
        {
            get
            {
                for (int c = 0; c < List.Count; c++)
                    if (List[c].X == x && List[c].Y == y)
                        return Wrapper(List[c]);
                throw new KeyNotFoundException();
            }
            set
            {
                for (int c = 0; c < List.Count; c++)
                    if (List[c].X == x && List[c].Y == y)
                    {
                        List[c] = Unwrapper(value);
                        return;
                    }
                throw new KeyNotFoundException();
            }
        }
    }
}
