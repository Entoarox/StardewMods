using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using StardewModdingAPI.Events;

using StardewValley;

namespace Entoarox.Framework
{
    internal class PlayerHelper : IPlayerHelper
    {
        internal static IPlayerHelper Singleton { get; } = new PlayerHelper();
        void IPlayerHelper.AddModifier(FarmerModifier modifier)
        {
            Modifiers.Add(modifier);
            UpdateCompound();
        }
        void IPlayerHelper.RemoveModifier(FarmerModifier modifier)
        {
            if (Modifiers.Contains(modifier))
                Modifiers.Remove(modifier);
            UpdateCompound();
        }
        bool IPlayerHelper.HasModifier(FarmerModifier modifier)
        {
            return Modifiers.Contains(modifier);
        }
        private static List<FarmerModifier> Modifiers = new List<FarmerModifier>();
        private static FarmerModifier Compound = new FarmerModifier();
        private static int MyUnique = (int)DateTime.Now.Ticks;
        private static float HealthCount = 0;
        private static float StaminaCount = 0;
        private static int[] ExpWatcher = new int[5];
        private static int[] OldLevels = new int[5];
        private static List<Point> OldLevelups;
        private static void RestoreLevels()
        {
            Game1.player.experiencePoints[Farmer.combatSkill] = ExpWatcher[Farmer.combatSkill];
            Game1.player.experiencePoints[Farmer.farmingSkill] = ExpWatcher[Farmer.farmingSkill];
            Game1.player.experiencePoints[Farmer.fishingSkill] = ExpWatcher[Farmer.fishingSkill];
            Game1.player.experiencePoints[Farmer.foragingSkill] = ExpWatcher[Farmer.foragingSkill];
            Game1.player.experiencePoints[Farmer.miningSkill] = ExpWatcher[Farmer.miningSkill];
            Game1.player.CombatLevel = OldLevels[Farmer.combatSkill];
            Game1.player.FarmingLevel = OldLevels[Farmer.farmingSkill];
            Game1.player.FishingLevel = OldLevels[Farmer.fishingSkill];
            Game1.player.ForagingLevel = OldLevels[Farmer.foragingSkill];
            Game1.player.MiningLevel = OldLevels[Farmer.miningSkill];
            Game1.player.newLevels = OldLevelups;
        }
        private static void UpdateWatchers()
        {
            ExpWatcher[Farmer.combatSkill] = Game1.player.experiencePoints[Farmer.combatSkill];
            ExpWatcher[Farmer.farmingSkill] = Game1.player.experiencePoints[Farmer.farmingSkill];
            ExpWatcher[Farmer.fishingSkill] = Game1.player.experiencePoints[Farmer.fishingSkill];
            ExpWatcher[Farmer.foragingSkill] = Game1.player.experiencePoints[Farmer.foragingSkill];
            ExpWatcher[Farmer.miningSkill] = Game1.player.experiencePoints[Farmer.miningSkill];
            OldLevels[Farmer.combatSkill] = Game1.player.CombatLevel;
            OldLevels[Farmer.farmingSkill] = Game1.player.FarmingLevel;
            OldLevels[Farmer.fishingSkill] = Game1.player.FishingLevel;
            OldLevels[Farmer.foragingSkill] = Game1.player.ForagingLevel;
            OldLevels[Farmer.miningSkill] = Game1.player.MiningLevel;
            OldLevelups = Game1.player.newLevels;
        }
        internal static void ResetForNewGame()
        {
            try
            {
                Game1.player.attackIncreaseModifier = 0;
                Game1.player.knockbackModifier = 0;
                Game1.player.critChanceModifier = 0;
                Game1.player.critPowerModifier = 0;
                Game1.player.weaponSpeedModifier = 0;
                Game1.player.weaponPrecisionModifier = 0;
                Game1.player.magneticRadius = Game1.tileSize * 2;
                UpdateWatchers();
                try
                {
                    if (Game1.player.leftRing != null)
                        Game1.player.leftRing.onEquip(Game1.player);
                    if (Game1.player.rightRing != null)
                        Game1.player.rightRing.onEquip(Game1.player);
                    foreach (Buff b in Game1.player.buffs)
                        b.addBuff();
                }
                catch (Exception err)
                {
                    EntoFramework.Logger.Warn("Unable to apply vanilla buffs, a unexpected error occured", err);
                }
                UpdateCompound();
            }
            catch(Exception err)
            {
                EntoFramework.Logger.Error("Unable to initialize the PlayerHelper class, a unexpected error occured", err);
            }
        }
        private static void UpdateSpeed()
        {
            SavedSpeed = 0;
            if (Game1.player.running)
                SavedSpeed = Compound.RunSpeedModifier;
            else
                SavedSpeed = Compound.WalkSpeedModifier;
            foreach (Buff b in Game1.buffsDisplay.otherBuffs)
            {
                int v = (Attributes.GetValue(b) as int[])[9];
                if (v != 0)
                    SavedSpeed += v;
            }
            if (Game1.buffsDisplay.food != null)
            {
                int v1 = (Attributes.GetValue(Game1.buffsDisplay.food) as int[])[9];
                if (v1 != 0)
                    SavedSpeed += v1;
            }
            if (Game1.buffsDisplay.drink != null)
            {
                int v2 = (Attributes.GetValue(Game1.buffsDisplay.drink) as int[])[9];
                if (v2 != 0)
                    SavedSpeed += v2;
            }
        }
        private static void UpdateCompound()
        {
            if (!Game1.hasLoadedGame)
                return;
            Game1.player.attackIncreaseModifier -= Compound.attackIncreaseModifier;
            Game1.player.knockbackModifier -= Compound.KnockbackModifier;
            Game1.player.critChanceModifier -= Compound.CritChanceModifier;
            Game1.player.critPowerModifier -= Compound.CritPowerModifier;
            Game1.player.weaponSpeedModifier -= Compound.WeaponSpeedModifier;
            Game1.player.weaponPrecisionModifier -= Compound.WeaponPrecisionModifier;
            Game1.player.magneticRadius -= Compound.MagnetRange;
            if (Compound.GlowDistance == 0)
                Utility.removeLightSource(MyUnique);
            SpeedState = 0;
            SavedSpeed = 0;
            Game1.player.addedSpeed = 0;
            Compound = new FarmerModifier();
            foreach (FarmerModifier mod in Modifiers)
            {
                Compound.MagnetRange += mod.MagnetRange;
                Compound.GlowDistance = Math.Max(Compound.GlowDistance, mod.GlowDistance);
                Compound.StaminaRegenModifier += mod.StaminaRegenModifier;
                Compound.HealthRegenModifier += mod.HealthRegenModifier;
                Compound.WalkSpeedModifier += mod.WalkSpeedModifier;
                Compound.RunSpeedModifier += mod.RunSpeedModifier;
                Compound.attackIncreaseModifier += mod.attackIncreaseModifier;
                Compound.KnockbackModifier += mod.KnockbackModifier;
                Compound.CritChanceModifier += mod.CritChanceModifier;
                Compound.CritPowerModifier += mod.CritPowerModifier;
                Compound.WeaponSpeedModifier += mod.WeaponSpeedModifier;
                Compound.WeaponPrecisionModifier += mod.WeaponPrecisionModifier;

                Compound.ExperienceModifierCombat += mod.ExperienceModifierCombat;
                Compound.ExperienceModifierFarming += mod.ExperienceModifierFarming;
                Compound.ExperienceModifierFishing += mod.ExperienceModifierFishing;
                Compound.ExperienceModifierForaging += mod.ExperienceModifierForaging;
                Compound.ExperienceModifierMining += mod.ExperienceModifierMining;
            }
            Compound.ExperienceModifierCombat /= Modifiers.Count+1;
            Compound.ExperienceModifierFarming /= Modifiers.Count+1;
            Compound.ExperienceModifierFishing /= Modifiers.Count+1;
            Compound.ExperienceModifierForaging /= Modifiers.Count+1;
            Compound.ExperienceModifierCombat /= Modifiers.Count+1;
            Game1.player.attackIncreaseModifier += Compound.attackIncreaseModifier;
            Game1.player.knockbackModifier += Compound.KnockbackModifier;
            Game1.player.critChanceModifier += Compound.CritChanceModifier;
            Game1.player.critPowerModifier += Compound.CritPowerModifier;
            Game1.player.weaponSpeedModifier += Compound.WeaponSpeedModifier;
            Game1.player.weaponPrecisionModifier += Compound.WeaponPrecisionModifier;
            Game1.player.magneticRadius += Compound.MagnetRange;
            if (Compound.GlowDistance > 0)
                Game1.currentLightSources.Add(new LightSource(Game1.lantern, new Vector2(Game1.player.position.X + (Game1.tileSize / 3), Game1.player.position.Y + Game1.tileSize), Compound.GlowDistance, new Color(0, 30, 150), MyUnique));
        }
        private static int SpeedState = 0;
        private static int SavedSpeed;
        private static System.Reflection.FieldInfo Attributes = typeof(Buff).GetField("buffAttributes", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        private static bool Freeze = false;
        internal static void Update(object s, EventArgs e)
        {
            if (Freeze)
                return;
            try
            {
                /* TEMPORARILY DISABLED DUE TO ISSUES
                // Player Experience handling
                for (int c = 0; c < 5; c++)
                {
                    // We check if the player has gained XP in any of the 5 skills
                    if (Game1.player.experiencePoints[c] != ExpWatcher[c])
                    {
                        // If they did, then we need to find out how much they earned
                        int cDiff = Game1.player.experiencePoints[c] - ExpWatcher[c];
                        // We change this earned amount based upon the modifier for each separate skill
                        switch (c)
                        {
                            case 0:
                                cDiff = (int)(cDiff * Compound.ExperienceModifierCombat);
                                break;
                            case 1:
                                cDiff = (int)(cDiff * Compound.ExperienceModifierFarming);
                                break;
                            case 2:
                                cDiff = (int)(cDiff * Compound.ExperienceModifierFishing);
                                break;
                            case 3:
                                cDiff = (int)(cDiff * Compound.ExperienceModifierForaging);
                                break;
                            case 4:
                                cDiff = (int)(cDiff * Compound.ExperienceModifierMining);
                                break;
                        }
                        // we reset the player to its state before they earned this XP
                        RestoreLevels();
                        // We check if the XP to be awarded is more then 0
                        if (cDiff > 0)
                        {
                            // We award the XP through stardews own system, so we dont have to deal with all the maths involved
                            Game1.player.gainExperience(c, cDiff);
                            // Finally, we update our local watchers so they wont trigger on the XP we just assigned while readying them for the next time XP is earned
                            UpdateWatchers();
                        }
                    }
                }
                */
                // Handle speed
                if (Game1.currentLocation.currentEvent != null && SavedSpeed != 0 && SpeedState == 1)
                {
                    Game1.player.addedSpeed = 0;
                    SpeedState = 2;
                }
                else if (Game1.currentLocation.currentEvent == null && SavedSpeed != 0)
                {
                    Game1.player.addedSpeed = SavedSpeed;
                    SpeedState = 1;
                }
                else if (SpeedState == 0 && (Compound.RunSpeedModifier != 0 || Compound.WalkSpeedModifier != 0))
                {
                    UpdateSpeed();
                    Game1.player.addedSpeed = SavedSpeed;
                    SpeedState = 1;
                }
                else if(SpeedState==1 && Game1.player.addedSpeed!=SavedSpeed)
                {
                    UpdateSpeed();
                    Game1.player.addedSpeed = SavedSpeed;
                }
                // handle regen
                if (Compound.HealthRegenModifier != 0)
                {
                    HealthCount += Compound.HealthRegenModifier;
                    int change = (int)HealthCount;
                    HealthCount -= change;
                    if (Game1.player.health + change < 0)
                        Game1.player.health = 0;
                    else if (Game1.player.health + change > Game1.player.maxHealth)
                        Game1.player.health = Game1.player.maxHealth;
                    else
                        Game1.player.health += change;
                }
                if (Compound.StaminaRegenModifier != 0)
                {
                    StaminaCount += Compound.StaminaRegenModifier;
                    int change = (int)StaminaCount;
                    StaminaCount -= change;
                    if (Game1.player.stamina + change < 0)
                        Game1.player.stamina = 0;
                    else if (Game1.player.stamina + change > Game1.player.maxStamina)
                        Game1.player.stamina = Game1.player.maxStamina;
                    else
                        Game1.player.stamina += change;
                }
                // Handle glowing
                if (Compound.GlowDistance == 0)
                    return;
                Utility.repositionLightSource(MyUnique, new Vector2(Game1.player.position.X + (Game1.tileSize / 3), Game1.player.position.Y));
                if (Game1.currentLocation.isOutdoors || Game1.currentLocation is StardewValley.Locations.MineShaft)
                    return;
                LightSource lightSource = Utility.getLightSource(MyUnique);
                if (lightSource == null)
                    return;
                lightSource.radius = Math.Min(3f, Compound.GlowDistance);
            }
            catch (Exception err)
            {
                EntoFramework.Logger.Fatal("Fatal error attempting to update player tick properties", err);
                Freeze = true;
            }
        }
        internal static void LocationEvents_CurrentLocationChanged(object s, EventArgsCurrentLocationChanged e)
        {
            try
            {
                if (Compound.GlowDistance > 0)
                    Game1.currentLightSources.Add(new LightSource(Game1.lantern, new Vector2(Game1.player.position.X + (Game1.tileSize / 3), Game1.player.position.Y + Game1.tileSize), Compound.GlowDistance, new Color(0, 30, 150), MyUnique));
            }
            catch (Exception err)
            {
                EntoFramework.Logger.Fatal("Fatal error attempting to handle player illumination", err);
                LocationEvents.CurrentLocationChanged -= LocationEvents_CurrentLocationChanged;
            }
        }
    }
}