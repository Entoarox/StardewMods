using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework
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
}
