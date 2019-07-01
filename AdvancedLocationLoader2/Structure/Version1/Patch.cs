using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.AdvancedLocationLoader2.Structure.Version1
{
    class Patch : MapFileLink
    {
        public int X;
        public int Y;
        public bool Destructive=true;
        public bool Full = true;
    }
}
