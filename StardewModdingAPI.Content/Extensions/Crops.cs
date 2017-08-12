using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewModdingAPI.Content.Extensions
{
    public static class Crops
    {
        public static ItemData GetItemDataFromId(this IContentRegistry registry, int id)
        {
            return new ItemData();
        }
        public static void RegisterCustomCrop(this IContentRegistry registry, CropData crop, ItemData seeds, ItemData produce)
        {

        }
    }
}
