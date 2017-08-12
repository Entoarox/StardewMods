using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley;

namespace Entoarox.Framework.Core
{
    internal class PlayerHelper : IPlayerHelper
    {
        private IPlayerModifierHelper _Modifiers;
        public IPlayerModifierHelper Modifiers
        {
            get
            {
                if (_Modifiers == null)
                    _Modifiers = new PlayerModifierHelper();
                return _Modifiers;
            }
        }
        public void MoveTo(int x, int y)
        {

        }
        public void MoveTo(string location, int x, int y)
        {

        }
        public void MoveTo(GameLocation location, int x, int y)
        {

        }
    }
}
