using System;
using StardewModdingAPI;

namespace Entoarox.Framework.Config
{
    [Obsolete]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ConfigDropdownAttribute : ConfigAttribute
    {
        /*********
        ** Accessors
        *********/
        internal string[] Options;


        /*********
        ** Public methods
        *********/
        /// <summary>Configs that can be accessed using <see cref="IModHelperExtensions.RegisterDynamicConfig{T}(IModHelper, T, Action{T})" /> must use this attribute or the parent <see cref="ConfigAttribute" /> attribute on properties they wish to be dynamic. This attribute is for members of the <see cref="string" /> type where multiple options should be shown in a dropdown.</summary>
        /// <param name="label">The label to display in front of this config option.</param>
        /// <param name="description">The description to show when the label for this config option is hovered over, should explain what the option does.</param>
        /// <param name="options">The list of dropdown options to display</param>
        public ConfigDropdownAttribute(string label, string description, string[] options)
            : base(label, description)
        {
            this.Options = options;
        }
    }
}
