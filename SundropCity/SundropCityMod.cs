using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.TerrainFeatures;

using SundropCity.TerrainFeatures;

using xTile;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Layers;
using xTile.Tiles;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Newtonsoft.Json.Linq;

namespace SundropCity
{
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
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saved += this.OnSaved;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;

            // Handle ALL not providing extra layer drawing
            if (!helper.ModRegistry.IsLoaded("Entoarox.AdvancedLocationLoader") || helper.ModRegistry.Get("Entoarox.AdvancedLocationLoader").Manifest.Version.IsOlderThan("1.5.0"))
                helper.Events.Player.Warped += this.OnWarped;
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var helper = this.Helper;

            // Load maps
            List<string> maps = new List<string>();
            foreach (string file in Directory.EnumerateFiles(Path.Combine(this.Helper.DirectoryPath, "assets", "Maps")))
            {
                string ext = Path.GetExtension(file);
                this.Monitor.Log($"Checking file: {file} (ext: {ext})", LogLevel.Trace);
                if (!ext.Equals(".tbin"))
                    continue;
                string map = Path.GetFileName(file);
                this.Monitor.Log("Found sundrop location: " + map, LogLevel.Trace);
                try
                {
                    this.Helper.Content.Load<Map>(Path.Combine("assets", "Maps", map));
                    maps.Add(map);
                }
                catch(Exception err)
                {
                    this.Monitor.Log("Unable to prepare [" + map + "] location, error follows\n" + err.ToString(), LogLevel.Error);
                }
            }
            this.Maps = maps;


