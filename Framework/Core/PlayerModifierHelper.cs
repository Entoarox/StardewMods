using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
#pragma warning disable 618

namespace Entoarox.Framework.Core
{
    internal class PlayerModifierHelper : IPlayerModifierHelper
    {
        /*********
        ** Fields
        *********/
        private static readonly List<PlayerModifier> Modifiers = new List<PlayerModifier>();
        private static readonly int MyUnique = (int)DateTime.Now.Ticks;
        private static readonly FieldInfo BuffAttributes = typeof(Buff).GetField("buffAttributes", BindingFlags.Instance | BindingFlags.NonPublic);
        private static float HealthOverflow;
        private static PlayerModifier Compound = new PlayerModifier(true)
        {
            ExperienceModifierCombat = 1,
            ExperienceModifierFarming = 1,
            ExperienceModifierFishing = 1,
            ExperienceModifierForaging = 1,
            ExperienceModifierMining = 1
        };


        /*********
        ** Accessors
        *********/
        public int Count => PlayerModifierHelper.Modifiers.Count;


        /*********
        ** Public methods
        *********/
        internal void ApplyModifiers()
        {
            if (!Context.IsWorldReady)
                return;
            Game1.player.attackIncreaseModifier += PlayerModifierHelper.Compound.AttackIncreaseModifier;
            Game1.player.knockbackModifier += PlayerModifierHelper.Compound.KnockbackModifier;
            Game1.player.critChanceModifier += PlayerModifierHelper.Compound.CritChanceModifier;
            Game1.player.critPowerModifier += PlayerModifierHelper.Compound.CritPowerModifier;
            Game1.player.weaponSpeedModifier += PlayerModifierHelper.Compound.WeaponSpeedModifier;
            Game1.player.weaponPrecisionModifier += PlayerModifierHelper.Compound.WeaponPrecisionModifier;
            Game1.player.MagneticRadius += PlayerModifierHelper.Compound.MagnetRange;
            if (PlayerModifierHelper.Compound.GlowDistance > 0)
                Game1.currentLightSources.Add(new LightSource(LightSource.lantern, new Vector2(Game1.player.position.X + Game1.tileSize / 3, Game1.player.position.Y + Game1.tileSize), PlayerModifierHelper.Compound.GlowDistance, new Color(0, 30, 150), PlayerModifierHelper.MyUnique));
        }

        public static void UpdateModifiers()
        {
            if (Game1.currentLocation.currentEvent != null)
                Game1.player.addedSpeed = 0;
            else
                Game1.player.addedSpeed = PlayerModifierHelper.GetVanillaSpeedBoost() + (Game1.player.running ? PlayerModifierHelper.Compound.RunSpeedModifier : PlayerModifierHelper.Compound.WalkSpeedModifier);
            if (PlayerModifierHelper.Compound.HealthRegenModifier != 0 && Game1.player.health < Game1.player.maxHealth)
            {
                PlayerModifierHelper.HealthOverflow += PlayerModifierHelper.Compound.HealthRegenModifier;
                int HealthChange = (int)PlayerModifierHelper.HealthOverflow;
                PlayerModifierHelper.HealthOverflow -= HealthChange;
                Game1.player.health = Math.Min(Game1.player.health + HealthChange, Game1.player.maxHealth);
            }

            if (PlayerModifierHelper.Compound.StaminaRegenModifier != 0 && Game1.player.stamina < Game1.player.MaxStamina)
                Game1.player.stamina = Math.Min(Game1.player.stamina + PlayerModifierHelper.Compound.StaminaRegenModifier, Game1.player.MaxStamina);
            LightSource lightSource = Utility.getLightSource(PlayerModifierHelper.MyUnique);
            if (lightSource != null)
                if (PlayerModifierHelper.Compound.GlowDistance == 0)
                    Game1.currentLightSources.Remove(lightSource);
                else
                {
                    Game1.currentLightSources.Remove(lightSource);
                    lightSource = new LightSource(1, new Vector2(Game1.player.position.X + Game1.tileSize / 3, Game1.player.position.Y), PlayerModifierHelper.Compound.GlowDistance);
                }
        }

        public void Add(PlayerModifier modifier)
        {
            PlayerModifierHelper.Modifiers.Add(modifier);
            this._UpdateCompound();
        }

        public void AddRange(IEnumerable<PlayerModifier> modifiers)
        {
            PlayerModifierHelper.Modifiers.AddRange(modifiers);
            this._UpdateCompound();
        }

        public void Remove(PlayerModifier modifier)
        {
            PlayerModifierHelper.Modifiers.Remove(modifier);
            this._UpdateCompound();
        }

        public void RemoveAll(Predicate<PlayerModifier> predicate)
        {
            PlayerModifierHelper.Modifiers.RemoveAll(predicate);
            this._UpdateCompound();
        }

        public void Clear()
        {
            PlayerModifierHelper.Modifiers.Clear();
            this._UpdateCompound();
        }

