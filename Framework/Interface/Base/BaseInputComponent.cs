using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Interface
{
    public abstract class BaseInputComponent : BaseDynamicComponent, IInputComponent
    {
        protected BaseInputComponent(string name, Rectangle bounds, int layer) : base(name, bounds, layer)
        {
        }
        public bool Selected { get; set; }

        abstract public void ReceiveInput(char input);
    }
}
