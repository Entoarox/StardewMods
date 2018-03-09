using Microsoft.Xna.Framework.Input;

namespace Entoarox.Framework.Interface
{
    interface IControllerComponent
    {
        bool ReceiveController(Buttons button);
    }
}
