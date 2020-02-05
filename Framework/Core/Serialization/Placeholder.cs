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
            get => "(Unknown Item Placeholder)";
            set => this.displayName = value;
        }


        /*********
        ** Public methods
        *********/
        public Placeholder(string id, JToken data)
        {
            if (Placeholder.Icon == null)
                Placeholder.Icon = EntoaroxFrameworkMod.SHelper.Content.Load<Texture2D>(Path.Combine("assets", "placeholder.png"));
            this.Id = id;
            this.Data = data;
            this.Price = 0;
            this.ItemIcon = Placeholder.Icon;
            this.IsDroppable = false;
            this.IsGiftable = false;
            this.IsTrashable = true;
            this.ItemCategory = "Placeholder";
            this.CanBeGrabbed = true;
        }

        public override string getDescription()
        {
            string str = this.Id.Split(' ')[0];
            return $"({str.Substring(0, str.Length - 1)})\n\nEntoaroxFramework could not load this custom item.\nIf you uninstalled the mod that added this item, reinstalling will restore it.\n\nIf this item is broken even with the mod installed,\nOr you intentionally uninstalled the mod then you can simply trash this item.";
        }

        public override int sellToStorePrice(long specificPlayerID = -1)
        {
            return 0;
        }
        public override bool canStackWith(ISalable other)
        {
            return other is Placeholder obj && obj.Id.Equals(this.Id) && obj.Data.Equals(this.Data);
        }
        public override bool clicked(Farmer who)
        {
            return who.addItemToInventoryBool(this);
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
