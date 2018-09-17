using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace Entoarox.Framework.UI
{
    public class KeyboardSubscriberProxy : IKeyboardSubscriber
    {
        /*********
        ** Fields
        *********/
        protected IKeyboardComponent Component;


        /*********
        ** Accessors
        *********/
        public bool Selected
        {
            get => this.Component.Selected;
            set
            {
                if (value == false)
                    this.Component.Selected = false;
            }
        }


        /*********
        ** Public methods
        *********/
        public KeyboardSubscriberProxy(IKeyboardComponent component)
        {
            this.Component = component;
        }

        public void RecieveTextInput(char chr)
        {
            if (this.Component.Selected)
                this.Component.TextReceived(chr);
        }

        public void RecieveTextInput(string str)
        {
            if (this.Component.Selected)
                this.Component.TextReceived(str);
        }

        public void RecieveCommandInput(char cmd)
        {
            if (this.Component.Selected)
                this.Component.CommandReceived(cmd);
        }

        public void RecieveSpecialInput(Keys key)
        {
            if (this.Component.Selected)
                this.Component.SpecialReceived(key);
        }
    }
}
