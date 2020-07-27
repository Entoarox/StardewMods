using System.Collections.Generic;

using StardewValley;

namespace SundropCity.Json
{
    class ShopData
    {
        public class ShopItem
        {
            // key
            public string Item;
            // value.int[0]
            public int? Price = null;
            // value.int[1]
            public int Amount = int.MaxValue;
            // value.int[2]?
            public string TradeItem = null;
            // value.int[3]?
            public int TradeAmount = 5;
        }
        public int Sprite = 0;
        public List<ShopItem> Items = new List<ShopItem>();

        public Dictionary<ISalable, int[]> GetStock()
        {
            Dictionary<ISalable, int[]> result = new Dictionary<ISalable, int[]>();
            foreach(var entry in this.Items)
            {
                ISalable mainItem = SundropCityMod.EMUApi.ResolveItemTypeId(entry.Item);
                int[] data = new int[2];
                if (mainItem == null || result.ContainsKey(mainItem))
                    continue;
                if (entry.TradeItem != null)
                {
                    ISalable tradeItem = SundropCityMod.EMUApi.ResolveItemTypeId("sdv.object:"+entry.TradeItem);
                    if (tradeItem == null)
                        continue;
                    data = new int[4];
                    data[2] = ((Item)tradeItem).ParentSheetIndex;
                    data[3] = entry.TradeAmount;
                }
                data[0] = entry.Price ?? mainItem.salePrice();
                data[1] = entry.Amount;
                result.Add(mainItem, data);
            }
            return result;
        }
    }
}
