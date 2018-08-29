using System;
using System.Collections.Generic;

using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using Entoarox.Framework;

namespace Entoarox.ShopExpander
{
    public class ShopExpanderMod : Mod
    {
        internal ShopExpanderConfig Config;
        private bool eventsActive = false;
        private byte skippedTick = 0;
        public override void Entry(IModHelper helper)
        {
            GameEvents.UpdateTick += this.Event_UpdateTick;
        }
        private void Event_UpdateTick(object s, EventArgs e)
        {
            if (this.skippedTick > 1)
            {
                MenuEvents.MenuChanged += this.Event_MenuChanged;
                this.Config = this.Helper.ReadConfig<ShopExpanderConfig>();
                GameEvents.UpdateTick -= this.Event_UpdateTick;
                foreach (Reference obj in this.Config.objects)
                    try
                    {
                        generateObject(obj.Owner, obj.Item, obj.Amount, obj.Conditions);
                    }
                    catch (Exception err)
                    {
                        this.Monitor.Log("Object failed to generate: " + obj.ToString(), LogLevel.Error,err);
                    }
            }
            else
                this.skippedTick++;
        }
        private void generateObject(string owner, int replacement, int stackAmount, string requirements)
        {
            if (owner == "???")
            {
                this.Monitor.Log("Attempt to add a object to a shop owned by `???`, this cant be done because `???` means the owner is unknown!", LogLevel.Error);
                return;
            }
            StardewValley.Object stack = new StardewValley.Object(replacement, stackAmount);
            if(stack.salePrice()==0)
            {
                this.Monitor.Log("Unable to add item to shop, it has no value: "+replacement, LogLevel.Error);
                return;
            }
            SObject obj = new SObject(stack, stackAmount);
            obj.targetedShop = owner;
            if (!this.affectedShops.Contains(owner))
                this.affectedShops.Add(owner);
            obj.requirements = requirements;
            //Monitor.Log($"RegisterObject({obj.Name}:{replacement}@{owner},{obj.stackAmount}*{obj.maximumStackSize()},'{requirements}')",LogLevel.Trace);
            if (AddedObjects.ContainsKey(obj.Name))
                return;
            AddedObjects.Add(obj.Name, obj);
            ReplacementStacks.Add(obj.Name, stack);
        }
        static private Dictionary<string, SObject> AddedObjects = new Dictionary<string, SObject>();
        static private Dictionary<string, Item> ReplacementStacks = new Dictionary<string, Item>();
        // If the inventory changes while this even is hooked, we need to check if any SObject instances are in it, so we can replace them
        private void Event_InventoryChanged(object send, EventArgs e)
        {
            for (int c = 0; c < Game1.player.Items.Count; c++)
            {
                if (Game1.player.Items[c] is SObject)
                {
                    SObject obj = (SObject)Game1.player.Items[c];
                    this.Monitor.Log("Reverting object: " + obj.Name+':'+obj.Stack, LogLevel.Trace);
                    Game1.player.Items[c] = obj.Revert();
                }
            }
        }
        // When the menu closes, remove the hook for the inventory changed event
        private void Event_MenuClosed(object send, EventArgsClickableMenuClosed e)
        {
            if (e.PriorMenu is ShopMenu && this.eventsActive)
            {
                PlayerEvents.InventoryChanged -= this.Event_InventoryChanged;
                MenuEvents.MenuClosed -= this.Event_MenuClosed;
            }
        }
        // Add a modified "stack" item to the shop
        private void AddItem(SObject item, string location)
        {
            // Check that makes sure only the items that the current shop is supposed to sell are added
            if (location != item.targetedShop)
            {
                this.Monitor.Log("Item(" + item.Name + ':' + item.stackAmount + '*' + item.maximumStackSize() + "){Location=false}", LogLevel.Trace);
                return;
            }
            if (!string.IsNullOrEmpty(item.requirements) && !this.Helper.Conditions().ValidateConditions(item.requirements))
            {
                this.Monitor.Log("Item(" + item.Name + ':' + item.stackAmount + '*' + item.maximumStackSize() + "){Location=true,Condition=false}", LogLevel.Trace);
                return;
            }
            if(item.stackAmount==1)
            {
                this.Monitor.Log("Item(" + item.Name + ':' + item.stackAmount + '*' + item.maximumStackSize() + "){Location=true,Condition=true,Stack=false}", LogLevel.Trace);
                Item reverted = item.Revert();
                this.forSale.Add(reverted);
                this.itemPriceAndStock.Add(reverted, new int[2] { reverted.salePrice(), int.MaxValue });
            }
            else
            {
                this.Monitor.Log("Item(" + item.Name + ':' + item.stackAmount + '*' + item.maximumStackSize() + "){Location=true,Condition=true,Stack=true}", LogLevel.Trace);
                this.forSale.Add(item);
                this.itemPriceAndStock.Add(item, new int[2] { item.salePrice(), int.MaxValue });
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
                this.Monitor.Log("Shop Menu active, checking for expansion", LogLevel.Trace);
                ShopMenu menu = (ShopMenu) Game1.activeClickableMenu;
                string shopOwner = "???";
                // There are by default two shops in the forest, and neither has a owner, so we need to manually resolve the shop owner
                if(menu.portraitPerson!=null)
                {
                    shopOwner = menu.portraitPerson.Name;
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
                        case "JojaMart":
                            shopOwner = "Joja";
                            break;
                    }
                }
                if (this.affectedShops.Contains(shopOwner))
                {
                    this.Monitor.Log("Shop owned by `"+shopOwner+"` gets modified, doing so now",LogLevel.Trace);
                    // Register to inventory changes so we can immediately replace bought stacks
                    this.eventsActive = true;
                    MenuEvents.MenuClosed += this.Event_MenuClosed;
                    PlayerEvents.InventoryChanged += this.Event_InventoryChanged;
                    // Reflect the required fields to be able to edit a shops stock
                    this.itemPriceAndStock = (Dictionary<Item, int[]>)this.Stock.GetValue(Game1.activeClickableMenu);
                    this.forSale = (List<Item>)this.Sale.GetValue(Game1.activeClickableMenu);
                    // Add our custom items to the shop
                    foreach (string key in AddedObjects.Keys)
                        AddItem(AddedObjects[key], shopOwner);
                    // Use reflection to set the changed values
                    this.Stock.SetValue(Game1.activeClickableMenu, this.itemPriceAndStock);
                    this.Sale.SetValue(Game1.activeClickableMenu, this.forSale);
                }
                else
                {
                    if(shopOwner.Equals("???"))
                        this.Monitor.Log("The shop owner could not be resolved, skipping shop", LogLevel.Trace);
                    else
                        this.Monitor.Log("The shop owned by `" + shopOwner + "` is not on the list, ignoring it");
                }
            }
        }
    }
}
