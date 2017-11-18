namespace Entoarox.Framework.UI
{
    public class KeyboardSubscriberProxy : StardewValley.IKeyboardSubscriber
    {
        protected IKeyboardComponent Component;
        public KeyboardSubscriberProxy(IKeyboardComponent component)
        {
            this.Component = component;
        }
        public bool Selected
        {
            get
            {
                return this.Component.Selected;
            }
            set
            {
                if (value == false)
                    this.Component.Selected = false;
            }
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
        public void RecieveSpecialInput(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (this.Component.Selected)
                this.Component.SpecialReceived(key);
        }
    }
}
