using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Objects;
using StardewValley.BellsAndWhistles;
using StardewValley.Projectiles;

namespace Entoarox.Framework.Core.ContentHelper
{
    using AssetHandlers;
    internal static class Utilities
    {
        private static IList<IAssetLoader> _AssetLoaders;
        internal static IList<IAssetLoader> AssetLoaders
        {
            get
            {
                if (_AssetLoaders == null)
                    SetupReflection();
                return _AssetLoaders;
            }
        }
        private static IList<IAssetEditor> _AssetEditors;
        internal static IList<IAssetEditor> AssetEditors
        {
            get
            {
                if (_AssetEditors == null)
                    SetupReflection();
                return _AssetEditors;
            }
        }
        internal static void PerformSetup()
        {
            var TypeHandler = new DeferredAssetHandler();
            AssetLoaders.Add(TypeHandler);
            AssetEditors.Add(TypeHandler);
            AssetEditors.Add(new DeferredTypeHandler());
            AssetLoaders.Add(new XnbLoader());
        }
        private static void SetupReflection()
        {
            Type t = Game1.content.GetType();
            var assetLoaders = t.GetProperty("Loaders", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var assetEditors = t.GetProperty("Injectors", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            _AssetLoaders = (IList<IAssetLoader>)assetLoaders.GetValue(ModEntry.SHelper.Content);
            _AssetEditors = (IList<IAssetEditor>)assetEditors.GetValue(ModEntry.SHelper.Content);
        }
        internal static void ReloadStaticReferences()
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
            if (Game1.player != null)
                Game1.player.FarmerRenderer = new FarmerRenderer(Game1.content.Load<Texture2D>("Characters\\Farmer\\farmer_" + (Game1.player.isMale ? "" : "girl_") + "base"));
        }
        internal static string ModName(object helper)
        {
            return (helper is IModLinked) ? ModEntry.SHelper.ModRegistry.Get((helper as IModLinked).ModID).Name : "Name Unknown";
        }
    }
}
