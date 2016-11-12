namespace Entoarox.Framework.Menus
{
    abstract public class BaseFormComponent : BaseInteractiveMenuComponent
    {
        public bool Disabled = false;
        public delegate void ValueChanged<T>(BaseFormComponent component,IComponentCollection collection, FrameworkMenu menu, T value);
        public delegate void ButtonPressed(ButtonFormComponent button, IComponentCollection collection, FrameworkMenu menu);
    }
}
