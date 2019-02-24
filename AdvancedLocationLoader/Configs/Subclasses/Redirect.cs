namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class Redirect
    {
        /*********
        ** Accessors
        *********/
#pragma warning disable CS0649
        public string FromFile;
        public string ToFile;
#pragma warning restore CS0649


        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"Redirect({this.FromFile} => {this.ToFile}{')'}";
        }
    }
}
