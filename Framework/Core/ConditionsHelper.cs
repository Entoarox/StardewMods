using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using StardewValley;

namespace Entoarox.Framework.Core
{
    public class ConditionsHelper// : IConditionsHelper
    {
        #region Delegates
        public delegate bool OperatorResolver(string argument, params string[] matches);
        public delegate bool ConditionResolver(string[] arguments, OperatorResolver operatorResolver);
        #endregion
        #region Setup
        private ConditionsHelper()
        {
            RegisterConditionResolver("weather", (args, resolver) => resolver(args[0], WeatherMap[Game1.weatherIcon]));
            RegisterConditionResolver("day", (args, resolver) => resolver(args[0], DayMap[Game1.dayOfMonth % 7]));
            RegisterConditionResolver("date", (args, resolver) => resolver(args[0], Game1.dayOfMonth.ToString()));
            RegisterConditionResolver("month", (args, resolver) => resolver(args[0], Game1.currentSeason));
            RegisterConditionResolver("year", (args, resolver) => resolver(args[0], Game1.year.ToString()));
            RegisterConditionResolver("special", (args, resolver) => {
                if (Game1.weatherIcon == 0)
                    return resolver(args[0], "wedding");
                if (Game1.weatherIcon == 1)
                    return resolver(args[0], "festival");
                return false;
            });
            RegisterConditionResolver("flag", (args, resolver) => resolver(args[0], Game1.player.mailReceived.ToArray()));
            RegisterConditionResolver("completed", (args, resolver) => {
                List<string> opts = new List<string>();
                if (Game1.stats.daysPlayed > 4)
                    opts.Add("landslide");
                if (Game1.stats.daysPlayed > 30)
                    opts.Add("earthquake");
                if ((Game1.getLocationFromName("Beach") as StardewValley.Locations.Beach).bridgeFixed.Value)
                    opts.Add("beach");
                return resolver(args[0], opts.ToArray());
            });
            RegisterConditionResolver("wallet", (args, resolver) => {
                List<string> opts = new List<string>();
                if (Game1.player.canUnderstandDwarves)
                    opts.Add("dwarfscroll");
                if (Game1.player.hasClubCard)
                    opts.Add("clubcard");
                if (Game1.player.hasDarkTalisman)
                    opts.Add("darktalisman");
                if (Game1.player.hasMagicInk)
                    opts.Add("magicink");
                if (Game1.player.hasRustyKey)
                    opts.Add("rustykey");
                if (Game1.player.hasSkullKey)
                    opts.Add("skullkey");
                if (Game1.player.hasSpecialCharm)
                    opts.Add("specialcharm");
                return resolver(args[0], opts.ToArray());
            });
            RegisterConditionResolver("married", (args, resolver) => {
                if (string.IsNullOrEmpty(Game1.player.spouse) || Game1.player.spouse.Contains("engaged"))
                    return resolver(args[0], "false");
                return resolver(args[0], Game1.player.spouse, "true");
            });
            RegisterConditionResolver("engaged", (args, resolver) => {
                if (string.IsNullOrEmpty(Game1.player.spouse) || !Game1.player.spouse.Contains("engaged"))
                    return false;
                return resolver(args[0], Game1.player.spouse.Substring(7), "true");
            });
            RegisterConditionResolver("divorced", (args, resolver) => {
                List<NPC> matches = null;
                foreach (NPC c in Utility.getAllCharacters())
                {
                    if (c.divorcedFromFarmer) matches.Add(c);
                }
                if (matches is null)
                    return false;
                var list = matches.Select(a => a.Name).ToList();
                list.Add("true");
                return resolver(args[0], list.ToArray());
            });
            RegisterConditionResolver("house", (args, resolver) => {
                return resolver(args[0], Game1.player.HouseUpgradeLevel.ToString());
            });
            RegisterConditionResolver("farm", (args, resolver) => {
                return resolver(args[0], FarmMap[Math.Min(5, Game1.whichFarm)]);
            });
            RegisterConditionResolver("event", (args, resolver) => {
                return resolver(args[0], Game1.player.eventsSeen.Select(a => a.ToString()).ToArray());
            });
            RegisterConditionResolver("money", (args, resolver) => {
                return resolver(args[0], Game1.player.money.ToString());
            });
            RegisterConditionResolver("carries", (args, resolver) => {
                var matches= Game1.player.Items.Where(a => a.Name == args[0]);
                if (!matches.Any())
                    return false;
                int c = 0;
                foreach (var match in matches)
                    c += match.Stack;
                return resolver(args[1], c.ToString());
            });
            /*TODO RegisterConditionResolver("shipped", (args, resolver) => {
                var matches = Game1.player.ship.Where(a => a.Name == args[0]);
                if (!matches.Any())
                    return false;
                int c = 0;
                foreach (var match in matches)
                    c += match.Stack;
                return resolver(args[1], c.ToString());
            });
            */
            //TODO RegisterConditionResolver("skill");
            RegisterConditionResolver("friendship", (args, resolver) => {
                return resolver(args[1], Game1.player.getFriendshipHeartLevelForNPC(args[0]).ToString());
            });
        }
        #endregion
        #region Public API
        public void RegisterConditionResolver(string field, ConditionResolver resolver)
        {
        }

