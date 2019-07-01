using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MagicJunimoPet.Abilities
{
    class PetAbility : Ability, IActiveAbility, IRestrictedAbility
    {
        private static readonly Texture2D Icon;
        private static Rectangle Region = new Rectangle(0, 0, 16, 16);
        private string Stamp = null;

        static PetAbility()
        {
            Icon = MJPModEntry.SHelper.Content.Load<Texture2D>("assets/icons.png");
        }
        public override string Id => "Pet";

        public override void DrawIcon(Rectangle region, SpriteBatch batch)
        {
            batch.Draw(Icon, region, Region, Color.White);
        }
        
        public void DrawTriggerIcon(Rectangle region, SpriteBatch batch)
        {
            this.DrawIcon(region, batch);
        }

        public string GetLabel()
        {
            return this.GetName();
        }

        public bool IsAvailable()
        {
            return !this.Stamp.Equals(this.GetStamp());
        }

        public void OnTrigger(Farmer who)
        {
            this.Stamp = this.GetStamp();
            this.Pet.ModExperience(5);
        }

        private string GetStamp()
        {
            return Game1.dayOfMonth.ToString() + Game1.currentSeason + Game1.year.ToString();
        }
    }
}
