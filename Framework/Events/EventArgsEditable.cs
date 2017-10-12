namespace Entoarox.Framework.Events
{
    public class EventArgsEditable<TReturn> : EventArgsArguments
    {
        public TReturn Value;
        public EventArgsEditable(TReturn value, object[] arguments=null) : base(arguments)
        {
            Value = value;
        }
    }
}