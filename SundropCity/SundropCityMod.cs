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
        private static Texture2D DemoSource;
        private static PaletteTexture DemoTex;
        private static readonly Dictionary<string, Color> DemoTexColors = new Dictionary<string, Color>()
        {
            ["DullDark"] = new Color(39,31,27),
            ["MediumDark"] = new Color(76,61,46),
            ["BrightDark"] = new Color(103,57,49),
            ["DullLight"] = new Color(143,77,87),
            ["MediumLight"] = new Color(189,106,98),
            ["BrightLight"] = new Color(255,174,112)
        };
        private static readonly Dictionary<string, Color> DemoTexSource = new Dictionary<string, Color>()
        {
            ["DullDark"] = new Color(57, 57, 57),
            ["MediumDark"] = new Color(81, 81, 81),
            ["BrightDark"] = new Color(119, 119, 119),
            ["DullLight"] = new Color(158, 158, 158),
            ["MediumLight"] = new Color(224, 224, 224),
            ["BrightLight"] = new Color(254, 254, 254)
        };

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
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saved += this.OnSaved;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;

            // Handle custom layer drawing
            helper.Events.Player.Warped += this.OnWarped;

            /**
            // Demo stuff
            DemoSource = helper.Content.Load<Texture2D>("assets/recolor.png");
            DemoTex = new PaletteTexture(DemoSource, DemoTexSource);
            helper.Events.Display.Rendered += this.DemoDraw;
            /**/
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.Monitor.Log("Performing init...", LogLevel.Trace);
            var helper = this.Helper;
            this.Maps = new List<string>();
            // Load maps
            this.Monitor.Log("Preparing maps...", LogLevel.Trace);
            Parallel.ForEach(Directory.EnumerateFiles(Path.Combine(this.Helper.DirectoryPath, "assets", "Maps")), file =>
            {
                string ext = Path.GetExtension(file);
                if (ext.Equals(".tbin"))
                {
                    string map = Path.GetFileName(file);
                    try
                    {
                        this.Helper.Content.Load<Map>(Path.Combine("assets", "Maps", map));
                        new GameLocation(this.Helper.Content.GetActualAssetKey(Path.Combine("assets", "Maps", map)), Path.GetFileNameWithoutExtension(map));
                        lock(this.Maps)
                            this.Maps.Add(map);
                    }
                    catch (Exception err)
                    {
                        this.Monitor.Log("Unable to prepare [" + map + "] location, error follows\n" + err.ToString(), LogLevel.Error);
                    }
                }
            });
            this.Monitor.Log("Reading parking data...", LogLevel.Trace);
            foreach(var map in helper.Content.Load<JObject>("assets/Data/ParkingSpots.json").Properties())
            {
                this.Monitor.Log("Parsing parking data for the [" + map.Name + "] map, found [" + (map.Value as JArray).Count + "] parking spaces.", LogLevel.Trace);
                JArray spotsArr = map.Value.ToObject<JArray>();
                string key = map.Name;
                List<ParkingSpot> spots = new List<ParkingSpot>();
                foreach (JArray spotArr in spotsArr)
                {
                    var values = spotArr.Children().ToArray();
                    List<Facing> facings = new List<Facing>();
                    foreach (JValue facing in values[2].ToObject<JArray>())
                        facings.Add((Facing)Enum.Parse(typeof(Facing), facing.ToObject<string>()));
                    spots.Add(new ParkingSpot(new Vector2(values[0].ToObject<int>(), values[1].ToObject<int>()), facings.ToArray()));
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
            {
                try
                {
                    new NPC(new AnimatedSprite(this.Helper.Content.GetActualAssetKey("assets/Characters/Sprites/" + info.Texture + ".png"), 18, 16, 32), Vector2.Zero, 2, info.Name)
                    {
                        Portrait = this.Helper.Content.Load<Texture2D>("assets/Characters/Portraits/" + info.Texture + ".png"),
                    };
                }
                catch (Exception err)
                {
                    this.Monitor.Log("Unable to prepare villager by name of [" + info.Name + "] due to a unexpected issue.\n" + err.ToString(), LogLevel.Error);
                }
            }
            this.Monitor.Log("Preparing cameos...", LogLevel.Trace);
            foreach(var info in this.Helper.Data.ReadJsonFile<CharacterInfo[]>("assets/Data/CameoSpawns.json"))
            {
                try
                {
                    new NPC(new AnimatedSprite(this.Helper.Content.GetActualAssetKey("assets/Characters/Cameos/" + info.Name + "Sprite.png"), 18, 16, 32), Vector2.Zero, 2, info.Name)
                    {
                        Portrait = this.Helper.Content.Load<Texture2D>("assets/Characters/Cameos/" + info.Name + "Portrait.png"),
                    };
                }
                catch (Exception err)
                {
                    this.Monitor.Log("Unable to prepare cameo character for [" + info.Name + "] due to a unexpected issue.\n" + err.ToString(), LogLevel.Warn);
                }
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
                         this.Monitor.Log("Spawned tourist.", LogLevel.Alert);
                         break;
                 }
             });
            this.Monitor.Log("Init complete!", LogLevel.Trace);
        }

        private void Setup()
        {
            var start = DateTime.Now;
            this.Monitor.Log("Performing setup...", LogLevel.Trace);
            GameLocation promenade = null;
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
                    this.Monitor.Log("Unable to add [" + map + "] location, error follows\n" + err.ToString(), LogLevel.Error);
                }
            });
            promenade = Game1.getLocationFromName("SundropPromenade");
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
                        Vector2 pos = new Vector2(info.Position[0], info.Position[1]);
                        NPC villagerNPC = new NPC(new AnimatedSprite(this.Helper.Content.GetActualAssetKey("assets/Characters/Sprites/" + info.Texture + ".png"), 18, 16, 32), pos * 64f, 2, info.Name)
                        {
                            Portrait = this.Helper.Content.Load<Texture2D>("assets/Characters/Portraits/" + info.Texture + ".png"),
                        };
                        villagerNPC.DefaultMap = info.Map;
                        villagerNPC.DefaultPosition = pos * 64f;
                        villagerNPC.setNewDialogue(info.Message);
                        lock (Game1.getLocationFromName(info.Map))
                            Game1.getLocationFromName(info.Map).addCharacter(villagerNPC);
                    }
                    catch (Exception err)
                    {
                        this.Monitor.Log("Unable to add villager by name of [" + info.Name + "] due to a unexpected issue, this character will not appear in your game as a result.\n" + err.ToString(), LogLevel.Error);
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
                        Vector2 pos = new Vector2(info.Position[0], info.Position[1]);
                        NPC cameoNPC = new NPC(new AnimatedSprite(this.Helper.Content.GetActualAssetKey("assets/Characters/Cameos/" + info.Name + "Sprite.png"), 18, 16, 32), pos * 64f, 2, info.Name)
                        {
                            Portrait = this.Helper.Content.Load<Texture2D>("assets/Characters/Cameos/" + info.Name + "Portrait.png"),
                        };
                        cameoNPC.DefaultMap = info.Map;
                        cameoNPC.DefaultPosition = pos * 64f;
                        cameoNPC.setNewDialogue(info.Message);
                        lock (Game1.getLocationFromName(info.Map))
                            Game1.getLocationFromName(info.Map).addCharacter(cameoNPC);
                    }
                    catch (Exception err)
                    {
                        this.Monitor.Log("Unable to add cameo character for [" + info.Name + "] due to a unexpected issue, this character will not appear in your game as a result.\n" + err.ToString(), LogLevel.Warn);
                    }
            });
            var npc = Game1.getCharacterFromName("Joe");
            npc.Schedule = this.Helper.Reflection.GetMethod(npc, "parseMasterSchedule").Invoke<Dictionary<int, SchedulePathDescription>>("610 80 20 2/630 23 20 2/710 80 20 2/730 23 20 2/810 80 20 2/830 23 20 2/910 80 20 2/930 23 20 2");
            NPC cake = new MrCake(new Vector2(22, 20), npc);
            cake.setNewDialogue("Mr. Cake looks at you with approval.");
            promenade?.addCharacter(cake);
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
                        var vect = new Vector2(x, y);
                        town.largeTerrainFeatures.Filter(a => a.tilePosition.Value != vect);
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
            promenade?.setTileProperty(3, 37, "Buildings", "Action", "Warp 119 56 Town");
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
        private void AddCitizen(string id, string name, string map, Vector2 pos, string dialogue)
        {
        }
        private void AddCameoChar(string name, string map, Vector2 pos, string dialogue)
        {
        }
        private void SetupLocation(GameLocation location)
        {
            if (!location.map.Properties.ContainsKey("IsSundropLocation"))
                location.map.Properties.Add("IsSundropLocation", true);
            if (location.map.GetLayer("SundropPaths") != null)
            {
                var pathsLayer = location.map.GetLayer("SundropPaths");
                for (int x = 0; x < pathsLayer.LayerWidth; x++)
                    for (int y = 0; y < pathsLayer.LayerHeight; y++)
                    {
                        var tile = pathsLayer.Tiles[x, y];
                        if (tile == null)
                            continue;
                        Vector2 vect = new Vector2(x, y);
                        switch (tile.TileIndex)
                        {
                            case 0:
                                if (!location.terrainFeatures.ContainsKey(vect))
                                    location.terrainFeatures.Add(vect, new SundropGrass());
                                break;
                            case 1:
                                if (!location.terrainFeatures.ContainsKey(vect))
                                    location.terrainFeatures.Add(vect, new Tree(456, 5));
                                break;
                        }
                    }
            }
            var layer = location.map.GetLayer("Back");
            var sheet = location.map.TileSheets.Where(_ => _.ImageSource.Contains("SundropPaths.png")).FirstOrDefault();
            if (sheet == null || sheet==default)
            {
                var img = this.Helper.Content.Load<Texture2D>("assets/Maps/SundropPaths.png");
                sheet = new TileSheet("PathsTilesheet", location.map, this.Helper.Content.GetActualAssetKey("assets/Maps/SundropPaths.png"), new Size(img.Width / 16, img.Height / 16), new Size(16, 16));
                location.map.AddTileSheet(sheet);
            }
            for (int x = 0; x < layer.LayerWidth; x++)
                for (int y = 0; y < layer.LayerHeight; y++)
                {
                    var tile = layer.Tiles[x, y];
                    if (tile == null)
                        layer.Tiles[x, y] = new StaticTile(layer, sheet, BlendMode.Additive, 2);
                }
        }
        private void SpawnCars(GameLocation location, double chance)
        {
            Random rand = new Random();
            this.Monitor.Log($"SpawnCars: Cleaning up any old car objects in the map.", LogLevel.Trace);
            if (location.largeTerrainFeatures.Any(_ => _ is SundropCar))
                foreach (LargeTerrainFeature car in location.largeTerrainFeatures.Where(_ => _ is SundropCar).ToArray())
                    location.largeTerrainFeatures.Remove(car);
            this.Monitor.Log($"SpawnCars: Checking if map has parking spots defined.", LogLevel.Trace);
            if (!ParkingSpots.ContainsKey(location.Name))
                return;
            this.Monitor.Log($"SpawnCars: Spawning cars in [{location.Name}] with a {chance * 100}% chance.", LogLevel.Trace);
            foreach (ParkingSpot spot in ParkingSpots[location.Name])
                if (rand.NextDouble() <= chance)
                {
                    Facing facing = spot.Facings[rand.Next(spot.Facings.Length)];
                    location.largeTerrainFeatures.Add(new SundropCar(spot.Target, facing));
                }
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button.IsActionButton() && Game1.activeClickableMenu == null && !Game1.player.UsingTool && !Game1.pickingTool && !Game1.menuUp && (!Game1.eventUp || Game1.currentLocation.currentEvent.playerControlSequence) && !Game1.nameSelectUp && Game1.numberOfSelectedItems == -1 && !Game1.fadeToBlack)
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
                Game1.locations.Remove(location);
        }

        private void OnWarped(object s, WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer)
                return;
            if (ParkingSpots.ContainsKey(e.NewLocation.Name))
                this.SpawnCars(e.NewLocation, .8);
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
        private void DemoDraw(object s, EventArgs e)
        {
            Game1.spriteBatch.Draw(DemoTex, new Microsoft.Xna.Framework.Rectangle(16, 16, DemoSource.Width * 4, DemoSource.Height * 4), DemoTexColors);
            Game1.spriteBatch.Draw(DemoSource, new Microsoft.Xna.Framework.Rectangle(32 + DemoSource.Width * 4, 16, DemoSource.Width * 4, DemoSource.Height * 4), Color.Brown);
        }
    }
}
