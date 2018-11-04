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
        public int AdoptionPrice
        {
            get => this._AdoptionPrice;
            set => this._AdoptionPrice = Math.Max(100, value);
        }

        public double RepeatedAdoptionPenality
        {
            get => this._RepeatedAdoptionPenality;
            set => this._RepeatedAdoptionPenality = Math.Max(0.0, Math.Min(0.9, value));
        }

        public bool UseMaxAdoptionLimit = false;
        public int MaxAdoptionLimit = 10;
        public bool AnimalsOnly = false;
    }
}
