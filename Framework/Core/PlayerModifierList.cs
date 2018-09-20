using System;
using System.Collections.Generic;

namespace Entoarox.Framework.Core
{
    /// <summary>An aggregate list of player modifiers.</summary>
    internal class PlayerModifierList : List<PlayerModifier>
    {
        /*********
        ** Accessors
        *********/
        /****
        ** Stats
        ****/
        /// <summary>The radius within which items are pulled towards the player. Additive (multiple modifiers stack).</summary>
        public int MagnetRange { get; private set; }

        /// <summary>The walk speed. Additive (multiple modifiers stack).</summary>
        public int WalkSpeed { get; private set; }

        /// <summary>The run speed. Additive (multiple modifiers stack).</summary>
        public int RunSpeed { get; private set; }

        /// <summary>The glow radius around the player. Not additive (largest modifier is used).</summary>
        public float GlowDistance { get; private set; }

        /// <summary>The amount of stamina to regenerate per tick. Additive (multiple modifiers stack).</summary>
        public float StaminaRegen { get; private set; }

        /// <summary>The amount of health to regenerate per tick. Additive (multiple modifiers stack).</summary>
        public float HealthRegen { get; private set; }

        /// <summary>The added attack power. Additive (multiple modifiers stack).</summary>
        public float Attack { get; private set; }

        /// <summary>The added knockback power. Additive (multiple modifiers stack).</summary>
        public float Knockback { get; private set; }

        /// <summary>The added critical chance. Additive (multiple modifiers stack).</summary>
        public float CritChance { get; private set; }

        /// <summary>The added critical power. Additive (multiple modifiers stack).</summary>
        public float CritPower { get; private set; }

        /// <summary>The added weapon speed. Additive (multiple modifiers stack).</summary>
        public float WeaponSpeed { get; private set; }

        /// <summary>The added weapon precision. Additive (multiple modifiers stack).</summary>
        public float WeaponPrecision { get; private set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Recalculate the aggregate values.</summary>
        public void Recalculate()
        {
            this.MagnetRange = 0;
            this.WalkSpeed = 0;
            this.RunSpeed = 0;
            this.GlowDistance = 0;
            this.StaminaRegen = 0;
            this.HealthRegen = 0;
            this.Attack = 0;
            this.Knockback = 0;
            this.CritChance = 0;
            this.CritPower = 0;
            this.WeaponSpeed = 0;
            this.WeaponPrecision = 0;
            foreach (PlayerModifier mod in this)
            {
                // additive stats
                this.MagnetRange += mod.MagnetRange;
                this.WalkSpeed += mod.WalkSpeedModifier;
                this.RunSpeed += mod.RunSpeedModifier;
                this.StaminaRegen += mod.StaminaRegenModifier;
                this.HealthRegen += mod.HealthRegenModifier;
                this.Attack += mod.AttackIncreaseModifier;
                this.Knockback += mod.KnockbackModifier;
                this.CritChance += mod.CritChanceModifier;
                this.CritPower += mod.CritPowerModifier;
                this.WeaponSpeed += mod.WeaponSpeedModifier;
                this.WeaponPrecision += mod.WeaponPrecisionModifier;

                // non-additive stats (largest value used)
                this.GlowDistance = Math.Max(this.GlowDistance, mod.GlowDistance);
            }
        }
    }
}
