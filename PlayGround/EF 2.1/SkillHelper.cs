using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

using SFarmer = StardewValley.Farmer;

namespace Entoarox.Framework.Core.Skills
{
    [Obsolete("This functionality is in development and not yet ready for use!", true)]
    class SkillHelper : ISkillHelper
    {
        private static int[] _Vanilla = { 0, 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };
        internal static Dictionary<string, int> _VanillaProfessions = new Dictionary<string, int>()
        {
            {"Rancher" ,0},
            {"Tiller",1 },
            {"Coopmaster",2 },
            {"Shepherd",3 },
            {"Artisan",4 },
            {"Agriculturist",5 },
            {"Fisher",6 },
            {"Trapper",7 },
            {"Angler",8 },
            {"Pirate",9 },
            {"Mariner",10 },
            {"Luremaster",11 },
            {"Forester",12 },
            {"Gatherer",13 },
            {"Lumberjack",14 },
            {"Tapper",15 },
            {"Botanist",16 },
            {"Tracker",17 },
            {"Miner",18 },
            {"Geologist",19 },
            {"Blacksmith",20 },
            {"Prospector",21 },
            {"Excavator",22 },
            {"Gemologist",23 },
            {"Fighter",24 },
            {"Scout",25 },
            {"Brute",26 },
            {"Defender",27 },
            {"Acrobat",28 },
            {"Desperado",29 }
        };
        private Dictionary<string, ISkill> _Skills = new Dictionary<string, ISkill>();
        private void _ValidateFarmer(SFarmer player)
        {
            if (!this._SkillData.ContainsKey(player.UniqueMultiplayerID))
                throw new KeyNotFoundException("There is no farmer by this name registered:" + player.Name);
        }
        private void _ValidateSkill(string skillName)
        {
            if (!this._Skills.ContainsKey(skillName))
                throw new KeyNotFoundException("There is no skill by this name registered: " + skillName);
        }
        private void _ValidateProfession(string skillName, string professionName)
        {
            if (!GetSkillInfo(skillName).Professions.Any(a => a.Name.Equals(professionName)))
                throw new KeyNotFoundException("The `" + skillName + "` skill does not have a profession by this name registered: " + skillName);
        }

        internal Dictionary<long, PlayerSkillData> _SkillData = new Dictionary<long, PlayerSkillData>();
        internal void Setup()
        {
            this.RegisterSkill("Farming", new VanillaSkill(0) {
                
            });
        }

        public void AddSkillExperience(SFarmer player, string skillName, int experience)
        {
            if (experience < 1)
                return;
            _ValidateFarmer(player);
            _ValidateSkill(skillName);
            PlayerSkillData.SkillInfo info = this._SkillData[player.UniqueMultiplayerID].Skills[skillName];
            info.Experience += experience;
            if (info.Experience >= TotalExperienceRequiredForLevel(skillName, info.Level + 1))
            {
#warning TODO: Setup the levelup menu here
                if ((IsActivePrestigeSkill(player, skillName) && (info.Level + 1) % 10 == 0) || (!IsActivePrestigeSkill(player, skillName) && GetSkillInfo(skillName).Professions.Any(a => a.SkillLevel==info.Level+1)))
                    GiveProfessionPoint(player, skillName);
            }
        }
        public void GiveProfessionPoint(SFarmer player, string skillName)
        {
            _ValidateFarmer(player);
            _ValidateSkill(skillName);
#warning TODO: Setup the profession selection menu here
        }

        public IProfession[] GetEarnableProfessions(SFarmer player, string skillName)
        {
#warning TODO: Manage available professions logic
            throw new NotImplementedException();
        }

        public int TotalExperienceRequiredForLevel(string skillName, int level)
        {
            _ValidateSkill(skillName);
            if (level < 11)
                return _Vanilla[level];
            return (int)Math.Round(Math.Floor(Math.Sqrt(Math.Pow(Math.Sqrt(level), Math.Sqrt(level)) / level) * 10000 / level) / 10 / (level / 5)) * 20;
        }

        public int GetLevelExperience(SFarmer player, string skillName)
        {
            _ValidateFarmer(player);
            _ValidateSkill(skillName);
            return this._SkillData[player.UniqueMultiplayerID].Skills[skillName].Experience - TotalExperienceRequiredForLevel(skillName, this._SkillData[player.UniqueMultiplayerID].Skills[skillName].Level);
        }

        public int GetPrestigeLevel(SFarmer player, string skillName)
        {
            _ValidateFarmer(player);
            _ValidateSkill(skillName);
            ISkill skill = GetSkillInfo(skillName);
            return Math.Max(0, this._SkillData[player.UniqueMultiplayerID].Skills[skillName].Level - skill.MaxSkillLevel);
        }

        public ISkill GetSkillInfo(string skillName)
        {
            return this._Skills.ContainsKey(skillName) ? this._Skills[skillName] : null;
        }

        public int GetSkillLevel(SFarmer player, string skillName)
        {
            _ValidateFarmer(player);
            _ValidateSkill(skillName);
            ISkill skill = GetSkillInfo(skillName);
            return Math.Min(skill.MaxSkillLevel, this._SkillData[player.UniqueMultiplayerID].Skills[skillName].Level);
        }

        public int GetTotalExperience(SFarmer player, string skillName)
        {
            _ValidateFarmer(player);
            _ValidateSkill(skillName);
            return this._SkillData[player.UniqueMultiplayerID].Skills[skillName].Experience;
        }

        public void GiveProfession(SFarmer player, string skillName, string professionName)
        {
            _ValidateFarmer(player);
            _ValidateSkill(skillName);
            _ValidateProfession(skillName, professionName);
            ISkill skill = GetSkillInfo(skillName);
            if(skill is VanillaSkill && !player.professions.Contains(_VanillaProfessions[professionName]))
                    player.professions.Add(_VanillaProfessions[professionName]);
            if (!this._SkillData[player.UniqueMultiplayerID].Skills[skillName].Professions.Contains(professionName))
                this._SkillData[player.UniqueMultiplayerID].Skills[skillName].Professions.Add(professionName);
        }

        public bool HasProfession(SFarmer player, string skillName, string professionName)
        {
            _ValidateFarmer(player);
            _ValidateSkill(skillName);
            _ValidateProfession(skillName, professionName);
            return this._SkillData[player.UniqueMultiplayerID].Skills[skillName].Professions.Contains(professionName);
        }

        public bool IsActivePrestigeSkill(SFarmer player, string skillName)
        {
            _ValidateFarmer(player);
            _ValidateSkill(skillName);
            ISkill skill = GetSkillInfo(skillName);
            return this._SkillData[player.UniqueMultiplayerID].Skills[skillName].Level <= skill.MaxSkillLevel;
        }

        public void RegisterSkill(string skillName, ISkill skill)
        {
            if (this._Skills.ContainsKey(skillName))
                throw new ArgumentException("A skill by this name has already been registered: " + skillName);
            this._Skills.Add(skillName, skill);
            foreach (PlayerSkillData data in this._SkillData.Values)
                data.Skills.Add(skillName, new PlayerSkillData.SkillInfo());
        }
    }
}
