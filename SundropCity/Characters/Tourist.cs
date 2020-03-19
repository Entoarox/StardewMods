using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using StardewValley;

namespace SundropCity.Characters
{
    using Json;
    internal class Tourist : NPC, Internal.ISundropTransient
    {
        public const int TOURIST_LINE_COUNT = 60;
        public const int RARITY_RANGE = 10;

        private static readonly IReadOnlyList<SpecialTourist> Specials = new List<SpecialTourist>()
        {
            new SpecialTourist("Wally", 16, 0.0001)
        };

        public const int TILE_SPAWN = 0;
        public const int TILE_BLOCK = 1;
        public const int TILE_KEEPMOVING = 2;
        public const int TILE_BROWSE = 4;
        public const int TILE_ARROW_DOWN = 5;
        public const int TILE_ARROW_RIGHT = 6;
        public const int TILE_ARROW_UP = 7;
        public const int TILE_ARROW_LEFT = 8;
        public const int TILE_WARP_DOWN = 10;
        public const int TILE_WARP_RIGHT = 11;
        public const int TILE_WARP_UP = 12;
        public const int TILE_WARP_LEFT = 13;

        protected AnimatedSprite Base;
        protected AnimatedSprite Hat;
        protected AnimatedSprite Hair;
        protected AnimatedSprite Shirt;
        protected AnimatedSprite Pants;
        protected AnimatedSprite Shoes;
        protected AnimatedSprite Accessory;
        protected AnimatedSprite Decoration;

        protected Color HairColor;
        protected Color PantsColor;
        protected Color ShoeColor;
        protected Color ShirtColor;
        protected Color HatColor;
        protected Color AccessoryColor;
        protected Color DecorationColor;

        protected byte Timer;
        protected byte Delay;
        protected byte Cooldown;
        protected byte Stuck;
        internal short TickAge;
        internal string Special;
        protected Vector2 OldPos;
        protected byte Skip = 0;

        internal static Dictionary<string, TouristPartsGroup> Parts = new Dictionary<string, TouristPartsGroup>()
        {
            ["spring"] = new TouristPartsGroup(),
            ["summer"] = new TouristPartsGroup(),
            ["fall"] = new TouristPartsGroup(),
            ["winter"] = new TouristPartsGroup(),
        };

        internal static Dictionary<string, List<Vector2>> WarpCache = new Dictionary<string, List<Vector2>>();
        internal static Dictionary<string, List<Vector2>> SpawnCache = new Dictionary<string, List<Vector2>>();
        internal static List<string> BaseColors = new List<string>();
        internal static List<string> MaleHair = new List<string>();
        internal static List<string> FemaleHair = new List<string>();
        internal static List<string> MaleShirt = new List<string>();
        internal static List<string> FemaleShirt = new List<string>();
        internal static List<string> MalePants = new List<string>();
        internal static List<string> FemalePants = new List<string>();
        internal static List<string> MaleShoes = new List<string>();
        internal static List<string> FemaleShoes = new List<string>();
        internal static List<string> MaleAccessory = new List<string>();
        internal static List<string> FemaleAccessory = new List<string>();
        internal static List<string> MaleHat = new List<string>();
        internal static List<string> FemaleHat = new List<string>();
        internal static List<string> FaceHairs = new List<string>();

        internal static string[] Seasons = { "spring", "summer", "fall", "winter" };
        internal static string[] SeasonsHot = { "spring", "summer", "fall" };
        internal static List<Color> HairColors = new List<Color>()
        {
            new Color(246, 255, 94),
            new Color(215, 209, 219),
            new Color(193, 197, 219),
            new Color(169, 170, 161),
            new Color(224, 198, 243),
            new Color(200, 195, 143),
            new Color(219, 192, 127),
            new Color(198, 198, 90),
            new Color(146, 86, 26),
            new Color(54, 37, 109),
            new Color(127, 134, 158),
            new Color(60, 52, 85),
            new Color(77, 40, 103),
            new Color(79, 40, 40),
            new Color(255, 64, 0),
            new Color(231, 137, 21),
            new Color(194, 70, 2),
            new Color(146, 22, 16),
            new Color(42, 45, 249),
            new Color(42, 249, 108),
            new Color(255, 0, 0),
            new Color(255, 103, 0),
            new Color(190, 4, 255),
            new Color(255, 0, 219),
        };