        public bool Contains(PlayerModifier modifier)
        {
            return PlayerModifierHelper.Modifiers.Contains(modifier);
        }

        public bool Exists(Predicate<PlayerModifier> predicate)
        {
            return PlayerModifierHelper.Modifiers.Exists(predicate);
        }

        public bool TrueForAll(Predicate<PlayerModifier> predicate)
        {
            return PlayerModifierHelper.Modifiers.TrueForAll(predicate);
        }


        /*********
        ** Protected methods
        *********/
        private void _UpdateCompound()
        {
            this.ResetModifiers();
            PlayerModifierHelper.Compound = new PlayerModifier(true)
            {
                ExperienceModifierCombat = 1,
                ExperienceModifierFarming = 1,
                ExperienceModifierFishing = 1,
                ExperienceModifierForaging = 1,
                ExperienceModifierMining = 1
            };
            foreach (PlayerModifier mod in PlayerModifierHelper.Modifiers)
            {
                PlayerModifierHelper.Compound.MagnetRange += mod.MagnetRange;
                PlayerModifierHelper.Compound.GlowDistance = Math.Max(PlayerModifierHelper.Compound.GlowDistance, mod.GlowDistance);
                PlayerModifierHelper.Compound.StaminaRegenModifier += mod.StaminaRegenModifier;
                PlayerModifierHelper.Compound.HealthRegenModifier += mod.HealthRegenModifier;
                PlayerModifierHelper.Compound.WalkSpeedModifier += mod.WalkSpeedModifier;
                PlayerModifierHelper.Compound.RunSpeedModifier += mod.RunSpeedModifier;
                PlayerModifierHelper.Compound.AttackIncreaseModifier += mod.AttackIncreaseModifier;
                PlayerModifierHelper.Compound.KnockbackModifier += mod.KnockbackModifier;
                PlayerModifierHelper.Compound.CritChanceModifier += mod.CritChanceModifier;
                PlayerModifierHelper.Compound.CritPowerModifier += mod.CritPowerModifier;
                PlayerModifierHelper.Compound.WeaponSpeedModifier += mod.WeaponSpeedModifier;
                PlayerModifierHelper.Compound.WeaponPrecisionModifier += mod.WeaponPrecisionModifier;

                PlayerModifierHelper.Compound.ExperienceModifierCombat += mod.ExperienceModifierCombat;
                PlayerModifierHelper.Compound.ExperienceModifierFarming += mod.ExperienceModifierFarming;
                PlayerModifierHelper.Compound.ExperienceModifierFishing += mod.ExperienceModifierFishing;
                PlayerModifierHelper.Compound.ExperienceModifierForaging += mod.ExperienceModifierForaging;
                PlayerModifierHelper.Compound.ExperienceModifierMining += mod.ExperienceModifierMining;
            }

            PlayerModifierHelper.Compound.Unlocked = false;
            this.ApplyModifiers();
        }

        /// <summary>This method calculates vanilla speed boost and returns it.</summary>
        private static int GetVanillaSpeedBoost()
        {
            int boost = 0, v = 0;
            foreach (Buff b in Game1.buffsDisplay.otherBuffs)
            {
                v = (PlayerModifierHelper.BuffAttributes.GetValue(b) as int[])[9];
                if (v != 0)
                    boost += v;
            }

            if (Game1.buffsDisplay.food != null)
            {
                v = (PlayerModifierHelper.BuffAttributes.GetValue(Game1.buffsDisplay.food) as int[])[9];
                if (v != 0)
                    boost += v;
            }

            if (Game1.buffsDisplay.drink != null)
            {
                v = (PlayerModifierHelper.BuffAttributes.GetValue(Game1.buffsDisplay.drink) as int[])[9];
                if (v != 0)
                    boost += v;
            }

            return boost;
        }

        private void ResetModifiers()
        {
            if (!Context.IsWorldReady)
                return;
            Game1.player.attackIncreaseModifier -= PlayerModifierHelper.Compound.AttackIncreaseModifier;
            Game1.player.knockbackModifier -= PlayerModifierHelper.Compound.KnockbackModifier;
            Game1.player.critChanceModifier -= PlayerModifierHelper.Compound.CritChanceModifier;
            Game1.player.critPowerModifier -= PlayerModifierHelper.Compound.CritPowerModifier;
            Game1.player.weaponSpeedModifier -= PlayerModifierHelper.Compound.WeaponSpeedModifier;
            Game1.player.weaponPrecisionModifier -= PlayerModifierHelper.Compound.WeaponPrecisionModifier;
            Game1.player.MagneticRadius -= PlayerModifierHelper.Compound.MagnetRange;
            if (PlayerModifierHelper.Compound.GlowDistance == 0)
                Utility.removeLightSource(PlayerModifierHelper.MyUnique);
            Game1.player.addedSpeed = 0;
            PlayerModifierHelper.HealthOverflow = 0;
        }
    }
}
