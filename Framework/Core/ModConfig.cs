namespace Entoarox.Framework.Core
{
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>When true, the game will skip past the credits and straight to the main menu.</summary>
        public bool SkipCredits = false;

        /// <summary>If extra, possibly somewhat cheaty console commands should be enabled.</summary>
        public bool TrainerCommands = true;

        /// <summary>If every location should be treated like the greenhouse when it comes to crops.</summary>
        public bool GreenhouseEverywhere = false;
    }
}
