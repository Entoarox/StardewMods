namespace Entoarox.Framework.Core
{
    internal class FrameworkConfig
    {
        //[JsonComment("If the Game Patcher module should be loaded, this fixes bugs in vanilla SDV that affect modding, but in some cases has been known to cause issues.")]
        public bool GamePatcher = true;
        //[JsonComment("When true, the game will skip past the credits and straight to the main menu.")]
        public bool SkipCredits = false;
        //[JsonComment("If update notifications should show up in the game, or only in the console.")]
        public bool IngameUpdateNotices = true;
        //[JsonComment("If extra, possibly somewhat cheaty console commands should be enabled.")]
        public bool TrainerCommands = true;
        //[JsonComment("If every location should be treated like the greenhouse when it comes to crops.")]
        public bool GreenhouseEverywhere = false;
        //[JsonComment("If furniture should be allowed to be placed in every location, rather then just the House and in Sheds.")]
        public bool DecorateEverywhere = false;
    }
}