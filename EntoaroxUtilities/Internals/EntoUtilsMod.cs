using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

using StardewValley;

using xTile.Layers;
using xTile.Dimensions;

namespace Entoarox.Utilities.Internals
{
    using Api;
    using Helpers;
    using Tools;
    using xTile.Tiles;

    internal class EntoUtilsMod : Mod
    {
        internal static EntoUtilsMod Instance;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.OnGameLoopGameLaunched;
            helper.Events.World.LocationListChanged += this.OnLocationListChanged;
            helper.Events.GameLoop.DayStarted += this.OnLocationListChanged;
            //helper.ConsoleCommands.Add("emu_forcelayers", "Force-reloads all the custom layers.", (c, a) => this.OnLocationListChanged(null, null));
            helper.Content.AssetLoaders.Add(new AssetHandlers.HelperSheetLoader());
            Instance = this;
        }

        public override object GetApi()
        {
            return EntoUtilsApi.Instance;
        }

        private void OnLocationListChanged(object s, EventArgs e)
        {
            this.Monitor.StartTimer("ExtendedLayers", "Reloading extended layers...");
            foreach(var location in Game1.locations)
            {
                var l = location.map.GetLayer("Back");
                l.BeforeDraw -= this.ExtendedDrawBack;
                l.BeforeDraw += this.ExtendedDrawBack;
                var i = location.map.TileSheets.FirstOrDefault(_ => _.ImageSource.Contains("__EMU_HELPER_TILESHEET"));
                if (i == null)
                {
                    i = new TileSheet("__EMU_HELPER_TILESHEET", location.map, "__EMU_HELPER_TILESHEET", new Size(1, 1), new Size(16, 16));
                    location.map.AddTileSheet(i);
                }
                Parallel.For(0, l.LayerWidth, x =>
                {
                    Parallel.For(0, l.LayerHeight, y =>
                    {
                        if (l.Tiles[x, y] == null)
                            l.Tiles[x, y] = new StaticTile(l, i, BlendMode.Additive, 0);
                    });
                });
                l = location.map.GetLayer("Buildings");
                l.AfterDraw -= this.ExtendedDrawBuildings;
                l.AfterDraw += this.ExtendedDrawBuildings;
                l = location.map.GetLayer("Front");
                l.BeforeDraw -= this.ExtendedDrawFront;
                l.BeforeDraw += this.ExtendedDrawFront;
                l = location.map.GetLayer("AlwaysFront");
                if (l == null)
                    continue;
                l.AfterDraw -= this.ExtendedDrawAlwaysFront;
                l.AfterDraw += this.ExtendedDrawAlwaysFront;
            }
            this.Monitor.StopTimer("ExtendedLayers", "Reloaded extended layers in {{TIME}} milliseconds.");
        }
        private void ExtendedDrawBack(object s, LayerEventArgs e)
        {
            Game1.currentLocation?.Map.GetLayer("FarBack")?.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
            Game1.currentLocation?.Map.GetLayer("MidBack")?.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
        }
        private void ExtendedDrawBuildings(object s, LayerEventArgs e)
        {
            Game1.currentLocation?.Map.GetLayer("Shadows")?.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
        }
        private void ExtendedDrawFront(object s, LayerEventArgs e)
        {
            Game1.currentLocation?.Map.GetLayer("LowFront")?.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
        }
        private void ExtendedDrawAlwaysFront(object s, LayerEventArgs e)
        {
            Game1.currentLocation?.Map.GetLayer("AlwaysFront2")?.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
        }

        private void OnGameLoopGameLaunched(object s, EventArgs e)
        {
            this.Helper.Events.GameLoop.GameLaunched -= this.OnGameLoopGameLaunched;
            try
            {
                this.Monitor.StartTimer("TypeId", "Initializing TypeId...");
                // Register typeId's, both for vanilla and any mods that we explicitly support (if loaded)
                var euApi = EntoUtilsApi.Instance;
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
                if (ModApi.IsLoadedAndApiAvailable<IJsonAssetsApi>("spacechase0.JsonAssets", out var jaApi))
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
                this.Monitor.StopTimer("TypeId", "TypeId initialization took {{TIME}} milliseconds.");
            }
            catch(Exception err)
            {
                this.Monitor.Log("TypeId failed to load due to a exception being thrown: \n" + err, LogLevel.Error);
            }
            this.OnLocationListChanged(null, null);
        }
    }
}
