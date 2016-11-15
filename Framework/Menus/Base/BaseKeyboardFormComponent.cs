using Microsoft.Xna.Framework.Input;

using StardewValley;

namespace Entoarox.Framework.Menus
{
    abstract public class BaseKeyboardFormComponent : BaseFormComponent, IKeyboardSubscriber
    {
        private FrameworkMenu Menu;
        private IComponentCollection Collection;
        public bool Selected {get;set; }
        public override void FocusGained(IComponentCollection c, FrameworkMenu m)
        {
            Game1.keyboardDispatcher.Subscriber = this;
            Selected = true;
            Menu = m;
            Collection = c;
            m.TextboxActive = true;
        }
        public override void FocusLost(IComponentCollection c, FrameworkMenu m)
        {
            Game1.keyboardDispatcher.Subscriber = null;
            Selected = false;
            m.TextboxActive = false;
        }
        public virtual void KeyReceived(char r, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void TextReceived(string r, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void CommandReceived(char r, IComponentCollection c, FrameworkMenu m)
        {

        }
        public virtual void SpecialReceived(Keys r, IComponentCollection c, FrameworkMenu m)
        {

        }
        void IKeyboardSubscriber.RecieveTextInput(char inputChar)
        {
            if (Disabled || !Selected)
                return;
            KeyReceived(inputChar, Collection, Menu);
        }
        void IKeyboardSubscriber.RecieveTextInput(string text)
        {
            if (Disabled || !Selected)
                return;
            TextReceived(text, Collection, Menu);
        }
        void IKeyboardSubscriber.RecieveCommandInput(char c)
        {
            if (Disabled || !Selected)
                return;
            CommandReceived(c, Collection, Menu);
        }
        void IKeyboardSubscriber.RecieveSpecialInput(Keys k)
        {
            if (Disabled || !Selected)
                return;
            SpecialReceived(k, Collection, Menu);
        }
    }
}
