using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.Framework.Interface
{
    public interface IControllerComponent
    {
        bool ReceiveController(Microsoft.Xna.Framework.Input.Buttons button);
    }
}
