using System.ComponentModel;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    // Collector classes to minimise repeated typing
    abstract public class TileInfo
    {
        public string MapName;
        public int TileX;
        public int TileY;
        public string Conditions;
        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Optional;
    }
    abstract public class MapFileLink
    {
        public string MapName;
        public string FileName;
    }
    // Final classes 
    public class ShopItem // ShopConfig
    {
        public int Id;
        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool BigCraftable;
        public int? Price;
        [DefaultValue(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int Stack;
        [DefaultValue(int.MaxValue)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int Stock;
        public string Conditions;
        public override string ToString()
        {
            return "ShopItem(" + Id + ":" + (BigCraftable ? "craftable" : "object") + "){Price=" + Price.ToString() + ",Stack=" + Stack.ToString() + ",Stock=" + Stock.ToString() + ",Conditions=" + Conditions + "}";
        }
    }
    // LocationConfig1_2
    public class About
    {
        public string ModName;
        public string Author;
        public List<string> ThanksTo;
        public string Website;
        public string Description;
        public string Version;
    }
    public class Location : MapFileLink
    {
        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Outdoor;
        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Farmable;
        [DefaultValue("Default")]
        [JsonProperty(DefaultValueHandling =DefaultValueHandling.Populate)]
        public string Type;
        public override string ToString()
        {
            return "Location(" + MapName + "," + FileName + "){Outdoor=" + Outdoor + ",Farmable=" + Farmable + ",Type=" + Type + "}";
        }
    }
    public class Override : MapFileLink
    {
        //public bool? Partial;
        public override string ToString()
        {
            return "Override(" + MapName + "," + FileName + ")";
        }
    }
    public class Redirect
    {
        public string FromFile;
        public string ToFile;
        public override string ToString()
        {
            return "Redirect(" + FromFile + " => " + ToFile + ')';
        }
    }
    public class TeleporterDestination
    {
        public string MapName;
        public int TileX;
        public int TileY;
        [DefaultValue(0)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int Direction;
        public string ItemText;
        public override bool Equals(object a)
        {
            if (a is TeleporterDestination)
            {
                TeleporterDestination b = (TeleporterDestination)a;
                return b.MapName == MapName && b.TileX == TileX && b.TileY == TileY;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return MapName + "@[" + TileX + "," + TileY + "]:" + Direction;
        }
    }
    public class TeleporterList
    {
        
        public string ListName;
        public List<TeleporterDestination> Destinations;
        public override string ToString()
        {
            return "Minecart(" + ListName + ") => [" + string.Join(",", Destinations) + ']';
        }
    }
    public class Tilesheet : MapFileLink
    {
        public string SheetId;
        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Seasonal;
        public Dictionary<string, string> Properties = new Dictionary<string, string>();
        public override string ToString()
        {
            return "Tilesheet(" + MapName + ":" + SheetId + "," + (FileName == null ? "null" : FileName) + "){Seasonal=" + Seasonal + "}";
        }
    }
    public class Tile : TileInfo
    {
        public string SheetId;
        public string LayerId;
        public int? TileIndex;
        public int[] TileIndexes;
        public int? Interval;
        public override string ToString()
        {
            if(TileIndex!=null)
                if (SheetId != null)
                    return "Tile(" + MapName + "@[" + TileX + ',' + TileY + "]:" + LayerId + " = `" + SheetId + ":" + TileIndex + "`)";
                else
                    return "Tile(" + MapName + "@[" + TileX + ',' + TileY + "]:" + LayerId + " = `" + TileIndex + "`)";
            else
                if (SheetId != null)
                    return "Tile(" + MapName + "@[" + TileX + ',' + TileY + "]:" + LayerId + " = `" + SheetId + ":" + string.Join(",", TileIndexes) + "`)";
                else
                    return "Tile(" + MapName + "@[" + TileX + ',' + TileY + "]:" + LayerId + " = `" + string.Join(",", TileIndexes) + "`)";
        }
    }
    public class Property : TileInfo
    {
        public string Key;
        public string Value;
        public string LayerId;
        public override string ToString()
        {
            return "Property(" + MapName + "@[" + TileX + ',' + TileY + "]:" + LayerId + " => `" + Key + "` = " + Value + ')';
        }
    }
    public class Warp : TileInfo
    {
        public string TargetName;
        public int TargetX;
        public int TargetY;
        [JsonIgnore]
        public new bool Optional;
        public override string ToString()
        {
            return "Warp(" + MapName + "@[" + TileX + ',' + TileY + "] => " + TargetName + "@[" + TargetX + ',' + TargetY + "])";
        }
    }
    public class Conditional
    {
        public string Name;
        public int Item;
        public int Amount;
        public string Question;
        public override string ToString()
        {
            return "Conditional(" + Name + "[" + Item + ':' + Amount + "] = `" + Question + "`)";
        }
    }
}
