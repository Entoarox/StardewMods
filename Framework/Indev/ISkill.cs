using System;
using Entoarox.Framework.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Indev
{
    internal interface ISkill
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The maximum level this skill can reach. If prestige is enabled, this is the point where prestige becomes possible to gain.</summary>
        int MaxSkillLevel { get; }

        /// <summary>The name that is visually displayed for this skill.</summary>
        string DisplayName { get; }

        /// <summary>The unique icon for this skill.</summary>
        Texture2D Icon { get; }

        /// <summary>The list of all professions for this skill.</summary>
        IProfession[] Professions { get; }

        /// <summary>If this skill allows for prestige levels.</summary>
        bool CanPrestige { get; }

        /// <summary>How many prestige professions can be earned before prestige stops. (Only applies if this skill allows prestige levels.)</summary>
        int MaxPrestigeLevel { get; }

        /// <summary>Modifies how much experience per level is required as a percentage of the default.</summary>
        float ExperienceMultiplier { get; }

        /// <summary>The color used by mods such as Experience Bars for this skill.</summary>
        Color Color { get; }

        /// <summary>A method that is called when the levelup menu is shown for this skill. It allows you to draw the bonusses the player has gotten from this levelup.</summary>
        Action<int, IComponentCollection> Callback { get; }
    }
}
