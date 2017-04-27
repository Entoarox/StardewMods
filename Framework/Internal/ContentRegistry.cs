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
        private static SmartContentManager TempSmartManager;
        private static SmartDisplayDevice SmartDevice;
        private static FieldInfo TempContent;
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
            TempSmartManager = new SmartContentManager(Game1.content.ServiceProvider, Game1.content.RootDirectory);
            SmartDevice = new SmartDisplayDevice(SmartManager, Game1.game1.GraphicsDevice);
            TempContent = typeof(Game1).GetField("_temporaryContent", BindingFlags.NonPublic | BindingFlags.Static);
            Update(null, null);
            Events.MoreEvents.FireSmartManagerReady();
        }
        internal static void Update(object s, EventArgs e)
        {
            Game1.content = SmartManager;
            Game1.mapDisplayDevice = SmartDevice;
            Game1.game1.xTileContent = SmartManager;
            TempContent.SetValue(null, TempSmartManager);
        }
    }
}