        internal static List<Color> ClothingColors = new List<Color>()
        {
            new Color(255, 134, 73),
            new Color(255, 73, 94),
            new Color(353, 88, 78),
            new Color(240, 197, 0),
            new Color(199, 240, 0),
            new Color(114, 204, 47),
            new Color(67, 204, 130),
            new Color(67, 203, 204),
            new Color(67, 134, 204),
            new Color(45, 60, 153),
            new Color(89, 45, 153),
            new Color(165, 16, 164),
            new Color(84, 133, 140),
            new Color(114, 147, 152),
            new Color(72, 115, 49),
            new Color(122, 170, 97),
            new Color(244, 255, 73),
            new Color(182, 130, 64),
            new Color(115, 50, 22),
            new Color(97, 60, 43),
            new Color(255, 66, 45),
            new Color(182, 24, 24),
            new Color(50, 12, 61),
            new Color(43, 26, 85),
        };
        internal static AnimatedSprite BlankSprite = new AnimatedSprite();

        internal static List<Vector2> GetSpawnPoints(GameLocation location, HashSet<int> validTiles)
        {
            var layer = location.map.GetLayer("Tourists");
            if (layer == null)
                return null;
            List<Vector2> validPoints = new List<Vector2>();
            var npc = new Tourist();
            Parallel.For(0, layer.LayerWidth, x =>
            {
                Parallel.For(0, layer.LayerHeight, y =>
                {
                    try
                    {
                        var box = new Rectangle(x * 64 + 16, y * 64 + 16, 32, 32);
                        int index = GetTileIndex(location, x, y);
                        if (index == -1 || !validTiles.Contains(index) || location.isCollidingPosition(box, Game1.viewport, npc))
                            return;
                        validPoints.Add(new Vector2(x, y));
                    }
                    catch
                    { }
                });
            });
            return validPoints;
        }

        internal static int GetTileIndex(GameLocation loc, int x, int y)
        {
            var layer = loc.map.GetLayer("Tourists");
            if (layer == null)
                return -1;
            return layer.Tiles[x, y]?.TileIndex ?? -1;
        }
        private static Dictionary<string, string> VariableCache = new Dictionary<string, string>();
        private static string PartVariables(string part)
        {
            if (!VariableCache.ContainsKey(part))
                VariableCache.Add(part, Path.GetFileNameWithoutExtension(part).Split('_')[1]);
            return VariableCache[part];
        }

