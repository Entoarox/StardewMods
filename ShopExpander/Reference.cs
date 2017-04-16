namespace Entoarox.ShopExpander
{
    public class Reference
    {
        public string Owner;
        public int Item;
        public int Amount;
        public string Conditions = null;
        public Reference(string owner, int item, int amount, string conditions = null)
        {
            this.Owner = owner;
            this.Item = item;
            this.Amount = amount;
            this.Conditions = conditions;
        }
    }
}
