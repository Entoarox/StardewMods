using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Characters;

namespace MagicJunimoPet
{
    public class SmartPet : Pet
    {
        public PetState State = PetState.Wandering;
        public Character Target = null;
        public Dictionary<string, Ability> Abilities;
        public Dictionary<string, Behaviour> Behaviours;
#pragma warning disable CS0067
        public event Action<SmartPet> OnLevelUp;
#pragma warning restore CS0067
#pragma warning disable CS0414
        public string CurrentBehaviour = "Default";

        private bool ExperienceDecay = true;
#pragma warning restore CS0414

        public SmartPet(AnimatedSprite sprite, Behaviour defaultBehaviour, Dictionary<string, Ability> defaultAbilities = null, Action<SmartPet> levelCallback=null)
        {
            this.Sprite = sprite;
            this.HideShadow = true;
            this.Breather = false;
            this.willDestroyObjectsUnderfoot = false;

            this.Behaviours.Add("Default", defaultBehaviour);
            this.Abilities = defaultAbilities ?? new Dictionary<string, Ability>();
            if (levelCallback != null)
                this.OnLevelUp += levelCallback;
        }
        public void ModExperience(int amount)
        {
            this.ExperienceDecay = false;
            this.friendshipTowardFarmer.Value += amount;
        }
        public void AddAbility(Ability ability)
        {
            if (ability.Pet != null)
                throw new ArgumentException("Invalid ability, already assigned to a pet.", nameof(ability));
            this.Abilities.Add(ability.Id, ability);
            ability.Pet = this;
        }
        public void AddAbility<T>(params object[] arguments) where T : Ability, new()
        {
            Ability ability = (Ability)Activator.CreateInstance(typeof(T), arguments);
            ability.Pet = this;
            this.Abilities.Add(ability.Id, ability);
        }
        public override void update(GameTime time, GameLocation location)
        {
            base.update(time, location);
            foreach (var ability in this.Abilities.Where(_ => _.Value is ITickingAbility).Select(_ => _.Value as ITickingAbility))
                ability.OnUpdate(time);
        }
    }
}
