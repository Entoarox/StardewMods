using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

namespace Entoarox.Framework.Core.Utilities
{
    static class Events
    {
        public static void FireEventSafely<TArgs>(string name, Delegate evt, TArgs args, bool obsolete = false) where TArgs : EventArgs
        {
            if (evt == null)
                return;
            foreach (var handler in evt.GetInvocationList())
                try
                {
                    if (typeof(TArgs) == typeof(EventArgs))
                        (handler as EventHandler).Invoke(null, args);
                    else
                        (handler as EventHandler<TArgs>).Invoke(null, args);
                    if (obsolete)
                        EntoaroxFrameworkMod.Logger.LogOnce($"The {handler.Method.Name} event handler is using the deprecated {evt.Target.GetType().Name}.{name} event.", LogLevel.Warn);
                }
                catch (Exception ex)
                {
                    EntoaroxFrameworkMod.Logger.Log($"The {handler.Method.Name} event handler failed handling the {evt.Target.GetType().Name}.{name} event:\n{ex}", LogLevel.Error);
                }
        }
    }
}
