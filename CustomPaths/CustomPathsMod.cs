using System;
using System.Collections.Generic;
using System.Linq;
using Entoarox.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace Entoarox.CustomPaths
{
    /// <summary>The mod entry class.</summary>
    public class CustomPathsMod : Mod
    {
        public static Dictionary<string, CustomPathInfo> Map = new Dictionary<string, CustomPathInfo>();
        private static readonly string[] Seasons = { "spring", "summer", "fall", "winter" };
        private PlayerModifier Modifier;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var helper = this.Helper;

            foreach (IContentPack pack in helper.ContentPacks.GetOwned())
            {
                foreach (CustomPathConfig path in pack.ReadJsonFile<List<CustomPathConfig>>("paths.json"))
                {
                    string key = pack.Manifest.UniqueID + "<" + path.Name + ">";
                    if (CustomPathsMod.Map.ContainsKey(key))
                    {
                        this.Monitor.Log("Encountered duplicate path name `" + path.Name + "` in the `" + pack.Manifest.UniqueID + "` Content Pack, duplicate is being skipped", LogLevel.Warn);
                        continue;
                    }

                    if (path.Seasonal)
                    {
                        Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
                        foreach (string season in CustomPathsMod.Seasons)
                            textures.Add(season, pack.LoadAsset<Texture2D>(path.File + "_" + season + ".png"));
                        CustomPathsMod.Map.Add(key, new CustomPathInfo(textures, path));
                    }
                    else
                        CustomPathsMod.Map.Add(key, new CustomPathInfo(pack.LoadAsset<Texture2D>(path.File + ".png"), path));

                    this.Monitor.Log("Path added: " + key, LogLevel.Trace);
                }
            }

            if (CustomPathsMod.Map.Any(a => !string.IsNullOrEmpty(a.Value.Salesman)))
            {
                this.Monitor.Log("One or more paths are for sale, enabling menu hook", LogLevel.Trace);
                helper.Events.Display.MenuChanged += this.Event_MenuChanged;
            }

            if (CustomPathsMod.Map.Any(a => a.Value.Speed > 0))
            {
                this.Monitor.Log("One or more paths give a speed boost, enabling update hook", LogLevel.Trace);
                helper.Events.GameLoop.UpdateTicked += this.GameEvents_UpdateTick;
            }
        }

        public void GameEvents_UpdateTick(object s, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            Vector2 pos = Game1.player.getTileLocation();
            if (Game1.currentLocation.terrainFeatures.ContainsKey(pos) && Game1.currentLocation.terrainFeatures != null && Game1.currentLocation.terrainFeatures[pos] is CustomPath path)
            {
                int boost = CustomPathsMod.Map[path.Id].Speed;
                if (this.Modifier != null)
                {
                    if (this.Modifier.RunSpeedModifier == boost)
                        return;
                    this.Helper.Player().Modifiers.Remove(this.Modifier);
                }

                this.Modifier = new PlayerModifier
                {
                    RunSpeedModifier = boost,
                    WalkSpeedModifier = boost
                };
                this.Helper.Player().Modifiers.Add(this.Modifier);
            }
            else if (this.Modifier != null)
            {
                this.Helper.Player().Modifiers.Remove(this.Modifier);
                this.Modifier = null;
            }
        }

        private void Event_MenuChanged(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu is ShopMenu)
            {
                // When it is a shop menu, I need to perform some logic to identify the HatMouse, Traveler or ClintUpgrade shops as I cannot simply use their owner for that
                this.Monitor.Log("Shop Menu active, checking for inclusion", LogLevel.Trace);
                ShopMenu menu = (ShopMenu)Game1.activeClickableMenu;
                string shopOwner = "???";
                // There are by default two shops in the forest, and neither has a owner, so we need to manually resolve the shop owner
                if (menu.portraitPerson != null)
                {
                    shopOwner = menu.portraitPerson.Name;
                    // Clint has two shops, we need to check if this is the tool upgrade shop and modify the owner if that is the case
                    if (shopOwner == "Clint" && menu.potraitPersonDialogue == "I can upgrade your tools with more power. You'll have to leave them with me for a few days, though.")
                        shopOwner = "ClintUpgrade";
                }
                else
                    switch (Game1.currentLocation.Name)
                    {
                        case "Forest":
                            if (menu.potraitPersonDialogue == "Hiyo, poke. Did you bring coins? Gud. Me sell hats.")
                                shopOwner = "HatMouse";
                            else
                            {
                                // The merchant is a bit harder to determine then the mouse
                                List<string> matches = new List<string>
                                {
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

                if (CustomPathsMod.Map.Any(a => a.Value.Salesman.Equals(shopOwner)))
                {
                    IReflectedField<Dictionary<Item, int[]>> refStock = this.Helper.Reflection.GetField<Dictionary<Item, int[]>>(Game1.activeClickableMenu, "itemPriceAndStock");
                    IReflectedField<List<Item>> refSale = this.Helper.Reflection.GetField<List<Item>>(Game1.activeClickableMenu, "forSale");
                    this.Monitor.Log("Shop owned by `" + shopOwner + "` gets edited, adding paths", LogLevel.Trace);
                    Dictionary<Item, int[]> stock = refStock.GetValue();
                    List<Item> sale = refSale.GetValue();
                    // Add our custom items to the shop
                    foreach (KeyValuePair<string, CustomPathInfo> item in CustomPathsMod.Map.Where(a => a.Value.Salesman.Equals(shopOwner) && (string.IsNullOrEmpty(a.Value.Requirements) || this.Helper.Conditions().ValidateConditions(a.Value.Requirements))))
                    {
                        CustomPathObject obj = new CustomPathObject(item.Key) { Stack = int.MaxValue };
                        stock.Add(obj, new[] { item.Value.Price * 2, int.MaxValue });
                        sale.Add(obj);
                    }

                    refStock.SetValue(stock);
                    refSale.SetValue(sale);
                }
                else
                {
                    if (shopOwner.Equals("???"))
                        this.Monitor.Log("The shop owner could not be resolved, skipping shop", LogLevel.Trace);
                    else
                        this.Monitor.Log("The shop owned by `" + shopOwner + "` is not on the list, ignoring it");
                }
            }
        }
    }
}
