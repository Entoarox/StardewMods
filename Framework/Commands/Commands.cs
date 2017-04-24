using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

using StardewValley;

namespace Entoarox.Framework.Commands
{
    static class Commands
    {
        internal static List<string> Farms = new List<string>() { "standard", "river", "forest", "hilltop", "wilderniss" };
        private static string Verify;
        internal static void farm(string command, string[] args)
        {
            if(!Game1.hasLoadedGame)
            {
                EntoFramework.Logger.Log("You need to load a game before you can use this command.", LogLevel.Error);
                return;
            }
            switch(command)
            {
                case "farm_settype":
                    if(args.Length==0)
                        EntoFramework.Logger.Log("Please provide the type you wish to change your farm to.", LogLevel.Error);
                    else if (Farms.Contains(args[0]))
                    {
                        Game1.whichFarm = Farms.IndexOf(args[0]);
                        EntoFramework.Logger.Log($"Changed farm type to `{args[0]}`, please sleep in a bed then quit&restart to finalize this change.", LogLevel.Alert);
                    }
                    else
                        EntoFramework.Logger.Log("Unknown farm type: " + args[0], LogLevel.Error);
                    break;
                case "farm_clear":
                    if (Verify == null || args.Length == 0 || !args[0].Equals(Verify))
                    {
                        Verify = new Random().Next().ToString();
                        EntoFramework.Logger.Log($"This will remove all objects, natural and user-made from your farm, use `farm_clear {Verify}` to verify that you actually want to do this.", LogLevel.Alert);
                        return;
                    }
                    Farm farm = Game1.getFarm();
                    farm.objects.Clear();
                    break;
            }
        }
        internal static void player(string command, string[] args)
        {
            if (!Game1.hasLoadedGame)
            {
                EntoFramework.Logger.Log("You need to load a game before you can use this command.", LogLevel.Error);
                return;
            }
            switch(command)
            {
                case "player_warp":
                    try
                    {
                        int x = Convert.ToInt32(args[1]);
                        int y = Convert.ToInt32(args[2]);
                        Game1.warpFarmer(args[0], x, y, false);
                        EntoFramework.Logger.Log("Player warped.", LogLevel.Alert);
                    }
                    catch(Exception err)
                    {
                        EntoFramework.Logger.Log(LogLevel.Error, "A error occured trying to warp: ", err);
                    }
                    break;
            }
        }
    }
}
