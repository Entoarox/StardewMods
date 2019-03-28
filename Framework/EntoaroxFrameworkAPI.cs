using System.Collections.Generic;
using System.Linq;
using Entoarox.Framework.Core;

namespace Entoarox.Framework
{
    public class EntoaroxFrameworkAPI
    {
        /*********
        ** Fields
        *********/
        private static PlayerModifier Modifier;
        private static readonly Dictionary<string, int> RunBoosts = new Dictionary<string, int>();
        private static readonly Dictionary<string, int> WalkBoosts = new Dictionary<string, int>();

        /*********
        ** Public methods
        *********/
        public void AddBoost(string id, int amount, bool forWalking = true, bool forRunning = true)
        {
            if (forWalking)
                EntoaroxFrameworkAPI.WalkBoosts[id] = amount;
            if (forRunning)
                EntoaroxFrameworkAPI.RunBoosts[id] = amount;
            EntoaroxFrameworkAPI.Recalculate();
        }

        public void RemoveBoost(string id)
        {
            EntoaroxFrameworkAPI.WalkBoosts.Remove(id);
            EntoaroxFrameworkAPI.RunBoosts.Remove(id);
            EntoaroxFrameworkAPI.Recalculate();
        }


        /*********
        ** Protected methods
        *********/
        private static void Recalculate()
        {
            if (EntoaroxFrameworkAPI.Modifier == null)
                EntoaroxFrameworkAPI.Modifier = new PlayerModifier();
            EntoaroxFrameworkMod.SHelper.Player().Modifiers.Remove(EntoaroxFrameworkAPI.Modifier);
            EntoaroxFrameworkAPI.Modifier.WalkSpeedModifier = EntoaroxFrameworkAPI.WalkBoosts.Values.Aggregate((total, next) => total + next);
            EntoaroxFrameworkAPI.Modifier.RunSpeedModifier = EntoaroxFrameworkAPI.RunBoosts.Values.Aggregate((total, next) => total + next);
            EntoaroxFrameworkMod.SHelper.Player().Modifiers.Add(EntoaroxFrameworkAPI.Modifier);
        }
    }
}
