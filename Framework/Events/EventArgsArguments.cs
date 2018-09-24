using System;

namespace Entoarox.Framework.Events
{
    public class EventArgsArguments : EventArgs
    {
        /*********
        ** Accessors
        *********/
        public object[] Arguments;


        /*********
        ** Public methods
        *********/
        public EventArgsArguments(object[] arguments = null)
        {
            this.Arguments = arguments ?? new object[0];
        }
    }
}
