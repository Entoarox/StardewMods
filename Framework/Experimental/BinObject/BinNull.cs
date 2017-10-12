using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.Framework.Experimental.BinObject
{
    public sealed class BinNull : BinObject
    {
        internal BinNull() : base(BinType.Null)
        { }
        public static bool operator ==(BinNull first, BinNull second) => true;
        public static bool operator !=(BinNull first, BinNull second) => false;
        public static bool operator ==(BinNull first, object second) => second==null;
        public static bool operator !=(BinNull first, object second) => second!=null;
        public override bool Equals(object obj) => obj == null;
        public override int GetHashCode() => 0;
    }
}
