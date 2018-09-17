namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class Redirect
    {
        /*********
        ** Accessors
        *********/
        public string FromFile;
        public string ToFile;


        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"Redirect({this.FromFile} => {this.ToFile}{')'}";
        }
    }
}
