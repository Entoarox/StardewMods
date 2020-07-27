using Microsoft.Xna.Framework;

namespace Entoarox.ExtendedMinecart
{
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        public bool RefuelingEnabled = true;
        public double RefuelRequiredChance = 0.05;
        public bool AlternateDesertMinecart = false;
        public bool AlternateFarmMinecart = false;
        public bool FarmDestinationEnabled = true;
        public bool DesertDestinationEnabled = true;
        public bool WoodsDestinationEnabled = true;
        public bool BeachDestinationEnabled = true;
        public bool WizardDestinationEnabled = true;
        public bool UseCustomFarmDestination = false;
        public Point CustomFarmDestinationPoint = new Point(0, 0);
    }
}
