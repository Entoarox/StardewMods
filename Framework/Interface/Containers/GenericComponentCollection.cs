using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Interface
{
    public class GenericComponentCollection : BaseComponentCollection
    {
        public GenericComponentCollection(string name, Rectangle bounds, int layer=0) : base(name, bounds, layer)
        {

        }
    }
}
