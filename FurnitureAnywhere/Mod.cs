using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Objects;

using Entoarox.Framework;
using Entoarox.Framework.Extensions;
using Entoarox.Framework.Events;


using Microsoft.Xna.Framework;

namespace Entoarox.FurnitureAnywhere
{
    public class FurnitureAnywhereMod : Mod
    {
        public override void Entry(IModHelper helper)
        {
            VersionChecker.AddCheck("FurnitureAnywhere", typeof(FurnitureAnywhereMod).Assembly.GetName().Version, "https://raw.githubusercontent.com/Entoarox/StardewMods/master/VersionChecker/FurnitureAnywhere.json");
            MoreEvents.ActiveItemChanged += MoreEvents_ActiveItemChanged;
            LocationEvents.CurrentLocationChanged += TriggerItemChangedEvent;
            MenuEvents.MenuChanged += TriggerItemChangedEvent;
            MenuEvents.MenuClosed += TriggerItemChangedEvent;
            EntoFramework.GetTypeRegistry().RegisterType<AnywhereFurniture>();
        }
        private void RestoreVanillaObjects()
        {
            for (int c = 0; c < Game1.player.items.Count; c++)
                if (Game1.player.items[c] != null && Game1.player.items[c] is AnywhereFurniture)
                    Game1.player.items[c] = (Game1.player.items[c] as AnywhereFurniture).Revert();
        }
        private void InitSpecialObject(Item i)
        {
            for (int c = 0; c < Game1.player.items.Count; c++)
                if (Game1.player.items[c] != null && Game1.player.items[c].Equals(i))
                    Game1.player.items[c] = new AnywhereFurniture(Game1.player.items[c] as Furniture);
        }
        internal void MoreEvents_ActiveItemChanged(object s, EventArgsActiveItemChanged e)
        {
            try
            {
                if (e.OldItem != null && e.OldItem is AnywhereFurniture)
                    RestoreVanillaObjects();
                if (e.NewItem != null && e.NewItem is Furniture && !(Game1.currentLocation is StardewValley.Locations.DecoratableLocation) && Game1.activeClickableMenu==null)
                    InitSpecialObject(e.NewItem);
            }
            catch(Exception err)
            {
                Monitor.Log(LogLevel.Error,"Failed to run logic check due to unexpected error", err);
            }
        }
        internal void TriggerItemChangedEvent(object s, EventArgs e)
        {
            MoreEvents_ActiveItemChanged(null, new EventArgsActiveItemChanged(Game1.player.CurrentItem, Game1.player.CurrentItem));
        }
    }
    public class AnywhereFurniture : Furniture
    {
        public AnywhereFurniture()
        {

        }
        public AnywhereFurniture(Furniture item) : base(item.parentSheetIndex,item.tileLocation)
        {
            defaultBoundingBox = item.defaultBoundingBox;
            boundingBox = item.boundingBox;
            currentRotation = item.currentRotation;
            rotations = item.rotations;
            rotate();
            rotate();
            rotate();
            rotate();
        }
        public Furniture Revert()
        {
            Furniture self=new Furniture(parentSheetIndex, tileLocation);
            self.defaultBoundingBox = defaultBoundingBox;
            self.boundingBox = boundingBox;
            self.currentRotation = currentRotation;
            self.rotations = rotations;
            self.rotate();
            self.rotate();
            self.rotate();
            self.rotate();
            return self;
        }
        public override bool isPassable()
        {
            return furniture_type == 12;
        }
        public override string getCategoryName()
        {
            return "FurnitureAnywhere";
        }
        public override bool performObjectDropInAction(StardewValley.Object dropIn, bool probe, StardewValley.Farmer who)
        {
            return false;
        }
        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            for (int index1 = 0; index1 < boundingBox.Width / Game1.tileSize; ++index1)
            {
                for (int index2 = 0; index2 < boundingBox.Height / Game1.tileSize; ++index2)
                {
                    Vector2 key = tile + new Vector2(index1, index2);
                    if (l.Objects.ContainsKey(key))
                    {
                        if(l.objects[key] is Furniture)
                        {
                            Vector2 vector2 = key * Game1.tileSize - new Vector2(Game1.tileSize / 2);
                            Furniture furniture = (Furniture)l.objects[key];
                            if (furniture.furniture_type == 11 && (furniture.getBoundingBox(furniture.tileLocation).Contains((int)vector2.X, (int)vector2.Y) && furniture.heldObject == null && getTilesWide() == 1))
                                return true;
                            if ((furniture.furniture_type != 12 || furniture_type == 12) && furniture.getBoundingBox(furniture.tileLocation).Contains((int)vector2.X, (int)vector2.Y))
                                return false;
                        }
                        return false;
                    }
                }
            }
            if (parentSheetIndex == 710 && l.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Water", "Back") != null && (!l.objects.ContainsKey(tile) && l.doesTileHaveProperty((int)tile.X + 1, (int)tile.Y, "Water", "Back") != null) && l.doesTileHaveProperty((int)tile.X - 1, (int)tile.Y, "Water", "Back") != null || l.doesTileHaveProperty((int)tile.X, (int)tile.Y + 1, "Water", "Back") != null && l.doesTileHaveProperty((int)tile.X, (int)tile.Y - 1, "Water", "Back") != null || (parentSheetIndex == 105 && bigCraftable && (l.terrainFeatures.ContainsKey(tile) && l.terrainFeatures[tile] is StardewValley.TerrainFeatures.Tree) && !l.objects.ContainsKey(tile) || name != null && name.Contains("Bomb") && (!l.isTileOccupiedForPlacement(tile, this) || l.isTileOccupiedByFarmer(tile) != null)))
                return true;
            return !l.isTileOccupiedForPlacement(tile, this);
        }
        public override bool placementAction(GameLocation location, int x, int y, StardewValley.Farmer who = null)
        {
            Point point = new Point(x / Game1.tileSize, y / Game1.tileSize);
            tileLocation = new Vector2(point.X, point.Y);
            if (furniture_type == 6 || furniture_type == 13 || parentSheetIndex == 1293)
            {
                Game1.showRedMessage("Can only be placed in House");
                return false;
            }
            for (int index1 = point.X; index1 < point.X + getTilesWide(); ++index1)
            {
                for (int index2 = point.Y; index2 < point.Y + getTilesHigh(); ++index2)
                {
                    if (location.doesTileHaveProperty(index1, index2, "NoFurniture", "Back") != null)
                    {
                        Game1.showRedMessage("Furniture can't be placed here");
                        return false;
                    }
                    if (location.getTileIndexAt(index1, index2, "Buildings") != -1)
                        return false;
                }
            }
            boundingBox = new Rectangle(x / Game1.tileSize * Game1.tileSize, y / Game1.tileSize * Game1.tileSize, boundingBox.Width, boundingBox.Height);
            foreach (Character character in location.getFarmers())
            {
                if (character.GetBoundingBox().Intersects(boundingBox))
                {
                    Game1.showRedMessage("Can't place on top of a person.");
                    return false;
                }
            }
            foreach (KeyValuePair<Vector2,StardewValley.Object> i in location.objects)
            {
                if (i.Value is Furniture)
                {
                    Furniture furniture = (Furniture)i.Value;
                    if (furniture.getBoundingBox(furniture.tileLocation).Intersects(boundingBox))
                    {
                        Game1.showRedMessage("Furniture can't be placed here");
                        return false;
                    }
                }
            }
            updateDrawPosition();
            if (!performDropDownAction(who))
            {
                StardewValley.Object @object = (StardewValley.Object)getOne();
                @object.shakeTimer = 50;
                @object.tileLocation = tileLocation;
                @object.performDropDownAction(who);
                if (location.objects.ContainsKey(tileLocation))
                {
                    if (location.objects[tileLocation].ParentSheetIndex != parentSheetIndex)
                    {
                        Game1.createItemDebris(location.objects[tileLocation], tileLocation * Game1.tileSize, Game1.random.Next(4), null);
                        location.objects[tileLocation] = @object;
                    }
                }
                else
                    location.objects.Add(tileLocation, @object);
                @object.initializeLightSource(tileLocation);
            }
            Game1.playSound("woodyStep");
            return true;
        }
        public override Item getOne()
        {
            AnywhereFurniture furniture = new AnywhereFurniture(this);
            furniture.drawPosition = drawPosition;
            furniture.defaultBoundingBox = defaultBoundingBox;
            furniture.boundingBox = boundingBox;
            furniture.currentRotation = currentRotation;
            furniture.rotations = rotations;
            furniture.rotate();
            furniture.rotate();
            furniture.rotate();
            furniture.rotate();
            return furniture;
        }
        public override bool clicked(StardewValley.Farmer who)
        {
            Game1.haltAfterCheck = false;
            if (furniture_type == 11 && who.ActiveObject != null && (who.ActiveObject != null && heldObject == null))
                return false;
            if (heldObject == null && (who.ActiveObject == null || !(who.ActiveObject is Furniture)))
                return who.addItemToInventoryBool(Revert());
            if (heldObject == null || !who.addItemToInventoryBool(heldObject))
                return false;
            heldObject.performRemoveAction(tileLocation, who.currentLocation);
            heldObject = null;
            Game1.playSound("coin");
            return true;
        }
    }
}
