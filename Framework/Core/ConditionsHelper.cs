using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StardewValley;
using StardewValley.Locations;

namespace Entoarox.Framework.Core
{
    public class ConditionsHelper // : IConditionsHelper
    {
        /*********
        ** Fields
        *********/
        private readonly Dictionary<string, ConditionResolver> Cache = new Dictionary<string, ConditionResolver>();
        private static readonly string[] WeatherMap = { "sun", "sun", "sun", "pinkleaves", "rain", "storm", "brownleaves", "snow" };
        private static readonly string[] DayMap = { "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday" };
        private static readonly string[] FarmMap = { "default", "fishing", "forest", "hilltop", "wilderness", "other" };
        private static readonly Regex OpAny = new Regex(@"^\((?:.*?)\|(?:.*?)\)$");
        private static readonly Regex OpSet = new Regex(@"^([\d]+)~([\d]+)$");
        private static readonly Regex OpMin = new Regex(@"^([\d]+)>$");
        private static readonly Regex OpMax = new Regex(@"^<([\d]+)$");


        /*********
        ** Accessors
        *********/
        public delegate bool OperatorResolver(string argument, params string[] matches);
        public delegate bool ConditionResolver(string[] arguments, OperatorResolver operatorResolver);


        /*********
        ** Public methods
        *********/
        public void RegisterConditionResolver(string field, ConditionResolver resolver) { }

        public bool ResolveConditionList(string list)
        {
            string[] conds = list.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string cond in conds)
            {
                if (!this.ResolveCondition(cond))
                    return false;
            }

            return true;
        }

        public bool ResolveCondition(string condition)
        {
            if (string.IsNullOrEmpty(condition))
                return true;

            string[] split = condition.Split(':');

            if (split.Length == 2)
            {
                if (this.Cache.ContainsKey(split[0]))
                    return this.Cache[split[0]](split[1].Split(','), this.ResolveOperators);
                if (this.Cache.ContainsKey(split[0].Substring(1)))
                    return !this.Cache[split[0].Substring(1)](split[1].Split(','), this.ResolveOperators);
            }

            return false;
        }


        /*********
        ** Protected methods
        *********/
        private ConditionsHelper()
        {
            this.RegisterConditionResolver("weather", (args, resolver) => resolver(args[0], ConditionsHelper.WeatherMap[Game1.weatherIcon]));
            this.RegisterConditionResolver("day", (args, resolver) => resolver(args[0], ConditionsHelper.DayMap[Game1.dayOfMonth % 7]));
            this.RegisterConditionResolver("date", (args, resolver) => resolver(args[0], Game1.dayOfMonth.ToString()));
            this.RegisterConditionResolver("month", (args, resolver) => resolver(args[0], Game1.currentSeason));
            this.RegisterConditionResolver("year", (args, resolver) => resolver(args[0], Game1.year.ToString()));
            this.RegisterConditionResolver("special", (args, resolver) =>
            {
                if (Game1.weatherIcon == 0)
                    return resolver(args[0], "wedding");
                if (Game1.weatherIcon == 1)
                    return resolver(args[0], "festival");
                return false;
            });
            this.RegisterConditionResolver("flag", (args, resolver) => resolver(args[0], Game1.player.mailReceived.ToArray()));
            this.RegisterConditionResolver("completed", (args, resolver) =>
            {
                List<string> opts = new List<string>();
                if (Game1.stats.daysPlayed > 4)
                    opts.Add("landslide");
                if (Game1.stats.daysPlayed > 30)
                    opts.Add("earthquake");
                if ((Game1.getLocationFromName("Beach") as Beach).bridgeFixed.Value)
                    opts.Add("beach");
                return resolver(args[0], opts.ToArray());
            });
            this.RegisterConditionResolver("wallet", (args, resolver) =>
            {
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
            this.RegisterConditionResolver("married", (args, resolver) =>
            {
                if (string.IsNullOrEmpty(Game1.player.spouse) || Game1.player.spouse.Contains("engaged"))
                    return resolver(args[0], "false");
                return resolver(args[0], Game1.player.spouse, "true");
            });
            this.RegisterConditionResolver("engaged", (args, resolver) =>
            {
                if (string.IsNullOrEmpty(Game1.player.spouse) || !Game1.player.spouse.Contains("engaged"))
                    return false;
                return resolver(args[0], Game1.player.spouse.Substring(7), "true");
            });
            this.RegisterConditionResolver("divorced", (args, resolver) =>
            {
                List<NPC> matches = new List<NPC>();
                Utility.getAllCharacters(matches);
                matches = matches.Where(a => a.divorcedFromFarmer).ToList();
                if (!matches.Any())
                    return false;

                List<string> names = matches.Select(a => a.Name).ToList();
                names.Add("true");
                return resolver(args[0], names.ToArray());
            });
            this.RegisterConditionResolver("house", (args, resolver) => resolver(args[0], Game1.player.HouseUpgradeLevel.ToString()));
            this.RegisterConditionResolver("farm", (args, resolver) => resolver(args[0], ConditionsHelper.FarmMap[Math.Min(5, Game1.whichFarm)]));
            this.RegisterConditionResolver("event", (args, resolver) => resolver(args[0], Game1.player.eventsSeen.Select(a => a.ToString()).ToArray()));
            this.RegisterConditionResolver("money", (args, resolver) => resolver(args[0], Game1.player.money.ToString()));
            this.RegisterConditionResolver("carries", (args, resolver) =>
            {
                IEnumerable<Item> matches = Game1.player.Items.Where(a => a.Name == args[0]);
                if (!matches.Any())
                    return false;
                int c = 0;
                foreach (Item match in matches)
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
            this.RegisterConditionResolver("friendship", (args, resolver) => { return resolver(args[1], Game1.player.getFriendshipHeartLevelForNPC(args[0]).ToString()); });
        }

        private bool ResolveOperators(string arg, params string[] values)
        {
            try
            {
                foreach (string value in values)
                {
                    if (arg.Equals(value))
                        return true;
                    if (ConditionsHelper.OpAny.IsMatch(arg))
                    {
                        string[] parts = arg.Substring(1, arg.Length - 2).Split('|');

                        foreach (string part in parts)
                        {
                            if (this.ResolveOperators(part, values))
                                return true;
                        }
                    }
                    else if (int.TryParse(value, out int result))
                    {
                        if (ConditionsHelper.OpSet.IsMatch(arg))
                        {
                            Match match = ConditionsHelper.OpMin.Match(arg);
                            int min = Convert.ToInt32(match.Groups[0].Value);
                            int max = Convert.ToInt32(match.Groups[1].Value);

                            if (result >= min && result <= max)
                                return true;
                        }
                        else if (ConditionsHelper.OpMin.IsMatch(arg))
                        {
                            Match match = ConditionsHelper.OpMin.Match(arg);
                            int min = Convert.ToInt32(match.Groups[0].Value);

                            if (result >= min)
                                return true;
                        }
                        else if (ConditionsHelper.OpMax.IsMatch(arg))
                        {
                            Match match = ConditionsHelper.OpMax.Match(arg);
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
    }
}
