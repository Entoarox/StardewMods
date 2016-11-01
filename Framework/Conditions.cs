using System;
using System.Collections.Generic;
using System.Linq;

using StardewValley;

namespace Entoarox.Framework
{
    public static class Conditions
    {
        public static bool CheckCondition(string condition)
        {
            // Check if this is a negated condition
            if (condition.StartsWith("!"))
                // Check if it is negated multiple times, as this risks a infinite loop situation
                if (condition.StartsWith("!!"))
                    // If we find multiple negations, we forcibly return false
                    return false;
                else
                    // If we dont find multiple negations, we negate the result of a positive condition lookup
                    return !CheckCondition(condition.Substring(1));
            // If it is not a negated condition, we perform a positive condition lookup
            else
                // First, we check for the mostly-static conditions
                switch (condition)
                {
                    // Conditions for the possible weather states
                    case "weatherSun":
                    case "weatherRain":
                    case "weatherSnow":
                    case "weatherStorm":
                    case "weatherFestival":
                    case "weatherWedding":
                    case "weatherFallDebris":
                    case "weatherSummerDebris":
                        return condition == WeatherTypes[Game1.weatherIcon];
                    // Special condition for "Any debris weather"
                    case "weatherDebris":
                        return Game1.weatherIcon == 3 || Game1.weatherIcon == 6;
                    // Special condition for "Weather only possible in the current season"
                    case "weatherSeasonal":
                        switch (Game1.currentSeason)
                        {
                            case "summer":
                                return Game1.weatherIcon == 3;
                            case "fall":
                                return Game1.weatherIcon == 6;
                            case "winter":
                                return Game1.weatherIcon == 7;
                            default:
                                return false;
                        }
                    // Condition for if one of the two special weather states is the current state
                    case "weatherSpecial":
                        return Game1.weatherIcon < 2;
                    // Conditions for each of the 7 days
                    case "sunday":
                    case "monday":
                    case "tuesday":
                    case "wednesday":
                    case "thursday":
                    case "friday":
                    case "saturday":
                        return condition == Weekdays[Game1.dayOfMonth % 7];
                    // Special condition for either saturday or sunday
                    case "weekend":
                        int day = Game1.dayOfMonth % 7;
                        return day == 0 || day == 6;
                    // Conditions for each of the 4 seasons
                    case "spring":
                    case "summer":
                    case "fall":
                    case "winter":
                        return condition == Game1.currentSeason;
                    // Generic condition that is true whenever the player is married, irrelevant of the person they are married to
                    case "married":
                        return Game1.player.spouse != null && !Game1.player.spouse.Contains("engaged");
                    // Generic condition that is true whenever the player is engaged to be married, irrelevant of the person they are engaged to
                    case "engaged":
                        return Game1.player.spouse != null && Game1.player.spouse.Contains("engaged");
                    // Condition that is only true after the earthquake event has happened
                    case "earthquake":
                        return Game1.stats.daysPlayed > 31;
                    // Condition that is only true after the player has received the rusty key
                    case "rustyKey":
                        return Game1.player.hasRustyKey;
                    // Condition that is only true after the player has received the skull key
                    case "skullKey":
                        return Game1.player.hasSkullKey;
                    // Condition that is only true after the player has received the club card
                    case "clubMember":
                        return Game1.player.hasClubCard;
                    // Condition that is only true after the player has learned to speak the dwarven language
                    case "dwarfSpeak":
                        return Game1.player.canUnderstandDwarves;
                    // New and updated condition handling for more complex conditions
                    default:
                        // Handle marriage for any NPC instead of just the default bachelors (Backwards compatible)
                        if (condition.StartsWith("married"))
                            return (Game1.player.spouse != null && Game1.player.spouse == condition.Substring(7));
                        // Handle engagement for any NPC instead of just the default bachelors *new*
                        if (condition.StartsWith("married"))
                            return (Game1.player.spouse != null && Game1.player.spouse.Contains(condition.Substring(7)) && Game1.player.spouse.Contains("engaged"));
                        // Allow year-based conditions for during any specific year
                        if (condition.StartsWith("year="))
                            return Game1.year == Convert.ToInt32(condition.Substring(5));
                        // Allow year-based conditions for after any specific year
                        if (condition.StartsWith("year>"))
                            return Game1.year > Convert.ToInt32(condition.Substring(5));
                        // House upgrade level conditions
                        if (condition.StartsWith("houseLevel="))
                            return Game1.player.houseUpgradeLevel == Convert.ToInt32(condition.Substring(11));
                        // Farm type conditions
                        if (condition.StartsWith("farmType="))
                            return Game1.whichFarm == Convert.ToInt32(condition.Substring(9));
                        // Time conditions *new*
                        if (condition.StartsWith("time>"))
                            return Game1.timeOfDay > Convert.ToInt32(condition.Substring(5));
                        // Check if the condition is a deprecated one, and if so, evaluate its replacement instead
                        if (DeprecatedConditions.ContainsKey(condition))
                            return CheckCondition(DeprecatedConditions[condition]);
                        // Check for mail flags
                        return Game1.player.mailReceived.Contains(condition);
                }
        }
        // Keeps a cache of previously collision-checked conditions, as collision-checking does have some impact
        private static List<string> ConditionCache = new List<string>();
        /**
         * Checks a character-separated list of conditions
         * A conflict check is performed the first time a condition string is asked to be checked
         * The overhead of this conflict check is offset due to most improperly formatted condition lists never being processed
         * Further, this conflict check enables clear and obvious answers to be given as to why the condition list is improperly formatted
         * The result of a conflict check is put into a cache for the remainder of the current session
         * Thus, the second call and after have little to no extra overhead, no matter if correctly formatted or conflicting
         *
         * To decrease unique combinations, condition lists are sorted before any other logic is used on them
         * By sorting, the total amount of possible permutations is brought down greatly
         * This means that the cache for a condition list to have been evaluated previously increases proportionally
         *
         * It should be noted that strict mode should be used during development
         * Strict mode causes extra checks to be used to find performance-causing issues on top of any conflict-related ones
         */
        public static bool CheckConditionList(string conditionlist, char seperator = ',', int limit = 5)
        {
            string conditions = SortedConditionList(conditionlist, seperator);
            if (!ConditionCache.Contains(conditions))
                ConditionCache.Add(conditions);
            string[] conds = conditions.Split(seperator);
            if (conds.Length > limit)
                return false;
            foreach (string condition in conds)
                if (!CheckCondition(condition))
                    return false;
            return true;
        }
        private static string SortedConditionList(string list, char seperator)
        {
            string[] indexes = list.Split(seperator);
            Array.Sort(indexes);
            return string.Join(seperator.ToString(), indexes);
        }
        // Generic template for deprecated conditions
        private static string ConditionDeprecated = "The `$1` condition has been deprecated, use the `$2` condition instead";
        // Generic template for conditions that have their negation in the same condition list
        private static string ConditionAndNegation = "The `$1` condition and its negation `$2` are forcing a false state";
        // Generic template for conditions that are mutually exclusive (If one is true, the other is false)
        private static string ConditionExclusive = "The `$1` and `$2` conditions cannot be used at the same time";
        // Generic template for conditions where one condition needs to be true for the second to apply
        private static string ConditionImplied = "The `$1` condition is not needed as it has to be true for `$2` to apply";
        // Generic template for conditions that do not meet the formatting requirements
        private static string ConditionFormat = "The `$1` condition is not formatted properly";
        // Utility method to replace placeholders in a string with other strings
        private static string Format(string cond, string[] vars)
        {
            for (int c = 0; c < vars.Length; c++)
                cond = cond.Replace("$" + c.ToString(), vars[c]);
            return cond;
        }
        /**
         * This method is designed to find and report as many possible problems a condition list can have
         * Some edge-cases are currently not being detected, due to the difficulty in doing so
         * The reason behind this is that unlike the true/false checker, this method needs to deal with all conditions at the same time
         * It is likely that this method will be further expanded in the future in order to detect problems it currently does not
         *
         * Currently the following conflicts cannot be detected at all:
         * - The use of multiple `!year=` conditions when a single `year>` condition would suffice
         * 
         * 
         * By default, some minor issues are ignored by the conflict detection
         * This is because these conflicts only affect performance rather then act as potential bugs
         * It is therefore recommended, that when creating or updating a condition-using mod, to do so with strict conflict detection enabled
         * When strict mode is enabled, these minor issues are reported just like bug-causing ones are
         * Depending on how often your condition list has to be evaluated, these performance-related conflicts could have a major impact
     
         * A good example are `!summer,!winter,!fall` and `spring`
         * While these condition lists are functionally identical, the second one only has a single resolution call
         * The first one on the other hand, has a total of 6 resolution calls
         * It is 6 resolution calls instead of 6 because of negated conditions needing to have inversion applied to them
         * For this reason, strict mode will report `!year=1` as a condition that should be replaced by `year>1`
         *
         * Other such issues are when possible detected and reported, should strict mode be enabled
         * Depending on the complexity of the condition list, up to 60% in resolution time can be gained
         *
         * At this time, strict mode causes the following issues to be reported:
         * - Deprecated conditions being used
         * - Any condition that is in the list more then once
         * - Use of 3 negative season conditions when a single positive condition will suffice
         * - Use of negative conditions for monday through friday, when using the positive `weekend` condition will suffice
         * - Use of negative weekday conditions for 6 days when a single positive condition will suffice
         * - Use of the `!year=1` two-pass condition instead of the equivalent `year>1` single-pass condition
         * - Use of a condition when another condition in the list cannot be true unless that condition is already true
         */
        private static string CheckSnow(List<string> cache)
        {
            if (cache.Contains("!winter"))
                return Format(ConditionExclusive, new string[] { "weatherSnow", "!winter" });
            if (cache.Contains("spring"))
                return Format(ConditionExclusive, new string[] { "weatherSnow", "spring" });
            if (cache.Contains("summer"))
                return Format(ConditionExclusive, new string[] { "weatherSnow", "summer" });
            if (cache.Contains("fall"))
                return Format(ConditionExclusive, new string[] { "weatherSnow", "fall" });
            return null;
        }
        public static string FindConflictingConditions(string[] conditions, int limit, bool strict)
        {
            return FindConflictingConditions(string.Join(",", conditions), ',', limit, strict);
        }
        public static string FindConflictingConditions(string conds, char seperator = ',', int limit = 5, bool strict = true)
        {
            string[] conditions = conds.Split(seperator);
            // Force the limit to be 4~16
            // Less then 4 would not allow enough freedom
            // While more then 16 should not be possible with the current conditions
            if (limit < 4)
                limit = 4;
            if (limit > 16)
                limit = 16;
            if (conditions.Length > limit)
                return "The conditions list contains more then the maximum `" + limit + "` conditions allowed";
            List<string> cache = new List<string>();
            string weekdayFound = null;
            string seasonFound = null;
            string marriageFound = null;
            string engagementFound = null;
            int negatedSeasons = 0;
            int negatedDays = 0;
            string positiveYearMod = null;
            int positiveYearStart = 0;
            int negativeYearStart = 0;
            string houseLevel = null;
            string weatherCondition = null;
            foreach (string condition in conditions)
            {
                /*
                    There are times where a condition becomes deprecated due to a change in the condition system
                    Deprecated conditions are only reported as being deprecated if strict conflict detection is used
                    Otherwise, the deprecated conditions are treated as generic conditions

                    Note that deprecated conditions are internally replaced by their newer equivalent when resolving
                    This means that deprecated conditions not only do not have full conflict resultion, they also are one step slower to process

                    Further, as said earlier, deprecated conditions are treated as generic conditions rather then connected conditions
                    What this means, is that where `year>1,year>2` would be reported as having conflicts, the same is not true for `secondYear,thirdYear`
                    This is because of the difference between generic and connected conditions
                    Generic conditions are true/false flags that do not have a connection to any other condition
                    Connected conditions are true/false flags that do have this connected behaviour

                    Because of this, extra resolution logic exists in order to detect and report as many of the possible conflicts as it can
                    Deprecated conditions do not trigger this resolution logic, and have in fact been purposefully excluded from it


                */
                if (strict)
                    if (DeprecatedConditions.ContainsKey(condition))
                        return Format(ConditionDeprecated, new string[] { condition, DeprecatedConditions[condition] });
                    else if (DeprecatedConditions.ContainsKey('!' + condition))
                        return Format(ConditionDeprecated, new string[] { '!' + condition, '!' + DeprecatedConditions[condition] });
                // Check for conditions where another condition would be faster to resolve the true/false state but the end-result is the same
                if (strict && ConditionRecommend.ContainsKey(condition))
                    return "The `" + condition + "` condition should be replaced by the functionally equivalent `" + ConditionRecommend[condition] + "` condition as it is faster";
                // Check for conditions that are at least double-negatives
                if (condition.StartsWith("!!"))
                    return "The `" + condition + "` condition performs negation more then once";
                // Check if a condition is in the list more then once
                if (strict && cache.Contains(condition))
                    return "The `" + condition + "` condition has been specified more then once";
                // Check if the positive to this condition is in the list, since that would result in a always-false situation
                else if (condition.StartsWith("!") && cache.Contains(condition.Substring(1)))
                    return Format(ConditionAndNegation, new string[] { condition.Substring(1), condition });
                // Check if the negative to this condition is in the list, since that would result in a always-false situation
                else if (cache.Contains('!' + condition))
                    return Format(ConditionAndNegation, new string[] { condition, '!' + condition });
                // Check for weather conditions
                if (condition.StartsWith("weather"))
                {
                    if (weatherCondition != null)
                        return Format(ConditionExclusive, new string[] { condition, weatherCondition });
                    else
                        weatherCondition = condition;
                    string t = CheckSnow(cache);
                    if (t != null)
                        return t;
                }
                // Check for house upgrade based conditions where conflicts may occur
                if (condition.StartsWith("houseLevel="))
                {
                    if (houseLevel != null)
                        return Format(ConditionExclusive, new string[] { condition, houseLevel });
                    else
                        houseLevel = condition;
                    if (Convert.ToInt32(condition.Substring(11)) < 0)
                        return Format(ConditionFormat, new string[] { condition });
                }
                // Check for the positive year-based conditions where conflicts may occur
                if (condition.StartsWith("year"))
                {
                    if (positiveYearMod != null)
                        return Format(ConditionExclusive, new string[] { condition, positiveYearMod });
                    positiveYearMod = condition;
                    if (condition.StartsWith("year>"))
                    {
                        try
                        {
                            int posYear = Convert.ToInt32(condition.Substring(5));
                            if (posYear < 1)
                                return Format(ConditionFormat, new string[] { condition });
                            if (negativeYearStart < posYear)
                                return Format(ConditionExclusive, new string[] { condition, "!year>" + negativeYearStart });
                            positiveYearStart = posYear;
                        }
                        catch
                        {
                            return Format(ConditionFormat, new string[] { condition });
                        }
                    }
                }
                // Check for the negative year-based conditions where conflicts may occur
                else if (condition.StartsWith("!year>"))
                    try
                    {
                        if (negativeYearStart > 0)
                            return Format(ConditionExclusive, new string[] { condition, "!year>" + negativeYearStart });
                        int negYear = Convert.ToInt32(condition.Substring(5));
                        if (negYear < 1)
                            return Format(ConditionFormat, new string[] { condition });
                        if (positiveYearStart >= negYear)
                            return Format(ConditionExclusive, new string[] { condition, "year>" + positiveYearStart });
                        negativeYearStart = negYear;
                    }
                    catch
                    {
                        return Format(ConditionFormat, new string[] { condition });
                    }
                // Check for situations where marriage conditions may conflict
                else if (condition.StartsWith("married"))
                    if (engagementFound != null)
                        return Format(ConditionExclusive, new string[] { condition, marriageFound });
                    else if (strict && condition == "married" && marriageFound != null)
                        return Format(ConditionImplied, new string[] { condition, marriageFound });
                    else if (strict && cache.Contains("married"))
                        return Format(ConditionImplied, new string[] { "married", condition });
                    else if (marriageFound != null)
                        return Format(ConditionExclusive, new string[] { condition, marriageFound });
                    else
                        marriageFound = condition;
                // Check for situations where engagement conditions may conflict
                else if (condition.StartsWith("engaged"))
                    if (engagementFound != null)
                        return Format(ConditionExclusive, new string[] { condition, engagementFound });
                    else if (strict && condition == "engaged" && engagementFound != null)
                        return Format(ConditionImplied, new string[] { condition, engagementFound });
                    else if (strict && cache.Contains("engaged"))
                        return Format(ConditionImplied, new string[] { "engaged", condition });
                    else if (marriageFound != null)
                        return Format(ConditionExclusive, new string[] { condition, engagementFound });
                    else
                        engagementFound = condition;
                // Check for multiple weekday or season conditions where neither have been negated, since it cannot be multiple days or seasons at the same time
                // Also check if any negated weekday or season conditions apply, since having both a fixed day/season and a not day/season is pointless
                else
                    switch (condition)
                    {
                        case "sunday":
                        case "monday":
                        case "tuesday":
                        case "wednesday":
                        case "thursday":
                        case "friday":
                        case "saturday":
                            if (weekdayFound != null)
                                return Format(ConditionExclusive, new string[] { condition, weekdayFound });
                            else
                                weekdayFound = condition;
                            if (!strict)
                                break;
                            string[] matchedDays = (string[])cache.Intersect(WeekdaysInverted);
                            if (matchedDays.Any())
                                return Format(ConditionImplied, new string[] { condition, matchedDays[0] });
                            break;
                        case "spring":
                        case "summer":
                        case "fall":
                        case "winter":
                            if (seasonFound != null)
                                return Format(ConditionExclusive, new string[] { condition, seasonFound });
                            else
                                seasonFound = condition;
                            string t = CheckSnow(cache);
                            if (t != null)
                                return t;
                            if (!strict)
                                break;
                            string[] matchedSeasons = (string[])cache.Intersect(SeasonsInverted);
                            if (matchedSeasons.Any())
                                return Format(ConditionImplied, new string[] { condition, matchedSeasons[0] });
                            break;
                        case "!spring":
                        case "!summer":
                        case "!fall":
                        case "!winter":
                            negatedSeasons++;
                            if (strict && negatedSeasons > 2)
                                return "The condition list contains more then 2 negative season conditions, use a single positive season condition instead";
                            string ts = CheckSnow(cache);
                            if (ts != null)
                                return ts;
                            break;
                        case "!sunday":
                        case "!monday":
                        case "!tuesday":
                        case "!wednesday":
                        case "!thursday":
                        case "!friday":
                        case "!saturday":
                            if (!strict)
                                break;
                            negatedDays++;
                            if (negatedDays > 5)
                                return "The condition list contains more then 5 negative weekday conditions, use a single positive weekday condition instead";
                            if (negatedDays > 4 && !cache.Contains("!sunday") && !cache.Contains("!saturday"))
                                return "The condition list contains negated weekday conditions for all days except saturday and sunday, use the positive `weekend` condition instead";
                            break;
                    }
                // If this condition has passed all the checks, we cache the condition so other conditions in the list can refer to it
                cache.Add(condition);
            }
            return null;
        }
        // Lookup tables for logic operations
        private static string[] Weekdays = new string[] { "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday" };
        private static string[] WeekdaysInverted = new string[] { "!sunday", "!monday", "!tuesday", "!wednesday", "!thursday", "!friday", "!saturday" };
        private static string[] Seasons = new string[] { "spring", "summer", "fall", "winter" };
        private static string[] SeasonsInverted = new string[] { "!spring", "!summer", "!fall", "!winter" };
        private static string[] WeatherTypes = new string[] { "weatherWedding", "weatherFestival", "weatherSun", "weatherSummerDebris", "weatherRain", "weatherStorm", "weatherFallDebris", "weatherSnow" };
        private static string[] FarmTypes = new string[] { "farmDefault", "farmFishing", "farmForest", "farmHilltop", "farmWilderness" };
        // List of conditions that have been deprecated in favour of others for one reason or another
        private static Dictionary<string, string> DeprecatedConditions = new Dictionary<string, string>()
        {
            {"secondYear","year>1"},
            {"thirdYear","year>2"},
            {"noHouseUpgrade","houseLevel=0"},
            {"firstHouseUpgrade","houseLevel=1"},
            {"secondHouseUpgrade","houseLevel=2"},
            {"farmDefault","farmType=0" },
            {"farmFishing","farmType=1" },
            {"farmForest","farmType=2" },
            {"farmHilltop","farmType=3" },
            {"farmWilderness","farmType=4" }
        };
        // List of conditions where another condition would be functionally equivalent but faster to resolve
        private static Dictionary<string, string> ConditionRecommend = new Dictionary<string, string>()
            {
                {"!year=1","year>1"}
            };
    }
}