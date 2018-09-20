using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entoarox.AdvancedLocationLoader.Configs;
using Entoarox.Framework;
using StardewModdingAPI;
using StardewValley;
using Warp = Entoarox.AdvancedLocationLoader.Configs.Warp;

namespace Entoarox.AdvancedLocationLoader.Processing
{
    /// <summary>Reads config data from content packs.</summary>
    internal class ConfigReader
    {
        /*********
        ** Fields
        *********/
        /// <summary>The condition names that are already taken.</summary>
        private readonly HashSet<string> AddedConditionNames = new HashSet<string>();

        /// <summary>The teleporters which have already been added.</summary>
        private readonly List<TeleporterList> AddedTeleporters = new List<TeleporterList>();

        /// <summary>The affected locations.</summary>
        private readonly HashSet<string> AffectedLocations = new HashSet<string>();

        /// <summary>The valid location types.</summary>
        private readonly HashSet<string> LocationTypes = new HashSet<string> { "Default", "Cellar", "Greenhouse", "Sewer", "BathHousePool", "Desert", "Decoratable" };

        /// <summary>The valid layer values.</summary>
        private readonly HashSet<string> ValidLayers = new HashSet<string> { "Back", "Buildings", "Paths", "Front", "AlwaysFront" };

        /// <summary>Writes messages to the log.</summary>
        private readonly IMonitor Monitor;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="monitor">Writes messages to the log.</param>
        public ConfigReader(IMonitor monitor)
        {
            this.Monitor = monitor;
        }

        /// <summary>Read config data from a content pack.</summary>
        /// <param name="contentPack">The content pack to load.</param>
        public ContentPackData Load(IContentPack contentPack)
        {
            LocationConfig config = this.ReadConfig(contentPack);
            if (config == null)
                return null;

            return this.ValidateData(contentPack, config);
        }


        /*********
        ** Protected methods
        *********/
        /// <summary>Read config data from a content pack.</summary>
        /// <param name="contentPack">The content pack to load.</param>
        private LocationConfig ReadConfig(IContentPack contentPack)
        {
            // get config path
            string configPath;
            if (File.Exists(Path.Combine(contentPack.DirectoryPath, "locations.json")))
                configPath = "locations.json"; // SMAPI content pack
            else if (File.Exists(Path.Combine(contentPack.DirectoryPath, "manifest.json")))
                configPath = "manifest.json"; // ALL location mod
            else
            {
                this.Monitor.Log("   Skipped: can't find a locations.json or manifest.json file.", LogLevel.Error);
                return null;
            }

            // get format version
            ISemanticVersion formatVersion;
            try
            {
                string rawVersion = contentPack.ReadJsonFile<LoaderVersionConfig>(configPath)?.LoaderVersion;
                if (rawVersion == null)
                {
                    this.Monitor.Log($"   Skipped: config doesn't specify a {nameof(LoaderVersionConfig.LoaderVersion)} field.", LogLevel.Error);
                    return null;
                }
                formatVersion = new SemanticVersion(rawVersion);
            }
            catch (Exception ex)
            {
                this.Monitor.Log("   Skipped: can't parse config file to check loader version.", LogLevel.Error, ex);
                return null;
            }

            // read data
            switch ($"{formatVersion.MajorVersion}.{formatVersion.MinorVersion}")
            {
                case "1.1":
                    return this.ReadConfig_1_1(contentPack, configPath);
                case "1.2":
                    return this.ReadConfig_1_2(contentPack, configPath);
                default:
                    this.Monitor.Log($"Skipped {contentPack.Manifest.Name}: config file format {formatVersion} isn't supported, must be 1.1 or 1.2.", LogLevel.Error);
                    return null;
            }
        }

