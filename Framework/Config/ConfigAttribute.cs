using System;

using Microsoft.Xna.Framework.Input;

using StardewModdingAPI;

namespace Entoarox.Framework.Config
{
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
    [Obsolete("The config system is still in development, it should not be used at this time")]
    public class ConfigAttribute : Attribute
    {
        internal string Label;
        internal string Description;
        /// <summary>
        /// Configs that can be accessed using <see cref="IModHelperExtensions.RegisterDynamicConfig{T}(IModHelper, T, Action{T})"/> must use this attribute or a child of it on properties they wish to be dynamic.
        /// This attribute when used creates a component depending on the type of the member it is attached to.
        /// </summary>
        /// <remarks>
        /// The following types, unless given a specific attribute are handled as such:
        /// 
        /// <see cref="string"/>
        /// - A normal textbox
        /// 
        /// <see cref="sbyte"/>, <see cref="short"/>, <see cref="int"/>, <see cref="long"/>, 
        /// <see cref="byte"/>, <see cref="ushort"/>, <see cref="uint"/>, <see cref="ulong"/>
        /// - A PlusMinus textbox limited to the maximum and minimum values of the respective type
        /// 
        /// <see cref="float"/>, <see cref="double"/>
        /// - A regex limited textbox, with allowance for a single dot somewhere in the number but otherwise only allowing the 0~9 characters
        /// 
        /// <see cref="bool"/>
        /// - A checkbox
        /// 
        /// <see cref="Keys"/>
        /// - A button that prompts the user to press any key on their keyboard
        /// 
        /// <see cref="Buttons"/>
        /// - A button that prompts the user to press any button on their controller
        /// 
        /// <see cref="SButtons"/>
        /// - A button that prompts the user to give any input (Keyboard, Controller or Mouse) [SMAPI 2]
        /// 
        /// <see cref="enum"/>
        /// - A dropdown with each of the enum values
        /// </remarks>
        /// <param name="label">The label to display in front of this config option</param>
        /// <param name="description">The description to show when the label for this config option is hovered over, should explain what the option does</param>
        public ConfigAttribute(string label, string description)
        {
            this.Label = label;
            this.Description = description;
        }
    }
}
