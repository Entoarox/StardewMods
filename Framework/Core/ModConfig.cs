namespace Entoarox.Framework.Core
{
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>If the Game Patcher module should be loaded, this fixes bugs in vanilla SDV that affect modding, but in some cases has been known to cause issues.</summary>
        public bool GamePatcher = true;

        /// <summary>When true, the game will skip past the credits and straight to the main menu.</summary>
        public bool SkipCredits = false;

        /// <summary>If extra, possibly somewhat cheaty console commands should be enabled.</summary>
        public bool TrainerCommands = true;

        /// <summary>If every location should be treated like the greenhouse when it comes to crops.</summary>
        public bool GreenhouseEverywhere = false;

        /// <summary>If furniture should be allowed to be placed in every location, rather then just the House and in Sheds.</summary>
        public bool DecorateEverywhere = false;
    }
}
