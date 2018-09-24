using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Entoarox.AdvancedLocationLoader.Menus
{
    internal class CarpenterMenu : IClickableMenu
    {
        private ClickableTextureComponent backButton;
        private readonly List<BluePrint> blueprints;
        private readonly List<string> BuildableLocations = new List<string>();
        private string buildingDescription;
        private string buildingName;
        private Building buildingToMove;
        private ClickableTextureComponent cancelButton;
        private int currentBlueprintIndex;
        private Building currentBuilding;
        private ClickableTextureComponent demolishButton;
        private bool demolishing;
        private bool drawBG = true;
        private ClickableTextureComponent forwardButton;
        private bool freeze;
        private string hoverText = "";
        private readonly List<Item> ingredients = new List<Item>();
        private readonly bool magicalConstruction;
        public int maxHeightOfBuildingViewer = 8 * Game1.tileSize;
        public int maxWidthOfBuildingViewer = 7 * Game1.tileSize;
        public int maxWidthOfDescription = 6 * Game1.tileSize;
        private ClickableTextureComponent moveButton;
        private bool moving;
        private ClickableTextureComponent okButton;
        private int price;
        private string TargetLocation;
        private ClickableTextureComponent upgradeIcon;
        private bool upgrading;

        public CarpenterMenu(bool magicalConstruction = false)
        {
            this.magicalConstruction = magicalConstruction;
            Game1.player.forceCanMove();
            this.resetBounds();
            this.blueprints = new List<BluePrint>();
            if (magicalConstruction)
            {
                this.blueprints.Add(new BluePrint("Junimo Hut"));
                this.blueprints.Add(new BluePrint("Earth Obelisk"));
                this.blueprints.Add(new BluePrint("Water Obelisk"));
                this.blueprints.Add(new BluePrint("Gold Clock"));
            }
            else
            {
                this.blueprints.Add(new BluePrint("Coop"));
                this.blueprints.Add(new BluePrint("Barn"));
                this.blueprints.Add(new BluePrint("Well"));
                this.blueprints.Add(new BluePrint("Silo"));
                this.blueprints.Add(new BluePrint("Mill"));
                this.blueprints.Add(new BluePrint("Shed"));
                bool bigCoop = false;
                bool delCoop = false;
                bool bigBarn = false;
                bool delBarn = false;
                bool stable = true;
                foreach (GameLocation l in Game1.locations)
                    if (l is BuildableGameLocation)
                    {
                        BuildableGameLocation bl = (BuildableGameLocation)l;
                        if (bl.isBuildingConstructed("Coop"))
                            bigCoop = true;
                        if (bl.isBuildingConstructed("Big Coop"))
                            delCoop = true;
                        if (bl.isBuildingConstructed("Barn"))
                            bigBarn = true;
                        if (bl.isBuildingConstructed("Big Barn"))
                            delBarn = true;
                        if (bl.isBuildingConstructed("Stable"))
                            stable = false;
                        this.BuildableLocations.Add(l.Name);
                    }

                if (stable)
                    this.blueprints.Add(new BluePrint("Stable"));
                this.blueprints.Add(new BluePrint("Slime Hutch"));
                if (bigCoop)
                    this.blueprints.Add(new BluePrint("Big Coop"));
                if (delCoop)
                    this.blueprints.Add(new BluePrint("Deluxe Coop"));
                if (bigBarn)
                    this.blueprints.Add(new BluePrint("Big Barn"));
                if (delBarn)
                    this.blueprints.Add(new BluePrint("Deluxe Barn"));
            }

            this.setNewActiveBlueprint();
        }

        public BluePrint CurrentBlueprint => this.blueprints[this.currentBlueprintIndex];

        private void resetBounds()
        {
            this.xPositionOnScreen = Game1.viewport.Width / 2 - this.maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - this.maxHeightOfBuildingViewer / 2 - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 2;
            this.width = this.maxWidthOfBuildingViewer + this.maxWidthOfDescription + IClickableMenu.spaceToClearSideBorder * 2 + Game1.tileSize;
            this.height = this.maxHeightOfBuildingViewer + IClickableMenu.spaceToClearTopBorder;
            this.initialize(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, true);
            this.okButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize * 3 - Game1.pixelZoom * 3, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, new Rectangle(366, 373, 16, 16), Game1.pixelZoom, false);
            this.cancelButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47, -1, -1), 1f, false);
            this.backButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), Game1.pixelZoom, false);
            this.forwardButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - Game1.tileSize * 4 + Game1.tileSize / 4, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), Game1.pixelZoom, false);
            this.demolishButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_Demolish"), new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize * 2 - Game1.pixelZoom * 2, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize - Game1.pixelZoom, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, new Rectangle(348, 372, 17, 17), Game1.pixelZoom, false);
            this.upgradeIcon = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - Game1.tileSize * 2 + Game1.tileSize / 2, this.yPositionOnScreen + Game1.pixelZoom * 2, 9 * Game1.pixelZoom, 13 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(402, 328, 9, 13), Game1.pixelZoom, false);
            this.moveButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings"), new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize * 4 - Game1.pixelZoom * 5, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, new Rectangle(257, 284, 16, 16), Game1.pixelZoom, false);
        }

        public void setNewActiveBlueprint()
        {
            this.currentBuilding = !this.blueprints[this.currentBlueprintIndex].name.Contains("Coop") ? !this.blueprints[this.currentBlueprintIndex].name.Contains("Barn") ? !this.blueprints[this.currentBlueprintIndex].name.Contains("Mill") ? !this.blueprints[this.currentBlueprintIndex].name.Contains("Junimo Hut") ? new Building(this.blueprints[this.currentBlueprintIndex], Vector2.Zero) : new JunimoHut(this.blueprints[this.currentBlueprintIndex], Vector2.Zero) : new Mill(this.blueprints[this.currentBlueprintIndex], Vector2.Zero) : new Barn(this.blueprints[this.currentBlueprintIndex], Vector2.Zero) : new Coop(this.blueprints[this.currentBlueprintIndex], Vector2.Zero);
            this.price = this.blueprints[this.currentBlueprintIndex].moneyRequired;
            this.ingredients.Clear();
            foreach (KeyValuePair<int, int> keyValuePair in this.blueprints[this.currentBlueprintIndex].itemsRequired)
                this.ingredients.Add(new Object(keyValuePair.Key, keyValuePair.Value, false, -1, 0));
            this.buildingDescription = this.blueprints[this.currentBlueprintIndex].description;
            this.buildingName = this.blueprints[this.currentBlueprintIndex].name;
        }

        public override void performHoverAction(int x, int y)
        {
            this.cancelButton.tryHover(x, y, 0.1f);
            base.performHoverAction(x, y);
            if (string.IsNullOrEmpty(this.TargetLocation))
            {
                this.backButton.tryHover(x, y, 1f);
                this.forwardButton.tryHover(x, y, 1f);
                this.okButton.tryHover(x, y, 0.1f);
                this.demolishButton.tryHover(x, y, 0.1f);
                this.moveButton.tryHover(x, y, 0.1f);
                if (this.CurrentBlueprint.isUpgrade() && this.upgradeIcon.containsPoint(x, y))
                    this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Upgrade", this.CurrentBlueprint.nameOfBuildingToUpgrade);
                else if (this.demolishButton.containsPoint(x, y))
                    this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Demolish");
                else if (this.moveButton.containsPoint(x, y))
                    this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings");
                else if (this.okButton.containsPoint(x, y) && this.CurrentBlueprint.doesFarmerHaveEnoughResourcesToBuild())
                    this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Build");
                else
                    this.hoverText = "";
            }
            else
            {
                if (!this.upgrading && !this.demolishing && !this.moving || this.freeze)
                    return;
                foreach (Building building in ((BuildableGameLocation)Game1.getLocationFromName(this.TargetLocation)).buildings)
                    building.color.Value = Color.White;
                Building building1 = ((BuildableGameLocation)Game1.getLocationFromName(this.TargetLocation)).getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize)) ?? ((BuildableGameLocation)Game1.getLocationFromName(this.TargetLocation)).getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY() + Game1.tileSize * 2) / Game1.tileSize)) ?? ((BuildableGameLocation)Game1.getLocationFromName(this.TargetLocation)).getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY() + Game1.tileSize * 3) / Game1.tileSize));
                if (this.upgrading)
                {
                    if (building1 != null && this.CurrentBlueprint.nameOfBuildingToUpgrade != null && this.CurrentBlueprint.nameOfBuildingToUpgrade.Equals(building1.buildingType.Value))
                        building1.color.Value = Color.Lime * 0.8f;
                    else
                    {
                        if (building1 == null)
                            return;
                        building1.color.Value = Color.Red * 0.8f;
                    }
                }
                else if (this.demolishing)
                {
                    if (building1 == null)
                        return;
                    building1.color.Value = Color.Red * 0.8f;
                }
                else
                {
                    if (!this.moving || building1 == null)
                        return;
                    building1.color.Value = Color.Lime * 0.8f;
                }
            }
        }

        public override bool readyToClose()
        {
            if (base.readyToClose())
                return this.buildingToMove == null;
            return false;
        }

        public override void receiveKeyPress(Keys key)
        {
            if (this.freeze)
                return;
            if (string.IsNullOrEmpty(this.TargetLocation))
                base.receiveKeyPress(key);
            if (Game1.globalFade || string.IsNullOrEmpty(this.TargetLocation))
                return;
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
                Game1.globalFadeToBlack(this.returnToCarpentryMenu, 0.02f);
            else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                Game1.panScreen(0, 4);
            else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                Game1.panScreen(4, 0);
            else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
                Game1.panScreen(0, -4);
            else
            {
                if (!Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                    return;
                Game1.panScreen(-4, 0);
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (string.IsNullOrEmpty(this.TargetLocation) || Game1.globalFade)
                return;
            int num1 = Game1.getOldMouseX() + Game1.viewport.X;
            int num2 = Game1.getOldMouseY() + Game1.viewport.Y;
            if (num1 - Game1.viewport.X < Game1.tileSize)
                Game1.panScreen(-8, 0);
            else if (num1 - (Game1.viewport.X + Game1.viewport.Width) >= -Game1.tileSize * 2)
                Game1.panScreen(8, 0);
            if (num2 - Game1.viewport.Y < Game1.tileSize)
                Game1.panScreen(0, -8);
            else if (num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -Game1.tileSize)
                Game1.panScreen(0, 8);
            foreach (Keys key in Game1.oldKBState.GetPressedKeys())
                this.receiveKeyPress(key);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.freeze)
                return;
            if (string.IsNullOrEmpty(this.TargetLocation))
                base.receiveLeftClick(x, y, playSound);
            if (this.cancelButton.containsPoint(x, y))
            {
                if (string.IsNullOrEmpty(this.TargetLocation))
                {
                    this.exitThisMenu(true);
                    Game1.player.forceCanMove();
                    Game1.playSound("bigDeSelect");
                }
                else
                {
                    if (this.moving && this.buildingToMove != null)
                    {
                        Game1.playSound("cancel");
                        return;
                    }

                    Game1.globalFadeToBlack(this.returnToCarpentryMenu, 0.02f);
                    Game1.playSound("smallSelect");
                    return;
                }
            }

            if (string.IsNullOrEmpty(this.TargetLocation) && this.backButton.containsPoint(x, y))
            {
                this.currentBlueprintIndex = this.currentBlueprintIndex - 1;
                if (this.currentBlueprintIndex < 0)
                    this.currentBlueprintIndex = this.blueprints.Count - 1;
                this.setNewActiveBlueprint();
                Game1.playSound("shwip");
                this.backButton.scale = this.backButton.baseScale;
            }

            if (string.IsNullOrEmpty(this.TargetLocation) && this.forwardButton.containsPoint(x, y))
            {
                this.currentBlueprintIndex = (this.currentBlueprintIndex + 1) % this.blueprints.Count;
                this.setNewActiveBlueprint();
                this.backButton.scale = this.backButton.baseScale;
                Game1.playSound("shwip");
            }

            if (string.IsNullOrEmpty(this.TargetLocation) && this.demolishButton.containsPoint(x, y))
            {
                Game1.globalFadeToBlack(this.setUpForBuildingPlacement, 0.02f);
                Game1.playSound("smallSelect");
                this.TargetLocation = "Farm";
                this.demolishing = true;
            }

            if (string.IsNullOrEmpty(this.TargetLocation) && this.moveButton.containsPoint(x, y))
            {
                Game1.globalFadeToBlack(this.setUpForBuildingPlacement, 0.02f);
                Game1.playSound("smallSelect");
                this.TargetLocation = "Farm";
                this.moving = true;
            }

            if (this.okButton.containsPoint(x, y) && string.IsNullOrEmpty(this.TargetLocation) && Game1.player.money >= this.price && this.blueprints[this.currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild())
            {
                Game1.globalFadeToBlack(this.setUpForBuildingPlacement, 0.02f);
                Game1.playSound("smallSelect");
                this.TargetLocation = "Farm";
            }

            if (string.IsNullOrEmpty(this.TargetLocation) || this.freeze || Game1.globalFade)
                return;
            if (this.demolishing)
            {
                Building buildingAt = ((BuildableGameLocation)Game1.getLocationFromName(this.TargetLocation)).getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize));
                if (buildingAt != null && (buildingAt.daysOfConstructionLeft.Value > 0 || buildingAt.daysUntilUpgrade.Value > 0))
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction"), Color.Red, 3500f));
                else if (buildingAt?.indoors.Value != null && buildingAt.indoors.Value is AnimalHouse house && house.animalsThatLiveHere.Count > 0)
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere"), Color.Red, 3500f));
                else
                {
                    if (buildingAt == null || !((BuildableGameLocation)Game1.getLocationFromName(this.TargetLocation)).destroyStructure(buildingAt))
                        return;
                    Game1.flashAlpha = 1f;
                    buildingAt.showDestroyedAnimation(Game1.getLocationFromName(this.TargetLocation));
                    Game1.playSound("explosion");
                    Utility.spreadAnimalsAround(buildingAt, (Farm)Game1.getLocationFromName(this.TargetLocation));
                    DelayedAction.fadeAfterDelay(this.returnToCarpentryMenu, 1500);
                    this.freeze = true;
                }
            }
            else if (this.upgrading)
            {
                Building buildingAt = ((BuildableGameLocation)Game1.getLocationFromName(this.TargetLocation)).getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize));
                if (buildingAt != null && this.CurrentBlueprint.name != null && buildingAt.buildingType.Equals(this.CurrentBlueprint.nameOfBuildingToUpgrade))
                {
                    this.CurrentBlueprint.consumeResources();
                    buildingAt.daysUntilUpgrade.Value = 2;
                    buildingAt.showUpgradeAnimation(Game1.getLocationFromName(this.TargetLocation));
                    Game1.playSound("axe");
                    DelayedAction.fadeAfterDelay(this.returnToCarpentryMenuAfterSuccessfulBuild, 1500);
                    this.freeze = true;
                }
                else
                {
                    if (buildingAt == null)
                        return;
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantUpgrade_BuildingType"), Color.Red, 3500f));
                }
            }
            else if (this.moving)
            {
                if (this.buildingToMove == null)
                {
                    this.buildingToMove = ((BuildableGameLocation)Game1.getLocationFromName(this.TargetLocation)).getBuildingAt(new Vector2((Game1.viewport.X + Game1.getMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getMouseY()) / Game1.tileSize));
                    if (this.buildingToMove == null)
                        return;
                    if (this.buildingToMove.daysOfConstructionLeft.Value > 0)
                        this.buildingToMove = null;
                    else
                    {
                        ((BuildableGameLocation)Game1.getLocationFromName(this.TargetLocation)).buildings.Remove(this.buildingToMove);
                        Game1.playSound("axchop");
                    }
                }
                else if (((BuildableGameLocation)Game1.getLocationFromName(this.TargetLocation)).buildStructure(this.buildingToMove, new Vector2((Game1.viewport.X + Game1.getMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getMouseY()) / Game1.tileSize), Game1.player))
                {
                    this.buildingToMove = null;
                    Game1.playSound("axchop");
                    DelayedAction.playSoundAfterDelay("dirtyHit", 50);
                    DelayedAction.playSoundAfterDelay("dirtyHit", 150);
                }
                else
                    Game1.playSound("cancel");
            }
            else if (this.tryToBuild())
            {
                this.CurrentBlueprint.consumeResources();
                DelayedAction.fadeAfterDelay(this.returnToCarpentryMenuAfterSuccessfulBuild, 2000);
                this.freeze = true;
            }
            else
                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild"), Color.Red, 3500f));
        }

        public bool tryToBuild()
        {
            return ((BuildableGameLocation)Game1.getLocationFromName(this.TargetLocation)).buildStructure(this.CurrentBlueprint, new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize), Game1.player, this.magicalConstruction);
        }

        public void returnToCarpentryMenu()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = Game1.getLocationFromName(this.magicalConstruction ? "WizardHouse" : "ScienceHouse");
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear(null, 0.02f);
            this.TargetLocation = null;
            this.resetBounds();
            this.upgrading = false;
            this.moving = false;
            this.freeze = false;
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.viewport.Location = new Location(5 * Game1.tileSize, 24 * Game1.tileSize);
            this.drawBG = true;
            this.demolishing = false;
            Game1.displayFarmer = true;
        }

        public void returnToCarpentryMenuAfterSuccessfulBuild()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = Game1.getLocationFromName(this.magicalConstruction ? "WizardHouse" : "ScienceHouse");
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear(this.robinConstructionMessage, 0.02f);
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.viewport.Location = new Location(5 * Game1.tileSize, 24 * Game1.tileSize);
            this.freeze = true;
            Game1.displayFarmer = true;
        }

        public void robinConstructionMessage()
        {
            this.exitThisMenu(true);
            Game1.player.forceCanMove();
            if (this.magicalConstruction)
                return;
            string path = "Data\\ExtraDialogue:Robin_" + (this.upgrading ? "Upgrade" : "New") + "Construction";
            if (Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason))
                path += "_Festival";
            Game1.drawDialogue(Game1.getCharacterFromName("Robin", false), Game1.content.LoadString(path, this.CurrentBlueprint.name.ToLower(), this.CurrentBlueprint.name.ToLower().Split(' ').Last()));
        }

        public void setUpForBuildingPlacement()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            this.hoverText = "";
            Game1.currentLocation = Game1.getLocationFromName(this.TargetLocation);
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear(null, 0.02f);
            //TargetLocation = "Farm";
            this.cancelButton.bounds.X = Game1.viewport.Width - Game1.tileSize * 2;
            this.cancelButton.bounds.Y = Game1.viewport.Height - Game1.tileSize * 2;
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.viewport.Location = new Location(49 * Game1.tileSize, 5 * Game1.tileSize);
            Game1.panScreen(0, 0);
            this.drawBG = false;
            this.freeze = false;
            Game1.displayFarmer = false;
            if (this.demolishing || this.CurrentBlueprint.nameOfBuildingToUpgrade == null || this.CurrentBlueprint.nameOfBuildingToUpgrade.Length <= 0 || this.moving)
                return;
            this.upgrading = true;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            this.resetBounds();
        }

        public override void draw(SpriteBatch b)
        {
            if (this.drawBG)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
            if (Game1.globalFade || this.freeze)
                return;
            if (string.IsNullOrEmpty(this.TargetLocation))
            {
                base.draw(b);
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen - Game1.tileSize * 3 / 2, this.yPositionOnScreen - Game1.tileSize / 4, this.maxWidthOfBuildingViewer + Game1.tileSize, this.maxHeightOfBuildingViewer + Game1.tileSize, this.magicalConstruction ? Color.RoyalBlue : Color.White);
                this.currentBuilding.drawInMenu(b, this.xPositionOnScreen + this.maxWidthOfBuildingViewer / 2 - this.currentBuilding.tilesWide.Value * Game1.tileSize / 2 - Game1.tileSize, this.yPositionOnScreen + this.maxHeightOfBuildingViewer / 2 - this.currentBuilding.getSourceRectForMenu().Height * Game1.pixelZoom / 2);
                if (this.CurrentBlueprint.isUpgrade())
                    this.upgradeIcon.draw(b);
                SpriteText.drawStringWithScrollBackground(b, this.buildingName, this.xPositionOnScreen + this.maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder - Game1.tileSize / 4 + Game1.tileSize + ((this.width - (this.maxWidthOfBuildingViewer + Game1.tileSize * 2)) / 2 - SpriteText.getWidthOfString("Deluxe Barn") / 2), this.yPositionOnScreen, "Deluxe Barn", 1f, -1);
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen + this.maxWidthOfBuildingViewer - Game1.tileSize / 4, this.yPositionOnScreen + Game1.tileSize * 5 / 4, this.maxWidthOfDescription + Game1.tileSize, this.maxWidthOfDescription + Game1.tileSize * 3 / 2, this.magicalConstruction ? Color.RoyalBlue : Color.White);
                if (this.magicalConstruction)
                {
                    Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, this.maxWidthOfDescription + Game1.tileSize / 2), Game1.dialogueFont, new Vector2(this.xPositionOnScreen + this.maxWidthOfDescription + Game1.tileSize - Game1.pixelZoom, this.yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom * 4 + Game1.pixelZoom), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0.0f, 3);
                    Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, this.maxWidthOfDescription + Game1.tileSize / 2), Game1.dialogueFont, new Vector2(this.xPositionOnScreen + this.maxWidthOfDescription + Game1.tileSize - 1, this.yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom * 4 + Game1.pixelZoom), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0.0f, 3);
                }

                Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, this.maxWidthOfDescription + Game1.tileSize / 2), Game1.dialogueFont, new Vector2(this.xPositionOnScreen + this.maxWidthOfDescription + Game1.tileSize, this.yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom * 4), this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                Vector2 location = new Vector2(this.xPositionOnScreen + this.maxWidthOfDescription + Game1.tileSize / 4 + Game1.tileSize, this.yPositionOnScreen + Game1.tileSize * 4 + Game1.tileSize / 2);
                SpriteText.drawString(b, "$", (int)location.X, (int)location.Y, 999999, -1, 999999, 1f, 0.88f, false, -1, "", -1);
                if (this.magicalConstruction)
                {
                    Utility.drawTextWithShadow(b, this.price + "g", Game1.dialogueFont, new Vector2(location.X + Game1.tileSize, location.Y + Game1.pixelZoom * 2), Game1.textColor * 0.5f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                    Utility.drawTextWithShadow(b, this.price + "g", Game1.dialogueFont, new Vector2((float)(location.X + Game1.tileSize + Game1.pixelZoom - 1.0), location.Y + Game1.pixelZoom * 2), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                }

                Utility.drawTextWithShadow(b, this.price + "g", Game1.dialogueFont, new Vector2(location.X + Game1.tileSize + Game1.pixelZoom, location.Y + Game1.pixelZoom), Game1.player.money >= this.price ? this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor : Color.Red, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                location.X -= Game1.tileSize / 4;
                location.Y -= Game1.tileSize / 3;
                foreach (Item obj in this.ingredients)
                {
                    location.Y += Game1.tileSize + Game1.pixelZoom;
                    obj.drawInMenu(b, location, 1f);
                    bool flag = !(obj is Object) || Game1.player.hasItemInInventory((obj as Object).ParentSheetIndex, obj.Stack);
                    if (this.magicalConstruction)
                    {
                        Utility.drawTextWithShadow(b, obj.Name, Game1.dialogueFont, new Vector2(location.X + Game1.tileSize + Game1.pixelZoom * 3, location.Y + Game1.pixelZoom * 6), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                        Utility.drawTextWithShadow(b, obj.Name, Game1.dialogueFont, new Vector2((float)(location.X + Game1.tileSize + Game1.pixelZoom * 4 - 1.0), location.Y + Game1.pixelZoom * 6), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                    }

                    Utility.drawTextWithShadow(b, obj.Name, Game1.dialogueFont, new Vector2(location.X + Game1.tileSize + Game1.pixelZoom * 4, location.Y + Game1.pixelZoom * 5), flag ? this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor : Color.Red, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                }

                this.backButton.draw(b);
                this.forwardButton.draw(b);
                this.okButton.draw(b, this.blueprints[this.currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild() ? Color.White : Color.Gray * 0.8f, 0.88f);
                this.demolishButton.draw(b);
                this.moveButton.draw(b);
            }
            else
            {
                string str;
                if (!this.upgrading)
                    str = this.demolishing ? Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Demolish") : Game1.content.LoadString("Strings\\UI:Carpenter_ChooseLocation");
                else
                    str = Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Upgrade", this.CurrentBlueprint.nameOfBuildingToUpgrade);
                string s = str;
                SpriteText.drawStringWithScrollBackground(b, s, Game1.viewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, Game1.tileSize / 4, "", 1f, -1);
                if (!this.upgrading && !this.demolishing && !this.moving)
                {
                    Vector2 vector2 = new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize);
                    for (int y = 0; y < this.CurrentBlueprint.tilesHeight; ++y)
                        for (int x = 0; x < this.CurrentBlueprint.tilesWidth; ++x)
                        {
                            int structurePlacementTile = this.CurrentBlueprint.getTileSheetIndexForStructurePlacementTile(x, y);
                            Vector2 tileLocation = new Vector2(vector2.X + x, vector2.Y + y);
                            if (!(Game1.currentLocation as BuildableGameLocation).isBuildable(tileLocation))
                                ++structurePlacementTile;
                            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, tileLocation * Game1.tileSize), new Rectangle(194 + structurePlacementTile * 16, 388, 16, 16), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.999f);
                        }
                }
                else if (this.moving && this.buildingToMove != null)
                {
                    Vector2 vector2 = new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize);
                    for (int y = 0; y < this.buildingToMove.tilesHigh.Value; ++y)
                        for (int x = 0; x < this.buildingToMove.tilesWide.Value; ++x)
                        {
                            int structurePlacementTile = this.buildingToMove.getTileSheetIndexForStructurePlacementTile(x, y);
                            Vector2 tileLocation = new Vector2(vector2.X + x, vector2.Y + y);
                            if (!(Game1.currentLocation as BuildableGameLocation).isBuildable(tileLocation))
                                ++structurePlacementTile;
                            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, tileLocation * Game1.tileSize), new Rectangle(194 + structurePlacementTile * 16, 388, 16, 16), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.999f);
                        }
                }
            }

            this.cancelButton.draw(b);
            this.drawMouse(b);
            if (this.hoverText.Length <= 0)
                return;
            IClickableMenu.drawHoverText(b, this.hoverText, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }
    }
}
