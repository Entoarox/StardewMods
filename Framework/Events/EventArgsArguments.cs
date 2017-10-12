using System;
namespace Entoarox.Framework.Events
{
    public class EventArgsArguments : EventArgs
    {
        public object[] Arguments;
        public EventArgsArguments(object[] arguments = null)
        {
            if (arguments == null)
                Arguments = new object[0];
            else
                Arguments = arguments;
        }
    }
}