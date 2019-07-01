using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

namespace Entoarox.AdvancedLocationLoader2
{
    public class ALLModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            this.LoadContent();
        }

        private void LoadContent()
        {
            foreach(var pack in this.Helper.ContentPacks.GetOwned())
            {

            }
        }
    }
}
