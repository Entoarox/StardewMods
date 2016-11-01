using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.AdvancedLocationLoader.Loaders
{
    class LoaderVersionAttribute : Attribute
    {
        public string Version;
        public LoaderVersionAttribute(string version)
        {
            Version = version;
        }
    }
    interface ILoader
    {
        void Load(string file);
    }
}
