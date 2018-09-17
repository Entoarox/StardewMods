namespace Entoarox.Framework.Events
{
    public class EventArgsEditable<TReturn> : EventArgsArguments
    {
        /*********
        ** Accessors
        *********/
        public TReturn Value;


        /*********
        ** Public methods
        *********/
        public EventArgsEditable(TReturn value, object[] arguments = null)
            : base(arguments)
        {
            this.Value = value;
        }
    }
}