        internal static void LoadResources()
        {
            string path = Path.Combine("assets","Characters","Tourists");
            List<Task> TaskList = new List<Task>();
            foreach(string part in new[]{ "Body", "Top", "Bottom", "Shoe", "Hair", "Accessory", "Decoration", "Hat"})
                TaskList.Add(LoadParts(path, part));
            Task.WaitAll(TaskList.ToArray());
        }
        private static Task LoadParts(string path, string folder)
        {
            return Task.Run(() =>
            {
                int i = 0;
                try
                {
                    foreach (string file in Directory.EnumerateFiles(Path.Combine(SundropCityMod.SHelper.DirectoryPath, path, folder)))
                    {
                        if (!Path.GetExtension(file).Equals(".png"))
                            continue;
                        string vars = PartVariables(file);
                        if (vars.Length < 3)
                            continue;
                        if (folder.Equals("Hair") && vars[2].Equals('s'))
                            continue;
                        //SundropCityMod.SMonitor.Log("Mapping file: " + folder + '/' + Path.GetFileName(file), StardewModdingAPI.LogLevel.Trace);
                        string key = Path.Combine(path, folder, Path.GetFileName(file));
                        SundropCityMod.SHelper.Content.Load<Texture2D>(key);
                        if (folder.Equals("Hair"))
                        {
                            try
                            {
                                string[] hairParts = Path.GetFileNameWithoutExtension(key).Split('_');
                                hairParts[1] = hairParts[1].Replace('x', 's');
                                string hathair = key.Replace(Path.GetFileNameWithoutExtension(key), string.Join("_", hairParts));
                                SundropCityMod.SHelper.Content.Load<Texture2D>(hathair);
                            }
                            catch
                            {
                                SundropCityMod.SMonitor.Log("Missing hat variant for hair texture: " + folder + '/' + Path.GetFileName(file), StardewModdingAPI.LogLevel.Warn);
                                continue;
                            }
                        }
                        int rarity = vars.Length > 3 ? RARITY_RANGE - Convert.ToInt32(vars[3].ToString(), RARITY_RANGE) : RARITY_RANGE;
                        string[] seasons;
                        int seasonNum = Convert.ToInt32(vars[1].ToString());
                        switch(seasonNum)
                        {
                            case 0:
                                seasons = Seasons;
                                break;
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                seasons = new[] { Seasons[seasonNum-1] };
                                break;
                            case 5:
                                seasons = SeasonsHot;
                                break;
                            default:
                                SundropCityMod.SMonitor.Log("Unable to resolve ("+seasonNum+") as season code from prefix: " + folder + '/' + Path.GetFileName(file), StardewModdingAPI.LogLevel.Warn);
                                continue;
                        }
                        foreach (string season in Seasons)
                        {
                            var cache = Parts[season];
                            switch(vars[0])
                            {
                                case 'f':
                                    for (int c = 0; c < rarity; c++)
                                        cache.Female[folder].Add(SundropCityMod.SHelper.Content.GetActualAssetKey(key));
                                    break;
                                case 'm':
                                    for (int c = 0; c < rarity; c++)
                                        cache.Male[folder].Add(SundropCityMod.SHelper.Content.GetActualAssetKey(key));
                                    break;
                                case 'a':
                                    for (int c = 0; c < rarity; c++)
                                    {
                                        cache.Female[folder].Add(SundropCityMod.SHelper.Content.GetActualAssetKey(key));
                                        cache.Male[folder].Add(SundropCityMod.SHelper.Content.GetActualAssetKey(key));
                                    }
                                    break;
                                case 'g':
                                    for (int c = 0; c < rarity; c++)
                                        cache.Girl[folder].Add(SundropCityMod.SHelper.Content.GetActualAssetKey(key));
                                    break;
                                case 'b':
                                    for (int c = 0; c < rarity; c++)
                                        cache.Boy[folder].Add(SundropCityMod.SHelper.Content.GetActualAssetKey(key));
                                    break;
                                case 'c':
                                    for (int c = 0; c < rarity; c++)
                                    {
                                        cache.Girl[folder].Add(SundropCityMod.SHelper.Content.GetActualAssetKey(key));
                                        cache.Boy[folder].Add(SundropCityMod.SHelper.Content.GetActualAssetKey(key));
                                    }
                                    break;
                            }
                        }
                        i++;
                    }
                }
                catch(Exception err)
                {
                    SundropCityMod.SMonitor.Log("Loading `" + folder +"` tourists parts failed due to error on index ("+i+").\n"+err, StardewModdingAPI.LogLevel.Error);
                }
            });
        }
        private static T Select<T>(List<T> parts)
        {
            return parts[Game1.random.Next(parts.Count)];
        }

        public Tourist(Vector2 position)
        {
            this.Speed = 2;
            this.Position = position;
            this.Name = "SundropTourist" + Guid.NewGuid();
            this.faceDirection(Game1.random.Next(4));
            this.Sprite = BlankSprite;
            this.RandomizeLook();
        }

        protected Tourist()
        {

        }

