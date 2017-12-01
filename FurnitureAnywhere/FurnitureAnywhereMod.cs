using System;
using System.Reflection;
using System.Collections.Generic;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Objects;

using Entoarox.Framework;
using Entoarox.Framework.Events;

using Harmony;

using Microsoft.Xna.Framework;

namespace Entoarox.FurnitureAnywhere
{
    public class FurnitureAnywhereMod : Mod
    {
        public override void Entry(IModHelper helper)
        {
            //var harmony = HarmonyInstance.Create("Entoarox.FurnitureAnywhere");
            //harmony.PatchAll(Assembly.GetExecutingAssembly());
            this.Helper.RequestUpdateCheck("https://raw.githubusercontent.com/Entoarox/StardewMods/master/FurnitureAnywhere/update.json");
            MoreEvents.ActiveItemChanged += this.MoreEvents_ActiveItemChanged;
            LocationEvents.CurrentLocationChanged += this.TriggerItemChangedEvent;
            MenuEvents.MenuChanged += this.TriggerItemChangedEvent;
            MenuEvents.MenuClosed += this.TriggerItemChangedEvent;
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
            SaveEvents.AfterSave += this.SaveEvents_AfterSave_AfterLoad;
            SaveEvents.AfterLoad += this.SaveEvents_AfterSave_AfterLoad;
            this.Helper.Content.RegisterSerializerType<AnywhereFurniture>();
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
                this.Monitor.Log("Failed to run logic check due to unexpected error",LogLevel.Error, err);
            }
        }
        internal void TriggerItemChangedEvent(object s, EventArgs e)
        {
            MoreEvents_ActiveItemChanged(null, new EventArgsActiveItemChanged(Game1.player.CurrentItem, Game1.player.CurrentItem));
        }
        internal void SaveEvents_BeforeSave(object s, EventArgs e)
        {
            this.Monitor.Log("Packaging furniture...");
            IterateFurniture(this.SleepFurniture);
        }
        internal void SaveEvents_AfterSave_AfterLoad(object s, EventArgs e)
        {
            this.Monitor.Log("Restoring furniture...");
            IterateFurniture(this.WakeupFurniture);
        }
        internal void IterateFurniture(Action<GameLocation> handler)
        {
            for (int c= 0;c<Game1.locations.Count;c++)
            {
                var loc = Game1.locations[c];
                handler(loc);
                if (loc is StardewValley.Locations.BuildableGameLocation)
                    foreach (StardewValley.Buildings.Building build in (loc as StardewValley.Locations.BuildableGameLocation).buildings)
                        if (build.indoors != null)
                            handler(build.indoors);
            }
        }
        internal void SleepFurniture(GameLocation loc)
        {
            List<Vector2> Positions = new List<Vector2>();
            foreach (KeyValuePair<Vector2,StardewValley.Object> obj in loc.objects)
                if (obj.Value is AnywhereFurniture)
                    Positions.Add(obj.Key);
            foreach (Vector2 pos in Positions)
                loc.objects[pos] = new Chest(false)
                {
                    items=new List<Item>() { (loc.objects[pos] as AnywhereFurniture).Revert() },
                    type = "AnywhereSerializedContainer",
                    tileLocation = pos
                };
        }
        internal void WakeupFurniture(GameLocation loc)
        {
            List<Vector2> Positions = new List<Vector2>();
            foreach (KeyValuePair<Vector2, StardewValley.Object> obj in loc.objects)
                if (obj.Value is Chest && (obj.Value as Chest).type.Equals("AnywhereSerializedContainer"))
                    Positions.Add(obj.Key);
            foreach (Vector2 pos in Positions)
                loc.objects[pos] = new AnywhereFurniture((loc.objects[pos] as Chest).items[0] as Furniture)
                {
                    tileLocation = pos
                };
        }
    }
}
