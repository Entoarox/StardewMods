using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

namespace Entoarox.Utilities.Internals
{
    using Api;
    using Helpers;
    internal class EntoUtilsMod : Mod
    {
        internal static EntoUtilsMod Instance;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.OnGameLoopGameLaunched;
            Instance = this;
        }

        public override object GetApi()
        {
            return EntoUtilsApi.Instance;
        }

        private void OnGameLoopGameLaunched(object s, EventArgs e)
        {
            var start = DateTime.Now;
            this.Helper.Events.GameLoop.GameLaunched -= this.OnGameLoopGameLaunched;
            // Register typeId's, both for vanilla and any mods that we explicitly support (if loaded)
            var euApi = EntoUtilsApi.Instance;
            this.Monitor.Log("Initializing TypeId...");
            Data.Init();
            this.Monitor.Log("Initializing SDV TypeId support...");
            euApi.RegisterItemTypeHandler("sdv.object", TypeHandlers.StardewObject);
            euApi.RegisterItemTypeHandler("sdv.craftable", TypeHandlers.StardewCraftable);
            euApi.RegisterItemTypeHandler("sdv.boots", TypeHandlers.StardewBoots);
            euApi.RegisterItemTypeHandler("sdv.furniture", TypeHandlers.StardewFurniture);
            euApi.RegisterItemTypeHandler("sdv.hat", TypeHandlers.StardewHat);
            euApi.RegisterItemTypeHandler("sdv.ring", TypeHandlers.StardewRing);
            euApi.RegisterItemTypeHandler("sdv.floor", TypeHandlers.StardewFloor);
            euApi.RegisterItemTypeHandler("sdv.wallpaper", TypeHandlers.StardewWallpaper);
            euApi.RegisterItemTypeHandler("sdv.weapon", TypeHandlers.StardewWeapon);
            euApi.RegisterItemTypeHandler("sdv.clothing", TypeHandlers.StardewClothing);
            euApi.RegisterItemTypeHandler("sdv.tool", TypeHandlers.StardewTool);
            euApi.RegisterItemTypeHandler("sdv.artisan", TypeHandlers.StardewArtisan);
            euApi.RegisterItemTypeResolver(TypeResolvers.StardewResolver);
            /**
             * Build-in compatibility for mods with public API's that the TypeId registry can hook into
             * Mods which do not provide a public API will need to provide TypeId support from their end
             */
            if(ModApi.IsLoadedAndApiAvailable<IJsonAssetsApi>("spacechase0.JsonAssets", out var jaApi))
            {
                this.Monitor.Log("JsonAssets detected, initializing TypeId support...");
                Data.JaApi = jaApi;
                euApi.RegisterItemTypeHandler("ja.object", TypeHandlers.JsonAssetsObject);
                euApi.RegisterItemTypeHandler("ja.craftable", TypeHandlers.JsonAssetsCraftable);
                euApi.RegisterItemTypeHandler("ja.hat", TypeHandlers.JsonAssetsHat);
                euApi.RegisterItemTypeHandler("ja.weapon", TypeHandlers.JsonAssetsWeapon);
                euApi.RegisterItemTypeHandler("ja.clothing", TypeHandlers.JsonAssetsClothing);
                euApi.RegisterItemTypeResolver(TypeResolvers.JsonAssetsResolver);
            }
            if (ModApi.IsLoadedAndApiAvailable<ICustomFarmingApi>("Platonymous.CustomFarming", out var cfApi))
            {
                this.Monitor.Log("CustomFarmingRedux detected, initializing TypeId support...");
                Data.CfApi = cfApi;
                euApi.RegisterItemTypeHandler("cf.machine", TypeHandlers.CustomFarmingMachine);
            }
            if (ModApi.IsLoadedAndApiAvailable<IPrismaticToolsApi>("stokastic.PrismaticTools", out var ptApi))
            {
                this.Monitor.Log("PrismaticTools detected, initializing TypeId support...");
                Data.PtApi = ptApi;
                euApi.RegisterItemTypeHandler("pt.item", TypeHandlers.PrismaticToolsItem);
                euApi.RegisterItemTypeResolver(TypeResolvers.PrismaticToolsResolver);
            }
            var end = DateTime.Now;
            var time = end.Subtract(start);
            this.Monitor.Log("TypeId initialization took " + time.TotalMilliseconds + " milliseconds.");
        }
    }
}
