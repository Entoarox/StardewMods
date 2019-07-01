using System;
using System.Linq;
using System.Collections.Generic;
using Entoarox.CustomBooks.Menu;
using Entoarox.Framework.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Entoarox.CustomBooks
{
    /// <summary>The mod entry class.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Accessors
        *********/
        public static Bookshelf Shelf;
        public static IModHelper SHelper;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            SHelper = helper;
            ItemEvents.AfterSerialize += this.ItemEvents_AfterSerialize;
            ItemEvents.BeforeDeserialize += this.ItemEvents_BeforeDeserialize;
            helper.Events.Input.ButtonReleased += this.InputEvents_ButtonReleased;
            helper.ConsoleCommands.Add("custombooks", "custombooks (spawn|clear)", this.Command_Custombooks);
        }


        /*********
        ** Protected methods
        *********/
        private void ItemEvents_AfterSerialize(object s, EventArgs e)
        {
            this.Monitor.Log("Saving custom book data...", LogLevel.Trace);
        }

        private void ItemEvents_BeforeDeserialize(object s, EventArgs e)
        {
            this.Monitor.Log("Reading custom book data...", LogLevel.Trace);
            ModEntry.Shelf = new Bookshelf();
        }

        private void InputEvents_ButtonReleased(object s, ButtonReleasedEventArgs e)
        {
            if ((!Game1.eventUp || Game1.currentLocation.currentEvent != null && Game1.currentLocation.currentEvent.showActiveObject) && !Game1.player.FarmerSprite.pauseForSingleAnimation && !Game1.player.isRidingHorse() && !Game1.player.bathingClothes.Value && e.Button == SButton.MouseRight)
                (Game1.player.CurrentItem as Book)?.Activate();
        }

        private void Command_Custombooks(string cmd, string[] args)
        {
            switch (args[0])
            {
                case "spawn":
                    string id = ModEntry.Shelf.CreateBook();
                    ModEntry.Shelf.Books[id].SetPages(new List<Page> { new TextPage("This is a book added using the debug command.") });
                    Game1.player.addItemToInventory(new Book(id));
                    break;
                case "clear":
                    Game1.player.Items=Game1.player.Items.Where(a => !(a is Book)).ToList();
                    break;
            }
        }
    }
}
