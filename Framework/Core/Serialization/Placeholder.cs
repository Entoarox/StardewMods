using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;

using StardewValley;

using Newtonsoft.Json.Linq;

namespace Entoarox.Framework.Core.Serialization
{
    class Placeholder : CustomObject
    {
        private static Texture2D Icon;

        public string Id;
        public JToken Data;

        public Placeholder(string id, JToken data)
        {
            if (Icon == null)
                Icon = EntoaroxFrameworkMod.SHelper.Content.Load<Texture2D>(System.IO.Path.Combine("Content", "placeholder.png"));
            this.Id = id;
            this.Data = data;
            this.ItemIcon = Icon;
            this.IsDroppable = false;
            this.IsGiftable = false;
            this.IsTrashable = true;
            this.ItemCategory = "Placeholder";
        }
        public override string DisplayName { get => "(Unknown Item)"; set => this.displayName = value; }

        protected override Item Copy()
        {
            return new Placeholder(this.Id, this.Data);
        }
        public override string getDescription()
        {
            return "(" + this.Id + ")\n\nEntoaroxFramework could not load this custom item.\nReinstalling the mod that adds it should repair the item.\n\nIf you removed the mod in question, you can just sell or trash this item.";
        }
    }
}