        internal void RandomizeLook()
        {
            var specials = Specials.Where(_ => !_.IsActive && (_.Season == null || _.Season == Game1.currentSeason) && Game1.random.NextDouble() < _.Chance);
            if (specials.Any())
            {
                var special = Select(specials.ToList());
                this.Special = special.Id;
                special.IsActive = true;
                Game1.showGlobalMessage(SundropCityMod.SHelper.Translation.Get("Tourist." + this.Special + ".Enter"));
                this.Base = new AnimatedSprite(SundropCityMod.SHelper.Content.GetActualAssetKey("assets/Characters/Tourists/Special/" + this.Special + ".png"), 0, 20, 34);
            }
            else
            {
                if (this.Special!=null)
                {
                    if (this.TickAge < short.MaxValue/2)
                        return;
                    var special = Specials.First(_ => _.Id.Equals(this.Special));
                    special.IsActive = false;
                    Game1.showGlobalMessage(SundropCityMod.SHelper.Translation.Get("Tourist." + this.Special + ".Leave"));
                    this.Special = null;
                }
                bool isFemale = Game1.random.NextDouble() < 0.5;
                bool isChild = false;// Game1.random.NextDouble() < 0.2;
                Dictionary<string, List<string>> parts = isFemale ? (isChild ? Parts[Game1.currentSeason].Girl : Parts[Game1.currentSeason].Female) : (isChild ? Parts[Game1.currentSeason].Boy : Parts[Game1.currentSeason].Male);
                this.Base = new AnimatedSprite(Select(parts["Body"]), 0, 20, 34);
                string shirt = Select(parts["Top"]);
                this.Shirt = new AnimatedSprite(shirt, 0, 20, 34);
                this.ShirtColor = PartVariables(shirt)[2] == 'c' ? Select(ClothingColors) : Color.White;
                string pants = Select(parts["Bottom"]);
                this.Pants = new AnimatedSprite(pants, 0, 20, 34);
                this.PantsColor = PartVariables(pants)[2] == 'c' ? Color.White : Select(ClothingColors);
                string shoes = Select(parts["Shoe"]);
                this.Shoes = new AnimatedSprite(shoes, 0, 20, 34);
                this.ShoeColor = PartVariables(shoes)[2] == 'c' ? Color.White : Select(ClothingColors);
                string hat = null;
                if (Game1.random.NextDouble() < 0.5)
                {
                    hat = Select(parts["Hat"]);
                    this.Hat = new AnimatedSprite(hat, 0, 20, 34);
                    this.HatColor = PartVariables(hat)[2] == 'c' || PartVariables(hat)[2] == 'm' ? Select(ClothingColors) : Color.White;
                }
                else
                    this.Hat = null;
                string hair = Select(parts["Hair"]);
                if (hat != null && PartVariables(hat)[2] != 's' && PartVariables(hat)[2] != 'm')
                {
                    string[] hairParts = Path.GetFileNameWithoutExtension(hair).Split('_');
                    hairParts[1] = hairParts[1].Replace('x', 's');
                    hair = hair.Replace(Path.GetFileNameWithoutExtension(hair), string.Join("_", hairParts));
                }
                this.Hair = new AnimatedSprite(hair, 0, 20, 34);
                this.HairColor = Select(HairColors);
                if (isFemale || Game1.random.NextDouble() < 0.334)
                {
                    string decoration = Select(parts["Decoration"]);
                    this.Decoration = new AnimatedSprite(decoration, 0, 20, 34);
                    this.DecorationColor = PartVariables(decoration)[2] == 'c' ? Select(ClothingColors) : Path.GetFileName(decoration)[2] == 's' ? this.HairColor : Color.White;
                }
                else
                    this.Decoration = null;
                if (Game1.random.NextDouble() < 0.334)
                {
                    string accessory = Select(parts["Accessory"]);
                    this.Accessory = new AnimatedSprite(accessory, 0, 20, 34);
                    this.AccessoryColor = PartVariables(accessory)[2] == 'c' ? Select(ClothingColors) : Color.White;
                }
                else
                    this.Accessory = null;
            }
        }

