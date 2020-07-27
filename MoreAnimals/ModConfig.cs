using System;

namespace Entoarox.MorePetsAndAnimals
{
    public class ModConfig
    {
        /*********
        ** Fields
        *********/
        private int _AdoptionPrice = 500;
        private double _RepeatedAdoptionPenality = 0.1;


        /*********
        ** Accessors
        *********/

        // How much it costs to adopt a pet, has to be at least 100 - default 500
        public int AdoptionPrice
        {
            get => this._AdoptionPrice;
            set => this._AdoptionPrice = Math.Max(100, value);
        }

        // The increasing chance for how likely it is for the box to be empty depending on how many pets you already own, set to 0 to disable - default 0.1
        public double RepeatedAdoptionPenality
        {
            get => this._RepeatedAdoptionPenality;
            set => this._RepeatedAdoptionPenality = Math.Max(0.0, Math.Min(0.9, value));
        }

        // If true, then once `MaxAdoptionLimit` pets are adopted, the box will always be empty
        public bool UseMaxAdoptionLimit = false;
        // The maximum pets allowed if `UseMaxAdoptionLimit` is true
        public int MaxAdoptionLimit = 10;
        // If the pet feature of MoreAnimals should be disabled (Removes the adoption box from the game and disables pet skinning)
        public bool AnimalsOnly = false;
        // If true, then the box is more likely to hold the pet type you currently have the least of
        public bool BalancedPetTypes = true;
        // If true, then the pet in the box is more likely to use a skin you do not already have
        public bool BalancedPetSkins = true;
        // If true, and `BalancedPetSkins` is also true, then the pet in the box will only have a reused skin if you already have a pet with one of each skin
        public bool ForceUniqueSkins = false;
        // If true, then the daily pet limit of 1 is disabled
        public bool DisableDailyLimit = false;

        public string[] ExtraTypes=new string[0];
    }
}
