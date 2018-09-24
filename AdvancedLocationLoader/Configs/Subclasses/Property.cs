namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class Property : TileInfo
    {
        /*********
        ** Accessors
        *********/
        public string Key;
        public string LayerId;
        public string Value;


        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"Property({this.MapName}@[{this.TileX}{','}{this.TileY}]:{this.LayerId} => `{this.Key}` = {this.Value}{')'}";
        }
    }
}
