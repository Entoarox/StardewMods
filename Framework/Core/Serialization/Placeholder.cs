using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using StardewValley;

namespace Entoarox.Framework.Core.Serialization
{
    internal class Placeholder : CustomObject
    {
        /*********
        ** Fields
        *********/
        private static Texture2D Icon;


        /*********
        ** Accessors
        *********/
        public string Id;
        public JToken Data;

        public override string DisplayName
        {
            get => "(Unknown Item)";
            set => this.displayName = value;
        }


        /*********
        ** Public methods
        *********/
        public Placeholder(string id, JToken data)
        {
            if (Placeholder.Icon == null)
                Placeholder.Icon = EntoaroxFrameworkMod.SHelper.Content.Load<Texture2D>(Path.Combine("Content", "placeholder.png"));
            this.Id = id;
            this.Data = data;
            this.ItemIcon = Placeholder.Icon;
            this.IsDroppable = false;
            this.IsGiftable = false;
            this.IsTrashable = true;
            this.ItemCategory = "Placeholder";
        }

        public override string getDescription()
        {
            return $"({this.Id})\n\nEntoaroxFramework could not load this custom item.\nReinstalling the mod that adds it should repair the item.\n\nIf you removed the mod in question, you can just sell or trash this item.";
        }


        /*********
        ** Protected methods
        *********/
        protected override Item Copy()
        {
            return new Placeholder(this.Id, this.Data);
        }
    }
}
