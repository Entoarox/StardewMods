using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using StardewModdingAPI;

using StardewValley;

namespace Entoarox.Framework.Core
{
#pragma warning disable CS0618
    internal class PlayerModifierHelper : IPlayerModifierHelper
    {
        private static System.Reflection.FieldInfo _ref_buffAttributes = typeof(Buff).GetField("buffAttributes", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        private static List<PlayerModifier> _Modifiers = new List<PlayerModifier>();
        private static float _HealthOverflow = 0;
        private static PlayerModifier _Compound = new PlayerModifier(true)
        {
            ExperienceModifierCombat = 1,
            ExperienceModifierFarming = 1,
            ExperienceModifierFishing = 1,
            ExperienceModifierForaging = 1,
            ExperienceModifierMining = 1
        };
        private static int _MyUnique = (int)DateTime.Now.Ticks;
        private void _UpdateCompound()
        {
            _ResetModifiers();
            _Compound = new PlayerModifier(true)
            {
                ExperienceModifierCombat = 1,
                ExperienceModifierFarming = 1,
                ExperienceModifierFishing = 1,
                ExperienceModifierForaging = 1,
                ExperienceModifierMining = 1
            };
            foreach(PlayerModifier mod in _Modifiers)
            {
                _Compound.MagnetRange += mod.MagnetRange;
                _Compound.GlowDistance = Math.Max(_Compound.GlowDistance, mod.GlowDistance);
                _Compound.StaminaRegenModifier += mod.StaminaRegenModifier;
                _Compound.HealthRegenModifier += mod.HealthRegenModifier;
                _Compound.WalkSpeedModifier += mod.WalkSpeedModifier;
                _Compound.RunSpeedModifier += mod.RunSpeedModifier;
                _Compound.AttackIncreaseModifier += mod.AttackIncreaseModifier;
                _Compound.KnockbackModifier += mod.KnockbackModifier;
                _Compound.CritChanceModifier += mod.CritChanceModifier;
                _Compound.CritPowerModifier += mod.CritPowerModifier;
                _Compound.WeaponSpeedModifier += mod.WeaponSpeedModifier;
                _Compound.WeaponPrecisionModifier += mod.WeaponPrecisionModifier;

                _Compound.ExperienceModifierCombat += mod.ExperienceModifierCombat;
                _Compound.ExperienceModifierFarming += mod.ExperienceModifierFarming;
                _Compound.ExperienceModifierFishing += mod.ExperienceModifierFishing;
                _Compound.ExperienceModifierForaging += mod.ExperienceModifierForaging;
                _Compound.ExperienceModifierMining += mod.ExperienceModifierMining;
            }
            _Compound.Unlocked = false;
            _ApplyModifiers();
        }
        /// <summary>
        /// This method calculates vanilla speed boost and returns it
        /// </summary>
        private static int _GetVanillaSpeedBoost()
        {
            int Boost = 0, v=0;
            foreach (Buff b in Game1.buffsDisplay.otherBuffs)
            {
                v = (_ref_buffAttributes.GetValue(b) as int[])[9];
                if (v != 0)
                    Boost += v;
            }
            if (Game1.buffsDisplay.food != null)
            {
                v = (_ref_buffAttributes.GetValue(Game1.buffsDisplay.food) as int[])[9];
                if (v != 0)
                    Boost += v;
            }
            if (Game1.buffsDisplay.drink != null)
            {
                v = (_ref_buffAttributes.GetValue(Game1.buffsDisplay.drink) as int[])[9];
                if (v != 0)
                    Boost += v;
            }
            return Boost;
        }
        private void _ResetModifiers()
        {
            if (!Context.IsWorldReady)
                return;
            Game1.player.attackIncreaseModifier -= _Compound.AttackIncreaseModifier;
            Game1.player.knockbackModifier -= _Compound.KnockbackModifier;
            Game1.player.critChanceModifier -= _Compound.CritChanceModifier;
            Game1.player.critPowerModifier -= _Compound.CritPowerModifier;
            Game1.player.weaponSpeedModifier -= _Compound.WeaponSpeedModifier;
            Game1.player.weaponPrecisionModifier -= _Compound.WeaponPrecisionModifier;
            Game1.player.MagneticRadius -= _Compound.MagnetRange;
            if (_Compound.GlowDistance == 0)
                Utility.removeLightSource(_MyUnique);
            Game1.player.addedSpeed = 0;
            _HealthOverflow = 0;
        }
        internal void _ApplyModifiers()
        {
            if (!Context.IsWorldReady)
                return;
            Game1.player.attackIncreaseModifier += _Compound.AttackIncreaseModifier;
            Game1.player.knockbackModifier += _Compound.KnockbackModifier;
            Game1.player.critChanceModifier += _Compound.CritChanceModifier;
            Game1.player.critPowerModifier += _Compound.CritPowerModifier;
            Game1.player.weaponSpeedModifier += _Compound.WeaponSpeedModifier;
            Game1.player.weaponPrecisionModifier += _Compound.WeaponPrecisionModifier;
            Game1.player.MagneticRadius += _Compound.MagnetRange;
            if (_Compound.GlowDistance > 0)
                Game1.currentLightSources.Add(new LightSource(LightSource.lantern, new Vector2(Game1.player.position.X + (Game1.tileSize / 3), Game1.player.position.Y + Game1.tileSize), _Compound.GlowDistance, new Color(0, 30, 150), _MyUnique));
        }
#pragma warning restore CS0618
        internal static void _UpdateModifiers()
        {
            if(Game1.currentLocation.currentEvent != null)
                Game1.player.addedSpeed = 0;
            else
                Game1.player.addedSpeed= _GetVanillaSpeedBoost() + (Game1.player.running ? _Compound.RunSpeedModifier : _Compound.WalkSpeedModifier);
            if(_Compound.HealthRegenModifier != 0 && Game1.player.health < Game1.player.maxHealth)
            {
                _HealthOverflow += _Compound.HealthRegenModifier;
                int HealthChange = (int)_HealthOverflow;
                _HealthOverflow -= HealthChange;
                Game1.player.health = Math.Min(Game1.player.health + HealthChange, Game1.player.maxHealth);
            }
            if (_Compound.StaminaRegenModifier != 0 && Game1.player.stamina < Game1.player.MaxStamina)
                Game1.player.stamina = Math.Min(Game1.player.stamina + _Compound.StaminaRegenModifier, Game1.player.MaxStamina);
            LightSource lightSource = Utility.getLightSource(_MyUnique);
            if (lightSource != null)
                if (_Compound.GlowDistance == 0)
                    Game1.currentLightSources.Remove(lightSource);
                else
                {
                    Game1.currentLightSources.Remove(lightSource);
                    lightSource = new LightSource(1, new Vector2(Game1.player.position.X + (Game1.tileSize / 3), Game1.player.position.Y), _Compound.GlowDistance);
                }
        }
        public int Count
        {
            get => _Modifiers.Count;
        }
        public void Add(PlayerModifier modifier)
        {
            _Modifiers.Add(modifier);
            _UpdateCompound();
        }
        public void AddRange(IEnumerable<PlayerModifier> modifiers)
        {
            _Modifiers.AddRange(modifiers);
            _UpdateCompound();
        }
        public void Remove(PlayerModifier modifier)
        {
            _Modifiers.Remove(modifier);
            _UpdateCompound();
        }
        public void RemoveAll(Predicate<PlayerModifier> predicate)
        {
            _Modifiers.RemoveAll(predicate);
            _UpdateCompound();
        }
        public void Clear()
        {
            _Modifiers.Clear();
            _UpdateCompound();
        }
        public bool Contains(PlayerModifier modifier) => _Modifiers.Contains(modifier);
        public bool Exists(Predicate<PlayerModifier> predicate) => _Modifiers.Exists(predicate);
        public bool TrueForAll(Predicate<PlayerModifier> predicate) => _Modifiers.TrueForAll(predicate);
    }
}
