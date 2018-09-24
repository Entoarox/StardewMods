using System;
using System.Collections.Generic;
using System.Linq;
using SFarmer = StardewValley.Farmer;

namespace Entoarox.Framework.Indev
{
    [Obsolete("This functionality is in development and not yet ready for use!", true)]
    internal class SkillHelper : ISkillHelper
    {
        /*********
        ** Fields
        *********/
        private readonly Dictionary<string, ISkill> Skills = new Dictionary<string, ISkill>();
        private static readonly int[] Vanilla = { 0, 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };


        /*********
        ** Accessors
        *********/
        internal static Dictionary<string, int> VanillaProfessions = new Dictionary<string, int>
        {
            { "Rancher", 0 },
            { "Tiller", 1 },
            { "Coopmaster", 2 },
            { "Shepherd", 3 },
            { "Artisan", 4 },
            { "Agriculturist", 5 },
            { "Fisher", 6 },
            { "Trapper", 7 },
            { "Angler", 8 },
            { "Pirate", 9 },
            { "Mariner", 10 },
            { "Luremaster", 11 },
            { "Forester", 12 },
            { "Gatherer", 13 },
            { "Lumberjack", 14 },
            { "Tapper", 15 },
            { "Botanist", 16 },
            { "Tracker", 17 },
            { "Miner", 18 },
            { "Geologist", 19 },
            { "Blacksmith", 20 },
            { "Prospector", 21 },
            { "Excavator", 22 },
            { "Gemologist", 23 },
            { "Fighter", 24 },
            { "Scout", 25 },
            { "Brute", 26 },
            { "Defender", 27 },
            { "Acrobat", 28 },
            { "Desperado", 29 }
        };

        internal Dictionary<long, PlayerSkillData> SkillData = new Dictionary<long, PlayerSkillData>();


        /*********
        ** Public methods
        *********/
        public void Setup()
        {
            this.RegisterSkill("Farming", new VanillaSkill(0));
        }

        public void AddSkillExperience(SFarmer player, string skillName, int experience)
        {
            if (experience < 1)
                return;
            this._ValidateFarmer(player);
            this._ValidateSkill(skillName);
            PlayerSkillInfo info = this.SkillData[player.UniqueMultiplayerID].Skills[skillName];
            info.Experience += experience;
            if (info.Experience >= this.TotalExperienceRequiredForLevel(skillName, info.Level + 1))
            {
                // TODO: Setup the levelup menu here
                if (this.IsActivePrestigeSkill(player, skillName) && (info.Level + 1) % 10 == 0 || !this.IsActivePrestigeSkill(player, skillName) && this.GetSkillInfo(skillName).Professions.Any(a => a.SkillLevel == info.Level + 1))
                    this.GiveProfessionPoint(player, skillName);
            }
        }

        public void GiveProfessionPoint(SFarmer player, string skillName)
        {
            this._ValidateFarmer(player);
            this._ValidateSkill(skillName);
            // TODO: Setup the profession selection menu here
        }

        public IProfession[] GetEarnableProfessions(SFarmer player, string skillName)
        {
            // TODO: Manage available professions logic
            throw new NotImplementedException();
        }

        public int TotalExperienceRequiredForLevel(string skillName, int level)
        {
            this._ValidateSkill(skillName);
            if (level < 11)
                return SkillHelper.Vanilla[level];
            return (int)Math.Round(Math.Floor(Math.Sqrt(Math.Pow(Math.Sqrt(level), Math.Sqrt(level)) / level) * 10000 / level) / 10 / (level / 5)) * 20;
        }

        public int GetLevelExperience(SFarmer player, string skillName)
        {
            this._ValidateFarmer(player);
            this._ValidateSkill(skillName);
            return this.SkillData[player.UniqueMultiplayerID].Skills[skillName].Experience - this.TotalExperienceRequiredForLevel(skillName, this.SkillData[player.UniqueMultiplayerID].Skills[skillName].Level);
        }

        public int GetPrestigeLevel(SFarmer player, string skillName)
        {
            this._ValidateFarmer(player);
            this._ValidateSkill(skillName);
            ISkill skill = this.GetSkillInfo(skillName);
            return Math.Max(0, this.SkillData[player.UniqueMultiplayerID].Skills[skillName].Level - skill.MaxSkillLevel);
        }

        public ISkill GetSkillInfo(string skillName)
        {
            return this.Skills.ContainsKey(skillName) ? this.Skills[skillName] : null;
        }

        public int GetSkillLevel(SFarmer player, string skillName)
        {
            this._ValidateFarmer(player);
            this._ValidateSkill(skillName);
            ISkill skill = this.GetSkillInfo(skillName);
            return Math.Min(skill.MaxSkillLevel, this.SkillData[player.UniqueMultiplayerID].Skills[skillName].Level);
        }

        public int GetTotalExperience(SFarmer player, string skillName)
        {
            this._ValidateFarmer(player);
            this._ValidateSkill(skillName);
            return this.SkillData[player.UniqueMultiplayerID].Skills[skillName].Experience;
        }

        public void GiveProfession(SFarmer player, string skillName, string professionName)
        {
            this._ValidateFarmer(player);
            this._ValidateSkill(skillName);
            this._ValidateProfession(skillName, professionName);
            ISkill skill = this.GetSkillInfo(skillName);
            if (skill is VanillaSkill && !player.professions.Contains(SkillHelper.VanillaProfessions[professionName]))
                player.professions.Add(SkillHelper.VanillaProfessions[professionName]);
            if (!this.SkillData[player.UniqueMultiplayerID].Skills[skillName].Professions.Contains(professionName))
                this.SkillData[player.UniqueMultiplayerID].Skills[skillName].Professions.Add(professionName);
        }

        public bool HasProfession(SFarmer player, string skillName, string professionName)
        {
            this._ValidateFarmer(player);
            this._ValidateSkill(skillName);
            this._ValidateProfession(skillName, professionName);
            return this.SkillData[player.UniqueMultiplayerID].Skills[skillName].Professions.Contains(professionName);
        }

        public bool IsActivePrestigeSkill(SFarmer player, string skillName)
        {
            this._ValidateFarmer(player);
            this._ValidateSkill(skillName);
            ISkill skill = this.GetSkillInfo(skillName);
            return this.SkillData[player.UniqueMultiplayerID].Skills[skillName].Level <= skill.MaxSkillLevel;
        }

        public void RegisterSkill(string skillName, ISkill skill)
        {
            if (this.Skills.ContainsKey(skillName))
                throw new ArgumentException("A skill by this name has already been registered: " + skillName);
            this.Skills.Add(skillName, skill);
            foreach (PlayerSkillData data in this.SkillData.Values)
                data.Skills.Add(skillName, new PlayerSkillInfo());
        }


        /*********
        ** Protected methods
        *********/
        private void _ValidateFarmer(SFarmer player)
        {
            if (!this.SkillData.ContainsKey(player.UniqueMultiplayerID))
                throw new KeyNotFoundException("There is no farmer by this name registered:" + player.Name);
        }

        private void _ValidateSkill(string skillName)
        {
            if (!this.Skills.ContainsKey(skillName))
                throw new KeyNotFoundException("There is no skill by this name registered: " + skillName);
        }

        private void _ValidateProfession(string skillName, string professionName)
        {
            if (!this.GetSkillInfo(skillName).Professions.Any(a => a.Name.Equals(professionName)))
                throw new KeyNotFoundException("The `" + skillName + "` skill does not have a profession by this name registered: " + skillName);
        }
    }
}
