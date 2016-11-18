using Microsoft.Xna.Framework.Input;

using StardewValley;

namespace Entoarox.Framework.Menus
{
    abstract public class BaseKeyboardFormComponent : BaseFormComponent, IKeyboardSubscriber
    {
        public bool Selected {get; set; }
        public override void FocusGained(IComponentContainer c, FrameworkMenu m)
        {
            Game1.keyboardDispatcher.Subscriber = this;
            Selected = true;
            m.TextboxActive = true;
        }
        public override void FocusLost(IComponentContainer c, FrameworkMenu m)
        {
            Game1.keyboardDispatcher.Subscriber = null;
            Selected = false;
            m.TextboxActive = false;
        }
        public virtual void KeyReceived(char r, IComponentContainer c, FrameworkMenu m)
        {

        }
        public virtual void TextReceived(string r, IComponentContainer c, FrameworkMenu m)
        {

        }
        public virtual void CommandReceived(char r, IComponentContainer c, FrameworkMenu m)
        {

        }
        public virtual void SpecialReceived(Keys r, IComponentContainer c, FrameworkMenu m)
        {

        }
        void IKeyboardSubscriber.RecieveTextInput(char inputChar)
        {
            if (Disabled || !Selected)
                return;
            KeyReceived(inputChar, Parent, Parent.GetAttachedMenu());
        }
        void IKeyboardSubscriber.RecieveTextInput(string text)
        {
            if (Disabled || !Selected)
                return;
            TextReceived(text, Parent, Parent.GetAttachedMenu());
        }
        void IKeyboardSubscriber.RecieveCommandInput(char c)
        {
            if (Disabled || !Selected)
                return;
            CommandReceived(c, Parent, Parent.GetAttachedMenu());
        }
        void IKeyboardSubscriber.RecieveSpecialInput(Keys k)
        {
            if (Disabled || !Selected)
                return;
            SpecialReceived(k, Parent, Parent.GetAttachedMenu());
        }
    }
}
