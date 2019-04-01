using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SundropCity.Hotel
{
    class Upgrade
    {
        public string Id;
        public string[] Requirements;
        public Upgrade(string id, string[] requirements)
        {
            this.Id = id;
            this.Requirements = requirements;
        }
    }
}
