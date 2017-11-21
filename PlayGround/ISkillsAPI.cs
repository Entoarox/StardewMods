using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SFarmer= StardewValley.Farmer;

namespace PlayGround
{
    interface IProfession
    {
        /// <summary>
        /// The skill level required before this profession can be unlocked
        /// </summary>
        int SkillLevel { get; }
        /// <summary>
        /// The internal name of the parent profession needed to unlock this one (or `null` for no parent)
        /// </summary>
        string Parent { get; }
        /// <summary>
        /// The internal name used for this profession by the code
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The name that is visually displayed for this profession
        /// </summary>
        string DisplayName { get; }
        /// <summary>
        /// A short description of the benefits this profession gives
        /// </summary>
        string Description { get; }
        /// <summary>
        /// The icon for this profession
        /// </summary>
        Texture2D Icon { get; }
    }
    interface ISkill
    {
        /// <summary>
        /// The maximum level this skill can reach
        /// If prestige is enabled, this is the point where prestige becomes possible to gain
        /// </summary>
        int MaxSkillLevel { get; }
        /// <summary>
        /// The name that is visually displayed for this skill
        /// </summary>
        string DisplayName { get; }
        /// <summary>
        /// The unique icon for this skill
        /// </summary>
        Texture2D Icon { get; }
        /// <summary>
        /// The list of all professions for this skill
        /// </summary>
        IProfession[] Professions { get; }
        /// <summary>
        /// If this skill allows for prestige levels
        /// </summary>
        bool CanPrestige { get; }
        /// <summary>
        /// How many prestige professions can be earned before prestige stops
        /// (Only applies if this skill allows prestige levels)
        /// </summary>
        int MaxPrestigeLevel { get; }
        /// <summary>
        /// The color used by mods such as Experience Bars for this skill
        /// </summary>
        Color Color { get; }
        Action<int, SpriteBatch, Rectangle> Callback { get; }
    }
    interface ISkillsAPI
    {
        /// <summary>
        /// Register a new skill
        /// </summary>
        /// <param name="skillName">The internal name used for this skill</param>
        /// <param name="skill">All the properties of the skill</param>
        /// <returns></returns>
        int RegisterSkill(string skillName, ISkill skill);
        /// <summary>
        /// Retrieves the properties for a given skill, or `null` if no such skill exists
        /// </summary>
        /// <param name="skillName">The internal name of the skill</param>
        /// <returns></returns>
        ISkill GetSkillInfo(string skillName);
        /// <summary>
        /// Add experience to a skill
        /// Levelups and prestige is automatically handled by the API
        /// </summary>
        /// <param name="skillName">The internal name of the skill</param>
        /// <param name="experience">How much experience should be added</param>
        void AddSkillExperience(string skillName, int experience);
        /// <summary>
        /// Returns `true` if the given skill is currently earning prestige experience instead of level experience
        /// </summary>
        /// <param name="skillName">The internal name of the skill</param>
        /// <returns></returns>
        bool IsActivePrestigeSkill(string skillName);
        /// <summary>
        /// Gets the vanilla-compatible ID for the skill
        /// </summary>
        /// <param name="skillName">The internal name of the skill</param>
        /// <returns></returns>
        int GetSkillId(string skillName);
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
