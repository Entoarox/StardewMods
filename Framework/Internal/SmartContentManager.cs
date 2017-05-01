using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.Framework
{
    internal class SmartContentManager : LocalizedContentManager
    {
        private static Dictionary<string, SmartContentManager> Registry = new Dictionary<string, SmartContentManager>();
        private static Dictionary<string,string> HandledFiles = new Dictionary<string,string>();
        private static Dictionary<string, string> XnbMatches = new Dictionary<string, string>();
        private static Dictionary<string, string> TextureMatches = new Dictionary<string, string>();
        private static Dictionary<string, KeyValuePair<Type, Delegate>> DelegateMatches = new Dictionary<string, KeyValuePair<Type, Delegate>>();
        private static Dictionary<string, object> TextureCache = new Dictionary<string, object>();
        private static LocalizedContentManager ModContent;
        public SmartContentManager(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider, rootDirectory)
        {
            ModContent = new LocalizedContentManager(serviceProvider, Path.Combine(StardewModdingAPI.Constants.ExecutionPath, "Mods"));
        }
        public override T Load<T>(string assetName)
        {
            assetName = Normalize(assetName);
            if (HandledFiles.ContainsKey(assetName))
            {
                if (XnbMatches.ContainsKey(assetName))
                    return ModContent.Load<T>(MakeModsRelative(XnbMatches[assetName]));
                if (TextureMatches.ContainsKey(assetName) && typeof(T) == typeof(Texture2D))
                {
                    if (!TextureCache.ContainsKey(assetName))
                        TextureCache.Add(assetName, Texture2D.FromStream(Game1.graphics.GraphicsDevice, new FileStream(TextureMatches[assetName], FileMode.Open)));
                    return (T)TextureCache[assetName];
                }
                if (DelegateMatches.ContainsKey(assetName) && DelegateMatches[assetName].Key == typeof(T))
                    return ((FileLoadMethod<T>)DelegateMatches[assetName].Value)(base.Load<T>, assetName);
            }
            return base.Load<T>(assetName);
        }
        private static string Normalize(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar == '/' ? '\\' : '/', Path.DirectorySeparatorChar);
        }
        private static string MakeModsRelative(string fileName)
        {
            return fileName.Replace(ModContent.RootDirectory, "");
        }
        private static bool CanRegister(string assetName, string redirect)
        {
            if (HandledFiles.ContainsKey(assetName))
            {
                if (!HandledFiles[assetName].Equals(redirect))
                    EntoFramework.Logger.Log("ContentManager: Multiple redirects for the `"+assetName+"` file found, the first will be used." + Environment.NewLine + "> "+HandledFiles[assetName] + Environment.NewLine + "> "+ redirect, StardewModdingAPI.LogLevel.Error);
                return false;
            }
            HandledFiles.Add(assetName, redirect);
            return true;
        }
        internal static void RegisterTexture(string assetName, string fileName)
        {
            assetName = Normalize(assetName);
            fileName = Normalize(fileName);
            if (CanRegister(assetName,fileName) && File.Exists(fileName))
                TextureMatches.Add(assetName, fileName);
        }
        internal static void RegisterXnb(string assetName, string fileName)
        {
            assetName = Normalize(assetName);
            fileName = Normalize(fileName);
            if (!CanRegister(assetName,fileName))
                return;
            if (File.Exists(fileName + ".xnb"))
                XnbMatches.Add(assetName, fileName);
        }
        internal static void RegisterHandler<T>(string assetName, FileLoadMethod<T> handler)
        {
            assetName = Normalize(assetName);
            if (CanRegister(assetName, handler.GetType().AssemblyQualifiedName))
                DelegateMatches.Add(assetName, new KeyValuePair<Type, Delegate>(typeof(T), handler));
        }
    }
}
