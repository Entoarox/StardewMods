using System;
using System.Collections.Generic;

using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Entoarox.Framework;

namespace Entoarox.ShopExpander
{
    public class Reference
    {
        public string Owner;
        public int Item;
        public int Amount;
        public string Conditions = null;
        public Reference(string owner, int item, int amount, string conditions=null)
        {
            Owner = owner;
            Item = item;
            Amount = amount;
            Conditions = conditions;
        }
    }
    public class ShopExpanderConfig
    {
        public List<Reference> objects = new List<Reference>() {
            new Reference("Robin",388,111,"year=1"),
            new Reference("Robin",390,111,"year=1"),
            new Reference("Robin",388,333,"earthquake"),
            new Reference("Robin",390,333,"earthquake"),
            new Reference("Robin",388,999,"year>1"),
            new Reference("Robin",390,999,"year>1"),

            new Reference("Pierre", 472, 10,"year=1,spring" ),
            new Reference("Pierre", 473, 10,"year=1,spring" ),
            new Reference("Pierre", 474, 10,"year=1,spring" ),
            new Reference("Pierre", 475, 10,"year=1,spring" ),
            new Reference("Pierre", 427, 10,"year=1,spring" ),
            new Reference("Pierre", 429, 10,"year=1,spring" ),
            new Reference("Pierre", 477, 10,"year=1,spring" ),

            new Reference("Pierre", 480, 10,"year=1,summer" ),
            new Reference("Pierre", 482, 10,"year=1,summer" ),
            new Reference("Pierre", 483, 10,"year=1,summer" ),
            new Reference("Pierre", 484, 10,"year=1,summer" ),
            new Reference("Pierre", 479, 10,"year=1,summer" ),
            new Reference("Pierre", 302, 10,"year=1,summer" ),
            new Reference("Pierre", 456, 10,"year=1,summer" ),
            new Reference("Pierre", 455, 10,"year=1,summer" ),

            new Reference("Pierre", 487, 10,"year=1,fall" ),
            new Reference("Pierre", 488, 10,"year=1,fall" ),
            new Reference("Pierre", 490, 10,"year=1,fall" ),
            new Reference("Pierre", 299, 10,"year=1,fall" ),
            new Reference("Pierre", 301, 10,"year=1,fall" ),
            new Reference("Pierre", 492, 10,"year=1,fall" ),
            new Reference("Pierre", 491, 10,"year=1,fall" ),
            new Reference("Pierre", 493, 10,"year=1,fall" ),
            new Reference("Pierre", 425, 10,"year=1,fall" ),

            new Reference("Pierre", 472, 50,"year>1,spring" ),
            new Reference("Pierre", 473, 50,"year>1,spring" ),
            new Reference("Pierre", 474, 50,"year>1,spring" ),
            new Reference("Pierre", 475, 50,"year>1,spring" ),
            new Reference("Pierre", 427, 50,"year>1,spring" ),
            new Reference("Pierre", 429, 50,"year>1,spring" ),
            new Reference("Pierre", 477, 50,"year>1,spring" ),
            new Reference("Pierre", 476, 50,"year>1,spring" ),

            new Reference("Pierre", 480, 50,"year>1,summer" ),
            new Reference("Pierre", 482, 50,"year>1,summer" ),
            new Reference("Pierre", 483, 50,"year>1,summer" ),
            new Reference("Pierre", 484, 50,"year>1,summer" ),
            new Reference("Pierre", 479, 50,"year>1,summer" ),
            new Reference("Pierre", 302, 50,"year>1,summer" ),
            new Reference("Pierre", 456, 50,"year>1,summer" ),
            new Reference("Pierre", 455, 50,"year>1,summer" ),
            new Reference("Pierre", 485, 50,"year>1,summer" ),

            new Reference("Pierre", 487, 50,"year>1,fall" ),
            new Reference("Pierre", 488, 50,"year>1,fall" ),
            new Reference("Pierre", 490, 50,"year>1,fall" ),
            new Reference("Pierre", 299, 50,"year>1,fall" ),
            new Reference("Pierre", 301, 50,"year>1,fall" ),
            new Reference("Pierre", 492, 50,"year>1,fall" ),
            new Reference("Pierre", 491, 50,"year>1,fall" ),
            new Reference("Pierre", 493, 50,"year>1,fall" ),
            new Reference("Pierre", 425, 50,"year>1,fall" ),
            new Reference("Pierre", 489, 50,"year>1,fall" ),
        };
    }
    public class ShopExpanderMod : Mod
    {
        internal ShopExpanderConfig Config;
        private bool eventsActive = false;
        public override void Entry(IModHelper helper)
        {
            MenuEvents.MenuChanged += Event_MenuChanged;
            GameEvents.LoadContent += Event_LoadContent;
            Config = helper.ReadConfig<ShopExpanderConfig>();
        }
        private void generateObject(string owner, int replacement, int stackAmount, string requirements)
        {
            if (owner == "???")
            {
                Monitor.Log("Attempt to add a object to a shop owned by `???`, this cant be done because `???` means the owner is unknown!", LogLevel.Error);
                return;
            }
            StardewValley.Object stack = new StardewValley.Object(replacement, stackAmount);
            if(stack.salePrice()==0)
            {
                Monitor.Log("Unable to add item to shop, it has no value: "+replacement, LogLevel.Error);
                return;
            }
            SObject obj = new SObject(stack, stackAmount);
            obj.targetedShop = owner;
            if (!affectedShops.Contains(owner))
                affectedShops.Add(owner);
            obj.requirements = requirements;
            Monitor.Log($"RegisterObject({obj.Name}:{replacement}@{owner},{obj.stackAmount}*{obj.maximumStackSize()},'{requirements}')",LogLevel.Trace);
            AddedObjects.Add(obj.Name, obj);
            ReplacementStacks.Add(obj.Name, stack);
        }
        static private Dictionary<string, SObject> AddedObjects = new Dictionary<string, SObject>();
        static private Dictionary<string, Item> ReplacementStacks = new Dictionary<string, Item>();
        // Load our custom objects and their textures
        private void Event_LoadContent(object sender, EventArgs e)
        {
            foreach(Reference obj in Config.objects)
                try
                {
                    generateObject(obj.Owner, obj.Item, obj.Amount, obj.Conditions);
                }
                catch(Exception err)
                {
                    Monitor.Log(LogLevel.Error, "Object failed to generate: " + obj.ToString(), err);
                }
        }
        // If the inventory changes while this even is hooked, we need to check if any SObject instances are in it, so we can replace them
        private void Event_InventoryChanged(object send, EventArgs e)
        {
            for (int c = 0; c < Game1.player.Items.Count; c++)
            {
                if (Game1.player.Items[c] is SObject)
                {
                    SObject obj = (SObject)Game1.player.Items[c];
                    Monitor.Log("Reverting object: " + obj.Name+':'+obj.Stack, LogLevel.Trace);
                    Game1.player.Items[c] = obj.Revert();
                }
            }
        }
        // When the menu closes, remove the hook for the inventory changed event
        private void Event_MenuClosed(object send, EventArgsClickableMenuClosed e)
        {
            if (e.PriorMenu is ShopMenu && eventsActive)
            {
                PlayerEvents.InventoryChanged -= Event_InventoryChanged;
                MenuEvents.MenuClosed -= Event_MenuClosed;
            }
        }
        // Add a modified "stack" item to the shop
        private void addItem(SObject item, string location)
        {
            // Check that makes sure only the items that the current shop is supposed to sell are added
            if (location != item.targetedShop)
            {
                Monitor.Log("Item(" + item.Name + ':' + item.stackAmount + '*' + item.maximumStackSize() + "){Location=false}", LogLevel.Trace);
                return;
            }
            if (!Conditions.CheckConditionList(item.requirements, ','))
            {
                Monitor.Log("Item(" + item.Name + ':' + item.stackAmount + '*' + item.maximumStackSize() + "){Location=false,Condition=false}", LogLevel.Trace);
                return;
            }
            if(item.stackAmount==1)
            {
                Monitor.Log("Item(" + item.Name + ':' + item.stackAmount + '*' + item.maximumStackSize() + "){Location=true,Condition=true,Stack=false}", LogLevel.Trace);
                Item reverted = item.Revert();
                forSale.Add(reverted);
                itemPriceAndStock.Add(reverted, new int[2] { reverted.salePrice(), int.MaxValue });
            }
            else
            {
                Monitor.Log("Item(" + item.Name + ':' + item.stackAmount + '*' + item.maximumStackSize() + "){Location=true,Condition=true,Stack=true}", LogLevel.Trace);
                forSale.Add(item);
                itemPriceAndStock.Add(item, new int[2] { item.salePrice(), int.MaxValue });
            }
        }
        private Dictionary<Item, int[]> itemPriceAndStock;
        private List<Item> forSale;
        private List<string> affectedShops=new List<string>();
        private System.Reflection.FieldInfo Stock = typeof(ShopMenu).GetField("itemPriceAndStock", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        private System.Reflection.FieldInfo Sale = typeof(ShopMenu).GetField("forSale", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        // Listen to the MenuChanged event so I can check if the current menu is that for a shop
        private void Event_MenuChanged(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu is ShopMenu)
            {
                // When it is a shop menu, I need to perform some logic to identify the HatMouse, Traveler or ClintUpgrade shops as I cannot simply use their owner for that
                Monitor.Log("Shop Menu active, checking for expansion", LogLevel.Trace);
                ShopMenu menu = (ShopMenu) Game1.activeClickableMenu;
                string shopOwner = "???";
                // There are by default two shops in the forest, and neither has a owner, so we need to manually resolve the shop owner
                if(menu.portraitPerson!=null)
                {
                    shopOwner = menu.portraitPerson.name;
                    // Clint has two shops, we need to check if this is the tool upgrade shop and modify the owner if that is the case
                    if (shopOwner == "Clint" && menu.potraitPersonDialogue == "I can upgrade your tools with more power. You'll have to leave them with me for a few days, though.")
                        shopOwner = "ClintUpgrade";
                }
                else
                {
                    switch(Game1.currentLocation.Name)
                    {
                        case "Forest":
                            if (menu.potraitPersonDialogue == "Hiyo, poke. Did you bring coins? Gud. Me sell hats.")
                                shopOwner = "HatMouse";
                            else
                            {
                                // The merchant is a bit harder to determine then the mouse
                                List<string> matches = new List<string>{
                                    "I've got a little bit of everything. Take a look!",
                                    "I smuggled these goods out of the Gotoro Empire. Why do you think they're so expensive?",
                                    "I'll have new items every week, so make sure to come back!",
                                    "Beautiful country you have here. One of my favorite stops. The pig likes it, too.",
                                    "Let me see... Oh! I've got just what you need: "
                                };
                                // We only set the owner if it actually is the traveler, so custom unowned shops will simply remain unidentified
                                if (matches.Contains(menu.potraitPersonDialogue) || menu.potraitPersonDialogue.Substring(0, matches[4].Length) == matches[4])
                                    shopOwner = "Traveler";
                            }
                            break;
                        case "Hospital":
                            shopOwner = "Hospital";
                            break;
                        case "Club":
                            shopOwner = "MisterQi";
                            break;
                    }
                }
                if (affectedShops.Contains(shopOwner))
                {
                    Monitor.Log("Shop owned by `"+shopOwner+"` gets modified, doing so now",LogLevel.Trace);
                    // Register to inventory changes so we can immediately replace bought stacks
                    eventsActive = true;
                    MenuEvents.MenuClosed += Event_MenuClosed;
                    PlayerEvents.InventoryChanged += Event_InventoryChanged;
                    // Reflect the required fields to be able to edit a shops stock
                    itemPriceAndStock = (Dictionary<Item, int[]>)Stock.GetValue(Game1.activeClickableMenu);
                    forSale = (List<Item>)Sale.GetValue(Game1.activeClickableMenu);
                    // Add our custom items to the shop
                    foreach (string key in AddedObjects.Keys)
                        addItem(AddedObjects[key], shopOwner);
                    // Use reflection to set the changed values
                    Stock.SetValue(Game1.activeClickableMenu, itemPriceAndStock);
                    Sale.SetValue(Game1.activeClickableMenu, forSale);
                }
                else
                {
                    if(shopOwner.Equals("???"))
                        Monitor.Log("The shop owner could not be resolved, skipping shop", LogLevel.Trace);
                    else
                        Monitor.Log("The shop owned by `" + shopOwner + "` is not on the list, ignoring it");
                }
            }
        }
    }
    public class SObject : StardewValley.Object
    {
        public string targetedShop;
        public int stackAmount;
        private StardewValley.Object Item;
        public int MaxStackSize;
        public string requirements;
        public SObject(StardewValley.Object item, int stack)
        {
            Item = item;
            stackAmount = stack;
            parentSheetIndex = item.parentSheetIndex;
            price = salePrice() * stack;
            MaxStackSize = (int)Math.Floor(999d / stack);
        }
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            SpriteBatch spriteBatch1 = spriteBatch;
            Texture2D texture = Game1.shadowTexture;
            Vector2 vector2_1 = location;
            double num1 = (Game1.tileSize / 2);
            int num2 = Game1.tileSize * 3 / 4;
            int num3 = Game1.pixelZoom;
            double num4 = num2;
            Vector2 vector2_2 = new Vector2((float)num1, (float)num4);
            Vector2 position = vector2_1 + vector2_2;
            Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
            Color color = Color.White * 0.5f;
            double num5 = 0.0;
            Vector2 origin = new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y);
            double num6 = 3.0;
            int num7 = 0;
            double num8 = layerDepth - 9.99999974737875E-05;
            spriteBatch1.Draw(texture, position, sourceRectangle, color, (float)num5, origin, (float)num6, (SpriteEffects)num7, (float)num8);
            spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2((int)((Game1.tileSize / 2) * (double)scaleSize), (int)((Game1.tileSize / 2) * scaleSize)), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, parentSheetIndex, 16, 16)), Color.White * transparency, 0.0f, new Vector2(8f, 8f) * scaleSize, Game1.pixelZoom * scaleSize, SpriteEffects.None, layerDepth);
            var _scale = 0.5f + scaleSize;
            Game1.drawWithBorder(getStackNumber().ToString(), Color.Black, Color.White, location + new Vector2(Game1.tileSize - Game1.tinyFont.MeasureString(getStackNumber().ToString()).X * _scale, Game1.tileSize - (float)((double)Game1.tinyFont.MeasureString(getStackNumber().ToString()).Y * 3.0f / 4.0f) * _scale), 0.0f, _scale, 1f, true);
        }
        public override int salePrice()
        {
            return Item.salePrice() * stackAmount;
        }
        public override string Name
        {
            get { return stackAmount.ToString()+' '+Item.Name; }
            set { }
        }
        public int getStackNumber()
        {
            return (stack * stackAmount);
        }
        public override int maximumStackSize()
        {
            return MaxStackSize;
        }
        public new bool canStackWith(Item obj)
        {
            return obj.canStackWith(this) && obj is SObject && (Stack + obj.Stack) < maximumStackSize();
        }
        public override string getDescription()
        {
            return Item.getDescription();
        }
        public override Color getCategoryColor()
        {
            return Item.getCategoryColor();
        }
        public override string getCategoryName()
        {
            return Item.getCategoryName();
        }
        public override bool isPlaceable()
        {
            return false;
        }
        public override bool isPassable()
        {
            return false;
        }
        public SObject Clone()
        {
            var obj = new SObject(Item, stackAmount);
            obj.targetedShop = targetedShop;
            obj.requirements = requirements;

            return obj;
        }
        public override Item getOne()
        {
            return Clone();
        }
        public StardewValley.Object Revert()
        {
            return new StardewValley.Object(Item.parentSheetIndex, stackAmount * stack);
        }
    }
}
