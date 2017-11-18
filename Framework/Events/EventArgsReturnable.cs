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
                this.ReturnSet = true;
                this._Value = value;
            }
            get
            {
                return this._Value;
            }
        }
        public EventArgsReturnable(object[] arguments = null) : base(arguments)
        {

        }
    }
}