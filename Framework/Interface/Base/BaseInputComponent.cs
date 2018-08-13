using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Interface
{
    public abstract class BaseInputComponent : BaseDynamicComponent, IInputComponent
    {
        protected BaseInputComponent(string name, Rectangle bounds, int layer) : base(name, bounds, layer)
        {
        }
        public bool Selected { get; set; }

        public abstract void ReceiveInput(char input);
    }
}