        public bool ResolveConditionList(string list)
        {
            string[] conds = list.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string cond in conds)
                if (!ResolveCondition(cond))
                    return false;
            return true;
        }

        public bool ResolveCondition(string condition)
        {
            if (string.IsNullOrEmpty(condition))
                return true;

            string[] split = condition.Split(new[] { ':' });

            if (split.Length == 2)
                if (this.Cache.ContainsKey(split[0]))
                    return this.Cache[split[0]](split[1].Split(','), this.ResolveOperators);
                else if (this.Cache.ContainsKey(split[0].Substring(1)))
                    return !this.Cache[split[0].Substring(1)](split[1].Split(','), this.ResolveOperators);
            return false;
        }
        #endregion
        #region Internal Mechanics
        private Dictionary<string, ConditionResolver> Cache = new Dictionary<string, ConditionResolver>();
        private static string[] WeatherMap = new string[] { "sun", "sun", "sun", "pinkleaves", "rain", "storm", "brownleaves", "snow" };
        private static string[] DayMap = new string[] { "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday" };
        private static string[] FarmMap = new string[] { "default", "fishing", "forest", "hilltop", "wilderness", "other" };
        private static Regex OpAny = new Regex(@"^\((?:.*?)\|(?:.*?)\)$");
        private static Regex OpSet = new Regex(@"^([\d]+)~([\d]+)$");
        private static Regex OpMin = new Regex(@"^([\d]+)>$");
        private static Regex OpMax = new Regex(@"^<([\d]+)$");

        private bool ResolveOperators(string arg, params string[] values)
        {
            try
            {
                foreach (string value in values)
                {
                    if (arg.Equals(value))
                        return true;
                    if (OpAny.IsMatch(arg))
                    {
                        string[] parts = arg.Substring(1, arg.Length - 2).Split(new[] { '|' });

                        foreach (string part in parts)
                            if (ResolveOperators(part, values))
                                return true;
                    }
                    else if (int.TryParse(value, out int result))
                    {
                        if (OpSet.IsMatch(arg))
                        {
                            var match = OpMin.Match(arg);
                            int min = Convert.ToInt32(match.Groups[0].Value);
                            int max = Convert.ToInt32(match.Groups[1].Value);

                            if (result >= min && result <= max)
                                return true;
                        }
                        else if (OpMin.IsMatch(arg))
                        {
                            var match = OpMin.Match(arg);
                            int min = Convert.ToInt32(match.Groups[0].Value);

                            if (result >= min)
                                return true;
                        }
                        else if (OpMax.IsMatch(arg))
                        {
                            var match = OpMax.Match(arg);
                            int max = Convert.ToInt32(match.Groups[0].Value);

                            if (result <= max)
                                return true;
                        }
                    }
                }
            }
            catch
            {
            }
            return false;
        }
        #endregion
    }
}
