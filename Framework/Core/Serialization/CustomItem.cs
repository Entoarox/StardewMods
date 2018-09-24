using Newtonsoft.Json.Linq;

namespace Entoarox.Framework.Core.Serialization
{
    internal class InstanceState
    {
        /*********
        ** Accessors
        *********/
        public string Type;
        public JToken Data;


        /*********
        ** Public methods
        *********/
        public InstanceState() { }

        public InstanceState(string type, JToken data)
        {
            this.Type = type;
            this.Data = data;
        }
    }
}
