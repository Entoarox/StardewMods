using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.Utilities.Internals.Api
{
    interface ICustomFarmingApi
    {
        StardewValley.Item getCustomObject(string id);
    }
}
