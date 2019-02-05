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
        /// <summary>The price that must be paid to adopt a pet.</summary>
        public int AdoptionPrice
        {
            get => this._AdoptionPrice;
            set => this._AdoptionPrice = Math.Max(100, value);
        }

        /// <summary>A penalty which reduces the chance of a pet in the adoption box based on the number of pets the player already has.</summary>
        public double RepeatedAdoptionPenality
        {
            get => this._RepeatedAdoptionPenality;
            set => this._RepeatedAdoptionPenality = Math.Max(0.0, Math.Min(0.9, value));
        }

        /// <summary>Whether to apply the <see cref="MaxAdoptionLimit"/> limit.</summary>
        public bool UseMaxAdoptionLimit = false;

        /// <summary>If <see cref="UseMaxAdoptionLimit"/> is true, the maximum number of pets the player can adopt (including the original pet).</summary>
        public int MaxAdoptionLimit = 10;

        /// <summary>True to disable the pet adoption box.</summary>
        public bool AnimalsOnly = false;

        /// <summary>Whether to ensure an even distribution of skins and pet types, if possible.</summary>
        public bool UseBalancedDistribution = false;
    }
}
