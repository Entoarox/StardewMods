using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Interface
{
    public abstract class BaseInputComponent : BaseDynamicComponent, IInputComponent
    {
        /*********
        ** Accessors
        *********/
        public bool Selected { get; set; }


        /*********
        ** Public methods
        *********/
        public abstract void ReceiveInput(char input);


        /*********
        ** Protected methods
        *********/
        protected BaseInputComponent(string name, Rectangle bounds, int layer)
            : base(name, bounds, layer) { }
    }
}
