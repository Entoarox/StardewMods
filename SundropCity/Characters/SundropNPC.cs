using System.Collections.Generic;

using Microsoft.Xna.Framework;

using StardewValley;

namespace SundropCity.Characters
{
    class SundropNPC : NPC, Internal.ISundropTransient
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
