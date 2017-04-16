using System;

namespace MorePets
{
    public class MorePetsConfig
    {
        private int _AdoptionPrice = 500;
        public int AdoptionPrice
        {
            get
            {
                return this._AdoptionPrice;
            }
            set
            {
                this._AdoptionPrice = Math.Max(100, value);
            }
        }
        private double _RepeatedAdoptionPenality = 0.1;
        public double RepeatedAdoptionPenality
        {
            get
            {
                return this._RepeatedAdoptionPenality;
            }
            set
            {
                this._RepeatedAdoptionPenality = Math.Max(0.0, Math.Min(0.9, value));
            }
        }
        public bool UseMaxAdoptionLimit = false;
        public int MaxAdoptionLimit = 10;
        public bool DebugMode = false;
    }
}
