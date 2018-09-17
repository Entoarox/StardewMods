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
            PlayerEvents.Warped += this.TriggerItemChangedEvent;
            MenuEvents.MenuChanged += this.TriggerItemChangedEvent;
            MenuEvents.MenuClosed += this.TriggerItemChangedEvent;
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
            SaveEvents.AfterSave += this.SaveEvents_AfterSave_AfterLoad;
            SaveEvents.AfterLoad += this.SaveEvents_AfterSave_AfterLoad;
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

        private void TriggerItemChangedEvent(object s, EventArgs e)
        {
            this.MoreEvents_ActiveItemChanged(null, new EventArgsActiveItemChanged(Game1.player.CurrentItem, Game1.player.CurrentItem));
        }

        private void SaveEvents_BeforeSave(object s, EventArgs e)
        {
            this.Monitor.Log("Packaging furniture...");
            this.IterateFurniture(this.SleepFurniture);
        }

        private void SaveEvents_AfterSave_AfterLoad(object s, EventArgs e)
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
