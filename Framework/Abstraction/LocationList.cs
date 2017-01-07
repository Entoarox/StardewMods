using System;
using System.Collections;
using System.Collections.Generic;

namespace Entoarox.Framework.Abstraction
{
    public class LocationList : IEnumerable<GameLocation>
    {
        public class LocationEnumerator : IEnumerator<GameLocation>
        {
            private int index = -1;
            private GameLocation current
            {
                get
                {
                    if (index >= 0 && StardewValley.Game1.locations.Count > index)
                        return GameLocation.Wrap(StardewValley.Game1.locations[index]);
                    throw new InvalidOperationException();
                }
            }
            public GameLocation Current => current;
            object IEnumerator.Current => current;
            public bool MoveNext()
            {
                if (StardewValley.Game1.locations.Count <= index + 1)
                    return false;
                index++;
                return true;
            }
            public void Reset()
            {
                index =-1;
            }
            public void Dispose()
            {

            }
        }
        internal LocationList()
        {

        }
        public GameLocation this[int index]
        {
            get
            {
                if (index >= 0 && StardewValley.Game1.locations.Count > index)
                    return GameLocation.Wrap(StardewValley.Game1.locations[index]);
                throw new IndexOutOfRangeException();
            }
        }
        public GameLocation this[string name]
        {
            get
            {
                StardewValley.GameLocation loc = StardewValley.Game1.getLocationFromName(name);
                if (loc == null)
                    throw new KeyNotFoundException();
                return GameLocation.Wrap(loc);
            }
        }
        private LocationEnumerator Enumerator()
        {
            return new LocationEnumerator();
        }
        public IEnumerator<GameLocation> GetEnumerator()
        {
            return Enumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Enumerator();
        }
    }
}
