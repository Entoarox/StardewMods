using System;

namespace Entoarox.Framework
{
    public class PlayerModifier
    {
        internal bool Unlocked;
        internal PlayerModifier(bool unlocked)
        {
            this.Unlocked = unlocked;
        }
        public PlayerModifier()
        {
            this.Unlocked = false;
        }
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public int MagnetRange { get; set; } = 0;

        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public int WalkSpeedModifier { get; set; } = 0;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public int RunSpeedModifier { get; set; } = 0;

        /**
         * <summary>Not additive, largest modifier used</summary>
         */
        public float GlowDistance { get; set; } = 0;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float StaminaRegenModifier { get; set; } = 0;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float HealthRegenModifier { get; set; } = 0;

        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float AttackIncreaseModifier { get; set; } = 0;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float KnockbackModifier { get; set; } = 0;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float CritChanceModifier { get; set; } = 0;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float CritPowerModifier { get; set; } = 0;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float WeaponSpeedModifier { get; set; } = 0;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float WeaponPrecisionModifier { get; set; } = 0;

        private float _Farming = 1;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        [Obsolete("This API member is not yet functional in the current development build.")]
        public float ExperienceModifierFarming { get { return this.Unlocked ? this._Farming : Math.Min(5f,Math.Max(-1f, this._Farming)); } set { this._Farming = value; } }
        private float _Fishing = 1;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        [Obsolete("This API member is not yet functional in the current development build.")]
        public float ExperienceModifierFishing { get { return this.Unlocked ? this._Fishing : Math.Min(5f, Math.Max(-1f, this._Fishing)); } set { this._Fishing = value; } }
        private float _Foraging = 1;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        [Obsolete("This API member is not yet functional in the current development build.")]
        public float ExperienceModifierForaging { get { return this.Unlocked ? this._Foraging : Math.Min(5f, Math.Max(-1f, this._Foraging)); } set { this._Foraging = value; } }
        private float _Mining = 1;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        [Obsolete("This API member is not yet functional in the current development build.")]
        public float ExperienceModifierMining { get { return this.Unlocked ? this._Mining : Math.Min(5f, Math.Max(-1f, this._Mining)); } set { this._Mining = value; } }
        private float _Combat = 1;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        [Obsolete("This API member is not yet functional in the current development build.")]
        public float ExperienceModifierCombat { get { return this.Unlocked ? this._Combat : Math.Min(5f, Math.Max(-1f, this._Farming)); } set { this._Combat = value; } }
    }
}