        private void DoWarp()
        {
            string name = this.currentLocation.Name;
            try
            {
                if (!WarpCache.ContainsKey(name))
                {
                    SundropCityMod.SMonitor.Log($"Tourst at {name} failed to warp, current location is not in warp cache.", StardewModdingAPI.LogLevel.Error);
                    return;
                }
                var validWarps = WarpCache[this.currentLocation.Name];
                if (validWarps.Count == 0)
                {
                    SundropCityMod.SMonitor.Log($"Tourst at {name} failed to warp, current location has no warp points.", StardewModdingAPI.LogLevel.Error);
                    return;
                }
                var target = validWarps[Game1.random.Next(validWarps.Count)];
                int dir = GetTileIndex(this.currentLocation, (int)target.X, (int)target.Y) - TILE_WARP_DOWN;
                if (dir > 3)
                {
                    SundropCityMod.SMonitor.Log($"Tourst at {name} failed to warp, facing direction out of bounds.", StardewModdingAPI.LogLevel.Error);
                    return;
                }
                this.RandomizeLook();
                this.Position = target * 64f;
                this.faceDirection(dir);
                this.setMovingInFacingDirection();
                this.Timer = 10;
                this.Delay = 0;
                this.Stuck = 0;
                this.TickAge = 0;
                this.Cooldown = byte.MaxValue;
            }
            catch(Exception err)
            {
                SundropCityMod.SMonitor.Log($"Tourst at {name} failed to warp, unhandled exception.\n{err}", StardewModdingAPI.LogLevel.Error);
            }
        }
        public override bool isColliding(GameLocation l, Vector2 tile)
        {
            return GetTileIndex(l, (int)tile.X, (int)tile.Y) == TILE_BLOCK;
        }
        public override void update(GameTime time, GameLocation location)
        {
            // Handle vanilla stuffs
            base.update(time, location);
            if (this.currentLocation.farmers.Count==0)
                return;
            if(!Utility.isOnScreen(this.Position, 8))
            {
                this.Skip++;
                if (this.Skip%3 > 0)
                    return;
            }
            // Set some default vars
            if(this.TickAge<short.MaxValue)
                this.TickAge++;
            var point = this.getTileLocationPoint();
            var backLayer = this.currentLocation.map.GetLayer("Back");
            int index = GetTileIndex(this.currentLocation, point.X, point.Y);
            // Check if we are in a keep moving region
            if (index == TILE_KEEPMOVING)
            {
                if (this.Timer % 2 == 1)
                    this.Timer--;
                else
                    this.Timer += 2;
                if (!this.isMoving())
                    this.Delay++;
                else
                    this.Delay = 0;
                this.setMovingInFacingDirection();
                if (this.Delay > 3)
                    if (!Utility.isOnScreen(this.Position, 8))
                    {
                        this.DoWarp();
                        return;
                    }
            }
            // If warp cooldown is active, decrement
            if (this.Cooldown > 0)
                this.Cooldown--;
            // Slows down update rate on the "Am I stuck?" check
            if (this.Timer > 0)
                this.Timer--;
            // When the timer is 0 do a stuck check
            else if (!this.isMoving() || Game1.random.NextDouble() <= 0.05)
            {
                // If the stuck check matches (Or the tourist has a random change of mind) set the timer
                this.Timer = 5;
                // And increase delay by 1
                this.Delay++;
                // If stuck, we add an extra bit of delay
                if (!this.isMoving())
                    this.Delay += 2;
            }
            // When within reach of a arrow tile prefer to follow it
            else if (index >= TILE_ARROW_DOWN && index <= TILE_ARROW_LEFT && Game1.random.NextDouble() < 0.1)
            {
                if (this.FacingDirection != index - TILE_ARROW_DOWN || Game1.random.NextDouble() < 0.25)
                {
                    this.Halt();
                    this.faceDirection(index - TILE_ARROW_DOWN);
                    this.setMovingInFacingDirection();
                    this.Timer = 10;
                    this.Delay = 0;
                }
            }
            // When within reach of a warp tile, see if we should warp
            else if(index >= TILE_WARP_DOWN && index <= TILE_WARP_LEFT && this.Cooldown == 0 || this.position.X < 0 || this.position.Y < 0 || this.position.Y > backLayer.DisplayHeight || this.position.X > backLayer.DisplayWidth)
                this.DoWarp();
            // If on a browse tile, maybe start browsing for a while?
            else if(index == TILE_BROWSE && Game1.random.NextDouble() < 0.25)
            {
                this.Halt();
                this.Timer = 120;
                this.Delay = 0;
            }
            // If nothing else needs to be done
            else
            {
                // Check if delay is bigger then 0, if so decrease by 3
                if (this.Delay > 0)
                    this.Delay -= 3;
                // Reset stuck counter
                this.Stuck = 0;
                // Set the timer
                this.Timer = 10;
            }
            // If delay is at 6 or more
            if (this.Delay > 6)
            {
                // Increment stuck counter
                this.Stuck++;
                // We set it to 0
                this.Delay = 0;
                // Set the timer
                this.Timer = 25;
                // And make the NPC get ready to move in a random direction
                this.Halt();
                int cur = this.FacingDirection;
                if (index >= TILE_ARROW_DOWN && index <= TILE_ARROW_LEFT && cur != index - TILE_ARROW_DOWN)
                    cur = index - TILE_ARROW_DOWN;
                else if (index >= TILE_WARP_DOWN && index <= TILE_WARP_LEFT && cur != index - TILE_WARP_DOWN)
                    cur = index - TILE_WARP_DOWN;
                else
                    cur = Game1.random.Next(4);
                this.faceDirection(cur);
                this.setMovingInFacingDirection();
            }
            // Check if stuck counter has reached 5 & this NPC is off screen
            if((this.Stuck>5 || this.TickAge==short.MaxValue) && !Utility.isOnScreen(this.Position, 8))
                this.DoWarp();
            // If this is the master game
            if (Game1.IsMasterGame && this.Base.CurrentAnimation == null)
                this.MovePosition(time, Game1.viewport, location);
            // We remember our position for the stuck check
            this.OldPos = this.Position;
        }
        public override bool hasSpecialCollisionRules()
        {
            return true;
        }
        public override Rectangle GetBoundingBox()
        {
            return new Rectangle((int)this.Position.X + 16, (int)this.Position.Y + 16, 32, 32);
        }

