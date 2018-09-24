namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class Override : MapFileLink
    {
        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"Override({this.MapName},{this.FileName})";
        }
    }
}
