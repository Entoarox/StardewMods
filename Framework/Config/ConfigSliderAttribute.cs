using System;
using StardewModdingAPI;

namespace Entoarox.Framework.Config
{
    [Obsolete]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ConfigSliderAttribute : ConfigAttribute
    {
        /*********
        ** Accessors
        *********/
        internal bool Numeric;
        internal string[] OptionsString;
        internal int[] OptionsInt;


        /*********
        ** Public methods
        *********/
        /// <summary>Configs that can be accessed using <see cref="IModHelperExtensions.RegisterDynamicConfig{T}(IModHelper, T, Action{T})" /> must use this attribute or the parent <see cref="ConfigAttribute" /> attribute on properties they wish to be dynamic. This attribute is for members of the <see cref="string" /> or <see cref="int" /> type where multiple options should be available in a slider.</summary>
        /// <param name="label">The label to display in front of this config option.</param>
        /// <param name="description">The description to show when the label for this config option is hovered over, should explain what the option does.</param>
        /// <param name="options">The list of dropdown options to make available</param>
        public ConfigSliderAttribute(string label, string description, string[] options)
            : base(label, description)
        {
            this.Numeric = false;
            this.OptionsString = options;
        }

        /// <summary>Configs that can be accessed using <see cref="IModHelperExtensions.RegisterDynamicConfig{T}(IModHelper, T, Action{T})" /> must use this attribute or the parent <see cref="ConfigAttribute" /> attribute on properties they wish to be dynamic. This attribute is for members of the <see cref="string" /> or <see cref="int" /> type where multiple options should be available in a slider.</summary>
        /// <param name="label">The label to display in front of this config option.</param>
        /// <param name="description">The description to show when the label for this config option is hovered over, should explain what the option does.</param>
        /// <param name="options">The list of slider options to make available</param>
        public ConfigSliderAttribute(string label, string description, int[] options)
            : base(label, description)
        {
            this.Numeric = true;
            this.OptionsInt = options;
        }
    }
}
