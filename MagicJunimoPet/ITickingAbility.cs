using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace MagicJunimoPet
{
    interface ITickingAbility
    {
        void OnUpdate(GameTime time);
    }
}
