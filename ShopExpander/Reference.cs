namespace Entoarox.ShopExpander
{
    internal class Reference
    {
        /*********
        ** Accessors
        *********/
        public string Owner;
        public int Item;
        public int Amount;
        public string Conditions = null;


        /*********
        ** Public methods
        *********/
        public Reference(string owner, int item, int amount, string conditions = null)
        {
            this.Owner = owner;
            this.Item = item;
            this.Amount = amount;
            this.Conditions = conditions;
        }
    }
}
