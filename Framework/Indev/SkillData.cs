using System.Collections.Generic;

namespace Entoarox.Framework.Indev
{
    class PlayerSkillData
    {
        public class SkillInfo
        {
            public int Experience = 0;
            public int Level = 0;
            public List<string> Professions = new List<string>();
        }
        public Dictionary<string, SkillInfo> Skills = new Dictionary<string, SkillInfo>();
    }
}
