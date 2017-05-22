using System;
using System.Reflection;
using System.Collections.Generic;

using StardewValley;
using StardewValley.Objects;
using StardewValley.BellsAndWhistles;
using StardewValley.Projectiles;

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
            Game1.NPCGiftTastes = Game1.content.Load<Dictionary<string, string>>("Data\\NPCGiftTastes");
            Game1.dialogueFont = Game1.content.Load<SpriteFont>("Fonts\\SpriteFont1");
            Game1.smallFont = Game1.content.Load<SpriteFont>("Fonts\\SmallFont");
            Game1.tinyFont = Game1.content.Load<SpriteFont>("Fonts\\tinyFont");
            Game1.tinyFontBorder = Game1.content.Load<SpriteFont>("Fonts\\tinyFontBorder");
            Game1.objectSpriteSheet = Game1.content.Load<Texture2D>("Maps\\springobjects");
            typeof(Game1).GetField("_toolSpriteSheet", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, Game1.content.Load<Texture2D>("TileSheets\\tools"));
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
            Furniture.furnitureTexture = Game1.content.Load<Texture2D>("TileSheets\\furniture");
            SpriteText.spriteTexture = Game1.content.Load<Texture2D>("LooseSprites\\font_bold");
            SpriteText.coloredTexture = Game1.content.Load<Texture2D>("LooseSprites\\font_colored");
            Tool.weaponsTexture = Game1.content.Load<Texture2D>("TileSheets\\weapons");
            Projectile.projectileSheet = Game1.content.Load<Texture2D>("TileSheets\\Projectiles");
            if (Game1.player!=null)
                Game1.player.FarmerRenderer = new FarmerRenderer(Game1.content.Load<Texture2D>("Characters\\Farmer\\farmer_" + (Game1.player.isMale ? "" : "girl_") + "base"));
        }
        void IContentRegistry.RegisterHandler<T>(string key, FileLoadMethod<T> method)
        {
            try
            {
                SmartContentManager.RegisterHandler(key, method);
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
                SmartContentManager.RegisterTexture(key, path);
            }
            catch (Exception err)
            {
                EntoFramework.Logger.ExitGameImmediately("Was unable to register texture file in loader" + err);
            }
        }
        void IContentRegistry.RegisterXnb(string key, string path)
        {
            try
            {
                SmartContentManager.RegisterXnb(key, path);
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
        private static WeakReference<SmartContentManager> MainSmartManager;
        private static WeakReference<SmartContentManager> TileSmartManager;
        private static WeakReference<SmartContentManager> TempSmartManager;
        private static xTile.Display.XnaDisplayDevice SmartDevice;
        private static FieldInfo TempContent;
        internal static string RootDirectory;
        internal static IServiceProvider ServiceProvider;
        internal static void Init()
        {
            if(TempContent==null)
                TempContent = typeof(Game1).GetField("_temporaryContent", BindingFlags.NonPublic | BindingFlags.Static);
            if (RootDirectory == null)
            {
                ServiceProvider = Game1.content.ServiceProvider;
                RootDirectory = Game1.content.RootDirectory;
                MainSmartManager = new WeakReference<SmartContentManager>(null);
                TileSmartManager = new WeakReference<SmartContentManager>(null);
                TempSmartManager = new WeakReference<SmartContentManager>(null);
            }
            if (!MainSmartManager.TryGetTarget(out SmartContentManager mainSmartManager))
            {
                mainSmartManager = new SmartContentManager(ServiceProvider, RootDirectory);
                MainSmartManager.SetTarget(mainSmartManager);
            }
            if (!TileSmartManager.TryGetTarget(out SmartContentManager tileSmartManager))
            {
                tileSmartManager=new SmartContentManager(ServiceProvider, RootDirectory);
                TileSmartManager.SetTarget(tileSmartManager);
            }
            if (!TempSmartManager.TryGetTarget(out SmartContentManager tempSmartManager))
            {
                tempSmartManager = new SmartContentManager(ServiceProvider, RootDirectory);
                TempSmartManager.SetTarget(tempSmartManager);
            }
            if (SmartDevice == null)
                SmartDevice = new xTile.Display.XnaDisplayDevice(mainSmartManager, Game1.game1.GraphicsDevice);
            Update(null, null);
            Events.MoreEvents.FireSmartManagerReady();
        }
        internal static void Update(object s, EventArgs e)
        {
            MainSmartManager.TryGetTarget(out SmartContentManager mainSmartManager);
            TileSmartManager.TryGetTarget(out SmartContentManager tileSmartManager);
            TempSmartManager.TryGetTarget(out SmartContentManager tempSmartManager);
            Game1.content = mainSmartManager;
            Game1.mapDisplayDevice = SmartDevice;
            Game1.game1.xTileContent = tileSmartManager;
            TempContent.SetValue(null, tempSmartManager);
        }
    }
}
