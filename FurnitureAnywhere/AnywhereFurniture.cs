using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;

namespace Entoarox.FurnitureAnywhere
{
    public class AnywhereFurniture : Furniture
    {
        public AnywhereFurniture()
        {

        }
        public AnywhereFurniture(Furniture item) : base(item.parentSheetIndex, item.tileLocation)
        {
            this.defaultBoundingBox = item.defaultBoundingBox;
            this.boundingBox = item.boundingBox;
            this.currentRotation = item.currentRotation;
            this.rotations = item.rotations;
            this.rotate();
            this.rotate();
            this.rotate();
            this.rotate();
        }
        public Furniture Revert()
        {
            Furniture self = new Furniture(this.parentSheetIndex, this.tileLocation);
            self.defaultBoundingBox = this.defaultBoundingBox;
            self.boundingBox = this.boundingBox;
            self.currentRotation = this.currentRotation;
            self.rotations = this.rotations;
            self.rotate();
            self.rotate();
            self.rotate();
            self.rotate();
            return self;
        }
        public override bool isPassable()
        {
            return this.furniture_type == 12;
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
            for (int index1 = 0; index1 < this.boundingBox.Width / Game1.tileSize; ++index1)
            {
                for (int index2 = 0; index2 < this.boundingBox.Height / Game1.tileSize; ++index2)
                {
                    Vector2 key = tile + new Vector2(index1, index2);
                    if (l.Objects.ContainsKey(key))
                    {
                        if (l.objects[key] is Furniture)
                        {
                            Vector2 vector2 = key * Game1.tileSize - new Vector2(Game1.tileSize / 2);
                            Furniture furniture = (Furniture)l.objects[key];
                            if (furniture.furniture_type == 11 && (furniture.getBoundingBox(furniture.tileLocation).Contains((int)vector2.X, (int)vector2.Y) && furniture.heldObject == null && this.getTilesWide() == 1))
                                return true;
                            if ((furniture.furniture_type != 12 || this.furniture_type == 12) && furniture.getBoundingBox(furniture.tileLocation).Contains((int)vector2.X, (int)vector2.Y))
                                return false;
                        }
                        return false;
                    }
                }
            }
            if (this.parentSheetIndex == 710 && l.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Water", "Back") != null && (!l.objects.ContainsKey(tile) && l.doesTileHaveProperty((int)tile.X + 1, (int)tile.Y, "Water", "Back") != null) && l.doesTileHaveProperty((int)tile.X - 1, (int)tile.Y, "Water", "Back") != null || l.doesTileHaveProperty((int)tile.X, (int)tile.Y + 1, "Water", "Back") != null && l.doesTileHaveProperty((int)tile.X, (int)tile.Y - 1, "Water", "Back") != null || (this.parentSheetIndex == 105 && this.bigCraftable && (l.terrainFeatures.ContainsKey(tile) && l.terrainFeatures[tile] is StardewValley.TerrainFeatures.Tree) && !l.objects.ContainsKey(tile) || this.name != null && this.name.Contains("Bomb") && (!l.isTileOccupiedForPlacement(tile, this) || l.isTileOccupiedByFarmer(tile) != null)))
                return true;
            return !l.isTileOccupiedForPlacement(tile, this);
        }
        public override bool placementAction(GameLocation location, int x, int y, StardewValley.Farmer who = null)
        {
            Point point = new Point(x / Game1.tileSize, y / Game1.tileSize);
            this.tileLocation = new Vector2(point.X, point.Y);
            if (this.furniture_type == 6 || this.furniture_type == 13 || this.parentSheetIndex == 1293)
            {
                Game1.showRedMessage("Can only be placed in House");
                return false;
            }
            for (int index1 = point.X; index1 < point.X + this.getTilesWide(); ++index1)
            {
                for (int index2 = point.Y; index2 < point.Y + this.getTilesHigh(); ++index2)
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
            this.boundingBox = new Rectangle(x / Game1.tileSize * Game1.tileSize, y / Game1.tileSize * Game1.tileSize, this.boundingBox.Width, this.boundingBox.Height);
            foreach (Character character in location.getFarmers())
            {
                if (character.GetBoundingBox().Intersects(this.boundingBox))
                {
                    Game1.showRedMessage("Can't place on top of a person.");
                    return false;
                }
            }
            foreach (KeyValuePair<Vector2, StardewValley.Object> i in location.objects)
            {
                if (i.Value is Furniture)
                {
                    Furniture furniture = (Furniture)i.Value;
                    if (furniture.getBoundingBox(furniture.tileLocation).Intersects(this.boundingBox))
                    {
                        Game1.showRedMessage("Furniture can't be placed here");
                        return false;
                    }
                }
            }
            this.updateDrawPosition();
            if (!this.performDropDownAction(who))
            {
                StardewValley.Object @object = (StardewValley.Object)this.getOne();
                @object.shakeTimer = 50;
                @object.tileLocation = this.tileLocation;
                @object.performDropDownAction(who);
                if (location.objects.ContainsKey(this.tileLocation))
                {
                    if (location.objects[this.tileLocation].ParentSheetIndex != this.parentSheetIndex)
                    {
                        Game1.createItemDebris(location.objects[this.tileLocation], this.tileLocation * Game1.tileSize, Game1.random.Next(4), null);
                        location.objects[this.tileLocation] = @object;
                    }
                }
                else
                    location.objects.Add(this.tileLocation, @object);
                @object.initializeLightSource(this.tileLocation);
            }
            Game1.playSound("woodyStep");
            return true;
        }
        public override Item getOne()
        {
            AnywhereFurniture furniture = new AnywhereFurniture(this);
            furniture.drawPosition = this.drawPosition;
            furniture.defaultBoundingBox = this.defaultBoundingBox;
            furniture.boundingBox = this.boundingBox;
            furniture.currentRotation = this.currentRotation;
            furniture.rotations = this.rotations;
            furniture.rotate();
            furniture.rotate();
            furniture.rotate();
            furniture.rotate();
            return furniture;
        }
        public override bool clicked(StardewValley.Farmer who)
        {
            Game1.haltAfterCheck = false;
            if (this.furniture_type == 11 && who.ActiveObject != null && (who.ActiveObject != null && this.heldObject == null))
                return false;
            if (this.heldObject == null && (who.ActiveObject == null || !(who.ActiveObject is Furniture)))
                return who.addItemToInventoryBool(this.Revert());
            if (this.heldObject == null || !who.addItemToInventoryBool(this.heldObject))
                return false;
            this.heldObject.performRemoveAction(this.tileLocation, who.currentLocation);
            this.heldObject = null;
            Game1.playSound("coin");
            return true;
        }
    }
}