using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Entoarox.AdvancedLocationLoader2
{
    /// <summary>The mod entry class.</summary>
    public class ALLModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.LoadContent();
        }

        private void LoadContent()
        {
            foreach (var pack in this.Helper.ContentPacks.GetOwned())
            {

            }
        }
    }
}
