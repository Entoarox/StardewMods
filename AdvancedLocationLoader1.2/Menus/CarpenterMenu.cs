using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Locations;


using xTile.Dimensions;

/// <summary>
/// WIP for 1.2.1+
/// </summary>
namespace Entoarox.AdvancedLocationLoader.Menus
{
    public class CarpenterMenu : IClickableMenu
    {
        private List<string> BuildableLocations = new List<string>();
        private string TargetLocation;
        public int maxWidthOfBuildingViewer = 7 * Game1.tileSize;
        public int maxHeightOfBuildingViewer = 8 * Game1.tileSize;
        public int maxWidthOfDescription = 6 * Game1.tileSize;
        private List<Item> ingredients = new List<Item>();
        private bool drawBG = true;
        private string hoverText = "";
        private List<BluePrint> blueprints;
        private int currentBlueprintIndex;
        private ClickableTextureComponent okButton;
        private ClickableTextureComponent cancelButton;
        private ClickableTextureComponent backButton;
        private ClickableTextureComponent forwardButton;
        private ClickableTextureComponent upgradeIcon;
        private ClickableTextureComponent demolishButton;
        private ClickableTextureComponent moveButton;
        private Building currentBuilding;
        private Building buildingToMove;
        private string buildingDescription;
        private string buildingName;
        private int price;
        private bool freeze;
        private bool upgrading;
        private bool demolishing;
        private bool moving;
        private bool magicalConstruction;

        public BluePrint CurrentBlueprint
        {
            get
            {
                return blueprints[currentBlueprintIndex];
            }
        }

        public CarpenterMenu(bool magicalConstruction = false)
        {
            this.magicalConstruction = magicalConstruction;
            Game1.player.forceCanMove();
            resetBounds();
            blueprints = new List<BluePrint>();
            if (magicalConstruction)
            {
                blueprints.Add(new BluePrint("Junimo Hut"));
                blueprints.Add(new BluePrint("Earth Obelisk"));
                blueprints.Add(new BluePrint("Water Obelisk"));
                blueprints.Add(new BluePrint("Gold Clock"));
            }
            else
            {
                blueprints.Add(new BluePrint("Coop"));
                blueprints.Add(new BluePrint("Barn"));
                blueprints.Add(new BluePrint("Well"));
                blueprints.Add(new BluePrint("Silo"));
                blueprints.Add(new BluePrint("Mill"));
                blueprints.Add(new BluePrint("Shed"));
                bool bigCoop = false;
                bool delCoop = false;
                bool bigBarn = false;
                bool delBarn = false;
                bool stable = true;
                foreach(GameLocation l in Game1.locations)
                {
                    if(l is BuildableGameLocation)
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
                        BuildableLocations.Add(l.Name);
                    }
                }
                if (stable)
                    blueprints.Add(new BluePrint("Stable"));
                blueprints.Add(new BluePrint("Slime Hutch"));
                if (bigCoop)
                    blueprints.Add(new BluePrint("Big Coop"));
                if (delCoop)
                    blueprints.Add(new BluePrint("Deluxe Coop"));
                if (bigBarn)
                    blueprints.Add(new BluePrint("Big Barn"));
                if (delBarn)
                    blueprints.Add(new BluePrint("Deluxe Barn"));
            }
            setNewActiveBlueprint();
        }

