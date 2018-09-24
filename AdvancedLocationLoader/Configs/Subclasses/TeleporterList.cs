using System.Collections.Generic;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class TeleporterList
    {
        /*********
        ** Accessors
        *********/
        public List<TeleporterDestination> Destinations;

        public string ListName;


        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"TeleporterList({this.ListName}) => {{{string.Join(",", this.Destinations)}{'}'}";
        }
    }
}
