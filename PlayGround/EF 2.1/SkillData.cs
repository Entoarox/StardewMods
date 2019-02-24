using System.Collections.Generic;

namespace Entoarox.Framework.Core.Skills
{
    class PlayerSkillData
    {
#pragma warning disable CS0649
        public class SkillInfo
        {
            public int Experience;
            public int Level;
            public List<string> Professions;
        }
        public Dictionary<string, SkillInfo> Skills;
#pragma warning restore CS0649
    }
}
