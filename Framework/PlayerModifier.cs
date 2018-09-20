namespace Entoarox.Framework
{
    public class PlayerModifier
    {
        /*********
        ** Accessors
        *********/
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
    }
}
