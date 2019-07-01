using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Entoarox.Framework;
using Entoarox.Framework.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace Entoarox.FurnitureAnywhere
{
    /// <summary>The mod entry class.</summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by SMAPI.")]
    internal class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ItemEvents.ActiveItemChanged += this.MoreEvents_ActiveItemChanged;
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.Saved += this.OnSaved;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            this.Helper.Content.RegisterSerializerType<AnywhereFurniture>();
        }


        /*********
        ** Protected methods
        *********/
        private void RestoreVanillaObjects()
        {
            for (int c = 0; c < Game1.player.Items.Count; c++)
            {
                if (Game1.player.Items[c] != null && Game1.player.Items[c] is AnywhereFurniture)
                    Game1.player.Items[c] = (Game1.player.Items[c] as AnywhereFurniture).Revert();
            }
        }

        private void InitSpecialObject(Item i)
        {
            for (int c = 0; c < Game1.player.Items.Count; c++)
            {
                if (Game1.player.Items[c] != null && Game1.player.Items[c].Equals(i))
                    Game1.player.Items[c] = new AnywhereFurniture(Game1.player.Items[c] as Furniture);
            }
        }

        private void MoreEvents_ActiveItemChanged(object s, EventArgsActiveItemChanged e)
        {
            try
            {
                if (e.OldItem is AnywhereFurniture)
                    this.RestoreVanillaObjects();
                if (e.NewItem is Furniture && !(Game1.currentLocation is DecoratableLocation) && Game1.activeClickableMenu == null)
                    this.InitSpecialObject(e.NewItem);
            }
            catch (Exception err)
            {
                this.Monitor.Log("Failed to run logic check due to unexpected error", LogLevel.Error, err);
            }
        }

        /// <summary>Raised after a player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            this.TriggerItemChangedEvent();
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            this.TriggerItemChangedEvent();
        }

        private void TriggerItemChangedEvent()
        {
            this.MoreEvents_ActiveItemChanged(null, new EventArgsActiveItemChanged(Game1.player.CurrentItem, Game1.player.CurrentItem));
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            this.Monitor.Log("Packaging furniture...");
            this.IterateFurniture(this.SleepFurniture);
        }

        /// <summary>Raised after the game finishes writing data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaved(object sender, SavedEventArgs e)
        {
            this.Monitor.Log("Restoring furniture...");
            this.IterateFurniture(this.WakeupFurniture);
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, EventArgs e)
        {
            this.Monitor.Log("Restoring furniture...");
            this.IterateFurniture(this.WakeupFurniture);
        }

        private void IterateFurniture(Action<GameLocation> handler)
        {
            foreach (GameLocation loc in Game1.locations)
            {
                handler(loc);
                if (!(loc is BuildableGameLocation)) continue;
                foreach (Building build in (loc as BuildableGameLocation).buildings)
                {
                    if (build.indoors.Value != null)
                        handler(build.indoors.Value);
                }
            }
        }

        private void SleepFurniture(GameLocation loc)
        {
            List<Vector2> positions = new List<Vector2>();
            foreach (KeyValuePair<Vector2, SObject> obj in loc.objects.Pairs)
                if (obj.Value is AnywhereFurniture)
                    positions.Add(obj.Key);
            foreach (Vector2 pos in positions)
            {
                Chest chest = new Chest(false)
                {
                    Type = "AnywhereSerializedContainer",
                    TileLocation = pos
                };
                chest.items.Set(new List<Item> { (loc.objects[pos] as AnywhereFurniture).Revert() });
                loc.objects[pos] = chest;
            }
        }

        private void WakeupFurniture(GameLocation loc)
        {
            List<Vector2> positions = new List<Vector2>();
            foreach (KeyValuePair<Vector2, SObject> obj in loc.objects.Pairs)
                if (obj.Value is Chest chest && chest.Type.Equals("AnywhereSerializedContainer"))
                    positions.Add(obj.Key);
            foreach (Vector2 pos in positions)
            {
                loc.objects[pos] = new AnywhereFurniture((loc.objects[pos] as Chest).items[0] as Furniture)
                {
                    TileLocation = pos
                };
            }
        }
    }
}
