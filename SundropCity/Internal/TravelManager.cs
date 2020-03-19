using System.Collections.Generic;
using System.Linq;

using StardewValley;

namespace SundropCity.Internal
{
    using Json;

    class TravelManager
    {
        public static TravelManager Instance = new TravelManager();
        private readonly Dictionary<string, TravelManagerTarget> Destinations;

        private TravelManager()
        {
            this.Destinations = SundropCityMod.SHelper.Data.ReadJsonFile<Dictionary<string, TravelManagerTarget>>("assets/Data/TravelList.json");
        }

        public void Trigger(string id)
        {
            List <Response>  options = new List<Response>();
            foreach(var dest in this.Destinations.Where(_ => !_.Key.Equals(id)))
                options.Add(new Response(dest.Key, SundropCityMod.SHelper.Translation.Get("Travel.Destination." + dest.Key)));
            options.Add(new Response("", SundropCityMod.SHelper.Translation.Get("Travel.Cancel")));
            Game1.currentLocation.createQuestionDialogue(SundropCityMod.SHelper.Translation.Get("Travel.Question"), options.ToArray(), this.AnswerQuestion, null);
        }

        private void AnswerQuestion(Farmer who, string whichAnswer)
        {
            if (!this.Destinations.ContainsKey(whichAnswer))
                return;
            var target = this.Destinations[whichAnswer];
            Game1.warpFarmer(target.Map, target.X, target.Y, false);
        }
    }
}
