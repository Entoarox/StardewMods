using System.Collections.Generic;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class TeleporterList
    {
        /*********
        ** Accessors
        *********/
#pragma warning disable CS0649
        public List<TeleporterDestination> Destinations;
#pragma warning restore CS0649

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
