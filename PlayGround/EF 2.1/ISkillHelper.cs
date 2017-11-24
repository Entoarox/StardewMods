using SFarmer= StardewValley.Farmer;

namespace Entoarox.Framework
{
    // Accessible as IModHelper::Skills()
    interface ISkillHelper
    {
        /// <summary>
        /// Gets the experience required at the given level in order to reach the next level
        /// </summary>
        /// <param name="level">The current level</param>
        /// <returns></returns>
        int TotalExperienceRequiredForLevel(string skillName, int level);
        /// <summary>
        /// Register a new skill
        /// </summary>
        /// <param name="skillName">The internal name used for this skill</param>
        /// <param name="skill">All the properties of the skill</param>
        /// <returns></returns>
        void RegisterSkill(string skillName, ISkill skill);
        /// <summary>
        /// Retrieves the properties for a given skill, or `null` if no such skill exists
        /// </summary>
        /// <param name="skillName">The internal name of the skill</param>
        /// <returns></returns>
        ISkill GetSkillInfo(string skillName);
        /// <summary>
        /// Returns `true` if the given skill is currently earning prestige experience instead of level experience
        /// </summary>
        /// <param name="skillName">The internal name of the skill</param>
        /// <returns></returns>
        bool IsActivePrestigeSkill(SFarmer player, string skillName);
        /// <summary>
        /// Add experience to a skill
        /// Levelups and prestige is automatically handled by the API
        /// </summary>
        /// <param name="player">The player instance of the wanted player</param>
        /// <param name="skillName">The internal name of the skill</param>
        /// <param name="experience">How much experience should be added</param>
        void AddSkillExperience(SFarmer player, string skillName, int experience);
        /// <summary>
        /// Gives a free profession point for the player's use in unlocking any available profession in the selected skill the next time they sleep
        /// </summary>
        /// <param name="player">The player instance of the wanted player</param>
        /// <param name="skillName">The internal name of the skill</param>
        void GiveProfessionPoint(SFarmer player, string skillName);
        /// <summary>
        /// Gets the current skill level of a player
        /// Does not account for prestige levels
        /// </summary>
        /// <param name="player">The player instance of the wanted player</param>
        /// <param name="skillName">The internal name of the skill</param>
        /// <returns></returns>
        int GetSkillLevel(SFarmer player, string skillName);
        /// <summary>
        /// Gets the current prestige level of a player
        /// Does not account for skill levels
        /// </summary>
        /// <param name="player">The player instance of the wanted player</param>
        /// <param name="skillName">The internal name of the skill</param>
        /// <returns></returns>
        int GetPrestigeLevel(SFarmer player, string skillName);
        /// <summary>
        /// Gets the experience the player has earned for the current level
        /// </summary>
        /// <param name="player">The player instance of the wanted player</param>
        /// <param name="skillName">The internal name of the skill</param>
        /// <returns></returns>
        int GetLevelExperience(SFarmer player, string skillName);
        /// <summary>
        /// Gets the experience the player has earned in total
        /// </summary>
        /// <param name="player">The player instance of the wanted player</param>
        /// <param name="skillName">The internal name of the skill</param>
        /// <returns></returns>
        int GetTotalExperience(SFarmer player, string skillName);
        /// <summary>
        /// Checks if a player has the given profession
        /// </summary>
        /// <param name="player">The player instance of the wanted player</param>
        /// <param name="skillName">The internal name of the skill</param>
        /// <param name="professionName">The internal name of the profession</param>
        /// <returns></returns>
        bool HasProfession(SFarmer player, string skillName, string professionName);
        /// <summary>
        /// Used to grant a player a new profession without checking any requirements
        /// </summary>
        /// <param name="player">The player instance of the wanted player</param>
        /// <param name="skillName">The internal name of the skill</param>
        /// <param name="professionName">The internal name of the profession</param>
        void GiveProfession(SFarmer player, string skillName, string professionName);
        /// <summary>
        /// Lists all professions that the player currently meets all but the level requirement for
        /// </summary>
        /// <param name="player">The player instance of the wanted player</param>
        /// <param name="skillName">The internal name of the skill</param>
        /// <returns></returns>
        IProfession[] GetEarnableProfessions(SFarmer player, string skillName);
    }
}
