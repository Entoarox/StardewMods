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
    class FeedAbility : Ability, IActiveAbility, IRestrictedAbility
    {
        private static readonly Texture2D Icon;
        private static Rectangle Region = new Rectangle(0, 0, 16, 16);
        private string Stamp = null;
        private readonly int FavoriteFood;

        static FeedAbility()
        {
            Icon = MJPModEntry.SHelper.Content.Load<Texture2D>("assets/icons.png");
        }

        public FeedAbility(int favoriteFood)
        {
            this.FavoriteFood = favoriteFood;
        }

        public override string Id => "Feed";

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
            //TODO: Implement feeding UI
        }

        private string GetStamp()
        {
            return Game1.dayOfMonth.ToString() + Game1.currentSeason + Game1.year.ToString();
        }
    }
}
