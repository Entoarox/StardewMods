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
            return "ShopItem(" + this.Id + ":" + (this.BigCraftable ? "craftable" : "object") + "){Price=" + this.Price.ToString() + ",Stack=" + this.Stack.ToString() + ",Stock=" + this.Stock.ToString() + ",Conditions=" + this.Conditions + "}";
        }
    }
    // LocationConfig1_2
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
            return "Location(" + this.MapName + "," + this.FileName + "){Outdoor=" + this.Outdoor + ",Farmable=" + this.Farmable + ",Type=" + this.Type + "}";
        }
    }
    public class Override : MapFileLink
    {
        //public bool? Partial;
        public override string ToString()
        {
            return "Override(" + this.MapName + "," + this.FileName + ")";
        }
    }
    public class Redirect
    {
        public string FromFile;
        public string ToFile;
        public override string ToString()
        {
            return "Redirect(" + this.FromFile + " => " + this.ToFile + ')';
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
                return b.MapName == this.MapName && b.TileX == this.TileX && b.TileY == this.TileY;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return this.MapName + "@[" + this.TileX + "," + this.TileY + "]:" + this.Direction;
        }
    }
    public class TeleporterList
    {
        
        public string ListName;
        public List<TeleporterDestination> Destinations;
        public override string ToString()
        {
            return "TeleporterList(" + this.ListName + ") => {" + string.Join(",", this.Destinations) + '}';
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
            return "Tilesheet(" + this.MapName + ":" + this.SheetId + "," + (this.FileName == null ? "null" : this.FileName) + "){Seasonal=" + this.Seasonal + "}";
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
            if(this.TileIndex !=null)
                if (this.SheetId != null)
                    return "Tile(" + this.MapName + "@[" + this.TileX + ',' + this.TileY + "]:" + this.LayerId + " = `" + this.SheetId + ":" + this.TileIndex + "`)";
                else
                    return "Tile(" + this.MapName + "@[" + this.TileX + ',' + this.TileY + "]:" + this.LayerId + " = `" + this.TileIndex + "`)";
            else
                if (this.SheetId != null)
                    return "Tile(" + this.MapName + "@[" + this.TileX + ',' + this.TileY + "]:" + this.LayerId + " = `" + this.SheetId + ":" + string.Join(",", this.TileIndexes) + "`)";
                else
                    return "Tile(" + this.MapName + "@[" + this.TileX + ',' + this.TileY + "]:" + this.LayerId + " = `" + string.Join(",", this.TileIndexes) + "`)";
        }
    }
    public class Property : TileInfo
    {
        public string Key;
        public string Value;
        public string LayerId;
        public override string ToString()
        {
            return "Property(" + this.MapName + "@[" + this.TileX + ',' + this.TileY + "]:" + this.LayerId + " => `" + this.Key + "` = " + this.Value + ')';
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
            return "Warp(" + this.MapName + "@[" + this.TileX + ',' + this.TileY + "] => " + this.TargetName + "@[" + this.TargetX + ',' + this.TargetY + "])";
        }
    }
    public class Conditional
    {
        public string Name;
        public int Item;
        public int Amount;
        public string Question;
        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string Success;
        public override string ToString()
        {
            return "Conditional(" + this.Name + "[" + this.Item + ':' + this.Amount + "] = `" + this.Question + "`)";
        }
    }
}
