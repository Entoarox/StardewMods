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

using Rectangle = Microsoft.Xna.Framework.Rectangle;

using Entoarox.Utilities.Tools;

namespace SundropCity
{
    using Json;
    using Internal;
    using Characters;
    using Toolbox;
    using Microsoft.Xna.Framework.Audio;

    /// <summary>The mod entry class.</summary>
    public class SundropCityMod : Mod
    {
        internal static IModHelper SHelper;
        internal static IMonitor SMonitor;
        internal static Config Config;
        internal static SystemData SystemData;
        internal static Dictionary<string, CustomFeature[]> AlphaFeatures;
#if !DISABLE_SOUND
        internal static SoundEffect Sound;
#endif

        internal static Dictionary<string, ParkingSpot[]> ParkingSpots = new Dictionary<string, ParkingSpot[]>();

        private readonly List<string> Maps = new List<string>();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Monitor.Log("Performing entry....", LogLevel.Debug);
            // Define internals
            SHelper = helper;
            SMonitor = this.Monitor;
            Config = helper.ReadConfig<Config>();
#if DEBUG
            Config.DebugFlags |= DebugFlags.Functions;
#endif
            this.Monitor.Log("Reading JSON data...", LogLevel.Trace);
            SystemData = helper.Data.ReadJsonFile<SystemData>("assets/Data/SystemData.json");
            AlphaFeatures = helper.Data.ReadJsonFile<Dictionary<string, CustomFeature[]>>("assets/Data/AlphaFeatures.json");


            this.Monitor.Log("Checking blacklisted files...", LogLevel.Trace);
            // Handle asset blacklist
            if (!Config.DebugFlags.HasFlag(DebugFlags.Files))
                foreach (string file in SystemData.FileBlacklist)
                {
                    if (file.Contains("../") || file.Contains("./"))
                        continue;
                    string path = Path.Combine(helper.DirectoryPath, "assets", file);
                    if (File.Exists(path))
                        File.Delete(path);
                }

            // Check if the quickload debug flag is set, if so, skip most of sundrop's loading process so the game loads ASAP
            if (Config.DebugFlags.HasFlag(DebugFlags.Quickload))
                return;

