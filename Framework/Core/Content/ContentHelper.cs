using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Objects;
using StardewValley.BellsAndWhistles;
using StardewValley.Projectiles;

namespace Entoarox.Framework.Core.Content
{
    using Plugins;

    internal class ContentHelper : IContentHelper
    {
        private FrameworkHelper FrameworkHelper;
        private string ModPath;
        internal ContentHelper(FrameworkHelper frameworkHelper)
        {
            FrameworkHelper = frameworkHelper;
            ModPath = FrameworkHelper.Mod.Helper.DirectoryPath.Replace(System.IO.Path.Combine(StardewModdingAPI.Constants.ExecutionPath, "Mods"), "");
        }
        internal string Normalize(string path)
        {
            return path.Replace('/', '\\');
        }
        internal void ReloadStaticReferences()
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
        T IContentHelper.Load<T>(string assetName, ContentSource source)
        {
            if (typeof(T) == typeof(Texture2D) && assetName.EndsWith(".png"))
                return (T)(object)Utilities.TextureHelper.GetTexture(assetName);
            if (assetName.EndsWith(".xnb"))
                assetName = assetName.Substring(0, assetName.Length - 4);
            return source == ContentSource.ModFolder ? ExtendibleContentManager.ModContent.Load<T>(Path.Combine(ModPath, assetName)) : StardewValley.Game1.content.Load<T>(assetName);
        }
        string IContentHelper.GetActualAssetKey(string assetName, ContentSource source)
        {
            return source == ContentSource.ModFolder ? Path.Combine(ModPath, assetName) : assetName;
        }
        void IContentHelper.RegisterContentHandler(IContentHandler handler)
        {
            ExtendibleContentManager.AddContentHandler(handler);
        }
        private void RegisterTexturePatch(string asset, Texture2D patch, Rectangle? destination, Rectangle? source)
        {
            asset = Normalize(asset);
            if (!TextureInjector.AssetMap.ContainsKey(asset))
                TextureInjector.AssetMap.Add(asset, new List<(Texture2D Texture, Rectangle Region, Rectangle Source)>());
            TextureInjector.AssetMap[asset].Add((Texture: patch, Region: destination ?? new Rectangle(0, 0, patch.Width, patch.Height), Source: source ?? new Rectangle(0, 0, patch.Width, patch.Height)));
            if (TextureInjector.AssetCache.ContainsKey(asset))
                TextureInjector.AssetCache.Remove(asset);
            ReloadStaticReferences();
        }
        void IContentHelper.RegisterTexturePatch(string asset, Texture2D patch, Rectangle? destination, Rectangle? source)
        {
            RegisterTexturePatch(asset, Utilities.TextureHelper.Premultiply(patch,FrameworkHelper.Mod.Monitor), destination, source);
        }
        void IContentHelper.RegisterTexturePatch(string asset, string patch, Rectangle? destination, Rectangle? source)
        {
            RegisterTexturePatch(asset, (this as IContentHelper).Load<Texture2D>(patch), destination, source);
        }
        void IContentHelper.RegisterDictionaryPatch<Tkey,TValue>(string asset, Dictionary<Tkey,TValue> patch)
        {
            asset = Normalize(asset);
            if (!DictionaryInjector.AssetMap.ContainsKey(asset))
                DictionaryInjector.AssetMap.Add(asset, new List<object>());
            DictionaryInjector.AssetMap[asset].Add(patch);
            if (DictionaryInjector.AssetCache.ContainsKey(asset))
                DictionaryInjector.AssetCache.Remove(asset);
            ReloadStaticReferences();
        }
        void IContentHelper.RegisterDictionaryPatch<TKey,TValue>(string asset, string patch)
        {
            (this as IContentHelper).RegisterDictionaryPatch(asset, (this as IContentHelper).Load<Dictionary<TKey, TValue>>(patch));
        }
        void IContentHelper.RegisterXnbReplacement(string asset, string replacement)
        {
            asset = Normalize(asset);
            if(!XnbLoader.AssetMap.ContainsKey(asset))
            {
                XnbLoader.AssetMap.Add(Normalize(asset), Path.Combine(ModPath,replacement));
                ReloadStaticReferences();
            }
        }
        void IContentHelper.RegisterLoader<T>(string asset, AssetLoader<T> loader)
        {
            DelegatedContentHandler.AssetLoadMap.Add(Normalize(asset), loader);
            ReloadStaticReferences();
        }
        void IContentHelper.RegisterLoader<T>(AssetLoader<T> loader)
        {
            DelegatedContentHandler.TypeLoadMap.Add(typeof(T), loader);
            ReloadStaticReferences();
        }
        void IContentHelper.RegisterInjector<T>(string asset, AssetInjector<T> injector)
        {
            asset = Normalize(asset);
            if (!DelegatedContentHandler.AssetInjectMap.ContainsKey(asset))
                DelegatedContentHandler.AssetInjectMap.Add(asset, new List<Delegate>());
            DelegatedContentHandler.AssetInjectMap[asset].Add(injector);
            ReloadStaticReferences();
        }
        void IContentHelper.RegisterInjector<T>(AssetInjector<T> injector)
        {
            if (!DelegatedContentHandler.TypeInjectMap.ContainsKey(typeof(T)))
                DelegatedContentHandler.TypeInjectMap.Add(typeof(T), new List<Delegate>());
            DelegatedContentHandler.TypeInjectMap[typeof(T)].Add(injector);
            ReloadStaticReferences();
        }
    }
}