        /// <summary>Validate the configuration for a content pack and remove invalid settings.</summary>
        /// <param name="contentPack">The content pack to load.</param>
        /// <param name="config">The config settings.</param>
        private ContentPackData ValidateData(IContentPack contentPack, LocationConfig config)
        {
            ContentPackData data = new ContentPackData { ContentPack = contentPack };

            string currentStep = "Entry";
            try
            {
                // validate locations
                if (config.Locations != null)
                {
                    currentStep = "Locations";
                    foreach (Location location in config.Locations)
                    {
                        if (!this.AssertFileExists(contentPack, "location", location.FileName))
                            continue;
                        if (!this.AffectedLocations.Add(location.MapName))
                        {
                            this.Monitor.Log($"   Skipped {location}: that map is already being modified.", LogLevel.Error);
                            continue;
                        }

                        if (!this.LocationTypes.Contains(location.Type))
                        {
                            this.Monitor.Log($"   Location {location} has unknown type, using 'Default' instead.", LogLevel.Warn);
                            location.Type = "Default";
                        }

                        data.Locations.Add(location);
                    }
                }

                // validate overrides
                if (config.Overrides != null)
                {
                    currentStep = "Overrides";
                    foreach (Override @override in config.Overrides)
                    {
                        if (!this.AssertFileExists(contentPack, "override", @override.FileName))
                            continue;
                        if (!this.AffectedLocations.Add(@override.MapName))
                        {
                            this.Monitor.Log($"   Skipped {@override}: that map is already being modified.", LogLevel.Error);
                            continue;
                        }

                        data.Overrides.Add(@override);
                    }
                }

                // validate redirects
                if (config.Redirects != null)
                {
                    currentStep = "Redirects";
                    foreach (Redirect redirect in config.Redirects)
                    {
                        if (!File.Exists(Path.Combine(Game1.content.RootDirectory, $"{redirect.FromFile}.xnb")))
                        {
                            this.Monitor.Log($"   Skipped {redirect}: file {redirect.FromFile}.xnb doesn't exist in the game's content folder.", LogLevel.Error);
                            continue;
                        }

                        if (!this.AssertFileExists(contentPack, "redirect", redirect.ToFile))
                            continue;

                        data.Redirects.Add(redirect);
                    }
                }

                // validate tilesheets
                if (config.Tilesheets != null)
                {
                    currentStep = "Tilesheets";
                    foreach (Tilesheet tilesheet in config.Tilesheets)
                    {
                        if (tilesheet.FileName != null)
                        {
                            if (tilesheet.Seasonal)
                            {
                                bool filesExist =
                                    this.AssertFileExists(contentPack, "tilesheet", $"{tilesheet.FileName}_spring")
                                    && this.AssertFileExists(contentPack, "tilesheet", $"{tilesheet.FileName}_summer")
                                    && this.AssertFileExists(contentPack, "tilesheet", $"{tilesheet.FileName}_fall")
                                    && this.AssertFileExists(contentPack, "tilesheet", $"{tilesheet.FileName}_winter");
                                if (!filesExist)
                                    continue;
                            }
                            else if (!this.AssertFileExists(contentPack, "tilesheet", tilesheet.FileName))
                                continue;
                        }

                        data.Tilesheets.Add(tilesheet);
                    }
                }

                // validate tiles
                if (config.Tiles != null)
                {
                    currentStep = "Tiles";
                    foreach (Tile tile in config.Tiles)
                    {
                        if (!this.ValidLayers.Contains(tile.LayerId))
                        {
                            this.Monitor.Log($"   Skipped {tile}: unknown layer '{tile.LayerId}'.", LogLevel.Error);
                            continue;
                        }

                        data.Tiles.Add(tile);
                    }
                }

                // validate properties
                if (config.Properties != null)
                {
                    currentStep = "Properties";
                    foreach (Property property in config.Properties)
                    {
                        if (!this.ValidLayers.Contains(property.LayerId))
                        {
                            this.Monitor.Log($"   Skipped `{property}`: unknown layer '{property.LayerId}'.",
                                LogLevel.Error);
                            continue;
                        }

                        data.Properties.Add(property);
                    }
                }

                // validate warps
                if (config.Warps != null)
                {
                    currentStep = "Warps";
                    foreach (Warp warp in config.Warps)
                        data.Warps.Add(warp);
                }

                // validate conditionals
                if (config.Conditionals != null)
                {
                    currentStep = "Conditionals";
                    foreach (Conditional condition in config.Conditionals)
                    {
                        if (condition.Item < -1)
                        {
                            this.Monitor.Log($"   Skipped {condition}, references null item.", LogLevel.Error);
                            continue;
                        }

                        if (condition.Amount < 1)
                        {
                            this.Monitor.Log($"   Skipped {condition}, item amount can't be less then 1.", LogLevel.Error);
                            continue;
                        }

                        if (!this.AddedConditionNames.Add(condition.Name))
                        {
                            this.Monitor.Log($"   Skipped {condition.Name}, another condition with this name already exists.", LogLevel.Error);
                            continue;
                        }

                        data.Conditionals.Add(condition);
                    }
                }

                // validate minecarts
                if (config.Teleporters != null)
                {
                    currentStep = "Teleporters";
                    foreach (TeleporterList list in config.Teleporters)
                    {
                        bool valid = true;
                        foreach (TeleporterList prevList in this.AddedTeleporters)
                        {
                            if (prevList.ListName == list.ListName)
                            {
                                valid = false;
                                foreach (TeleporterDestination dest in list.Destinations)
                                {
                                    if (prevList.Destinations.TrueForAll(a => !a.Equals(dest)))
                                        prevList.Destinations.Add(dest);
                                    else
                                        this.Monitor.Log($"   Can't add teleporter destination for the `{list.ListName}` teleporter, the destination already exists: `{dest}`.", LogLevel.Error);
                                }
                                this.Monitor.Log($"   Teleporter updated: {prevList}", LogLevel.Trace);
                                break;
                            }
                        }
                        if (valid)
                        {
                            this.AddedTeleporters.Add(list);
                            this.Monitor.Log($"   Teleporter created: {list}", LogLevel.Trace);
                        }
                    }
                }

                // validate shops
                if (config.Shops != null)
                {
                    currentStep = "Shops";
                    foreach (string shop in config.Shops)
                    {
                        try
                        {
                            ShopConfig shopConfig = contentPack.ReadJsonFile<ShopConfig>($"{shop}.json");
                            if (shopConfig == null)
                            {
                                this.Monitor.Log($"   Skipped shop '{shop}.json': file does not exist.", LogLevel.Error);
                                continue;
                            }
                            shopConfig.Name = shop;
                            data.Shops.Add(shopConfig);
                        }
                        catch (Exception ex)
                        {
                            this.Monitor.Log($"   Skipped shop '{shop}.json': unexpected error parsing file.", LogLevel.Error, ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"   Failed validating config (step: {currentStep}).", LogLevel.Error, ex);
            }

            return data;
        }

        /// <summary>Load data from a version 1.1 manifest.</summary>
        /// <param name="contentPack">The content pack to load.</param>
        /// <param name="configPath">The content pack's relative config file path.</param>
        private LocationConfig ReadConfig_1_1(IContentPack contentPack, string configPath)
        {
            // read raw data
            LocationConfig_1_1 raw;
            try
            {
                raw = contentPack.ReadJsonFile<LocationConfig_1_1>(configPath);
            }
            catch (Exception err)
            {
                this.Monitor.Log("   Skipped: can't parse config file (version 1.1).", LogLevel.Warn, err);
                return null;
            }

            // parse
            LocationConfig config = new LocationConfig();
            {
                // convert locations
                if (raw.Locations != null)
                {
                    foreach (IDictionary<string, string> location in raw.Locations)
                    {
                        config.Locations.Add(new Location
                        {
                            Farmable = location.ContainsKey("farmable") && Convert.ToBoolean(location["farmable"]),
                            Outdoor = location.ContainsKey("outdoor") && Convert.ToBoolean(location["outdoor"]),
                            FileName = location["file"],
                            MapName = location["name"],
                            Type = "Default"
                        });
                    }
                }

                // convert overrides
                if (raw.Overrides != null)
                {
                    foreach (IDictionary<string, string> @override in raw.Overrides)
                    {
                        config.Overrides.Add(new Override
                        {
                            FileName = @override["file"],
                            MapName = @override["name"]
                        });
                    }
                }

                // convert tilesheets
                if (raw.Tilesheets != null)
                {
                    foreach (IDictionary<string, string> sheet in raw.Tilesheets)
                    {
                        config.Tilesheets.Add(new Tilesheet
                        {
                            FileName = sheet["file"],
                            MapName = sheet["map"],
                            SheetId = sheet["sheet"],
                            Seasonal = sheet.ContainsKey("seasonal") && Convert.ToBoolean(sheet["seasonal"])
                        });
                    }
                }

                // convert tiles
                if (raw.Tiles != null)
                {
                    foreach (IDictionary<string, string> tile in raw.Tiles)
                    {
                        Tile newTile = new Tile
                        {
                            TileX = Convert.ToInt32(tile["x"]),
                            TileY = Convert.ToInt32(tile["y"]),
                            MapName = tile["map"],
                            LayerId = tile["layer"],
                            SheetId = tile.ContainsKey("sheet") ? tile["sheet"] : null
                        };

                        if (tile.ContainsKey("interval"))
                        {
                            newTile.Interval = Convert.ToInt32(tile["interval"]);
                            newTile.TileIndexes = tile["tileIndex"].Split(',').Select(p => Convert.ToInt32(p)).ToArray();
                        }
                        else
                            newTile.TileIndex = Convert.ToInt32(tile["tileIndex"]);

                        newTile.Conditions = tile.ContainsKey("conditions") ? tile["conditions"] : null;
                        config.Tiles.Add(newTile);
                    }
                }

                // convert properties
                if (raw.Properties != null)
                {
                    foreach (IList<string> property in raw.Properties)
                    {
                        config.Properties.Add(new Property
                        {
                            MapName = property[0],
                            LayerId = property[1],
                            TileX = Convert.ToInt32(property[2]),
                            TileY = Convert.ToInt32(property[3]),
                            Key = property[4],
                            Value = property[5]
                        });
                    }
                }

                // convert warps
                if (raw.Warps != null)
                {
                    foreach (IList<string> warp in raw.Warps)
                    {
                        config.Warps.Add(new Warp
                        {
                            MapName = warp[0],
                            TileX = Convert.ToInt32(warp[1]),
                            TileY = Convert.ToInt32(warp[2]),
                            TargetName = warp[3],
                            TargetX = Convert.ToInt32(warp[4]),
                            TargetY = Convert.ToInt32(warp[5])
                        });
                    }
                }

                // convert conditions
                if (raw.Conditions != null)
                {
                    foreach (KeyValuePair<string, IDictionary<string, string>> condition in raw.Conditions)
                    {
                        config.Conditionals.Add(new Conditional
                        {
                            Name = condition.Key,
                            Item = Convert.ToInt32(condition.Value["item"]),
                            Amount = Convert.ToInt32(condition.Value["amount"]),
                            Question = condition.Value["question"]
                        });
                    }
                }

                // convert minecarts
                if (raw.Minecarts != null)
                {
                    foreach (KeyValuePair<string, IDictionary<string, IList<string>>> set in raw.Minecarts)
                    {
                        TeleporterList newSet = new TeleporterList
                        {
                            ListName = set.Key
                        };
                        foreach (KeyValuePair<string, IList<string>> destination in set.Value)
                        {
                            newSet.Destinations.Add(new TeleporterDestination
                            {
                                ItemText = destination.Key,
                                MapName = destination.Value[0],
                                TileX = Convert.ToInt32(destination.Value[1]),
                                TileY = Convert.ToInt32(destination.Value[2]),
                                Direction = Convert.ToInt32(destination.Value[3])
                            });
                        }

                        config.Teleporters.Add(newSet);
                    }
                }

                // convert shops
                config.Shops = raw.Shops;
            }

            return config;
        }

        /// <summary>Load data from a version 1.1 manifest.</summary>
        /// <param name="contentPack">The content pack to load.</param>
        /// <param name="configPath">The content pack's relative config file path.</param>
        private LocationConfig ReadConfig_1_2(IContentPack contentPack, string configPath)
        {
            try
            {
                // read raw data
                LocationConfig config = contentPack.ReadJsonFile<LocationConfig>(configPath);

                // load child config
                if (config.Includes.Any())
                {
                    foreach (string include in config.Includes)
                    {
                        // parse file
                        string childPath = $"{include}.json";
                        LocationConfig childConfig;
                        try
                        {
                            childConfig = contentPack.LoadAsset<LocationConfig>(childPath);
                        }
                        catch (Exception err)
                        {
                            this.Monitor.Log($"   Skipped child config '{childPath}' because it can't be parsed.", LogLevel.Error, err);
                            continue;
                        }

                        // add data to parent config
                        foreach (Conditional entry in childConfig.Conditionals)
                            config.Conditionals.Add(entry);
                        foreach (Location entry in childConfig.Locations)
                            config.Locations.Add(entry);
                        foreach (Override entry in childConfig.Overrides)
                            config.Overrides.Add(entry);
                        foreach (Property entry in childConfig.Properties)
                            config.Properties.Add(entry);
                        foreach (Redirect entry in childConfig.Redirects)
                            config.Redirects.Add(entry);
                        foreach (string entry in childConfig.Shops)
                            config.Shops.Add(entry);
                        foreach (TeleporterList entry in childConfig.Teleporters)
                            config.Teleporters.Add(entry);
                        foreach (Tile entry in childConfig.Tiles)
                            config.Tiles.Add(entry);
                        foreach (Tilesheet entry in childConfig.Tilesheets)
                            config.Tilesheets.Add(entry);
                        foreach (Warp entry in childConfig.Warps)
                            config.Warps.Add(entry);
                    }
                }

                return config;
            }
            catch (Exception err)
            {
                this.Monitor.Log("   Skipped: can't parse config file (version 1.2).", LogLevel.Warn, err);
                return null;
            }
        }

        /// <summary>Assert that a file exist, logging a skip message if it doesn't.</summary>
        /// <param name="contentPack">The content path to search.</param>
        /// <param name="type">The category being loaded (like 'redirect').</param>
        /// <param name="path">The path to check.</param>
        /// <returns>Returns whether the file exists.</returns>
        private bool AssertFileExists(IContentPack contentPack, string type, string path)
        {
            if (!File.Exists(Path.Combine(contentPack.DirectoryPath, $"{path}.xnb")))
            {
                this.Monitor.Log($"   Skipped {type}: file {path}.xnb doesn't exist in the content pack.", LogLevel.Error);
                return false;
            }

            return true;
        }
    }
}
