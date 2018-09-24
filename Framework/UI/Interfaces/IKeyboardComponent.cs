using Microsoft.Xna.Framework.Input;

namespace Entoarox.Framework.UI
{
    public interface IKeyboardComponent
    {
        /*********
        ** Accessors
        *********/
        bool Selected { get; set; }


        /*********
        ** Public methods
        *********/
        void TextReceived(char chr);
        void TextReceived(string str);
        void CommandReceived(char cmd);
        void SpecialReceived(Keys key);
    }
}
