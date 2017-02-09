using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using StardewValley;

using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework
{
    internal class ContentRegistry : IContentRegistry
    {
        internal static IContentRegistry Singleton { get; } = new ContentRegistry();
        void IContentRegistry.RegisterHandler<T>(string key, FileLoadMethod<T> method)
        {
            try
            {
                switch (EntoFramework.LoaderType)
                {
                    case EntoFramework.LoaderTypes.SMAPI:
                        SmartManager.RegisterHandler(key, method);
                        break;
                    case EntoFramework.LoaderTypes.FarmHand:
                        registerHandler.MakeGenericMethod(typeof(T)).Invoke(null, new object[] {key,method});
                        break;
                }
            }
            catch (Exception err)
            {
                EntoFramework.Logger.ExitGameImmediately("Was unable to register custom handler in loader"+ err);
            }
        }
        void IContentRegistry.RegisterTexture(string key, string path)
        {
            try
            {
                switch (EntoFramework.LoaderType)
                {
                    case EntoFramework.LoaderTypes.SMAPI:
                        SmartManager.RegisterTexture(key, path);
                        break;
                    case EntoFramework.LoaderTypes.FarmHand:
                        dynamic xnb = Activator.CreateInstance(modXnb);
                        dynamic tex = Activator.CreateInstance(diskTexture);
                        xnb.Original = key;
                        xnb.Texture = key;
                        xnb.OwingMod = myManifest;
                        tex.Id = key;
                        tex.AbsoluteFilePath = path;
                        registerTexture.Invoke(null, new object[] { key, tex, myManifest });
                        registerXnb.Invoke(null, new object[] { key, xnb, myManifest });
                        break;
                }
            }
            catch(Exception err)
            {
                EntoFramework.Logger.ExitGameImmediately("Was unable to register texture file in loader"+err);
            }
        }
        void IContentRegistry.RegisterXnb(string key, string path)
        {
            try
            {
                switch (EntoFramework.LoaderType)
                {
                    case EntoFramework.LoaderTypes.SMAPI:
                        SmartManager.RegisterXnb(key, path);
                        break;
                    case EntoFramework.LoaderTypes.FarmHand:
                        dynamic xnb = Activator.CreateInstance(modXnb);
                        xnb.Original = key;
                        xnb.File = key;
                        xnb.OwingMod = myManifest;
                        xnb.AbsoluteFilePath = path;
                        registerXnb.Invoke(null, new object[] { key, xnb, myManifest });
                        break;
                }
            }
            catch (Exception err)
            {
                EntoFramework.Logger.ExitGameImmediately("Was unable to register xnb file in loader"+ err);
            }
        }
        private static Type modXnb;
        private static Type diskTexture;
        private static MethodInfo registerTexture;
        private static MethodInfo registerXnb;
        private static MethodInfo registerHandler;
        private static dynamic myManifest;
        private static SmartContentManager SmartManager;
        private static SmartDisplayDevice SmartDevice;
        internal static void Setup()
        {
            if (EntoFramework.LoaderType == EntoFramework.LoaderTypes.FarmHand)
            {
                try
                {
                    Type textureRegistry = Type.GetType("Farmhand.Registries.TextureRegistry");
                    Type xnbRegistry = Type.GetType("Farmhand.Registries.XnbRegistry");
                    Type modManifest = Type.GetType("Farmhand.Registries.Containers.ModManifest");
                    Type contentHandler = Type.GetType("Farmhand.Content.DelegatedContentInjector");
                    modXnb = Type.GetType("Farmhand.Registries.Containers.ModXnb");
                    diskTexture = Type.GetType("Farmhand.Registries.Containers.DiskTexture");
                    registerTexture = textureRegistry.GetMethod("RegisterItem", BindingFlags.Static | BindingFlags.Public);
                    registerXnb = xnbRegistry.GetMethod("RegisterItem", BindingFlags.Static | BindingFlags.Public);
                    registerHandler = contentHandler.GetMethod("RegisterFileLoader", BindingFlags.Static | BindingFlags.Public);
                    myManifest = Activator.CreateInstance(modManifest);
                    myManifest.ModDLL = "EntoaroxFramework.dll";
                    myManifest.Name = "Entoarox Framework";
                    myManifest.Author = "Entoarox";
                    myManifest.Version = EntoFramework.Version;
                    myManifest.Description = "A collection of framework classes to make modding stardew easier";
                    // Secure tests to make sure there are no crashes when register methods are called
                    dynamic xnb = Activator.CreateInstance(modXnb);
                    dynamic tex = Activator.CreateInstance(diskTexture);
                    xnb.Original = "";
                    xnb.Texture = "";
                    xnb.File = "";
                    xnb.OwingMod = myManifest;
                    xnb.AbsoluteFilePath = "";
                    tex.Id = "";
                    tex.AbsoluteFilePath = "";
                }
                catch (Exception err)
                {
                    EntoFramework.Logger.ExitGameImmediately("Was unable to hook into FarmHand content loading"+ err);
                    EntoFramework.LoaderType = EntoFramework.LoaderTypes.Unknown;
                }
            }
        }
        internal static void Init()
        {
            SmartManager = new SmartContentManager(Game1.content.ServiceProvider, Game1.content.RootDirectory);
            SmartDevice = new SmartDisplayDevice(SmartManager, Game1.game1.GraphicsDevice);
        }
        internal static void Update(object s, EventArgs e)
        {
            Game1.content = SmartManager;
            Game1.mapDisplayDevice = SmartDevice;
        }
    }
    internal class SmartDisplayDevice : xTile.Display.XnaDisplayDevice
    {
        public SmartDisplayDevice(Microsoft.Xna.Framework.Content.ContentManager contentManager, GraphicsDevice graphicsDevice) : base(contentManager, graphicsDevice)
        {

        }
    }
    internal class SmartContentManager : LocalizedContentManager
    {
        private Dictionary<string, SmartContentManager> Registry = new Dictionary<string, SmartContentManager>();
        private List<string> HandledFiles = new List<string>();
        private Dictionary<string, string> XnbMatches = new Dictionary<string, string>();
        private Dictionary<string, string> TextureMatches = new Dictionary<string, string>();
        private Dictionary<string, KeyValuePair<Type,Delegate>> DelegateMatches = new Dictionary<string, KeyValuePair<Type,Delegate>>();
        private Dictionary<string, object> TextureCache = new Dictionary<string, object>();
        public SmartContentManager(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider, rootDirectory)
        {
        }
        public override T Load<T>(string assetName)
        {
            assetName = assetName.Replace(Path.DirectorySeparatorChar=='/'?'\\':'/',Path.DirectorySeparatorChar);
            if (HandledFiles.Contains(assetName))
            {
                if (XnbMatches.ContainsKey(assetName))
                    return Registry[Path.GetDirectoryName(XnbMatches[assetName])].Load<T>(Path.GetFileName(XnbMatches[assetName]));
                if (TextureMatches.ContainsKey(assetName) && typeof(T) == typeof(Texture2D))
                {
                    if(!TextureCache.ContainsKey(assetName))
                        TextureCache.Add(assetName,Texture2D.FromStream(Game1.graphics.GraphicsDevice, new FileStream(TextureMatches[assetName], FileMode.Open)));
                    return (T)TextureCache[assetName];
                }
                if (DelegateMatches.ContainsKey(assetName) && DelegateMatches[assetName].Key == typeof(T))
                    return ((FileLoadMethod<T>)DelegateMatches[assetName].Value)(base.Load<T>, assetName);
            }
            return base.Load<T>(assetName);
        }
        private bool CanRegister(string assetName)
        {
            if (HandledFiles.Contains(assetName))
                return false;
            HandledFiles.Add(assetName);
            return true;
        }
        internal void RegisterTexture(string assetName, string fileName)
        {
            assetName = assetName.Replace(Path.DirectorySeparatorChar == '/' ? '\\' : '/', Path.DirectorySeparatorChar);
            if (CanRegister(assetName) && File.Exists(fileName))
                TextureMatches.Add(assetName, fileName);
        }
        internal void RegisterXnb(string assetName, string fileName)
        {
            assetName = assetName.Replace(Path.DirectorySeparatorChar == '/' ? '\\' : '/', Path.DirectorySeparatorChar);
            if (!CanRegister(assetName))
                return;
            string manager = Path.GetDirectoryName(fileName);
            if (!Registry.ContainsKey(manager))
                Registry.Add(manager, new SmartContentManager(ServiceProvider, manager));
            if (File.Exists(fileName + ".xnb"))
                XnbMatches.Add(assetName, fileName);
        }
        internal void RegisterHandler<T>(string assetName, FileLoadMethod<T> handler)
        {
            assetName = assetName.Replace(Path.DirectorySeparatorChar == '/' ? '\\' : '/', Path.DirectorySeparatorChar);
            if (CanRegister(assetName))
                DelegateMatches.Add(assetName, new KeyValuePair<Type, Delegate>(typeof(T), handler));
        }
    }
}
