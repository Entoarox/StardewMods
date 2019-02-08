using System.Diagnostics.CodeAnalysis;

namespace Entoarox.MorePetsAndAnimals.Framework
{
    /// <summary>A unique animal type variation for which skins can be added.</summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "The enum values are constructed dynamically.")]
    internal enum AnimalType
    {
        Cat,
        Dog
    }
}
