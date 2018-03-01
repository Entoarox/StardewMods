using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.Framework.Core.Serialization
{
    class CustomItem
    {
        public string Type;
        public string Data;

        public CustomItem()
        {

        }
        public CustomItem(string type, string data)
        {
            this.Type = type;
            this.Data = data;
        }
    }
}
