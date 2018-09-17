using System;

namespace Entoarox.Framework
{
    /// <summary>A location which can use extended functionality provided by the framework.</summary>
    public interface IAugmentedLocation
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The vanilla equivalent of this location. When replacing a vanilla location, this should be set to the type of that location in vanilla. If you are adding a new location, it should be set to null. When not null, the framework will during saving restore the vanilla version, then load the modded version again after loading.</summary>
        [Obsolete("This API member is not yet functional in the current development build.")]
        Type VanillaEquivalent { get; }

        /// <summary>Whether to treat the location as a greenhouse for crop behaviour.</summary>
        bool IsGreenhouse { get; }

        /// <summary>Whether to treat the location as a house/sheds for furniture behaviour. Flooring and wallpaper currently will not work in such locations. Serialization of decoratable locations is automatically handled by the framework.</summary>
        [Obsolete("This API member is not yet functional in the current development build.")]
        bool IsDecoratable { get; }
    }
}