            JObject dict = helper.Content.Load<JObject>("assets/Data/ParkingSpots.json");
            foreach(JProperty map in dict.Properties())
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
                ParkingSpots.Add(key, spots.ToArray());
            }
            this.Monitor.Log("Loading bundled car textures...", LogLevel.Trace);
            foreach (string file in Directory.EnumerateFiles(Path.Combine(helper.DirectoryPath, "assets/TerrainFeatures/Cars")))
            {
                string local = file.Replace(helper.DirectoryPath + Path.DirectorySeparatorChar, "");
                this.Monitor.Log("Checking file: " + local, LogLevel.Trace);
                if (Path.GetExtension(local).Equals(".png"))
                    SundropCar.Textures.Add(helper.Content.Load<Texture2D>(local));
            }
            this.Monitor.Log("Found and loaded [" + SundropCar.Textures.Count.ToString() + "] bundled car textures.", LogLevel.Trace);
            helper.ConsoleCommands.Add("sundrop_debug", "For debug use only", (cmd, args) =>
             {
                 switch(args[0])
                 {
                     case "car":
                         Facing facing = (Facing)Enum.Parse(typeof(Facing), args[1]);
                         Game1.currentLocation.largeTerrainFeatures.Add(new SundropCar(Game1.player.getTileLocation(), facing));
                         break;
                     case "parking":
                         this.SpawnCars(Game1.currentLocation, Convert.ToDouble(args[1]));
                         break;
                 }
             });
        }

        private void Setup()
        {
            this.Monitor.Log("Performing setup...", LogLevel.Trace);
            // Handle patches specific to other mods
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
            // Add new locations
            foreach (string map in this.Maps)
            {
                try
                {
                    this.Monitor.Log("Adding sundrop location: " + Path.GetFileNameWithoutExtension(map), LogLevel.Trace);
                    var loc = new GameLocation(this.Helper.Content.GetActualAssetKey(Path.Combine("assets", "Maps", map)), Path.GetFileNameWithoutExtension(map));
                    this.SetupLocation(loc);
                    Game1.locations.Add(loc);
                }
                catch(Exception err)
                {
                    this.Monitor.Log("Unable to add [" + map + "] location, error follows\n" + err.ToString(), LogLevel.Error);
                }
            }
            var promenade = Game1.getLocationFromName("SundropPromenade");
            if(promenade==null)
            {
                this.Monitor.Log("Promenade failed to load, cancelling further setup as a result.", LogLevel.Error);
                return;
            }
            // Setup warps to sundrop [TEMP: Will become warps to SundropBusStop map in the future]
            town.warps.Add(new Warp(120, 55, "SundropPromenade", 1, 29, false));
            town.warps.Add(new Warp(120, 56, "SundropPromenade", 1, 30, false));
            town.warps.Add(new Warp(120, 57, "SundropPromenade", 1, 31, false));
            town.warps.Add(new Warp(120, 58, "SundropPromenade", 1, 32, false));
            // Add warp back to Pelican [TEMP: Will be removed once proper travel is implemented]
            promenade.setTileProperty(3, 37, "Buildings", "Action", "Warp 119 56 Town");
            // Temp NPC spawns
            NPC npc = new NPC(new AnimatedSprite(this.Helper.Content.GetActualAssetKey("assets/Characters/Sprites/Joe.png"), 18, 16, 32), new Vector2(24, 20) * 64f, 2, "Joe")
            {
                Portrait = this.Helper.Content.Load<Texture2D>("assets/Characters/Portraits/Joe.png"),
                Breather = false,
                HideShadow = true
            };
            npc.DefaultMap = "SundropPromenade";
            npc.DefaultPosition = new Vector2(23, 20) * 64f;
            npc.Schedule = this.Helper.Reflection.GetMethod(npc, "parseMasterSchedule").Invoke<Dictionary<int, SchedulePathDescription>>("610 80 20 2/630 23 20 2/710 80 20 2/730 23 20 2/810 80 20 2/830 23 20 2/910 80 20 2/930 23 20 2");
            npc.setNewDialogue("Mr. Bones wanted me to take care of Mr. Cake for him while he deals with a corruption claim by this guy called Stubbs...$bNo idea why the cat even bothers, everyone knows Mr. Cake is best doggo!");
            promenade.addCharacter(npc);
            NPC cake = new MrCake(new Vector2(22, 20), npc);
            cake.setNewDialogue("Mr. Cake looks at you with approval.");
            promenade.addCharacter(cake);
            this.AddCameoChar("ChefRude", "SundropPromenade", new Vector2(24, 50), "So much work to do... $b#$sI just cant keep up...");
        }
        private void AddCameoChar(string name, string map, Vector2 pos, string dialogue)
        {
            NPC npc = new NPC(new AnimatedSprite(this.Helper.Content.GetActualAssetKey("assets/Characters/Cameos/" + name + "Sprite.png"), 18, 16, 32), pos * 64f, 2, name)
            {
                Portrait = this.Helper.Content.Load<Texture2D>("assets/Characters/Cameos/"+name+"Portrait.png"),
                Breather = false,
                HideShadow = true
            };
            npc.DefaultMap = map;
            npc.DefaultPosition =pos * 64f;
            npc.setNewDialogue(dialogue);
            Game1.getLocationFromName(map).addCharacter(npc);
        }
        private void SetupLocation(GameLocation location)
        {
            if (!location.map.Properties.ContainsKey("IsSundropLocation"))
                location.map.Properties.Add("IsSundropLocation", true);
            if (location.map.GetLayer("SundropPaths") != null)
            {
                var layer = location.map.GetLayer("SundropPaths");
                for (int x = 0; x < location.map.DisplayWidth; x++)
                    for (int y = 0; y < location.map.DisplayHeight; y++)
                    {
                        var tile = layer.Tiles[x, y];
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
            e.OldLocation.map.GetLayer("Back").BeforeDraw -= this.DrawExtraLayers1;
            e.NewLocation.map.GetLayer("Back").BeforeDraw += this.DrawExtraLayers1;
            e.OldLocation.map.GetLayer("Front").AfterDraw -= this.DrawExtraLayers2;
            e.NewLocation.map.GetLayer("Front").AfterDraw += this.DrawExtraLayers2;
        }
        private void DrawExtraLayers1(object s, LayerEventArgs e)
        {
            Game1.currentLocation.map.GetLayer("FarBack")?.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
            Game1.currentLocation.map.GetLayer("MidBack")?.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
        }
        private void DrawExtraLayers2(object s, LayerEventArgs e)
        {
            Game1.currentLocation.map.GetLayer("Front2")?.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
        }
    }
}
