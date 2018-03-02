using System.Collections.Generic;
using System.Linq;

namespace Entoarox.Framework
{
    using Core;
    public class EntoaroxFrameworkAPI
    {
        private static PlayerModifier Modifier;
        private static Dictionary<string, int> WalkBoosts = new Dictionary<string, int>();
        private static Dictionary<string, int> RunBoosts = new Dictionary<string, int>();
        private static void Recalculate()
        {
            if (Modifier == null)
                Modifier = new PlayerModifier();
            EntoaroxFrameworkMod.SHelper.Player().Modifiers.Remove(Modifier);
            Modifier.WalkSpeedModifier = WalkBoosts.Values.Aggregate((total, next) => total + next);
            Modifier.RunSpeedModifier = RunBoosts.Values.Aggregate((total, next) => total + next);
            EntoaroxFrameworkMod.SHelper.Player().Modifiers.Add(Modifier);
        }

        public void AddBoost(string id, int amount, bool forWalking=true, bool forRunning=true)
        {
            if (forWalking)
                WalkBoosts[id] = amount;
            if (forRunning)
                RunBoosts[id] = amount;
            Recalculate();
        }
        public void RemoveBoost(string id)
        {
            WalkBoosts.Remove(id);
            RunBoosts.Remove(id);
            Recalculate();
        }
    }
}
