using System;

namespace Entoarox.Framework
{
    public class PlayerModifier
    {
        internal bool Unlocked;
        internal PlayerModifier(bool unlocked)
        {
            Unlocked = unlocked;
        }
        public PlayerModifier()
        {
            Unlocked = false;
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
        [Obsolete("Experience modifiers are currently not functional yet.")]
        public float ExperienceModifierFarming { get { return Unlocked ? _Farming : Math.Min(5f,Math.Max(-1f, _Farming)); } set { _Farming = value; } }
        private float _Fishing = 1;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        [Obsolete("Experience modifiers are currently not functional yet.")]
        public float ExperienceModifierFishing { get { return Unlocked ? _Fishing : Math.Min(5f, Math.Max(-1f, _Fishing)); } set { _Fishing = value; } }
        private float _Foraging = 1;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        [Obsolete("Experience modifiers are currently not functional yet.")]
        public float ExperienceModifierForaging { get { return Unlocked ? _Foraging : Math.Min(5f, Math.Max(-1f, _Foraging)); } set { _Foraging = value; } }
        private float _Mining = 1;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        [Obsolete("Experience modifiers are currently not functional yet.")]
        public float ExperienceModifierMining { get { return Unlocked ? _Mining : Math.Min(5f, Math.Max(-1f, _Mining)); } set { _Mining = value; } }
        private float _Combat = 1;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        [Obsolete("Experience modifiers are currently not functional yet.")]
        public float ExperienceModifierCombat { get { return Unlocked ? _Combat : Math.Min(5f, Math.Max(-1f, _Farming)); } set { _Combat = value; } }
    }
}
