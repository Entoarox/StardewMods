using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.Framework
{
    internal class SmartContentManager : LocalizedContentManager
    {
        private Dictionary<string, SmartContentManager> Registry = new Dictionary<string, SmartContentManager>();
        private List<string> HandledFiles = new List<string>();
        private Dictionary<string, string> XnbMatches = new Dictionary<string, string>();
        private Dictionary<string, string> TextureMatches = new Dictionary<string, string>();
        private Dictionary<string, KeyValuePair<Type, Delegate>> DelegateMatches = new Dictionary<string, KeyValuePair<Type, Delegate>>();
        private Dictionary<string, object> TextureCache = new Dictionary<string, object>();
        public SmartContentManager(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider, rootDirectory)
        {
        }
        public override T Load<T>(string assetName)
        {
            assetName = assetName.Replace(Path.DirectorySeparatorChar == '/' ? '\\' : '/', Path.DirectorySeparatorChar);
            if (this.HandledFiles.Contains(assetName))
            {
                if (this.XnbMatches.ContainsKey(assetName))
                    return this.Registry[Path.GetDirectoryName(this.XnbMatches[assetName])].Load<T>(Path.GetFileName(this.XnbMatches[assetName]));
                if (this.TextureMatches.ContainsKey(assetName) && typeof(T) == typeof(Texture2D))
                {
                    if (!this.TextureCache.ContainsKey(assetName))
                        this.TextureCache.Add(assetName, Texture2D.FromStream(Game1.graphics.GraphicsDevice, new FileStream(this.TextureMatches[assetName], FileMode.Open)));
                    return (T)this.TextureCache[assetName];
                }
                if (this.DelegateMatches.ContainsKey(assetName) && this.DelegateMatches[assetName].Key == typeof(T))
                    return ((FileLoadMethod<T>)this.DelegateMatches[assetName].Value)(base.Load<T>, assetName);
            }
            return base.Load<T>(assetName);
        }
        private bool CanRegister(string assetName)
        {
            if (this.HandledFiles.Contains(assetName))
            {
                EntoFramework.Logger.Log("ContentManager: The `" + assetName + "` file is already being managed, this may cause issues", StardewModdingAPI.LogLevel.Warn);
                return false;
            }
            this.HandledFiles.Add(assetName);
            return true;
        }
        internal void RegisterTexture(string assetName, string fileName)
        {
            assetName = assetName.Replace(Path.DirectorySeparatorChar == '/' ? '\\' : '/', Path.DirectorySeparatorChar);
            if (this.CanRegister(assetName) && File.Exists(fileName))
                this.TextureMatches.Add(assetName, fileName);
        }
        internal void RegisterXnb(string assetName, string fileName)
        {
            assetName = assetName.Replace(Path.DirectorySeparatorChar == '/' ? '\\' : '/', Path.DirectorySeparatorChar);
            if (!this.CanRegister(assetName))
                return;
            string manager = Path.GetDirectoryName(fileName);
            if (!this.Registry.ContainsKey(manager))
                this.Registry.Add(manager, new SmartContentManager(this.ServiceProvider, manager));
            if (File.Exists(fileName + ".xnb"))
                this.XnbMatches.Add(assetName, fileName);
        }
        internal void RegisterHandler<T>(string assetName, FileLoadMethod<T> handler)
        {
            assetName = assetName.Replace(Path.DirectorySeparatorChar == '/' ? '\\' : '/', Path.DirectorySeparatorChar);
            if (this.CanRegister(assetName))
                this.DelegateMatches.Add(assetName, new KeyValuePair<Type, Delegate>(typeof(T), handler));
        }
    }
}