        public override void Halt()
        {
                this.moveUp = false;
                this.moveDown = false;
                this.moveRight = false;
                this.moveLeft = false;
                this.Speed = 2;
                this.addedSpeed = 0;
                this.Base?.StopAnimation();
                this.Decoration?.StopAnimation();
                this.Hat?.StopAnimation();
                this.Hair?.StopAnimation();
                this.Shirt?.StopAnimation();
                this.Pants?.StopAnimation();
                this.Shoes?.StopAnimation();
                this.Accessory?.StopAnimation();
        }

        public override void faceDirection(int direction)
        {
            if (direction == -3)
                return;
            this.FacingDirection = direction;
            this.Base?.faceDirection(direction);
                this.Decoration?.faceDirection(direction);
                this.Hat?.faceDirection(direction);
                this.Hair?.faceDirection(direction);
                this.Shirt?.faceDirection(direction);
                this.Pants?.faceDirection(direction);
                this.Shoes?.faceDirection(direction);
                this.Accessory?.faceDirection(direction);
        }
        protected void AnimateDown(GameTime time, int interval, string sound)
        {
            this.Base?.AnimateDown(time, interval, sound);
                this.Decoration?.AnimateDown(time, interval, sound);
                this.Hat?.AnimateDown(time, interval, sound);
                this.Hair?.AnimateDown(time, interval, sound);
                this.Shirt?.AnimateDown(time, interval, sound);
                this.Pants?.AnimateDown(time, interval, sound);
                this.Shoes?.AnimateDown(time, interval, sound);
                this.Accessory?.AnimateDown(time, interval, sound);
            }
        protected void AnimateUp(GameTime time, int interval, string sound)
        {
            this.Base?.AnimateUp(time, interval, sound);
                this.Decoration?.AnimateUp(time, interval, sound);
                this.Hat?.AnimateUp(time, interval, sound);
                this.Hair?.AnimateUp(time, interval, sound);
                this.Shirt?.AnimateUp(time, interval, sound);
                this.Pants?.AnimateUp(time, interval, sound);
                this.Shoes?.AnimateUp(time, interval, sound);
                this.Accessory?.AnimateUp(time, interval, sound);
            }
        protected void AnimateLeft(GameTime time, int interval, string sound)
        {
            this.Base?.AnimateLeft(time, interval, sound);
                this.Decoration?.AnimateLeft(time, interval, sound);
                this.Hat?.AnimateLeft(time, interval, sound);
                this.Hair?.AnimateLeft(time, interval, sound);
                this.Shirt?.AnimateLeft(time, interval, sound);
                this.Pants?.AnimateLeft(time, interval, sound);
                this.Shoes?.AnimateLeft(time, interval, sound);
                this.Accessory?.AnimateLeft(time, interval, sound);
            }

