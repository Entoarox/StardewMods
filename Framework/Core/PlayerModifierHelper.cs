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
        /// <summary>The player stat modifiers to apply.</summary>
        private readonly PlayerModifierList Mods = new PlayerModifierList();

        /// <summary>The last speed boost applied to <see cref="Character.addedSpeed"/>.</summary>
        private int LastMoveBoost = 0;

        /// <summary>The unique ID for the glow light source.</summary>
        private readonly int GlowID = (int)DateTime.Now.Ticks;

        /// <summary>The current vanilla buff values.</summary>
        private readonly FieldInfo BuffAttributes = typeof(Buff).GetField("buffAttributes", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>The health regen remainder after applying the integer amount.</summary>
        private float HealthRegenRemainder;


        /*********
        ** Accessors
        *********/
        public int Count => this.Mods.Count;


        /*********
        ** Public methods
        *********/
        /// <summary>Apply any updates needed on update tick.</summary>
        public void UpdateTick()
        {
            // walk/run speed
            if (Game1.currentLocation.currentEvent != null)
            {
                Game1.player.addedSpeed = 0;
                this.LastMoveBoost = 0;
            }
            else
            {
                this.ClearMoveBoost();
                this.LastMoveBoost = Game1.player.running ? this.Mods.RunSpeed : this.Mods.WalkSpeed;
                if (this.LastMoveBoost != 0)
                    Game1.player.addedSpeed = this.GetVanillaSpeedBoost() + this.LastMoveBoost;
            }

            // health regen
            if (this.Mods.HealthRegen != 0 && Game1.player.health < Game1.player.maxHealth)
            {
                this.HealthRegenRemainder += this.Mods.HealthRegen;
                Game1.player.health = Math.Min(Game1.player.health + (int)this.HealthRegenRemainder, Game1.player.maxHealth);
                this.HealthRegenRemainder %= 1;
            }

            // stamina regen
            if (this.Mods.StaminaRegen != 0 && Game1.player.stamina < Game1.player.MaxStamina)
                Game1.player.stamina = Math.Min(Game1.player.stamina + this.Mods.StaminaRegen, Game1.player.MaxStamina);

            // glow distance
            LightSource lightSource = Utility.getLightSource(this.GlowID);
            if (lightSource != null)
            {
                if (this.Mods.GlowDistance == 0)
                    Game1.currentLightSources.Remove(lightSource);
                else
                {
                    Game1.currentLightSources.Remove(lightSource);
                    lightSource = new LightSource(1, new Vector2(Game1.player.position.X + Game1.tileSize / 3, Game1.player.position.Y), this.Mods.GlowDistance);
                }
            }
        }

        public void Add(PlayerModifier modifier)
        {
            this.Mods.Add(modifier);
            this.OnModifiersChanged();
        }

        public void AddRange(IEnumerable<PlayerModifier> modifiers)
        {
            this.Mods.AddRange(modifiers);
            this.OnModifiersChanged();
        }

        public void Remove(PlayerModifier modifier)
        {
            this.Mods.Remove(modifier);
            this.OnModifiersChanged();
        }

        public void RemoveAll(Predicate<PlayerModifier> predicate)
        {
            this.Mods.RemoveAll(predicate);
            this.OnModifiersChanged();
        }

        public void Clear()
        {
            this.Mods.Clear();
            this.OnModifiersChanged();
        }

        public bool Contains(PlayerModifier modifier)
        {
            return this.Mods.Contains(modifier);
        }

        public bool Exists(Predicate<PlayerModifier> predicate)
        {
            return this.Mods.Exists(predicate);
        }

        public bool TrueForAll(Predicate<PlayerModifier> predicate)
        {
            return this.Mods.TrueForAll(predicate);
        }


        /*********
        ** Protected methods
        *********/
        /// <summary>Update the compound modifiers when the list of applied modifiers has changed.</summary>
        private void OnModifiersChanged()
        {
            // reset
            if (Context.IsWorldReady)
            {
                Game1.player.attackIncreaseModifier -= this.Mods.Attack;
                Game1.player.knockbackModifier -= this.Mods.Knockback;
                Game1.player.critChanceModifier -= this.Mods.CritChance;
                Game1.player.critPowerModifier -= this.Mods.CritPower;
                Game1.player.weaponSpeedModifier -= this.Mods.WeaponSpeed;
                Game1.player.weaponPrecisionModifier -= this.Mods.WeaponPrecision;
                Game1.player.MagneticRadius -= this.Mods.MagnetRange;
                this.ClearMoveBoost();
                this.HealthRegenRemainder = 0;
                if (this.Mods.GlowDistance == 0)
                    Utility.removeLightSource(this.GlowID);
            }

            // recalculate aggregates
            this.Mods.Recalculate();

            // reapply
            if (Context.IsWorldReady)
            {
                Game1.player.attackIncreaseModifier += this.Mods.Attack;
                Game1.player.knockbackModifier += this.Mods.Knockback;
                Game1.player.critChanceModifier += this.Mods.CritChance;
                Game1.player.critPowerModifier += this.Mods.CritPower;
                Game1.player.weaponSpeedModifier += this.Mods.WeaponSpeed;
                Game1.player.weaponPrecisionModifier += this.Mods.WeaponPrecision;
                Game1.player.MagneticRadius += this.Mods.MagnetRange;
                if (this.Mods.GlowDistance > 0)
                    Game1.currentLightSources.Add(new LightSource(LightSource.lantern, new Vector2(Game1.player.position.X + Game1.tileSize / 3, Game1.player.position.Y + Game1.tileSize), this.Mods.GlowDistance, new Color(0, 30, 150), this.GlowID));
            }
        }

        /// <summary>Remove the previously-applied speed boost.</summary>
        private void ClearMoveBoost()
        {
            if (this.LastMoveBoost != 0)
                Game1.player.addedSpeed = this.GetVanillaSpeedBoost();
            this.LastMoveBoost = 0;
        }

        /// <summary>Get the speed boost for all vanilla buffs.</summary>
        private int GetVanillaSpeedBoost()
        {
            int boost = 0, v = 0;
            foreach (Buff b in Game1.buffsDisplay.otherBuffs)
            {
                v = ((int[])this.BuffAttributes.GetValue(b))[9];
                if (v != 0)
                    boost += v;
            }

            if (Game1.buffsDisplay.food != null)
            {
                v = ((int[])this.BuffAttributes.GetValue(Game1.buffsDisplay.food))[9];
                if (v != 0)
                    boost += v;
            }

            if (Game1.buffsDisplay.drink != null)
            {
                v = ((int[])this.BuffAttributes.GetValue(Game1.buffsDisplay.drink))[9];
                if (v != 0)
                    boost += v;
            }

            return boost;
        }
    }
}
