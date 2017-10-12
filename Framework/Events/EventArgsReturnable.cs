namespace Entoarox.Framework.Events
{
    public class EventArgsReturnable<TReturn> : EventArgsArguments
    {
        internal bool ReturnSet = false;
        private TReturn _Value;
        public TReturn Value
        {
            set
            {
                ReturnSet = true;
                _Value = value;
            }
            get
            {
                return _Value;
            }
        }
        public EventArgsReturnable(object[] arguments = null) : base(arguments)
        {

        }
    }
}