        protected void AnimateRight(GameTime time, int interval, string sound)
        {
            this.Base?.AnimateRight(time, interval, sound);
                this.Decoration?.AnimateRight(time, interval, sound);
                this.Hat?.AnimateRight(time, interval, sound);
                this.Hair?.AnimateRight(time, interval, sound);
                this.Shirt?.AnimateRight(time, interval, sound);
                this.Pants?.AnimateRight(time, interval, sound);
                this.Shoes?.AnimateRight(time, interval, sound);
                this.Accessory?.AnimateRight(time, interval, sound);
            }

        protected void AnimateOnce(GameTime time)
        {
            this.Base?.animateOnce(time);
                this.Decoration?.animateOnce(time);
                this.Hat?.animateOnce(time);
                this.Hair?.animateOnce(time);
                this.Shirt?.animateOnce(time);
                this.Pants?.animateOnce(time);
                this.Shoes?.animateOnce(time);
                this.Accessory?.animateOnce(time);
            }

        public override void MovePosition(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation curLocation)
        {
            if (this.xVelocity != 0f || this.yVelocity != 0f)
                this.applyVelocity(this.currentLocation);
            else if (this.moveUp)
            {
                if (curLocation == null || this.isCharging || !curLocation.isCollidingPosition(this.nextPosition(0), viewport, false, 0, false, this))
                {
                    this.position.Y -= this.speed + this.addedSpeed;
                    if (!this.ignoreMovementAnimation)
                    {
                        this.AnimateUp(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, curLocation) ? "Cowboy_Footstep" : "");
                        this.faceDirection(0);
                    }
                }
                else if (!curLocation.isTilePassable(this.nextPosition(0), viewport))
                    this.Halt();
            }
            else if (this.moveRight)
            {
                if (curLocation == null || this.isCharging || !curLocation.isCollidingPosition(this.nextPosition(1), viewport, false, 0, false, this))
                {
                    this.position.X += (this.speed + this.addedSpeed);
                    if (!this.ignoreMovementAnimation)
                    {
                        this.AnimateRight(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, curLocation) ? "Cowboy_Footstep" : "");
                        this.faceDirection(1);
                    }
                }
                else if (!curLocation.isTilePassable(this.nextPosition(1), viewport))
                    this.Halt();
            }
            else if (this.moveDown)
            {
                if (curLocation == null || this.isCharging || !curLocation.isCollidingPosition(this.nextPosition(2), viewport, false, 0, false, this))
                {
                    this.position.Y += (this.speed + this.addedSpeed);
                    if (!this.ignoreMovementAnimation)
                    {
                        this.AnimateDown(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, curLocation) ? "Cowboy_Footstep" : "");
                        this.faceDirection(2);
                    }
                }
                else if (!curLocation.isTilePassable(this.nextPosition(2), viewport))
                    this.Halt();
            }
            else if (this.moveLeft)
            {
                if (curLocation == null || this.isCharging || !curLocation.isCollidingPosition(this.nextPosition(3), viewport, false, 0, false, this))
                {
                    this.position.X -= (this.speed + this.addedSpeed);
                    if (!this.ignoreMovementAnimation)
                    {
                        this.AnimateLeft(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, curLocation) ? "Cowboy_Footstep" : "");
                        this.faceDirection(3);
                    }
                }
                else if (!curLocation.isTilePassable(this.nextPosition(3), viewport))
                    this.Halt();
            }
            else
                this.AnimateOnce(time);
            if (this.blockedInterval >= 3000 && this.blockedInterval <= 3750f && !Game1.eventUp)
            {
                this.doEmote(Game1.random.NextDouble() < 0.5 ? 8 : 40);
                this.blockedInterval = 3750;
            }
            else if (this.blockedInterval >= 5000)
            {
                this.speed = 4;
                this.isCharging = true;
                this.blockedInterval = 0;
            }
        }

        public override void draw(SpriteBatch b, float alpha = 1)
        {
            if (!Utility.isOnScreen(this.Position, 128))
                return;
            float depth = Math.Max(0f, this.drawOnTop ? 0.991f : this.getStandingY() / 10000f);
            var positionVector = this.getLocalPosition(Game1.viewport) + new Vector2(this.Base.SpriteWidth * 2, this.GetBoundingBox().Height / 2f) + ((this.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero);
            var originVector = new Vector2(this.Base.SpriteWidth / 2f + 2, this.Base.SpriteHeight * 0.75f);
            float scaleFloat = Math.Max(0.2f, this.Scale) * 4f;
            var spriteEffects = this.flip || this.Base.CurrentAnimation != null && this.Base.CurrentAnimation[this.Base.currentAnimationIndex].flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            b.Draw(this.Base.Texture, positionVector, this.Base.SourceRect, Color.White * alpha, this.rotation, originVector, scaleFloat, spriteEffects, depth);
            if (this.Special!=null)
                return;
            if (this.Decoration != null)
                b.Draw(this.Decoration.Texture, positionVector, this.Base.SourceRect, this.DecorationColor * alpha, this.rotation, originVector, scaleFloat, spriteEffects, depth + 0.00001f);
            b.Draw(this.Shirt.Texture, positionVector, this.Base.SourceRect, this.ShirtColor * alpha, this.rotation, originVector, scaleFloat, spriteEffects, depth + 0.00002f);
            b.Draw(this.Pants.Texture, positionVector, this.Base.SourceRect, this.PantsColor * alpha, this.rotation, originVector, scaleFloat, spriteEffects, depth + 0.00003f);
            b.Draw(this.Shoes.Texture, positionVector, this.Base.SourceRect, this.ShoeColor * alpha, this.rotation, originVector, scaleFloat, spriteEffects, depth + 0.00004f);
            if (this.Accessory != null)
                b.Draw(this.Accessory.Texture, positionVector, this.Base.SourceRect, this.AccessoryColor * alpha, this.rotation, originVector, scaleFloat, spriteEffects, depth + 0.00005f);
            b.Draw(this.Hair.Texture, positionVector, this.Base.SourceRect, this.HairColor * alpha, this.rotation, originVector, scaleFloat, spriteEffects, depth + 0.00006f);
            if (this.Hat != null)
                b.Draw(this.Hat.Texture, positionVector, this.Base.SourceRect, this.HatColor * alpha, this.rotation, originVector, scaleFloat, spriteEffects, depth + 0.00007f);
        }

        public override bool checkAction(Farmer who, GameLocation l)
        {
            if (this.textAboveHeadTimer > 0)
                return false;
            if(this.Special==null)
            {
                this.showTextAboveHead(SundropCityMod.SHelper.Translation.Get("Tourist.Lines." + Game1.random.Next(TOURIST_LINE_COUNT)));
                return true;
            }
            var special = Specials.First(_ => _.Id.Equals(this.Special));
            if (special.Trigger?.Invoke(who) == true)
                return true;
            this.showTextAboveHead(SundropCityMod.SHelper.Translation.Get("Tourist." + this.Special + "." + Game1.random.Next(special.Lines)));
            return true;
        }
    }
}