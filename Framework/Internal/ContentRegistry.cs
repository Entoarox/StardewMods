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
        void IContentRegistry.ReloadStaticReferences()
        {
            Game1.daybg = Game1.content.Load<Texture2D>("LooseSprites\\daybg");
            Game1.nightbg = Game1.content.Load<Texture2D>("LooseSprites\\nightbg");
            Game1.menuTexture = Game1.content.Load<Texture2D>("Maps\\MenuTiles");
            Game1.lantern = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\lantern");
            Game1.windowLight = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\windowLight");
            Game1.sconceLight = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\sconceLight");
            Game1.cauldronLight = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\greenLight");
            Game1.indoorWindowLight = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\indoorWindowLight");
            Game1.shadowTexture = Game1.content.Load<Texture2D>("LooseSprites\\shadow");
            Game1.mouseCursors = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");
            Game1.animations = Game1.content.Load<Texture2D>("TileSheets\\animations");
            Game1.achievements = Game1.content.Load<Dictionary<int, string>>("Data\\Achievements");
            Game1.eventConditions = Game1.content.Load<Dictionary<string, bool>>("Data\\eventConditions");
            Game1.NPCGiftTastes = Game1.content.Load<Dictionary<string, string>>("Data\\NPCGiftTastes");
            Game1.dialogueFont = Game1.content.Load<SpriteFont>("Fonts\\SpriteFont1");
            Game1.smallFont = Game1.content.Load<SpriteFont>("Fonts\\SmallFont");
            Game1.borderFont = Game1.content.Load<SpriteFont>("Fonts\\BorderFont");
            Game1.tinyFont = Game1.content.Load<SpriteFont>("Fonts\\tinyFont");
            Game1.tinyFontBorder = Game1.content.Load<SpriteFont>("Fonts\\tinyFontBorder");
            Game1.smoothFont = Game1.content.Load<SpriteFont>("Fonts\\smoothFont");
            Game1.objectSpriteSheet = Game1.content.Load<Texture2D>("Maps\\springobjects");
            Game1.toolSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\tools");
            Game1.cropSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\crops");
            Game1.emoteSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\emotes");
            Game1.debrisSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\debris");
            Game1.bigCraftableSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\Craftables");
            Game1.rainTexture = Game1.content.Load<Texture2D>("TileSheets\\rain");
            Game1.buffsIcons = Game1.content.Load<Texture2D>("TileSheets\\BuffsIcons");
            Game1.objectInformation = Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation");
            Game1.bigCraftablesInformation = Game1.content.Load<Dictionary<int, string>>("Data\\BigCraftablesInformation");
            FarmerRenderer.hairStylesTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hairstyles");
            FarmerRenderer.shirtsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\shirts");
            FarmerRenderer.hatsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hats");
            FarmerRenderer.accessoriesTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\accessories");
            Tool.weaponsTexture = Game1.content.Load<Texture2D>("TileSheets\\weapons");
            if(Game1.player!=null)
                Game1.player.FarmerRenderer = new FarmerRenderer(Game1.content.Load<Texture2D>("Characters\\Farmer\\farmer_" + (Game1.player.isMale ? "" : "girl_") + "base"));
        }
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
        [Obsolete("This method is for a not yet implemented API",true)]
        void IContentRegistry.RegisterContentHandler(IContentHandler handler)
        {
            throw new NotImplementedException();
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
            Update(null, null);
            Events.MoreEvents.FireSmartManagerReady();
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
            {
                EntoFramework.Logger.Log("ContentManager: The `" + assetName + "` file is already being managed, this may cause issues", StardewModdingAPI.LogLevel.Warn);
                return false;
            }
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
