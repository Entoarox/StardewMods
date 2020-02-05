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

using Microsoft.Xna.Framework.Media;

using Netcode;

using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace SundropCity
{
    using Json;
    using Microsoft.Xna.Framework.Audio;

    /// <summary>The mod entry class.</summary>
    public class SundropCityMod : Mod
    {
        internal static IModHelper SHelper;
        internal static IMonitor SMonitor;

        internal static Dictionary<string, ParkingSpot[]> ParkingSpots = new Dictionary<string, ParkingSpot[]>();

        private readonly List<string> Maps = new List<string>();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Define internals
            SHelper = helper;
            SMonitor = this.Monitor;
#if DEBUG
            this.TriggerDebugCode();
#else
            // We only register content handlers in release builds, debug builds skip this so that SDV can load faster and the debug stuff can be tested quicker
            helper.Content.AssetLoaders.Add(new SundropTreeLoader());
            helper.Content.AssetEditors.Add(new SundropTownEditor());
            // Same with events
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saved += this.OnSaved;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;
            helper.Events.Specialized.LoadStageChanged += this.OnLoadStageChanged;
            helper.Events.World.LocationListChanged += this.OnWorldLocationListChanged;
            helper.Events.Player.Warped += this.OnWarped;
#endif
        }

        private void TriggerDebugCode()
        {
            var tSprite = new AnimatedSprite(this.Helper.Content.GetActualAssetKey("assets/Festivals/SurfingWave.png"), 0, 224, 80)
            {
                loop = true
            };
            var tSprite2 = new AnimatedSprite(this.Helper.Content.GetActualAssetKey("assets/Festivals/SurfingWave.png"), 0, 224, 80)
            {
                loop = true
            };
            var tSurfSprite = new AnimatedSprite(this.Helper.Content.GetActualAssetKey("assets/Festivals/SurfingWave.png"), 0, 224, 80)
            {
                loop = true
            };
            //int state = 0;
            int surfLoopIndex = 0;
            //List<FarmerSprite.AnimationFrame> loopAnim, loopAnim2, endAnim = null;
            var loopAnim = new List<FarmerSprite.AnimationFrame>() {
                new FarmerSprite.AnimationFrame(0, 120),
                new FarmerSprite.AnimationFrame(1, 110),
                new FarmerSprite.AnimationFrame(2, 100),
                new FarmerSprite.AnimationFrame(3, 90),
                new FarmerSprite.AnimationFrame(4, 90),
                new FarmerSprite.AnimationFrame(5, 100),
                new FarmerSprite.AnimationFrame(6, 120),
            };
            var loopAnim2 = new List<FarmerSprite.AnimationFrame>() {
                new FarmerSprite.AnimationFrame(35, 120),
                new FarmerSprite.AnimationFrame(36, 110),
                new FarmerSprite.AnimationFrame(37, 100),
                new FarmerSprite.AnimationFrame(38, 90),
                new FarmerSprite.AnimationFrame(39, 90),
                new FarmerSprite.AnimationFrame(40, 100),
                new FarmerSprite.AnimationFrame(41, 120),
            };
            /*
            endAnim = new List<FarmerSprite.AnimationFrame>()
            {
                new FarmerSprite.AnimationFrame(7, 120),
                new FarmerSprite.AnimationFrame(8, 110),
                new FarmerSprite.AnimationFrame(9, 105),
                new FarmerSprite.AnimationFrame(10, 100),
                new FarmerSprite.AnimationFrame(11, 90),
                new FarmerSprite.AnimationFrame(12, 80),
                new FarmerSprite.AnimationFrame(13, 70),
                new FarmerSprite.AnimationFrame(14, 64),
                new FarmerSprite.AnimationFrame(15, 1000, false,false, (farmer) => {
                    tSprite.setCurrentAnimation(loopAnim);
                    tSprite.loop = true;
                    state = 0;
                }, true)
            };
            */
            List<FarmerSprite.AnimationFrame>[] surfLoop = null;
            surfLoop = new[]{
                new List<FarmerSprite.AnimationFrame>()
                {
                    new FarmerSprite.AnimationFrame(16, 175),
                    new FarmerSprite.AnimationFrame(17, 185),
                    new FarmerSprite.AnimationFrame(18, 195),
                    new FarmerSprite.AnimationFrame(19, 175, false, false, farmer => {
                        tSurfSprite.setCurrentAnimation(surfLoop[surfLoopIndex]);
                    }, true),
                },
                new List<FarmerSprite.AnimationFrame>()
                {
                    new FarmerSprite.AnimationFrame(21, 175),
                    new FarmerSprite.AnimationFrame(22, 185),
                    new FarmerSprite.AnimationFrame(23, 195),
                    new FarmerSprite.AnimationFrame(24, 175, false, false, farmer => {
                        tSurfSprite.setCurrentAnimation(surfLoop[surfLoopIndex]);
                    }, true),
                },
                new List<FarmerSprite.AnimationFrame>()
                {
                    new FarmerSprite.AnimationFrame(26, 175),
                    new FarmerSprite.AnimationFrame(27, 185),
                    new FarmerSprite.AnimationFrame(28, 195),
                    new FarmerSprite.AnimationFrame(29, 175, false, false, farmer => {
                        tSurfSprite.setCurrentAnimation(surfLoop[surfLoopIndex]);
                    }, true),
                },
            };
            var bgTex = this.Helper.Content.Load<Texture2D>("assets/Festivals/SurfingBackground.png");
            tSprite.setCurrentAnimation(loopAnim);
            tSprite2.setCurrentAnimation(loopAnim2);
            tSurfSprite.setCurrentAnimation(surfLoop[0]);
            this.Helper.Events.Input.ButtonReleased += (s, e) =>
            {
                /*
                if (e.Button.Equals(SButton.F7) && state == 0)
                    state++;
                    */
                if (e.Button.Equals(SButton.F8))
                {
                    surfLoopIndex++;
                    if (surfLoopIndex >= surfLoop.Length)
                        surfLoopIndex = 0;
                }
            };
            this.Helper.Events.GameLoop.UpdateTicked += (s, e) =>
            {
                if (Game1.currentGameTime == null)
                    return;
                tSprite.animateOnce(Game1.currentGameTime);
                tSprite2.animateOnce(Game1.currentGameTime);
                tSurfSprite.animateOnce(Game1.currentGameTime);
            };
            double rw = 1280;
            double rh = 720;
            int mw = 1024;
            int mh = 576;
            int vw = Game1.viewport.Width;
            double mult = Math.Min(Game1.viewport.Width / rw, Game1.viewport.Height / rh);
            int dw = (int)(mw * mult);
            int dh = (int)(mh * mult);
            int mx = (Game1.viewport.Width - dw) / 2;
            int my = (Game1.viewport.Height - dh) / 2;
            var b = Game1.spriteBatch;
            this.Helper.Events.Display.WindowResized += (s, e) => {

                vw = Game1.viewport.Width;
                mult = Math.Min(Game1.viewport.Width / rw, Game1.viewport.Height / rh);
                dw = (int)(mw * mult);
                dh = (int)(mh * mult);
                mx = (Game1.viewport.Width - dw) / 2;
                my = (Game1.viewport.Height - dh) / 2;
            };
            this.Helper.Events.Display.Rendered += (s, e) =>
            {
                StardewValley.Menus.IClickableMenu.drawTextureBox(Game1.spriteBatch, mx - 12, my - 12, dw + 24, dh + 24, Color.White);
                b.End();
                b.GraphicsDevice.ScissorRectangle = new Rectangle(mx, my, dw, dh);
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState { ScissorTestEnable = true });
                b.Draw(bgTex, new Rectangle(mx, my, dw / 4, dh), new Rectangle(0, 0, 256, 576), Color.White);
                b.Draw(bgTex, new Rectangle(mx + dw / 4, my, dw / 4 * 3, dh), new Rectangle(256, 0, 256, 576), Color.White);
                b.Draw(bgTex, new Rectangle(mx, (int)Math.Ceiling(my + 3 * mult), dw, (int)Math.Ceiling(dh - 3 * mult)), new Rectangle(512, 0, 512, 285), Color.White);
                tSprite.draw(b, new Vector2((float)(mx + 16 * mult), (float)(my + 386 * mult)), 1.00f, 0, 0, Color.White, false, (float)(2 * mult), 0, false);
                tSurfSprite.draw(b, new Vector2((float)(mx + (16 + 24 * surfLoopIndex) * mult), (float)(my + (386 - 2 * surfLoopIndex) * mult)), .98f, 0, 0, Color.White, false, (float)(2 * mult), 0, false);
                tSprite2.draw(b, new Vector2((float)(mx + 16 * mult), (float)(my + 386 * mult)), .99f, 0, 0, Color.White, false, (float)(2 * mult), 0, false);
                b.End();
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                if (!Game1.options.hardwareCursor)
                    b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, (Game1.options.snappyMenus && Game1.options.gamepadControls) ? 44 : 0, 16, 16), Color.White * Game1.mouseCursorTransparency, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
            };
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialized at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.Monitor.Log("Performing init...", LogLevel.Debug);
            List<Task> tasks = new List<Task>();
            var start = DateTime.Now;
            var helper = this.Helper;
            tasks.Add(Task.Run(() =>
            {
                using (var monitor = new LogBuffer(this.Monitor))
                {
                    monitor.Log("Initializing sounds...", LogLevel.Trace);
                    try
                    {
                        //Music = new SoundManager();
                    }
                    catch (Exception err)
                    {
                        monitor.Log("Exception loading Sundrop music: " + err, LogLevel.Error);
                    }
                }
            }));
            // Load maps
            tasks.Add(Task.Run(() =>
            {
                using (var monitor = new LogBuffer(this.Monitor))
                {
                    monitor.Log("Initializing maps...", LogLevel.Trace);
                    foreach (string file in Directory.EnumerateFiles(Path.Combine(this.Helper.DirectoryPath, "assets", "Maps")))
                    {
                        string ext = Path.GetExtension(file);
                        if (ext == null || !ext.Equals(".tbin"))
                            continue;
                        string map = Path.GetFileName(file);
                        if (map == null)
                            continue;
                        try
                        {
                            monitor.Log("Map found: " + map, LogLevel.Trace);
                            var mapFile = this.Helper.Content.Load<Map>(Path.Combine("assets", "Maps", map));
                            foreach (string prop in new[] { "Light", "Warps" })
                                if (mapFile.Properties.ContainsKey(prop))
                                    mapFile.Properties[prop] = mapFile.Properties[prop].ToString().Replace("\n", " ").Replace(";", " ").Replace("  ", " ");
                            foreach (TileSheet sheet in mapFile.TileSheets)
                                if (sheet.ImageSource.EndsWith(".png"))
                                {
                                    monitor.Log(sheet.ImageSource, LogLevel.Trace);
                                    string xnb = sheet.ImageSource.Substring(0, sheet.ImageSource.Length - 4);
                                    try
                                    {
                                        Game1.content.Load<Texture2D>(xnb);
                                        sheet.ImageSource = xnb;
                                    }
                                    catch
                                    {
                                        Game1.content.Load<Texture2D>(sheet.ImageSource);
                                    }
                                }
                            var mapInst = new GameLocation(this.Helper.Content.GetActualAssetKey(Path.Combine("assets", "Maps", map)), Path.GetFileNameWithoutExtension(map));
                            this.Maps.Add(map);
                            Tourist.WarpCache.Add(Path.GetFileNameWithoutExtension(map), Tourist.GetSpawnPoints(mapInst, new HashSet<int>() { Tourist.TILE_WARP_DOWN, Tourist.TILE_WARP_LEFT, Tourist.TILE_WARP_RIGHT, Tourist.TILE_WARP_UP }));
                            Tourist.SpawnCache.Add(Path.GetFileNameWithoutExtension(map), Tourist.GetSpawnPoints(mapInst, new HashSet<int>() { Tourist.TILE_ARROW_DOWN, Tourist.TILE_ARROW_LEFT, Tourist.TILE_ARROW_RIGHT, Tourist.TILE_ARROW_UP, Tourist.TILE_BROWSE, Tourist.TILE_SPAWN }));
                        }
                        catch (Exception err)
                        {
                            monitor.Log("Unable to prepare `" + map + "` location, error follows\n" + err, LogLevel.Error);
                        }
                    }
                }
            }));
            tasks.Add(Task.Run(() =>
            {
                this.Monitor.Log("Mapping tourist graphics...", LogLevel.Trace);
                Tourist.LoadResources();
            }));
            tasks.Add(Task.Run(() =>
            {
                using (var monitor = new LogBuffer(this.Monitor))
                {
                    monitor.Log("Reading parking data...", LogLevel.Trace);
                    foreach (var map in helper.Content.Load<JObject>("assets/Data/ParkingSpots.json").Properties())
                    {
                        monitor.Log("Parsing parking data for the `" + map.Name + "` map, found (" + (map.Value as JArray)?.Count + ") parking spaces.", LogLevel.Trace);
                        var spotsArr = map.Value.ToObject<JArray>();
                        string name = map.Name;
                        var spots = new List<ParkingSpot>();
                        foreach (var spotArr in spotsArr.Cast<JArray>())
                        {
                            var values = spotArr.Children().ToArray();
                            spots.Add(new ParkingSpot(new Vector2(values[0].ToObject<int>(), values[1].ToObject<int>()), (from JValue facing in values[2].ToObject<JArray>() select (Facing)Enum.Parse(typeof(Facing), facing.ToObject<string>())).ToArray()));
                        }
                        ParkingSpots.Add(map.Name, spots.ToArray());
                    }
                }
            }));
            tasks.Add(Task.Run(() =>
            {
                using (var monitor = new LogBuffer(this.Monitor))
                {
                    monitor.Log("Reading car data...", LogLevel.Trace);
                    SundropCar.CarTypes = helper.Data.ReadJsonFile<List<CarType>>("assets/Data/Cars.json");
                    if (SundropCar.CarTypes == null)
                    {
                        monitor.Log("Unable to read Cars.json for car data, cars will not be able to spawn!", LogLevel.Error);
                        SundropCar.CarTypes = new List<CarType>();
                    }
                    else
                        Parallel.ForEach(SundropCar.CarTypes, car =>
                        {
                            car.Base = helper.Content.GetActualAssetKey("assets/TerrainFeatures/Drivables/" + car.Base + ".png");
                            car.Recolor = helper.Content.GetActualAssetKey("assets/TerrainFeatures/Drivables/" + car.Recolor + ".png");
                            car.Decals = car.Decals.Select(pair => new KeyValuePair<string, string>(pair.Key, helper.Content.GetActualAssetKey("assets/TerrainFeatures/Drivables/" + pair.Value + ".png"))).ToDictionary(pair => pair.Key, pair => pair.Value);
                        });
                }
            }));
            tasks.Add(Task.Run(() =>
            {
                using (var monitor = new LogBuffer(this.Monitor))
                {
                    monitor.Log("Preparing characters...", LogLevel.Trace);
                    // NPC Spawning
                    foreach (var info in this.Helper.Data.ReadJsonFile<CharacterInfo[]>("assets/Data/CharacterSpawns.json"))
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
                            monitor.Log("Unable to prepare villager by name of `" + info.Name + "` due to a unexpected issue.\n" + err, LogLevel.Error);
                        }
                }
            }));
            tasks.Add(Task.Run(() =>
            {
                using (var monitor = new LogBuffer(this.Monitor))
                {
                    monitor.Log("Preparing cameos...", LogLevel.Trace);
                    foreach (var info in this.Helper.Data.ReadJsonFile<CharacterInfo[]>("assets/Data/CameoSpawns.json"))
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
                            monitor.Log("Unable to prepare cameo character for `" + info.Name + "` due to a unexpected issue.\n" + err, LogLevel.Warn);
                        }
                }
            }));
            tasks.Add(Task.Run(() =>
            {
                this.Monitor.Log("Registering commands...", LogLevel.Trace);
                helper.ConsoleCommands.Add("sundrop_debug", "For debug use only", (cmd, args) =>
                 {
                     if (args.Length == 0)
                     {
                         this.Monitor.Log("Debug command should only be used if you are asked to by a Sundrop developer!", LogLevel.Error);
                         return;
                     }
                     switch (args[0])
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
                         case "code":
                             this.TriggerDebugCode();
                             break;
                         case "warp":
                             Game1.warpFarmer("Town", 100, 58, 1);
                             break;
                         case "hotel":
                             Game1.activeClickableMenu = new Hotel.Menu();
                             break;
                     }
                 });
            }));
            Task.WaitAll(tasks.ToArray());
            var end = DateTime.Now;
            var time = end.Subtract(start);
            this.Monitor.Log("Init took " + time.TotalMilliseconds + " milliseconds.", LogLevel.Debug);
        }

        private void EarlySetup()
        {
            this.Monitor.Log("Performing early setup...", LogLevel.Debug);
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
                    this.Monitor.Log("Unable to add `" + map + "` location, error follows\n" + err, LogLevel.Error);
                }
            });
            var promenade = Game1.getLocationFromName("SundropPromenade");
            if (promenade == null)
            {
                this.Monitor.Log("Promenade failed to load, please report this issue.", LogLevel.Error);
                return;
            }
            // Report how long EarlySetup took
            var end = DateTime.Now;
            var time = end.Subtract(start);
            this.Monitor.Log("Early setup took " + time.TotalMilliseconds + " milliseconds.", LogLevel.Debug);

            // TEMP STUFF
            var town = Game1.getLocationFromName("Town");
            promenade.setTileProperty(3, 37, "Buildings", "Action", "Warp 119 59 Town");
            town.warps.Add(new Warp(120, 57, "SundropPromenade", 1, 29, false));
            town.warps.Add(new Warp(120, 58, "SundropPromenade", 1, 30, false));
            town.warps.Add(new Warp(120, 59, "SundropPromenade", 1, 31, false));
            town.warps.Add(new Warp(120, 60, "SundropPromenade", 1, 32, false));

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

        private void LateSetup()
        {
            this.Monitor.Log("Performing late setup...", LogLevel.Debug);
            var promenade = Game1.getLocationFromName("SundropPromenade");
            var start = DateTime.Now;
            if (promenade == null)
                return;
            List<Task> tasks = new List<Task>
            {
                Task.Run(() =>
                {
                    this.Monitor.Log("Repairing locations...", LogLevel.Trace);
                    Parallel.ForEach(Game1.locations, loc =>
                    {
                        if (loc.map.Properties.ContainsKey("IsSundropLocation"))
                            this.SetupLocation(loc);
                    });
                }),
                Task.Run(() =>
                {
                    this.Monitor.Log("Spawning characters...", LogLevel.Trace);
                    Parallel.ForEach(this.Helper.Data.ReadJsonFile<CharacterInfo[]>("assets/Data/CharacterSpawns.json"), info =>
                    {
                        if (Game1.getLocationFromName(info.Map) == null)
                            this.Monitor.Log("Unable to add villager by name of `" + info.Name + "` because their default map failed to load, this character will not appear in your game as a result.", LogLevel.Error);
                        else
                            try
                            {
                                this.Monitor.Log("Adding sundrop villager: " + info.Name, LogLevel.Trace);
                                var pos = new Vector2(info.Position[0], info.Position[1]);
                                var villagerNpc = new NPC(new AnimatedSprite(this.Helper.Content.GetActualAssetKey("assets/Characters/Sprites/" + info.Texture + ".png"), 18, 16, 32), pos * 64f, 2, info.Name)
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
                                this.Monitor.Log("Unable to add villager by name of `" + info.Name + "` due to a unexpected issue, this character will not appear in your game as a result.\n" + err, LogLevel.Error);
                            }
                    });
                }),
                Task.Run(() =>
                {
                    this.Monitor.Log("Spawning cameos...", LogLevel.Trace);
                    Parallel.ForEach(this.Helper.Data.ReadJsonFile<CharacterInfo[]>("assets/Data/CameoSpawns.json"), info =>
                    {
                        if (Game1.getLocationFromName(info.Map) == null)
                            this.Monitor.Log("Unable to add cameo character for `" + info.Name + "` because their default map failed to load, this character will not appear in your game as a result.", LogLevel.Warn);
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
                                this.Monitor.Log("Unable to add cameo character for `" + info.Name + "` due to a unexpected issue, this character will not appear in your game as a result.\n" + err, LogLevel.Warn);
                            }
                    });
                })
            };
            Task.WaitAll(tasks.ToArray());
            var npc = Game1.getCharacterFromName("Joe");
            if (npc != null)
            {
                npc.Schedule = this.Helper.Reflection.GetMethod(npc, "parseMasterSchedule").Invoke<Dictionary<int, SchedulePathDescription>>("610 80 20 2/630 23 20 2/710 80 20 2/730 23 20 2/810 80 20 2/830 23 20 2/910 80 20 2/930 23 20 2");
                NPC cake = new MrCake(new Vector2(22, 20), npc);
                cake.setNewDialogue("Mr. Cake looks at you with approval.");
                promenade?.addCharacter(cake);
            }
            SundropCityMod.FixBushes();
            this.Monitor.Log("Forcing custom layer hooks to reload...");
            this.OnWorldLocationListChanged(null, null);
            var end = DateTime.Now;
            var time = end.Subtract(start);
            this.Monitor.Log("Late setup took " + time.TotalMilliseconds + " milliseconds, sundrop is now ready for you - enjoy!", LogLevel.Debug);
        }

        internal static void FixBushes()
        {
            var town = Game1.getLocationFromName("Town");
            var paths = town.map.GetLayer("Paths");
            int bushes = 0;
            foreach (LargeTerrainFeature feature in town.largeTerrainFeatures.Where(_ => _ is Bush).ToArray())
            {
                var tile = paths.Tiles[(int)feature.tilePosition.X, (int)feature.tilePosition.Y];
                if (tile == null || tile.TileIndex < 24 || tile.TileIndex > 26)
                {
                    bushes++;
                    town.largeTerrainFeatures.Remove(feature);
                }
            }
            SundropCityMod.SMonitor.Log("Removed (" + bushes + ") bushes that are in the way.", LogLevel.Trace);
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
            SundropCityMod.SMonitor.Log("Spawning cars...", LogLevel.Trace);
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
            SundropCityMod.SMonitor.Log("Spawning tourists...", LogLevel.Trace);
            if (!Tourist.SpawnCache.ContainsKey(loc.Name))
                return;
            var validPoints = Tourist.SpawnCache[loc.Name];
            if (validPoints.Count == 0)
                return;
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
                this.EarlySetup();
        }

        private void OnSaveLoaded(object s, EventArgs e)
        {
            this.LateSetup();
        }
        private void OnSaved(object s, EventArgs e)
        {
            this.LateSetup();
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
                if (location.characters.Any(_ => _.DefaultMap.StartsWith("Sundrop")))
                    foreach (var character in location.characters.Where(_ => _.DefaultMap.StartsWith("Sundrop")).ToArray())
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
        }
        private void OnWorldLocationListChanged(object s, EventArgs e)
        {
            this.Monitor.Log("LocationList changed, resetting custom layer hooks.");
            Parallel.ForEach(Game1.locations, location =>
            {
                var strs = new List<string>();
                if (location.map.GetLayer("FarBack") != null || location.map.GetLayer("MidBack") != null)
                {
                    strs.Add("FarBack/MidBack");
                    location.map.GetLayer("Back").BeforeDraw -= this.DrawExtraLayers1;
                    location.map.GetLayer("Back").BeforeDraw += this.DrawExtraLayers1;
                }
                if (location.map.GetLayer("Shadows") != null)
                {
                    strs.Add("Shadows");
                    location.map.GetLayer("Back").AfterDraw -= this.DrawExtraLayers4;
                    location.map.GetLayer("Back").AfterDraw += this.DrawExtraLayers4;
                }
                if (location.map.GetLayer("LowFront") != null)
                {
                    strs.Add("LowFront");
                    location.map.GetLayer("Front").BeforeDraw -= this.DrawExtraLayers2;
                    location.map.GetLayer("Front").BeforeDraw += this.DrawExtraLayers2;
                }
                if (location.map.GetLayer("AlwaysFront2") != null)
                {
                    strs.Add("AlwaysFront2");
                    location.map.GetLayer("AlwaysFront").AfterDraw -= this.DrawExtraLayers3;
                    location.map.GetLayer("AlwaysFront").AfterDraw += this.DrawExtraLayers3;
                }
                if(strs.Count>0)
                    this.Monitor.Log("Location: " + location.Name + " (" + string.Join(", ", strs) + ")");
            });
            this.Monitor.Log("Custom hooks have been reset.");
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
