using System.Collections.Generic;

namespace Entoarox.Framework.Indev
{
    class PlayerSkillData
    {
        public class SkillInfo
        {
            public int Experience;
            public int Level;
            public List<string> Professions;
        }
        public Dictionary<string, SkillInfo> Skills;
    }
}
