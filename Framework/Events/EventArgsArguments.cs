using System;
namespace Entoarox.Framework.Events
{
    public class EventArgsArguments : EventArgs
    {
        public object[] Arguments;
        public EventArgsArguments(object[] arguments = null)
        {
            this.Arguments = arguments ?? new object[0];
        }
    }
}
