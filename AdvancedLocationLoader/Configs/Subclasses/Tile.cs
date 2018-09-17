namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class Tile : TileInfo
    {
        /*********
        ** Accessors
        *********/
        public int? Interval;
        public string LayerId;
        public string SheetId;
        public int? TileIndex;
        public int[] TileIndexes;


        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            if (this.TileIndex != null)
                if (this.SheetId != null)
                    return $"Tile({this.MapName}@[{this.TileX}{','}{this.TileY}]:{this.LayerId} = `{this.SheetId}:{this.TileIndex}`)";
                else
                    return $"Tile({this.MapName}@[{this.TileX}{','}{this.TileY}]:{this.LayerId} = `{this.TileIndex}`)";
            if (this.SheetId != null)
                return $"Tile({this.MapName}@[{this.TileX}{','}{this.TileY}]:{this.LayerId} = `{this.SheetId}:{string.Join(",", this.TileIndexes)}`)";
            return $"Tile({this.MapName}@[{this.TileX}{','}{this.TileY}]:{this.LayerId} = `{string.Join(",", this.TileIndexes)}`)";
        }
    }
}