            this.Monitor.Log("Registering asset managers...", LogLevel.Trace);
            helper.Content.AssetLoaders.Add(new SundropTreeLoader());
            helper.Content.AssetEditors.Add(new SundropTownEditor());
            this.Monitor.Log("Registering events...", LogLevel.Trace);
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saved += this.OnSaved;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;
            helper.Events.Specialized.LoadStageChanged += this.OnLoadStageChanged;
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
            helper.Events.World.LocationListChanged += this.OnWorldLocationListChanged;
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
#if !DISABLE_SOUND
            // Load sounds
            tasks.Add(Task.Run(() =>
            {
                using (var monitor = new LogBuffer(this.Monitor))
                {
                    monitor.Log("Initializing sounds...", LogLevel.Trace);
                    try
                    {
                        Sound = SoundEffect.FromStream(File.OpenRead(Path.Combine(this.Helper.DirectoryPath, "assets", "Sounds", "SundropBeachNight.wav")));
                    }
                    catch (Exception err)
                    {
                        monitor.Log("Exception loading Sundrop music: " + err, LogLevel.Error);
                    }
                }
            }));
#endif
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
                            foreach (string prop in SystemData.Layers)
                                if (mapFile.Properties.ContainsKey(prop))
                                {
                                    string rep=mapFile.Properties[prop].ToString().Replace("\n", " ").Replace(";", " ").Replace("  ", " ");
                                    while(rep.Contains("  "))
                                        rep = rep.Replace("  ", " ");
                                    mapFile.Properties[prop] = rep;
                                }
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
                if (Config.DebugFlags.HasFlag(DebugFlags.Functions))
                    helper.ConsoleCommands.Add("sundrop_debug", "For debug use only", (cmd, args) =>
                    {
                        if (args.Length == 0)
                        {
                            this.Monitor.Log("Debug command should only be used if you are asked to by a Sundrop developer!", LogLevel.Error);
                            return;
                        }
                        switch (args[0])
                        {
                            case "tourists":
                                SundropCityMod.SpawnTourists(Game1.player.currentLocation, Convert.ToInt32(args[1]));
                                this.Monitor.Log("Executed tourist spawning algorithm.", LogLevel.Alert);
                                break;
                            case "code":
                                this.TriggerDebugCode();
                                this.Monitor.Log("Debug code triggered.", LogLevel.Alert);
                                break;
                            case "warp":
                                if (args.Length == 1)
                                    Game1.warpFarmer("Town", 100, 58, 1);
                                else
                                {
                                    if (Game1.getLocationFromName(args[1]) == null)
                                    {
                                        this.Monitor.Log("Unable to warp, target destination does not exist.", LogLevel.Error);
                                        break;
                                    }
                                    Game1.warpFarmer(args[1], Convert.ToSByte(args[2]) + 64, Convert.ToSByte(args[3]) + 64, false);
                                }
                                this.Monitor.Log("Warping player to target.", LogLevel.Alert);
                                break;
                            case "hotelMenu":
                                Game1.activeClickableMenu = new Hotel.Menu();
                                this.Monitor.Log("Menu for hotel triggered.", LogLevel.Alert);
                                break;
#if !DISABLE_SOUND
                            case "playSound":
                                 this.Monitor.Log("Playing custom music...", LogLevel.Alert);
                                 Game1.stopMusicTrack(Game1.MusicContext.Default);
                                 Sound.Play(1, 1, 1);
                                 break;
                             case "reportSound":
                                 this.Monitor.Log("AudioEngine Wrapper: " + Game1.audioEngine.GetType().Name, LogLevel.Alert);
                                 this.Monitor.Log("Wrapper Disposed: " + Game1.audioEngine.IsDisposed, LogLevel.Alert);
                                 this.Monitor.Log("AudioEngine Source: " + Game1.audioEngine.Engine.GetType().Name, LogLevel.Alert);
                                 this.Monitor.Log("Source Disposed: " + Game1.audioEngine.Engine.IsDisposed, LogLevel.Alert);
                                 break;
#endif
                            case "map":
                                this.Monitor.Log("Name of current map: " + Game1.currentLocation.Name, LogLevel.Alert);
                                break;
                            case "position":
                                var p = Game1.player.getTileLocationPoint();
                                this.Monitor.Log($"Current tile position: {p.X}x {p.Y}y", LogLevel.Alert);
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
            this.OnWorldLocationListChanged(null, null);
            this.Monitor.Log("Early setup took " + time.TotalMilliseconds + " milliseconds.", LogLevel.Debug);

            // TEMP STUFF
            var town = Game1.getLocationFromName("Town");
            promenade.setTileProperty(3, 37, "Buildings", "Action", "Warp 119 59 Town");
            town.warps.Add(new Warp(120, 57, "SundropPromenade", 1, 29, false));
            town.warps.Add(new Warp(120, 58, "SundropPromenade", 1, 30, false));
            town.warps.Add(new Warp(120, 59, "SundropPromenade", 1, 31, false));
            town.warps.Add(new Warp(120, 60, "SundropPromenade", 1, 32, false));
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
            /*
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
            */
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
            List<Exception> errs = new List<Exception>();
            if (!Tourist.SpawnCache.ContainsKey(loc.Name))
                return;
            var validPoints = Tourist.SpawnCache[loc.Name];
            if (validPoints.Count < 1)
            return;
            SundropCityMod.SMonitor.Log($"Attempting to spawn {amount} tourists across {validPoints.Count} spawning spaces.", LogLevel.Trace);
            int total = amount;
            int max = amount * 2;
            for (int c = 0; c < total; c++)
            {
                int n = Game1.random.Next(validPoints.Count);
                try
                {
                    loc.addCharacter(new Tourist(validPoints[n] * 64f));
                }
                catch (Exception err)
                {
                    if (err is IndexOutOfRangeException)
                        errs.Add(new AggregateException(new IndexOutOfRangeException("Index out of range with value: " + n), err));
                    else
                        errs.Add(err);
                    if (total < max)
                        total++;
                }
            }
            if (errs.Count == 0)
                return;
            SundropCityMod.SMonitor.Log("Encountered one or more errors while spawning tourists:", LogLevel.Warn);
            foreach (var err in errs)
                SundropCityMod.SMonitor.Log(err.ToString(), LogLevel.Warn);
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
                        case "SundropTravel":
                            TravelManager.Instance.Trigger(args[0]);
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
                if (location.largeTerrainFeatures.Any(_ => _ is ISundropTransient))
                    foreach (LargeTerrainFeature car in location.largeTerrainFeatures.Where(_ => _ is ISundropTransient).ToArray())
                        location.largeTerrainFeatures.Remove(car);
                if (location.terrainFeatures.Pairs.Any(_ => _.Value is ISundropTransient))
                    foreach (var feature in location.terrainFeatures.Pairs.Where(_ => _.Value is ISundropTransient).Select(_ => _.Key).ToArray())
                        location.terrainFeatures.Remove(feature);
                if (location.characters.Any(_ => _ is ISundropTransient))
                    foreach (var character in location.characters.Where(_ => _ is ISundropTransient).ToArray())
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
            this.Monitor.Log("Injecting AlphaFeature support...");
            Parallel.ForEach(Game1.locations, location =>
            {
                if (!AlphaFeatures.ContainsKey(location.Name))
                    return;
                location.map.GetLayer("Front").BeforeDraw -= this.DrawBuildingFeatures;
                location.map.GetLayer("Front").BeforeDraw += this.DrawBuildingFeatures;
                if (location.map.GetLayer("AlwaysFront") != null)
                {
                    location.map.GetLayer("AlwaysFront").BeforeDraw -= this.DrawAlwaysFrontFeatures;
                    location.map.GetLayer("AlwaysFront").BeforeDraw += this.DrawAlwaysFrontFeatures;
                }
            });
            this.Monitor.Log("Custom hooks have been injected.");
        }

        private void OnRenderedWorld(object s, EventArgs e)
        {
            if (!AlphaFeatures.ContainsKey(Game1.currentLocation?.Name))
                return;
            if (Game1.currentLocation.map.GetLayer("AlwaysFront") == null)
                this.DrawAlwaysFrontFeatures(null, null);
            foreach (var feature in AlphaFeatures[Game1.currentLocation.Name].Where(_ => _.Type == FeatureType.AboveAlwaysFront))
                feature.Render(Game1.spriteBatch);
        }
        private void DrawBuildingFeatures(object s, LayerEventArgs e)
        {
            if (AlphaFeatures.ContainsKey(Game1.currentLocation?.Name))
                foreach (var feature in AlphaFeatures[Game1.currentLocation.Name].Where(_ => _.Type == FeatureType.Inline))
                feature.Render(Game1.spriteBatch);
            Game1.currentLocation.map.GetLayer("LowFront")?.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
        }
        private void DrawAlwaysFrontFeatures(object s, LayerEventArgs e)
        {
            if (AlphaFeatures.ContainsKey(Game1.currentLocation?.Name))
                foreach (var feature in AlphaFeatures[Game1.currentLocation.Name].Where(_ => _.Type == FeatureType.BelowAlwaysFront))
                    feature.Render(Game1.spriteBatch);
        }
    }
}
