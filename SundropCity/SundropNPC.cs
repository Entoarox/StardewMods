using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using StardewValley;

namespace SundropCity
{
    class SundropNPC : NPC
    {
        public SundropNPC(AnimatedSprite sprite, Vector2 position, int facingDir, string name) : base(sprite, position, facingDir, name)
        {

        }
        public override void reloadSprite()
        {
            this.displayName = SundropCityMod.SHelper.Translation.Get("Character." + this.Name + ".Name").Default(this.Name);
        }
    }
}
