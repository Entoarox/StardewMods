using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;

namespace MagicJunimoPet.Behaviours
{
    class FollowBehaviour : Behaviour
    {
        private int Timer;
        public FollowBehaviour(SmartPet pet) : base(pet)
        {

        }
        public override void OnBehaviour(GameTime time)
        {
            float distance = 0;
            // If we are currently not in the Wandering state, we need to do some sanity checks:
            if (this.Pet.State != PetState.Wandering)
            {
                // We get the Follow ability for the pet
                this.Pet.Abilities.TryGetValue("Follow", out Ability followAbility);
                // We force a pet into wandering mode and send them to the farm if:
                // - Their Target is null
                // - Their Follow ability is null (Aka, they cant follow)
                if (this.Pet.Target == null || followAbility == null)
                {
                    this.Pet.State = PetState.Wandering;
                    this.DoWarpHome();
                    return;
                }
                // If the target is on another map, check what needs to be done
                if (this.Pet.Target.currentLocation != this.Pet.currentLocation)
                {
                    // We need to handle locations that are not in Game1.locations as "normally" being inaccessible by the pet (Since they are usually temporary locations)
                    if (!Game1.locations.Any(item => item == this.Pet.Target.currentLocation))
                    {
                        // A exception is the mines, which the pet can follow if the follow ability is rank 2
                        if (!(this.Pet.Target.currentLocation is MineShaft) || !(followAbility is IRankedAbility rankedFollow) || rankedFollow.Rank < 2)
                        {
                            this.Pet.State = PetState.Wandering;
                            this.DoWarpHome();
                            return;
                        }
                    }
                }
                distance = Vector2.Distance(this.Pet.Position, this.Pet.Target.Position);
            }
            // After the sanity checks are handled, we can handle the actual AI logic for each of the 3 states
            switch (this.Pet.State)
            {
                case PetState.Wandering:
                    this.OnBehaviour(time);
                    break;
                case PetState.Idling:
                    if (distance > 128)
                        this.Pet.State = PetState.Following;
                    this.OnBehaviour(time);
                    break;
                case PetState.Following:
                    if (distance > 64)
                    {
                        this.Timer = 5;
                    }
                    else if (this.Timer > 0)
                    {
                        this.Timer--;
                        if (this.Timer == 0)
                        {
                            this.Pet.State = PetState.Idling;
                            this.Pet.Halt();
                            this.Pet.faceDirection(this.Pet.Target.FacingDirection);
                        }
                    }
                    break;
            }
        }
        internal void DoWarpHome()
        {
            this.Pet.faceDirection(2);
            Game1.warpCharacter(this.Pet, "Farm", new Vector2(54f, 8f));
            this.Pet.position.X -= 64f;
        }
    }
}
