namespace Entoarox.Framework.Menus
{
    abstract public class BaseFormComponent : BaseInteractiveMenuComponent
    {
        public bool Disabled = false;
        public delegate void ValueChanged<T>(int optionKey, T value);
    }
}
