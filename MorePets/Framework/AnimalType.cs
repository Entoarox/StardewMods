using System.Diagnostics.CodeAnalysis;

namespace Entoarox.MorePetsAndAnimals.Framework
{
    /// <summary>A unique animal type variation for which skins can be added.</summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "The enum values are constructed dynamically.")]
    internal enum AnimalType
    {
        BabyBlueChicken,
        BabyBrownChicken,
        BabyBrownCow,
        BabyDuck, // Special: MorePets separates baby ducks from baby white chickens (BabyDuck.png as a copy of BabyWhite Chicken.png is bundled because of this)
        BabyGoat,
        BabyPig,
        BabyRabbit,
        BabySheep,
        BabyVoidChicken,
        BabyWhiteChicken,
        BabyWhiteCow,
        BlueChicken,
        BrownChicken,
        BrownCow,
        Cat,
        Dinosaur,
        Dog,
        Duck,
        Goat,
        Pig,
        Rabbit,
        ShearedSheep,
        Sheep,
        VoidChicken,
        WhiteChicken,
        WhiteCow
    }
}
