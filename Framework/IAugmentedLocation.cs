using System;

namespace Entoarox.Framework
{
    /// <summary>
    /// GameLocation instances that implement this interface are able to use extended functionality provided by the framework
    /// </summary>
    public interface IAugmentedLocation
    {
        /// <summary>
        /// The vanilla equivalent of this location, this should be set when replacing a vanilla location, to the type of that location in vanilla
        /// If you are adding a new location, it should be set to null
        /// When not null, the framework will during saving restore the vanilla version, then load the modded version again after loading
        /// </summary>
        Type VanillaEquivalent { get; }
        /// <summary>
        /// When true, it makes it so the location is treated identical to the Greenhouse for crop behaviour
        /// </summary>
        bool IsGreenhouse { get; }
        /// <summary>
        /// When true, it makes it so the location is treated identical to the House and Sheds for furniture behaviour
        /// Flooring and wallpaper currently will not work in such locations
        /// Serialization of decoratable locations is automatically handled by the framework
        /// </summary>
        [Obsolete("This functionality has not yet been implemented")]
        bool IsDecoratable { get; }
    }
}