        private void resetBounds()
        {
            xPositionOnScreen = Game1.viewport.Width / 2 - maxWidthOfBuildingViewer - spaceToClearSideBorder;
            yPositionOnScreen = Game1.viewport.Height / 2 - maxHeightOfBuildingViewer / 2 - spaceToClearTopBorder + Game1.tileSize / 2;
            width = maxWidthOfBuildingViewer + maxWidthOfDescription + spaceToClearSideBorder * 2 + Game1.tileSize;
            height = maxHeightOfBuildingViewer + spaceToClearTopBorder;
            initialize(xPositionOnScreen, yPositionOnScreen, width, height, true);
            okButton = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - Game1.tileSize * 3 - Game1.pixelZoom * 3, yPositionOnScreen + maxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(366, 373, 16, 16), Game1.pixelZoom, false);
            cancelButton = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - Game1.tileSize, yPositionOnScreen + maxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47, -1, -1), 1f, false);
            backButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(xPositionOnScreen + Game1.tileSize, yPositionOnScreen + maxHeightOfBuildingViewer + Game1.tileSize, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(352, 495, 12, 11), Game1.pixelZoom, false);
            forwardButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(xPositionOnScreen + maxWidthOfBuildingViewer - Game1.tileSize * 4 + Game1.tileSize / 4, yPositionOnScreen + maxHeightOfBuildingViewer + Game1.tileSize, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(365, 495, 12, 11), (float)Game1.pixelZoom, false);
            demolishButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_Demolish"), new Microsoft.Xna.Framework.Rectangle(xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - Game1.tileSize * 2 - Game1.pixelZoom * 2, yPositionOnScreen + maxHeightOfBuildingViewer + Game1.tileSize - Game1.pixelZoom, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(348, 372, 17, 17), Game1.pixelZoom, false);
            upgradeIcon = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(xPositionOnScreen + maxWidthOfBuildingViewer - Game1.tileSize * 2 + Game1.tileSize / 2, yPositionOnScreen + Game1.pixelZoom * 2, 9 * Game1.pixelZoom, 13 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(402, 328, 9, 13), Game1.pixelZoom, false);
            moveButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings"), new Microsoft.Xna.Framework.Rectangle(xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - Game1.tileSize * 4 - Game1.pixelZoom * 5, yPositionOnScreen + maxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(257, 284, 16, 16), Game1.pixelZoom, false);
        }

        public void setNewActiveBlueprint()
        {
            currentBuilding = !blueprints[currentBlueprintIndex].name.Contains("Coop") ? (!blueprints[currentBlueprintIndex].name.Contains("Barn") ? (!blueprints[currentBlueprintIndex].name.Contains("Mill") ? (!blueprints[currentBlueprintIndex].name.Contains("Junimo Hut") ? new Building(blueprints[currentBlueprintIndex], Vector2.Zero) : new JunimoHut(blueprints[currentBlueprintIndex], Vector2.Zero)) : new Mill(blueprints[currentBlueprintIndex], Vector2.Zero)) : new Barn(blueprints[currentBlueprintIndex], Vector2.Zero)) : new Coop(blueprints[currentBlueprintIndex], Vector2.Zero);
            price = blueprints[currentBlueprintIndex].moneyRequired;
            ingredients.Clear();
            foreach (KeyValuePair<int, int> keyValuePair in blueprints[currentBlueprintIndex].itemsRequired)
                ingredients.Add(new Object(keyValuePair.Key, keyValuePair.Value, false, -1, 0));
            buildingDescription = blueprints[currentBlueprintIndex].description;
            buildingName = blueprints[currentBlueprintIndex].name;
        }

        public override void performHoverAction(int x, int y)
        {
            cancelButton.tryHover(x, y, 0.1f);
            base.performHoverAction(x, y);
            if (string.IsNullOrEmpty(TargetLocation))
            {
                backButton.tryHover(x, y, 1f);
                forwardButton.tryHover(x, y, 1f);
                okButton.tryHover(x, y, 0.1f);
                demolishButton.tryHover(x, y, 0.1f);
                moveButton.tryHover(x, y, 0.1f);
                if (CurrentBlueprint.isUpgrade() && upgradeIcon.containsPoint(x, y))
                    hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Upgrade", CurrentBlueprint.nameOfBuildingToUpgrade);
                else if (demolishButton.containsPoint(x, y))
                    hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Demolish");
                else if (moveButton.containsPoint(x, y))
                    hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings");
                else if (okButton.containsPoint(x, y) && CurrentBlueprint.doesFarmerHaveEnoughResourcesToBuild())
                    hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Build");
                else
                    hoverText = "";
            }
            else
            {
                if (!upgrading && !demolishing && !moving || freeze)
                    return;
                foreach (Building building in ((BuildableGameLocation)Game1.getLocationFromName(TargetLocation)).buildings)
                    building.color = Color.White;
                Building building1 = ((BuildableGameLocation)Game1.getLocationFromName(TargetLocation)).getBuildingAt(new Vector2(((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), ((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize))) ?? ((BuildableGameLocation)Game1.getLocationFromName(TargetLocation)).getBuildingAt(new Vector2(((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), ((Game1.viewport.Y + Game1.getOldMouseY() + Game1.tileSize * 2) / Game1.tileSize))) ?? ((BuildableGameLocation)Game1.getLocationFromName(TargetLocation)).getBuildingAt(new Vector2(((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), ((Game1.viewport.Y + Game1.getOldMouseY() + Game1.tileSize * 3) / Game1.tileSize)));
                if (upgrading)
                {
                    if (building1 != null && CurrentBlueprint.nameOfBuildingToUpgrade != null && CurrentBlueprint.nameOfBuildingToUpgrade.Equals(building1.buildingType))
                    {
                        building1.color = Color.Lime * 0.8f;
                    }
                    else
                    {
                        if (building1 == null)
                            return;
                        building1.color = Color.Red * 0.8f;
                    }
                }
                else if (demolishing)
                {
                    if (building1 == null)
                        return;
                    building1.color = Color.Red * 0.8f;
                }
                else
                {
                    if (!moving || building1 == null)
                        return;
                    building1.color = Color.Lime * 0.8f;
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
            if (string.IsNullOrEmpty(TargetLocation))
                base.receiveKeyPress(key);
            if (Game1.globalFade || string.IsNullOrEmpty(TargetLocation))
                return;
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose())
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(returnToCarpentryMenu), 0.02f);
            else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                Game1.panScreen(0, 4);
            else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                Game1.panScreen(4, 0);
            else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
            {
                Game1.panScreen(0, -4);
            }
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
            if (string.IsNullOrEmpty(TargetLocation) || Game1.globalFade)
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
            if (freeze)
                return;
            if (string.IsNullOrEmpty(TargetLocation))
                base.receiveLeftClick(x, y, playSound);
            if (cancelButton.containsPoint(x, y))
            {
                if (string.IsNullOrEmpty(TargetLocation))
                {
                    exitThisMenu(true);
                    Game1.player.forceCanMove();
                    Game1.playSound("bigDeSelect");
                }
                else
                {
                    if (moving && buildingToMove != null)
                    {
                        Game1.playSound("cancel");
                        return;
                    }
                    Game1.globalFadeToBlack(new Game1.afterFadeFunction(returnToCarpentryMenu), 0.02f);
                    Game1.playSound("smallSelect");
                    return;
                }
            }
            if (string.IsNullOrEmpty(TargetLocation) && backButton.containsPoint(x, y))
            {
                currentBlueprintIndex = currentBlueprintIndex - 1;
                if (currentBlueprintIndex < 0)
                    currentBlueprintIndex = blueprints.Count - 1;
                setNewActiveBlueprint();
                Game1.playSound("shwip");
                backButton.scale = backButton.baseScale;
            }
            if (string.IsNullOrEmpty(TargetLocation) && forwardButton.containsPoint(x, y))
            {
                currentBlueprintIndex = (currentBlueprintIndex + 1) % blueprints.Count;
                setNewActiveBlueprint();
                backButton.scale = backButton.baseScale;
                Game1.playSound("shwip");
            }
            if (string.IsNullOrEmpty(TargetLocation) && demolishButton.containsPoint(x, y))
            {
                Game1.globalFadeToBlack(setUpForBuildingPlacement, 0.02f);
                Game1.playSound("smallSelect");
                TargetLocation = "Farm";
                demolishing = true;
            }
            if (string.IsNullOrEmpty(TargetLocation) && moveButton.containsPoint(x, y))
            {
                Game1.globalFadeToBlack(setUpForBuildingPlacement, 0.02f);
                Game1.playSound("smallSelect");
                TargetLocation = "Farm";
                moving = true;
            }
            if (okButton.containsPoint(x, y) && string.IsNullOrEmpty(TargetLocation) && (Game1.player.money >= price && blueprints[currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild()))
            {
                Game1.globalFadeToBlack(setUpForBuildingPlacement, 0.02f);
                Game1.playSound("smallSelect");
                TargetLocation = "Farm";
            }
            if (string.IsNullOrEmpty(TargetLocation) || freeze || Game1.globalFade)
                return;
            if (demolishing)
            {
                Building buildingAt = ((BuildableGameLocation)Game1.getLocationFromName(TargetLocation)).getBuildingAt(new Vector2(((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), ((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize)));
                if (buildingAt != null && (buildingAt.daysOfConstructionLeft > 0 || buildingAt.daysUntilUpgrade > 0))
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction"), Color.Red, 3500f));
                else if (buildingAt != null && buildingAt.indoors != null && (buildingAt.indoors is AnimalHouse && (buildingAt.indoors as AnimalHouse).animalsThatLiveHere.Count > 0))
                {
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere"), Color.Red, 3500f));
                }
                else
                {
                    if (buildingAt == null || !((BuildableGameLocation)Game1.getLocationFromName(TargetLocation)).destroyStructure(buildingAt))
                        return;
                    int num1 = buildingAt.tileY;
                    int num2 = buildingAt.tilesHigh;
                    Game1.flashAlpha = 1f;
                    buildingAt.showDestroyedAnimation(Game1.getLocationFromName(TargetLocation));
                    Game1.playSound("explosion");
                    Utility.spreadAnimalsAround(buildingAt, (Farm)Game1.getLocationFromName(TargetLocation));
                    DelayedAction.fadeAfterDelay(returnToCarpentryMenu, 1500);
                    freeze = true;
                }
            }
            else if (upgrading)
            {
                Building buildingAt = ((BuildableGameLocation)Game1.getLocationFromName(TargetLocation)).getBuildingAt(new Vector2(((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), ((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize)));
                if (buildingAt != null && CurrentBlueprint.name != null && buildingAt.buildingType.Equals(CurrentBlueprint.nameOfBuildingToUpgrade))
                {
                    CurrentBlueprint.consumeResources();
                    buildingAt.daysUntilUpgrade = 2;
                    buildingAt.showUpgradeAnimation(Game1.getLocationFromName(TargetLocation));
                    Game1.playSound("axe");
                    DelayedAction.fadeAfterDelay(returnToCarpentryMenuAfterSuccessfulBuild, 1500);
                    freeze = true;
                }
                else
                {
                    if (buildingAt == null)
                        return;
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantUpgrade_BuildingType"), Color.Red, 3500f));
                }
            }
            else if (moving)
            {
                if (buildingToMove == null)
                {
                    buildingToMove = ((BuildableGameLocation)Game1.getLocationFromName(TargetLocation)).getBuildingAt(new Vector2(((Game1.viewport.X + Game1.getMouseX()) / Game1.tileSize), ((Game1.viewport.Y + Game1.getMouseY()) / Game1.tileSize)));
                    if (buildingToMove == null)
                        return;
                    if (buildingToMove.daysOfConstructionLeft > 0)
                    {
                        buildingToMove = null;
                    }
                    else
                    {
                        ((BuildableGameLocation)Game1.getLocationFromName(TargetLocation)).buildings.Remove(buildingToMove);
                        Game1.playSound("axchop");
                    }
                }
                else if (((BuildableGameLocation)Game1.getLocationFromName(TargetLocation)).buildStructure(buildingToMove, new Vector2(((Game1.viewport.X + Game1.getMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getMouseY()) / Game1.tileSize)), false, Game1.player))
                {
                    buildingToMove = null;
                    Game1.playSound("axchop");
                    DelayedAction.playSoundAfterDelay("dirtyHit", 50);
                    DelayedAction.playSoundAfterDelay("dirtyHit", 150);
                }
                else
                    Game1.playSound("cancel");
            }
            else if (tryToBuild())
            {
                CurrentBlueprint.consumeResources();
                DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(returnToCarpentryMenuAfterSuccessfulBuild), 2000);
                freeze = true;
            }
            else
                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild"), Color.Red, 3500f));
        }

        public bool tryToBuild()
        {
            return ((BuildableGameLocation)Game1.getLocationFromName(TargetLocation)).buildStructure(CurrentBlueprint, new Vector2(((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), ((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize)), false, Game1.player, magicalConstruction);
        }

        public void returnToCarpentryMenu()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = Game1.getLocationFromName(magicalConstruction ? "WizardHouse" : "ScienceHouse");
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear(null, 0.02f);
            TargetLocation = null;
            resetBounds();
            upgrading = false;
            moving = false;
            freeze = false;
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.viewport.Location = new Location(5 * Game1.tileSize, 24 * Game1.tileSize);
            drawBG = true;
            demolishing = false;
            Game1.displayFarmer = true;
        }

        public void returnToCarpentryMenuAfterSuccessfulBuild()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = Game1.getLocationFromName(magicalConstruction ? "WizardHouse" : "ScienceHouse");
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear(new Game1.afterFadeFunction(robinConstructionMessage), 0.02f);
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.viewport.Location = new Location(5 * Game1.tileSize, 24 * Game1.tileSize);
            freeze = true;
            Game1.displayFarmer = true;
        }

        public void robinConstructionMessage()
        {
            exitThisMenu(true);
            Game1.player.forceCanMove();
            if (magicalConstruction)
                return;
            string path = "Data\\ExtraDialogue:Robin_" + (upgrading ? "Upgrade" : "New") + "Construction";
            if (Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason))
                path += "_Festival";
            Game1.drawDialogue(Game1.getCharacterFromName("Robin", false), Game1.content.LoadString(path, CurrentBlueprint.name.ToLower(), CurrentBlueprint.name.ToLower().Split(' ').Last()));
        }

        public void setUpForBuildingPlacement()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            hoverText = "";
            Game1.currentLocation = Game1.getLocationFromName(TargetLocation);
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear(null, 0.02f);
            //TargetLocation = "Farm";
            cancelButton.bounds.X = Game1.viewport.Width - Game1.tileSize * 2;
            cancelButton.bounds.Y = Game1.viewport.Height - Game1.tileSize * 2;
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.viewport.Location = new Location(49 * Game1.tileSize, 5 * Game1.tileSize);
            Game1.panScreen(0, 0);
            drawBG = false;
            freeze = false;
            Game1.displayFarmer = false;
            if (demolishing || CurrentBlueprint.nameOfBuildingToUpgrade == null || (CurrentBlueprint.nameOfBuildingToUpgrade.Length <= 0 || moving))
                return;
            upgrading = true;
        }

        public override void gameWindowSizeChanged(Microsoft.Xna.Framework.Rectangle oldBounds, Microsoft.Xna.Framework.Rectangle newBounds)
        {
            resetBounds();
        }

        public override void draw(SpriteBatch b)
        {
            if (drawBG)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
            if (Game1.globalFade || freeze)
                return;
            if (string.IsNullOrEmpty(TargetLocation))
            {
                base.draw(b);
                drawTextureBox(b, xPositionOnScreen - Game1.tileSize * 3 / 2, yPositionOnScreen - Game1.tileSize / 4, maxWidthOfBuildingViewer + Game1.tileSize, maxHeightOfBuildingViewer + Game1.tileSize, magicalConstruction ? Color.RoyalBlue : Color.White);
                currentBuilding.drawInMenu(b, xPositionOnScreen + maxWidthOfBuildingViewer / 2 - currentBuilding.tilesWide * Game1.tileSize / 2 - Game1.tileSize, yPositionOnScreen + maxHeightOfBuildingViewer / 2 - currentBuilding.getSourceRectForMenu().Height * Game1.pixelZoom / 2);
                if (CurrentBlueprint.isUpgrade())
                    upgradeIcon.draw(b);
                SpriteText.drawStringWithScrollBackground(b, buildingName, xPositionOnScreen + maxWidthOfBuildingViewer - spaceToClearSideBorder - Game1.tileSize / 4 + Game1.tileSize + ((width - (maxWidthOfBuildingViewer + Game1.tileSize * 2)) / 2 - SpriteText.getWidthOfString("Deluxe Barn") / 2), yPositionOnScreen, "Deluxe Barn", 1f, -1);
                drawTextureBox(b, xPositionOnScreen + maxWidthOfBuildingViewer - Game1.tileSize / 4, yPositionOnScreen + Game1.tileSize * 5 / 4, maxWidthOfDescription + Game1.tileSize, maxWidthOfDescription + Game1.tileSize * 3 / 2, magicalConstruction ? Color.RoyalBlue : Color.White);
                if (magicalConstruction)
                {
                    Utility.drawTextWithShadow(b, Game1.parseText(buildingDescription, Game1.dialogueFont, maxWidthOfDescription + Game1.tileSize / 2), Game1.dialogueFont, new Vector2((xPositionOnScreen + maxWidthOfDescription + Game1.tileSize - Game1.pixelZoom), (yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom * 4 + Game1.pixelZoom)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0.0f, 3);
                    Utility.drawTextWithShadow(b, Game1.parseText(buildingDescription, Game1.dialogueFont, maxWidthOfDescription + Game1.tileSize / 2), Game1.dialogueFont, new Vector2((xPositionOnScreen + maxWidthOfDescription + Game1.tileSize - 1), (yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom * 4 + Game1.pixelZoom)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0.0f, 3);
                }
                Utility.drawTextWithShadow(b, Game1.parseText(buildingDescription, Game1.dialogueFont, maxWidthOfDescription + Game1.tileSize / 2), Game1.dialogueFont, new Vector2((xPositionOnScreen + maxWidthOfDescription + Game1.tileSize), (yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom * 4)), magicalConstruction ? Color.PaleGoldenrod : Game1.textColor, 1f, -1f, -1, -1, magicalConstruction ? 0.0f : 0.25f, 3);
                Vector2 location = new Vector2((xPositionOnScreen + maxWidthOfDescription + Game1.tileSize / 4 + Game1.tileSize), (yPositionOnScreen + Game1.tileSize * 4 + Game1.tileSize / 2));
                SpriteText.drawString(b, "$", (int)location.X, (int)location.Y, 999999, -1, 999999, 1f, 0.88f, false, -1, "", -1);
                if (magicalConstruction)
                {
                    Utility.drawTextWithShadow(b, price.ToString() + "g", Game1.dialogueFont, new Vector2(location.X + Game1.tileSize, location.Y + (Game1.pixelZoom * 2)), Game1.textColor * 0.5f, 1f, -1f, -1, -1, magicalConstruction ? 0.0f : 0.25f, 3);
                    Utility.drawTextWithShadow(b, price.ToString() + "g", Game1.dialogueFont, new Vector2((float)(location.X + Game1.tileSize + Game1.pixelZoom - 1.0), location.Y + (Game1.pixelZoom * 2)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, magicalConstruction ? 0.0f : 0.25f, 3);
                }
                Utility.drawTextWithShadow(b, price.ToString() + "g", Game1.dialogueFont, new Vector2(location.X + Game1.tileSize + Game1.pixelZoom, location.Y + Game1.pixelZoom), Game1.player.money >= price ? (magicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, 1f, -1f, -1, -1, magicalConstruction ? 0.0f : 0.25f, 3);
                location.X -= (Game1.tileSize / 4);
                location.Y -= (Game1.tileSize / 3);
                foreach (Item obj in ingredients)
                {
                    location.Y += (Game1.tileSize + Game1.pixelZoom);
                    obj.drawInMenu(b, location, 1f);
                    bool flag = !(obj is Object) || Game1.player.hasItemInInventory((obj as Object).parentSheetIndex, obj.Stack, 0);
                    if (magicalConstruction)
                    {
                        Utility.drawTextWithShadow(b, obj.Name, Game1.dialogueFont, new Vector2(location.X + Game1.tileSize + (Game1.pixelZoom * 3), location.Y + (Game1.pixelZoom * 6)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, magicalConstruction ? 0.0f : 0.25f, 3);
                        Utility.drawTextWithShadow(b, obj.Name, Game1.dialogueFont, new Vector2((float)(location.X + Game1.tileSize + (Game1.pixelZoom * 4) - 1.0), location.Y + (Game1.pixelZoom * 6)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, magicalConstruction ? 0.0f : 0.25f, 3);
                    }
                    Utility.drawTextWithShadow(b, obj.Name, Game1.dialogueFont, new Vector2(location.X + Game1.tileSize + (Game1.pixelZoom * 4), location.Y + (Game1.pixelZoom * 5)), flag ? (magicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, 1f, -1f, -1, -1, magicalConstruction ? 0.0f : 0.25f, 3);
                }
                backButton.draw(b);
                forwardButton.draw(b);
                okButton.draw(b, blueprints[currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild() ? Color.White : Color.Gray * 0.8f, 0.88f);
                demolishButton.draw(b);
                moveButton.draw(b);
            }
            else
            {
                string str;
                if (!upgrading)
                    str = demolishing ? Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Demolish") : Game1.content.LoadString("Strings\\UI:Carpenter_ChooseLocation");
                else
                    str = Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Upgrade", CurrentBlueprint.nameOfBuildingToUpgrade);
                string s = str;
                SpriteText.drawStringWithScrollBackground(b, s, Game1.viewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, Game1.tileSize / 4, "", 1f, -1);
                if (!upgrading && !demolishing && !moving)
                {
                    Vector2 vector2 = new Vector2(((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), ((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize));
                    for (int y = 0; y < CurrentBlueprint.tilesHeight; ++y)
                    {
                        for (int x = 0; x < CurrentBlueprint.tilesWidth; ++x)
                        {
                            int structurePlacementTile = CurrentBlueprint.getTileSheetIndexForStructurePlacementTile(x, y);
                            Vector2 tileLocation = new Vector2(vector2.X + x, vector2.Y + y);
                            if (!(Game1.currentLocation as BuildableGameLocation).isBuildable(tileLocation))
                                ++structurePlacementTile;
                            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, tileLocation * Game1.tileSize), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194 + structurePlacementTile * 16, 388, 16, 16)), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.999f);
                        }
                    }
                }
                else if (moving && buildingToMove != null)
                {
                    Vector2 vector2 = new Vector2(((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), ((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize));
                    for (int y = 0; y < buildingToMove.tilesHigh; ++y)
                    {
                        for (int x = 0; x < buildingToMove.tilesWide; ++x)
                        {
                            int structurePlacementTile = buildingToMove.getTileSheetIndexForStructurePlacementTile(x, y);
                            Vector2 tileLocation = new Vector2(vector2.X + x, vector2.Y + y);
                            if (!(Game1.currentLocation as BuildableGameLocation).isBuildable(tileLocation))
                                ++structurePlacementTile;
                            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, tileLocation * Game1.tileSize), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194 + structurePlacementTile * 16, 388, 16, 16)), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.999f);
                        }
                    }
                }
            }
            cancelButton.draw(b);
            drawMouse(b);
            if (hoverText.Length <= 0)
                return;
            drawHoverText(b, hoverText, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }
    }
}
