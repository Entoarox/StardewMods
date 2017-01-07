using System;
using System.Collections;
using System.Collections.Generic;

namespace Entoarox.Framework.Abstraction
{
    public class WarpList : IEnumerable<Warp>
    {
        public class WarpEnumerator : IEnumerator<Warp>
        {
            private int index = -1;
            private Warp current
            {
                get
                {
                    if (index >= 0 && Location.warps.Count > index)
                        return new Warp(Location.warps[index].TargetName, Location.warps[index].TargetX, Location.warps[index].TargetY);
                    throw new InvalidOperationException();
                }
            }
            public Warp Current => current;
            object IEnumerator.Current => current;
            private StardewValley.GameLocation Location;
            internal WarpEnumerator(StardewValley.GameLocation location)
            {
                Location = location;
            }
            public bool MoveNext()
            {
                if (StardewValley.Game1.locations.Count <= index + 1)
                    return false;
                index++;
                return true;
            }
            public void Reset()
            {
                index = -1;
            }
            public void Dispose()
            {

            }
        }
        private StardewValley.GameLocation Location;
        internal WarpList(StardewValley.GameLocation location)
        {
            Location = location;
        }
        public Warp this[int x, int y]
        {
            get
            {
                for (int c = 0; c < Location.warps.Count; c++)
                    if (Location.warps[c].X == x && Location.warps[c].Y == y)
                        return new Warp(Location.warps[c].TargetName, Location.warps[c].TargetX, Location.warps[c].TargetY);
                throw new KeyNotFoundException();
            }
            set
            {
                for (int c = 0; c < Location.warps.Count; c++)
                    if (Location.warps[c].X == x && Location.warps[c].Y == y)
                    {
                        Location.warps[c] = new StardewValley.Warp(x, y, value.Destination, value.X, value.Y, false);
                        return;
                    }
                throw new KeyNotFoundException();
            }
        }
        public Warp this[int index]
        {
            get
            {
                if (index < 0 || Location.warps.Count <= index)
                    throw new IndexOutOfRangeException();
                return new Warp(Location.warps[index].TargetName, Location.warps[index].TargetX, Location.warps[index].TargetY);
            }
            set
            {
                if (index < 0 || Location.warps.Count <= index)
                    throw new IndexOutOfRangeException();
                Location.warps[index] = new StardewValley.Warp(Location.warps[index].X, Location.warps[index].Y, value.Destination, value.X, value.Y, false);
            }
        }
        private WarpEnumerator Enumerator()
        {
            return new WarpEnumerator(Location);
        }
        public IEnumerator<Warp> GetEnumerator()
        {
            return Enumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Enumerator();
        }
    }
}
