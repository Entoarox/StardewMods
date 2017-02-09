using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Entoarox.Framework.ContentManager
{
    public class SmartContentManager : StardewValley.LocalizedContentManager
    {
        public SmartContentManager(IServiceProvider serviceProvider, string rootDirectory, CultureInfo currentCulture, string languageCodeOverride) : base(serviceProvider, rootDirectory, currentCulture, languageCodeOverride)
        {
        }
        public SmartContentManager(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider, rootDirectory)
        {
        }
        public static List<ContentHandler> ContentHandlers { get; } = new List<ContentHandler>()
        {
            new DictionaryContentInjector(),
            new TextureContentInjector(),
            new XnbContentLoader(),
            new DelegatedAssetContentHandler(),
            new DelegatedTypeContentHandler()
        };
        public static List<string> ExpectedFailures { get; } = new List<string>
        {
            @"Characters\Dialogue\Bouncer",
            @"Characters\Dialogue\Gunther",
            @"Characters\Dialogue\Marlon",
            @"Characters\Dialogue\Henchman",
            @"Characters\Dialogue\Dudley",
            @"Characters\schedules\Wizard",
            @"Characters\schedules\Dwarf",
            @"Characters\schedules\Mister Qi",
            @"Characters\schedules\Sandy",
            @"Characters\schedules\Bouncer",
            @"Characters\schedules\Gunther",
            @"Characters\schedules\Marlon",
            @"Characters\schedules\Henchman",
            @"Data\Festivals\spring1",
            @"Data\Festivals\spring12"
        };
        public override T Load<T>(string assetName)
        {
            try
            {
                assetName = ContentHandler.GetPlatformSafePath(assetName);
                T asset = default(T);
                var loaders = ContentHandlers.Where(a => a.CanLoad<T>(assetName)).ToArray();
                if (loaders.Length > 1)
                    EntoFramework.Logger.Log("ContentManager: multiple loaders for `" + assetName + "` found, using first", StardewModdingAPI.LogLevel.Warn);
                if (loaders.Length > 0)
                    asset = loaders[0].Load(assetName, base.Load<T>);
                else
                    asset = base.Load<T>(assetName);
                var injectors = ContentHandlers.Where(a => a.CanInject<T>(assetName)).ToArray();
                foreach (ContentHandler injector in injectors)
                    injector.Inject(assetName, ref asset);
                return asset;
            }
            catch (Exception err)
            {
                if(!ExpectedFailures.Contains(assetName))
                    EntoFramework.Logger.Log(StardewModdingAPI.LogLevel.Error, "ContentManager: Encountered a issue loading asset:" + assetName, err);
                throw;
            }
        }
    }
}
