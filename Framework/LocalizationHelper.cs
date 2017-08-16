using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

using StardewValley;

namespace Entoarox.Framework
{
    [Obsolete("Use SMAPI's translation API instead.")]
    public class LocalizationHelper
    {
        private Dictionary<string, string> Strings;
        private string _File;
        public LocalizationHelper(string file)
        {
            _File = file;
        }
        private void InitData()
        {
            string l;
            if (Game1.content.LanguageCodeOverride != null)
                l = Game1.content.LanguageCodeOverride;
            else
                l = Game1.content.CurrentCulture.TwoLetterISOLanguageName;
            try
            {
                if (File.Exists(_File + ".json"))
                    Strings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(_File + ".json"));
                if (File.Exists(_File + "." + l + ".json"))
                {
                    Dictionary<string, string> localised = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(_File + "." + l + ".json"));
                    foreach (KeyValuePair<string, string> pair in localised)
                        if (Strings.ContainsKey(pair.Key))
                            Strings[pair.Key] = pair.Value;
                        else
                            Strings.Add(pair.Key, pair.Value);
                }
            }
            catch(Exception err)
            {
                EntoFramework.Logger.Log("Was unable to populate localization from file: "+_File+err,StardewModdingAPI.LogLevel.Error);
            }
            if (Strings == null)
                Strings = new Dictionary<string, string>();
        }
        [Obsolete("Use the new replacement localizer, as that removes the need to pass in object[] instances",true)]
        public string Localize(string key, object[] replacements)
        {
            return Localize(key, (string[])replacements);
        }
        public string Localize(string key, params string[] replacements)
        {
            if(Strings==null)
            InitData();
            if (!Strings.ContainsKey(key))
                return "=("+key+')';
            string str = Strings[key];
            for (var c = 0; c < replacements.Length; c++)
                str=str.Replace("{$" + c + "}", replacements[c]);
            return str;
        }
    }
}
