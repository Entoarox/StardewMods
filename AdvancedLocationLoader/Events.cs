using System;
using Entoarox.Framework;
using Entoarox.Framework.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Entoarox.AdvancedLocationLoader
{
    internal static class Events
    {
        internal static void TimeEvents_AfterDayStarted(object s, EventArgs e)
        {
            ModEntry.UpdateConditionalEdits();
            if (Game1.dayOfMonth == 1)
                ModEntry.UpdateTilesheets();
        }

        internal static void MoreEvents_ActionTriggered(object s, EventArgsActionTriggered e)
        {
            try
            {
                switch (e.Action)
                {
                    case "ALLMessage":
                        Actions.Message(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLRawMessage":
                        Actions.RawMessage(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLShift":
                        Actions.Shift(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLReact":
                        Actions.React(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLRandomMessage":
                        Actions.RandomMessage(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLMinecart":
                    case "ALLTeleporter":
                        Actions.Teleporter(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLCondition":
                    case "ALLConditional":
                        Actions.Conditional(e.Who, e.Arguments, e.Position);
                        break;
                    case "ALLShop":
                        Actions.Shop(e.Who, e.Arguments, e.Position);
                        break;
                    default:
                        return;
                }
                ModEntry.Logger.Log("ActionTriggered(" + e.Action + ")", LogLevel.Trace);
            }
            catch (Exception err)
            {
                ModEntry.Logger.ExitGameImmediately("Could not fire appropriate action response, a unexpected error happened", err);
            }
        }
        internal static void LocationEvents_CurrentLocationChanged(object s, EventArgs e)
        {
            LocationEvents.CurrentLocationChanged -= LocationEvents_CurrentLocationChanged;
            ModEntry.UpdateConditionalEdits();
        }
    }
}
