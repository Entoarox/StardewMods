namespace Entoarox.Framework.Interface
{
    internal interface IVisibilityObserver : IComponentContainer
    {
        /*********
        ** Methods
        *********/
        void VisibilityChanged(IComponent component);
    }
}
