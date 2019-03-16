using System;
using System.Linq;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace MagicJunimoPet
{
    public class MJPModEntry : Mod
    {
        internal static string TexturePath;
        internal static MagicJunimo Junimo;
        public override void Entry(IModHelper helper)
        {
            TexturePath = helper.Content.GetActualAssetKey("assets/junimo.png");
            helper.ConsoleCommands.Add("give_me_my_junimo", "Bully the game into giving you your junimo.", this.OnCommandReceived);
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;
        }
        private void SpawnJunimo()
        {
            Junimo = new MagicJunimo
            {
                currentLocation = Game1.getFarm()
            };
            Junimo.currentLocation.addCharacter(Junimo);
        }
        private void HandleHiddenJunimo(string junimo)
        {
            if (Game1.MasterPlayer.mailReceived.Contains("HasMagicJunimoPet") || Game1.MasterPlayer.mailReceived.Contains(junimo))
            {
                Game1.drawObjectDialogue(this.Helper.Translation.Get("EmptyMessage"));
                return;
            }
            // Count your jumino's
            int found = new[] { "HiddenJunimo1", "HiddenJunimo2", "HiddenJunimo3", "HiddenJunimo4", "HiddenJunimo5" }.Count(Game1.MasterPlayer.mailReceived.Contains)+1;
            // Add the current one, and play the sound
            Game1.playSound("junimoMeep1");
            Game1.MasterPlayer.mailReceived.Add(junimo);
            // Check if you've found all 5
            if (found==5)
            {
                Game1.drawObjectDialogue(this.Helper.Translation.Get("LastMessage"));
                Game1.MasterPlayer.mailReceived.Add("HasMagicJunimoPet");
            }
            // Check if this is your first
            else if(found==1)
                Game1.drawObjectDialogue(this.Helper.Translation.Get("FirstMessage"));
            // Message for 2 to 4
            else
                Game1.drawObjectDialogue(this.Helper.Translation.Get("FoundMessage", new { TotalFound = found }));
        }
        private void OnCommandReceived(string command, string[] arguments)
        {
            if (Junimo != null)
            {
                this.Monitor.Log("You already have a magic junimo.", LogLevel.Warn);
                return;
            }
            Game1.MasterPlayer.mailReceived.Add("HasMagicJunimoPet");
            this.Monitor.Log("You bully! Fine... your junimo will arrive tomorrow.", LogLevel.Alert);
        }
        private void OnButtonReleased(object s, ButtonReleasedEventArgs e)
        {
            if (Context.IsPlayerFree && e.Button.IsActionButton())
            {
                string action = Game1.currentLocation.doesTileHaveProperty((int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y, "Action", "Buildings");
                switch (action)
                {
                    case "HiddenJunimo1":
                    case "HiddenJunimo2":
                    case "HiddenJunimo3":
                    case "HiddenJunimo4":
                    case "HiddenJunimo5":
                        this.HandleHiddenJunimo(action);
                        break;
                }
            }
        }
        private void OnSaving(object s, EventArgs e)
        {
            foreach(GameLocation loc in Game1.locations)
                loc.characters.Filter(a => !(a is MagicJunimo));
        }
        private void OnDayStarted(object s, EventArgs e)
        {
            if (!Game1.MasterPlayer.mailReceived.Contains("ccDoorUnlock"))
                return;
            if(!Game1.MasterPlayer.mailReceived.Contains("HasMagicJunimoPet") && (Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") || Game1.MasterPlayer.hasCompletedCommunityCenter()) && !Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
            {
                Game1.playSound("junimoMeep1");
                DelayedAction.playSoundAfterDelay("junimoMeep1", 200);
                DelayedAction.playSoundAfterDelay("junimoMeep1", 400);
                Game1.MasterPlayer.mailReceived.Add("HasMagicJunimoPet");
            }
            if (Game1.MasterPlayer.mailReceived.Contains("HasMagicJunimoPet"))
                this.SpawnJunimo();
            Game1.getLocationFromName("Beach").setTileProperty(51, 9, "Buildings", "Action", "HiddenJunimo1");
            Game1.getLocationFromName("Mountain").setTileProperty(31, 4, "Buildings", "Action", "HiddenJunimo2");
            Game1.getLocationFromName("Forest").setTileProperty(73, 98, "Buildings", "Action", "HiddenJunimo3");
            Game1.getLocationFromName("Hospital").setTileProperty(6, 5, "Buildings", "Action", "HiddenJunimo4");
            Game1.getLocationFromName("Town").setTileProperty(92, 104, "Buildings", "Action", "HiddenJunimo5");
        }
    }
}
