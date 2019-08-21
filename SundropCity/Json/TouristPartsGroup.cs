using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SundropCity.Json
{
    class TouristPartsGroup
    {
        public readonly Dictionary<string, List<string>> Male = new Dictionary<string, List<string>>()
        {
            ["Body"]=new List<string>(),
            ["Top"]=new List<string>(),
            ["Bottom"] = new List<string>(),
            ["Shoe"] = new List<string>(),
            ["Accessory"] = new List<string>(),
            ["Hair"] = new List<string>(),
            ["Decoration"] = new List<string>(),
            ["Hat"] = new List<string>()
        };
        public readonly Dictionary<string, List<string>> Female = new Dictionary<string, List<string>>()
        {
            ["Body"] = new List<string>(),
            ["Top"]=new List<string>(),
            ["Bottom"] = new List<string>(),
            ["Shoe"] = new List<string>(),
            ["Accessory"] = new List<string>(),
            ["Hair"] = new List<string>(),
            ["Decoration"] = new List<string>(),
            ["Hat"] = new List<string>()
        };
    }
}
