using System;

namespace Entoarox.Framework
{
    public class PlayerModifier
    {
        /*********
        ** Fields
        *********/
        private float Combat = 1;
        private float Farming = 1;
        private float Fishing = 1;
        private float Foraging = 1;
        private float Mining = 1;


        /*********
        ** Accessors
        *********/
        internal bool Unlocked;

        /// <summary>Additive, multiple modifiers will stack.</summary>
        public int MagnetRange { get; set; } = 0;

        /// <summary>Additive, multiple modifiers will stack.</summary>
        public int WalkSpeedModifier { get; set; } = 0;

        /// <summary>Additive, multiple modifiers will stack.</summary>
        public int RunSpeedModifier { get; set; } = 0;

        /// <summary>Not additive, largest modifier used.</summary>
        public float GlowDistance { get; set; } = 0;

        /// <summary>Additive, multiple modifiers will stack</summary>
        public float StaminaRegenModifier { get; set; } = 0;

        /// <summary>Additive, multiple modifiers will stack</summary>
        public float HealthRegenModifier { get; set; } = 0;

        /// <summary>Additive, multiple modifiers will stack</summary>
        public float AttackIncreaseModifier { get; set; } = 0;

        /// <summary>Additive, multiple modifiers will stack</summary>
        public float KnockbackModifier { get; set; } = 0;

        /// <summary>Additive, multiple modifiers will stack</summary>
        public float CritChanceModifier { get; set; } = 0;

        /// <summary>Additive, multiple modifiers will stack</summary>
        public float CritPowerModifier { get; set; } = 0;

        /// <summary>Additive, multiple modifiers will stack</summary>
        public float WeaponSpeedModifier { get; set; } = 0;

        /// <summary>Additive, multiple modifiers will stack</summary>
        public float WeaponPrecisionModifier { get; set; } = 0;

        /// <summary>Additive, multiple modifiers will stack</summary>
        [Obsolete("This API member is not yet functional in the current development build.")]
        public float ExperienceModifierFarming
        {
            get => this.Unlocked ? this.Farming : Math.Min(5f, Math.Max(-1f, this.Farming));
            set => this.Farming = value;
        }

        /// <summary>Additive, multiple modifiers will stack</summary>
        [Obsolete("This API member is not yet functional in the current development build.")]
        public float ExperienceModifierFishing
        {
            get => this.Unlocked ? this.Fishing : Math.Min(5f, Math.Max(-1f, this.Fishing));
            set => this.Fishing = value;
        }

        /// <summary>Additive, multiple modifiers will stack</summary>
        [Obsolete("This API member is not yet functional in the current development build.")]
        public float ExperienceModifierForaging
        {
            get => this.Unlocked ? this.Foraging : Math.Min(5f, Math.Max(-1f, this.Foraging));
            set => this.Foraging = value;
        }

        /// <summary>Additive, multiple modifiers will stack</summary>
        [Obsolete("This API member is not yet functional in the current development build.")]
        public float ExperienceModifierMining
        {
            get => this.Unlocked ? this.Mining : Math.Min(5f, Math.Max(-1f, this.Mining));
            set => this.Mining = value;
        }

        /// <summary>Additive, multiple modifiers will stack</summary>
        [Obsolete("This API member is not yet functional in the current development build.")]
        public float ExperienceModifierCombat
        {
            get => this.Unlocked ? this.Combat : Math.Min(5f, Math.Max(-1f, this.Farming));
            set => this.Combat = value;
        }


        /*********
        ** Public methods
        *********/
        internal PlayerModifier(bool unlocked)
        {
            this.Unlocked = unlocked;
        }

        public PlayerModifier()
        {
            this.Unlocked = false;
        }
    }
}
