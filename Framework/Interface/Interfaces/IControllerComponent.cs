using Microsoft.Xna.Framework.Input;

namespace Entoarox.Framework.Interface
{
    internal interface IControllerComponent
    {
        /*********
        ** Methods
        *********/
        bool ReceiveController(Buttons button);
    }
}
