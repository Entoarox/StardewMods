namespace Entoarox.Framework.Interface
{
    interface IVisibilityObserver : IComponentContainer
    {
        void VisibilityChanged(IComponent component);
    }
}
