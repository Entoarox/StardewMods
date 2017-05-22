using System;
using System.Reflection;

using Microsoft.Xna.Framework.Content;

using StardewModdingAPI.Content;

using Farmhand.Content.Injectors;

namespace SmapiCompatibilityLayer
{
    /// <summary>
    /// Wrapper class for IContentHandler in SMAPI that can then be inserted into Farmhands system for its use.
    /// </summary>
    class IContentHandlerWrapper : IContentInjector, IContentLoader
    {
        private IContentHandler Handler;
        private MethodInfo CanLoad = typeof(IContentHandler).GetMethod("CanLoad");
        private MethodInfo CanInject = typeof(IContentHandler).GetMethod("CanInject");
        public IContentHandlerWrapper(IContentHandler handler)
        {
            Handler = handler;
        }
        public bool HandlesAsset(Type type, string assetName)
        {
            bool handles = false;
            if(Handler.IsLoader)
                handles = (bool)CanLoad.MakeGenericMethod(type).Invoke(Handler, new object[] { assetName });
            if (!handles && Handler.IsInjector)
                handles = (bool)CanInject.MakeGenericMethod(type).Invoke(Handler, new object[] { assetName });
            return handles;
        }
        public T Load<T>(ContentManager contentManager, string assetName)
        {
            return Handler.Load(assetName, contentManager.Load<T>);
        }
        public void Inject<T>(T obj, string assetName, ref object output)
        {
            T asset = (T)output;
            Handler.Inject<T>(assetName, ref asset);
            output = asset;
        }
    }
}