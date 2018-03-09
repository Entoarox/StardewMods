using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace Entoarox.Framework.Core.Serialization
{
    class InstanceState
    {
        public string Type;
        public JToken Data;

        public InstanceState()
        {

        }
        public InstanceState(string type, JToken data)
        {
            this.Type = type;
            this.Data = data;
        }
    }
}
