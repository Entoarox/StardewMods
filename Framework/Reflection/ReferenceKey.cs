using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.Framework.Reflection
{
    /// <summary>
    /// This internal type is used to generate and store the Key component in the internal Cache of all Reflected* classes
    /// </summary>
    internal struct ReferenceKey
    {
        private Type Type;
        private string Member;
        public ReferenceKey(Type type, string member)
        {
            Type = type;
            Member = member;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is ReferenceKey))
                return false;
            ReferenceKey value = (ReferenceKey)obj;
            return value.Type.Equals(Type) && value.Member.Equals(Member);
        }
        public override int GetHashCode()
        {
            return Type.GetHashCode() + Member.GetHashCode();
        }
    }
}
