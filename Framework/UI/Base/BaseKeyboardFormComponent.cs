using Microsoft.Xna.Framework.Input;

namespace Entoarox.Framework.UI
{
    public abstract class BaseKeyboardFormComponent : BaseFormComponent, IKeyboardComponent
    {
        /*********
        ** Accessors
        *********/
        public bool Selected { get; set; }


        /*********
        ** Public methods
        *********/
        public virtual void TextReceived(char chr) { }

        public virtual void TextReceived(string str) { }

        public virtual void CommandReceived(char cmd) { }

        public virtual void SpecialReceived(Keys key) { }
    }
}
