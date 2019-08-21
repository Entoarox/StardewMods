using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Newtonsoft.Json.Linq;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.TerrainFeatures;

using SundropCity.TerrainFeatures;

using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace SundropCity
{
    using Json;
    /// <summary>The mod entry class.</summary>
    public class SundropCityMod : Mod
    {
        internal static IModHelper SHelper;
        internal static IMonitor SMonitor;

        internal static Dictionary<string, ParkingSpot[]> ParkingSpots = new Dictionary<string, ParkingSpot[]>();

        private List<string> Maps;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Define internals
            SHelper = helper;
            SMonitor = this.Monitor;

            // Add custom loader to make custom tree work
            helper.Content.AssetLoaders.Add(new SundropLoader());

            // Setup events
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            //helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saved += this.OnSaved;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;
            helper.Events.Specialised.LoadStageChanged += this.OnLoadStageChanged;

            // Handle custom layer drawing
            helper.Events.Player.Warped += this.OnWarped;
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialized at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.Monitor.Log("Performing init...", LogLevel.Trace);
            var helper = this.Helper;
            this.Maps = new List<string>();
            // Load maps
            this.Monitor.Log("Preparing maps...", LogLevel.Trace);
            foreach(string file in Directory.EnumerateFiles(Path.Combine(this.Helper.DirectoryPath, "assets", "Maps")))
            {
                string ext = Path.GetExtension(file);
                if (ext==null || !ext.Equals(".tbin"))
                    continue;
                string map = Path.GetFileName(file);
                this.Monitor.Log("Currently preparing: " + map, LogLevel.Trace);
                if (map == null)
                    continue;
                try
                {
                    var mapFile=this.Helper.Content.Load<Map>(Path.Combine("assets", "Maps", map));
                    if (mapFile.Properties.ContainsKey("Light"))
                        mapFile.Properties["Light"] = mapFile.Properties["Light"].ToString().Replace("\n", "").Replace(";", "");
                    var mapInst=new GameLocation(this.Helper.Content.GetActualAssetKey(Path.Combine("assets", "Maps", map)), Path.GetFileNameWithoutExtension(map));
                    this.Maps.Add(map);
                    Tourist.WarpCache.Add(Path.GetFileNameWithoutExtension(map), Tourist.GetSpawnPoints(mapInst, new HashSet<int>() {Tourist.TILE_WARP_DOWN, Tourist.TILE_WARP_LEFT, Tourist.TILE_WARP_RIGHT, Tourist.TILE_WARP_UP}));
                    Tourist.SpawnCache.Add(Path.GetFileNameWithoutExtension(map), Tourist.GetSpawnPoints(mapInst, new HashSet<int>() {Tourist.TILE_ARROW_DOWN, Tourist.TILE_ARROW_LEFT, Tourist.TILE_ARROW_RIGHT, Tourist.TILE_ARROW_UP, Tourist.TILE_BROWSE, Tourist.TILE_SPAWN}));
                }
                catch (Exception err)
                {
                    this.Monitor.Log("Unable to prepare [" + map + "] location, error follows\n" + err, LogLevel.Error);
                }
            }
            this.Monitor.Log("Mapping tourist graphics...", LogLevel.Trace);
            Tourist.LoadResources();
            this.Monitor.Log("Reading parking data...", LogLevel.Trace);
            foreach(var map in helper.Content.Load<JObject>("assets/Data/ParkingSpots.json").Properties())
            {
                this.Monitor.Log("Parsing parking data for the [" + map.Name + "] map, found [" + (map.Value as JArray)?.Count + "] parking spaces.", LogLevel.Trace);
                var spotsArr = map.Value.ToObject<JArray>();
                string key = map.Name;
                var spots = new List<ParkingSpot>();
                foreach (var spotArr in spotsArr.Cast<JArray>())
                {
                    var values = spotArr.Children().ToArray();
                    spots.Add(new ParkingSpot(new Vector2(values[0].ToObject<int>(), values[1].ToObject<int>()), (from JValue facing in values[2].ToObject<JArray>() select (Facing) Enum.Parse(typeof(Facing), facing.ToObject<string>())).ToArray()));
                }
                lock(ParkingSpots)
                    ParkingSpots.Add(key, spots.ToArray());
            }
            this.Monitor.Log("Reading car data...", LogLevel.Trace);
            SundropCar.CarTypes = helper.Data.ReadJsonFile<List<CarType>>("assets/Data/Cars.json")?.Select(type =>
             {
                 type.Base = helper.Content.GetActualAssetKey("assets/TerrainFeatures/Drivables/" + type.Base + ".png");
                 type.Recolor = helper.Content.GetActualAssetKey("assets/TerrainFeatures/Drivables/" + type.Recolor + ".png");
                 type.Decals = type.Decals.Select(pair => new KeyValuePair<string, string>(pair.Key, helper.Content.GetActualAssetKey("assets/TerrainFeatures/Drivables/" + pair.Value + ".png"))).ToDictionary(pair => pair.Key, pair => pair.Value);
                 return type;
             }).ToList();
            if(SundropCar.CarTypes==null)
            {
                this.Monitor.Log("Unable to read Cars.json for car data, cars will not be able to spawn!", LogLevel.Error);
                SundropCar.CarTypes = new List<CarType>();
            }
            this.Monitor.Log("Preparing characters...", LogLevel.Trace);
            // NPC Spawning
            foreach(var info in this.Helper.Data.ReadJsonFile<CharacterInfo[]>("assets/Data/CharacterSpawns.json"))
                try
                {
                    // ReSharper disable once ObjectCreationAsStatement
                    new NPC(new AnimatedSprite(this.Helper.Content.GetActualAssetKey("assets/Characters/Sprites/" + info.Texture + ".png"), 18, 16, 32), Vector2.Zero, 2, info.Name)
                    {
                        Portrait = this.Helper.Content.Load<Texture2D>("assets/Characters/Portraits/" + info.Texture + ".png"),
                    };
                }
                catch (Exception err)
                {
                    this.Monitor.Log("Unable to prepare villager by name of [" + info.Name + "] due to a unexpected issue.\n" + err, LogLevel.Error);
                }
            this.Monitor.Log("Preparing cameos...", LogLevel.Trace);
            foreach(var info in this.Helper.Data.ReadJsonFile<CharacterInfo[]>("assets/Data/CameoSpawns.json"))
                try
                {
                    // ReSharper disable once ObjectCreationAsStatement
                    new NPC(new AnimatedSprite(this.Helper.Content.GetActualAssetKey("assets/Characters/Cameos/" + info.Name + "Sprite.png"), 18, 16, 32), Vector2.Zero, 2, info.Name)
                    {
                        Portrait = this.Helper.Content.Load<Texture2D>("assets/Characters/Cameos/" + info.Name + "Portrait.png"),
                    };
                }
                catch (Exception err)
                {
                    this.Monitor.Log("Unable to prepare cameo character for [" + info.Name + "] due to a unexpected issue.\n" + err, LogLevel.Warn);
                }
            this.Monitor.Log("Registering commands...", LogLevel.Trace);
            helper.ConsoleCommands.Add("sundrop_debug", "For debug use only", (cmd, args) =>
             {
                 switch(args[0])
                 {
                     case "car":
                         Facing facing = (Facing)Enum.Parse(typeof(Facing), args[1]);
                         Game1.currentLocation.largeTerrainFeatures.Add(new SundropCar(Game1.player.getTileLocation(), facing));
                         this.Monitor.Log("Spawned car.", LogLevel.Alert);
                         break;
                     case "parking":
                         this.SpawnCars(Game1.currentLocation, Convert.ToDouble(args[1]));
                         this.Monitor.Log("Performed car spawning algorithm.", LogLevel.Alert);
                         break;
                     case "tourist":
                         Game1.player.currentLocation.addCharacter(new Tourist(Utility.getRandomAdjacentOpenTile(Game1.player.getTileLocation(), Game1.player.currentLocation) * 64));
                         this.Monitor.Log("Spawned ", LogLevel.Alert);
                         break;
                     case "touristHorde":
                         int amount = Convert.ToInt32(args[1]);
                         var loc = Game1.player.currentLocation;
                         SundropCityMod.SpawnTourists(loc, amount);
                         break;
                 }
             });
            this.Monitor.Log("Init complete!", LogLevel.Trace);
        }

        private void Setup()
        {
            this.Monitor.Log("Performing setup...", LogLevel.Trace);
            var start = DateTime.Now;
            this.Monitor.Log("Adding locations...", LogLevel.Trace);
            // Add new locations
            Parallel.ForEach(this.Maps, map =>
            {
                try
                {
                    var loc = new GameLocation(this.Helper.Content.GetActualAssetKey(Path.Combine("assets", "Maps", map)), Path.GetFileNameWithoutExtension(map));
                    this.SetupLocation(loc);
                    lock (Game1.locations)
                    {
                        this.Monitor.Log("Adding sundrop location: " + Path.GetFileNameWithoutExtension(map), LogLevel.Trace);
                        Game1.locations.Add(loc);
                    }
                }
                catch (Exception err)
                {
                    this.Monitor.Log("Unable to add [" + map + "] location, error follows\n" + err, LogLevel.Error);
                }
            });
            var promenade = Game1.getLocationFromName("SundropPromenade");
            /*
            this.Monitor.Log("Spawning characters...", LogLevel.Trace);
            // NPC Spawning
            Parallel.ForEach(this.Helper.Data.ReadJsonFile<CharacterInfo[]>("assets/Data/CharacterSpawns.json"), info =>
            {
                if (Game1.getLocationFromName(info.Map) == null)
                    this.Monitor.Log("Unable to add villager by name of [" + info.Name + "] because their default map failed to load, this character will not appear in your game as a result.", LogLevel.Error);
                else
                    try
                    {
                        this.Monitor.Log("Adding sundrop villager: " + info.Name, LogLevel.Trace);
                        var pos = new Vector2(info.Position[0], info.Position[1]);
                        var villagerNpc = new NPC( new AnimatedSprite(this.Helper.Content.GetActualAssetKey("assets/Characters/Sprites/" + info.Texture + ".png"), 18, 16, 32), pos * 64f, 2, info.Name)
                        {
                            Portrait = this.Helper.Content.Load<Texture2D>("assets/Characters/Portraits/" + info.Texture + ".png"),
                            DefaultMap = info.Map,
                            DefaultPosition = pos * 64f
                        };
                        villagerNpc.setNewDialogue(info.Message);
                        lock (Game1.getLocationFromName(info.Map))
                            Game1.getLocationFromName(info.Map).addCharacter(villagerNpc);
                    }
                    catch (Exception err)
                    {
                        this.Monitor.Log("Unable to add villager by name of [" + info.Name + "] due to a unexpected issue, this character will not appear in your game as a result.\n" + err, LogLevel.Error);
                    }
            });
            this.Monitor.Log("Spawning cameos...", LogLevel.Trace);
            Parallel.ForEach(this.Helper.Data.ReadJsonFile<CharacterInfo[]>("assets/Data/CameoSpawns.json"), info =>
            {
                if (Game1.getLocationFromName(info.Map) == null)
                    this.Monitor.Log("Unable to add cameo character for [" + info.Name + "] because their default map failed to load, this character will not appear in your game as a result.", LogLevel.Warn);
                else
                    try
                    {
                        this.Monitor.Log("Adding sundrop cameo: " + info.Name, LogLevel.Trace);
                        var pos = new Vector2(info.Position[0], info.Position[1]);
                        var cameoNpc = new NPC(new AnimatedSprite(this.Helper.Content.GetActualAssetKey("assets/Characters/Cameos/" + info.Name + "Sprite.png"), 18, 16, 32), pos * 64f, 2, info.Name)
                        {
                            Portrait = this.Helper.Content.Load<Texture2D>("assets/Characters/Cameos/" + info.Name + "Portrait.png"),
                            DefaultMap = info.Map,
                            DefaultPosition = pos * 64f
                        };
                        cameoNpc.setNewDialogue(info.Message);
                        lock (Game1.getLocationFromName(info.Map))
                            Game1.getLocationFromName(info.Map).addCharacter(cameoNpc);
                    }
                    catch (Exception err)
                    {
                        this.Monitor.Log("Unable to add cameo character for [" + info.Name + "] due to a unexpected issue, this character will not appear in your game as a result.\n" + err, LogLevel.Warn);
                    }
            });
            var npc = Game1.getCharacterFromName("Joe");
            npc.Schedule = this.Helper.Reflection.GetMethod(npc, "parseMasterSchedule").Invoke<Dictionary<int, SchedulePathDescription>>("610 80 20 2/630 23 20 2/710 80 20 2/730 23 20 2/810 80 20 2/830 23 20 2/910 80 20 2/930 23 20 2");
            NPC cake = new MrCake(new Vector2(22, 20), npc);
            cake.setNewDialogue("Mr. Cake looks at you with approval.");
            promenade?.addCharacter(cake);
            */
            var end = DateTime.Now;
            var time=end.Subtract(start);
            this.Monitor.Log("Sundrop loading took " + time.TotalMilliseconds + " milliseconds.", LogLevel.Trace);
            if (promenade == null)
            {
                this.Monitor.Log("Promenade failed to load, please report this issue.", LogLevel.Error);
                return;
            }
            this.Monitor.Log("Patching town...", LogLevel.Trace);
            // Cross-mod compatibility
            var registry = this.Helper.Data.ReadJsonFile<Dictionary<string, string>>("assets/TownPatches/registry.json");
            string patchVersion = "Vanilla";
            foreach (var pair in registry)
            {
                if (!this.Helper.ModRegistry.IsLoaded(pair.Key))
                    continue;
                patchVersion = pair.Value;
                break;
            }
            // Setup for patching
            var town = Game1.getLocationFromName("Town");
            var patch = this.Helper.Content.Load<Map>("assets/TownPatches/" + patchVersion + ".tbin");
            var layers = town.map.Layers.Where(a => patch.GetLayer(a.Id) != null).Select(a => a.Id);
            // Perform patching
            foreach (string layer in layers)
            {
                Layer toLayer = town.map.GetLayer(layer),
                    fromLayer = patch.GetLayer(layer);
                // First patch area: The road
                for (int x = 96; x < 120; x++)
                    for (int y = 53; y < 61; y++)
                    {
                        var tile = fromLayer.Tiles[x, y];
                        if (tile == null)
                            toLayer.Tiles[x, y] = null;
                        else
                            toLayer.Tiles[x, y] = new StaticTile(toLayer, town.map.GetTileSheet(tile.TileSheet.Id), BlendMode.Additive, tile.TileIndex);
                        var vector = new Vector2(x, y);
                        town.largeTerrainFeatures.Filter(a => a.tilePosition.Value != vector);
                    }
                // Second patch area: replacing the pink tree
                for (int x = 110; x < 116; x++)
                    for (int y = 46; y < 53; y++)
                    {
                        var tile = fromLayer.Tiles[x, y];
                        if (tile == null)
                            toLayer.Tiles[x, y] = null;
                        else
                            toLayer.Tiles[x, y] = new StaticTile(toLayer, town.map.GetTileSheet(tile.TileSheet.Id), BlendMode.Additive, tile.TileIndex);
                    }
            }

            // TEMP STUFF
            promenade.setTileProperty(3, 37, "Buildings", "Action", "Warp 119 56 Town");
            town.warps.Add(new Warp(120, 55, "SundropPromenade", 1, 29, false));
            town.warps.Add(new Warp(120, 56, "SundropPromenade", 1, 30, false));
            town.warps.Add(new Warp(120, 57, "SundropPromenade", 1, 31, false));
            town.warps.Add(new Warp(120, 58, "SundropPromenade", 1, 32, false));
            this.Monitor.Log("Patching complete, enjoy sundrop!", LogLevel.Trace);

            // Lock residential in the 0.2.0 release, it is not release ready
            if(this.ModManifest.Version.IsOlderThan("0.3.0"))
            {
                var layer = Game1.getLocationFromName("SundropShoppingDistrict")?.map.GetLayer("Buildings");
                if (layer == null)
                    return;
                var sheet = Game1.getLocationFromName("SundropShoppingDistrict").map.GetTileSheet("sundropOutdoor");
                for(int y=47;y<=51;y++)
                    layer.Tiles[99, y] = new StaticTile(layer, sheet, BlendMode.Additive, 0);
            }
        }

        private void LoadTouristParts(string root, string relative, string folder, List<string> male, List<string> female)
        {
            foreach (string file in Directory.EnumerateFiles(Path.Combine(root, relative, folder)))
            {
                string ext = Path.GetExtension(file);
                if (ext==null || !ext.Equals(".png"))
                    continue;
                string part = Path.GetFileNameWithoutExtension(file);
                if (part == null || part[part.Length-1]=='h')
                    continue;
                string path = Path.Combine(relative, folder, part + ext);
                string key = this.Helper.Content.GetActualAssetKey(path);
                this.Helper.Content.Load<Texture2D>(path);
                switch (part[0])
                {
                    case 'f':
                        female.Add(key);
                        break;
                    case 'm':
                        male.Add(key);
                        break;
                    case 'g':
                        female.Add(key);
                        male.Add(key);
                        break;
                }
            }
        }
        private void SetupLocation(GameLocation location)
        {
            if (!location.map.Properties.ContainsKey("IsSundropLocation"))
                location.map.Properties.Add("IsSundropLocation", true);
            if (location.map.GetLayer("SundropPaths") != null)
            {
                var pathsLayer = location.map.GetLayer("SundropPaths");
                Parallel.For(0, pathsLayer.LayerWidth, x =>
                {
                    Parallel.For(0, pathsLayer.LayerHeight, y =>
                     {
                         var tile = pathsLayer.Tiles[x, y];
                         if (tile == null)
                             return;
                         Vector2 vector = new Vector2(x, y);
                         switch (tile.TileIndex)
                         {
                             case 0:
                                 lock (location.terrainFeatures)
                                     if (!location.terrainFeatures.ContainsKey(vector))
                                         location.terrainFeatures.Add(vector, new SundropGrass());
                                 break;
                             case 1:
                                 lock (location.terrainFeatures)
                                     if (!location.terrainFeatures.ContainsKey(vector))
                                         location.terrainFeatures.Add(vector, new Tree(456, 5));
                                 break;
                         }
                     });
                });
            }
            var layer = location.map.GetLayer("Back");
            var sheet = location.map.TileSheets.FirstOrDefault(_ => _.ImageSource.Contains("SundropPaths.png"));
            if (sheet == null)
            {
                var img = this.Helper.Content.Load<Texture2D>("assets/Maps/SundropPaths.png");
                sheet = new TileSheet("PathsTilesheet", location.map, this.Helper.Content.GetActualAssetKey("assets/Maps/SundropPaths.png"), new Size(img.Width / 16, img.Height / 16), new Size(16, 16));
                location.map.AddTileSheet(sheet);
            }
            Parallel.For(0, layer.LayerWidth, x =>
            {
                Parallel.For(0, layer.LayerHeight, y =>
                {
                    if (layer.Tiles[x, y] == null)
                        layer.Tiles[x, y] = new StaticTile(layer, sheet, BlendMode.Additive, 2);
                });
            });
        }
        private void SpawnCars(GameLocation location, double chance)
        {
            Random rand = new Random();
            if (location.largeTerrainFeatures.Any(_ => _ is SundropCar))
                foreach (LargeTerrainFeature car in location.largeTerrainFeatures.Where(_ => _ is SundropCar).ToArray())
                    location.largeTerrainFeatures.Remove(car);
            if (!ParkingSpots.ContainsKey(location.Name))
                return;
            Parallel.ForEach(ParkingSpots[location.Name], spot =>
            {
                if (rand.NextDouble() <= chance)
                {
                    Facing facing = spot.Facings[rand.Next(spot.Facings.Length)];
                    lock(location.largeTerrainFeatures)
                        location.largeTerrainFeatures.Add(new SundropCar(spot.Target, facing));
                }
            });
        }
        private static void SpawnTourists(GameLocation loc, int amount)
        {
            var validPoints = Tourist.SpawnCache[loc.Name];
            for (int c = 0; c < amount; c++)
                loc.addCharacter(new Tourist(validPoints[Game1.random.Next(validPoints.Count)] * 64f));
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button.IsActionButton() && Context.IsPlayerFree)
            {
                Vector2 grabTile = new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / Game1.tileSize;
                if (!Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
                    grabTile = Game1.player.GetGrabTile();
                Tile tile = Game1.currentLocation.map.GetLayer("Buildings").PickTile(new Location((int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize), Game1.viewport.Size);
                PropertyValue propertyValue = null;
                tile?.Properties.TryGetValue("Action", out propertyValue);
                if (propertyValue != null)
                {
                    string[] split = ((string)propertyValue).Split(' ');
                    string[] args = new string[split.Length - 1];
                    Array.Copy(split, 1, args, 0, args.Length);
                    switch(split[0])
                    {
                        case "SundropMessage":
                            Game1.drawDialogueNoTyping(this.Helper.Translation.Get(args[0]));
                            break;
                    }
                }
            }
        }

        private void OnLoadStageChanged(object s, LoadStageChangedEventArgs e)
        {
            if (e.NewStage == StardewModdingAPI.Enums.LoadStage.SaveLoadedBasicInfo)
                this.Setup();
        }

        private void OnSaveLoaded(object s, EventArgs e)
        {
            this.Setup();
        }
        private void OnSaved(object s, EventArgs e)
        {
            this.Setup();
        }
        private void OnSaving(object s, EventArgs e)
        {
            foreach (var location in Game1.locations.Where(_ => _.map.Properties.ContainsKey("IsSundropLocation")).ToArray())
            {
                if (location.largeTerrainFeatures.Any(_ => _ is SundropCar))
                    foreach (LargeTerrainFeature car in location.largeTerrainFeatures.Where(_ => _ is SundropCar).ToArray())
                        location.largeTerrainFeatures.Remove(car);
                if (location.terrainFeatures.Pairs.Any(_ => _.Value is SundropGrass))
                    foreach (var feature in location.terrainFeatures.Pairs.Where(_ => _.Value is SundropGrass).Select(_ => _.Key).ToArray())
                        location.terrainFeatures.Remove(feature);
                if (location.characters.Any(_ => _ is Tourist))
                    foreach (var character in location.characters.Where(_ => _ is Tourist).ToArray())
                        location.characters.Remove(character);
                if (location.characters.Any(_ => _ is MrCake))
                    foreach (var character in location.characters.Where(_ => _ is MrCake).ToArray())
                        location.characters.Remove(character);
            }
        }

        private void OnWarped(object s, WarpedEventArgs e)
        {
            if (e.Player == Game1.MasterPlayer)
            {
                if (ParkingSpots.ContainsKey(e.NewLocation.Name))
                    this.SpawnCars(e.NewLocation, .8);
                if (e.OldLocation.farmers.Count == 0 && e.OldLocation.map.Properties.ContainsKey("TouristCount"))
                    foreach (var character in new List<Tourist>(e.OldLocation.characters.Where(_ => _ is Tourist).Cast<Tourist>()))
                    {
                        if (character.Special!=null)
                        {
                            character.TickAge = short.MaxValue;
                            character.RandomizeLook();
                        }
                        e.OldLocation.characters.Remove(character);
                    }

                if (e.NewLocation.farmers.Count == 1 && e.NewLocation.map.Properties.ContainsKey("TouristCount"))
                    SundropCityMod.SpawnTourists(e.NewLocation, Convert.ToInt32((string)e.NewLocation.map.Properties["TouristCount"]));
            }
            if (e.IsLocalPlayer)
            {
                e.OldLocation.map.GetLayer("Back").BeforeDraw -= this.DrawExtraLayers1;
                e.NewLocation.map.GetLayer("Back").BeforeDraw += this.DrawExtraLayers1;
                e.OldLocation.map.GetLayer("Back").AfterDraw -= this.DrawExtraLayers4;
                e.NewLocation.map.GetLayer("Back").AfterDraw += this.DrawExtraLayers4;
                e.OldLocation.map.GetLayer("Front").BeforeDraw -= this.DrawExtraLayers2;
                e.NewLocation.map.GetLayer("Front").BeforeDraw += this.DrawExtraLayers2;
                if (e.OldLocation.map.GetLayer("AlwaysFront") != null)
                    e.OldLocation.map.GetLayer("AlwaysFront").AfterDraw -= this.DrawExtraLayers3;
                if (e.NewLocation.map.GetLayer("AlwaysFront") != null)
                    e.NewLocation.map.GetLayer("AlwaysFront").AfterDraw += this.DrawExtraLayers3;
            }
        }
        private void DrawExtraLayers1(object s, LayerEventArgs e)
        {
            Game1.currentLocation.map.GetLayer("FarBack")?.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
            Game1.currentLocation.map.GetLayer("MidBack")?.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
        }
        private void DrawExtraLayers2(object s, LayerEventArgs e)
        {
            Game1.currentLocation.map.GetLayer("LowFront")?.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
        }
        private void DrawExtraLayers3(object s, LayerEventArgs e)
        {
            Game1.currentLocation.map.GetLayer("AlwaysFront2")?.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
        }

        private void DrawExtraLayers4(object s, LayerEventArgs e)
        {
            Game1.currentLocation.map.GetLayer("Shadows")?.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
        }
    }
}
