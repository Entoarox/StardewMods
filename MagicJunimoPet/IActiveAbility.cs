using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace MagicJunimoPet
{
    interface IActiveAbility
    {
        string GetLabel();
        void OnTrigger(Farmer who);
        void DrawTriggerIcon(Rectangle region, SpriteBatch batch);
    }
}
