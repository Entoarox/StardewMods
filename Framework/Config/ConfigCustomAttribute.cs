using System;
using StardewModdingAPI;

namespace Entoarox.Framework.Config
{
    [Obsolete]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ConfigCustomAttribute : ConfigAttribute
    {
        /*********
        ** Accessors
        *********/
        internal Type Component;
        internal object[] Arguments;


        /*********
        ** Public methods
        *********/
        /// <summary>Configs that can be accessed using <see cref="IModHelperExtensions.RegisterDynamicConfig{T}(IModHelper, T, Action{T})" /> must use this attribute or the parent <see cref="ConfigAttribute" /> attribute on properties they wish to be dynamic. This attribute should be used if none of the default components used by the config attributes work for your config option.</summary>
        /// <param name="label">The label to display in front of this config option.</param>
        /// <param name="description">The description to show when the label for this config option is hovered over, should explain what the option does.</param>
        /// <param name="component">The custom component to create a instance of for this config option.</param>
        /// <param name="arguments">The arguments to give to the constructor of the custom component.</param>
        public ConfigCustomAttribute(string label, string description, Type component, object[] arguments)
            : base(label, description)
        {
            this.Component = component;
            this.Arguments = arguments;
        }
    }
}
