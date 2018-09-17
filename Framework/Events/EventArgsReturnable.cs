namespace Entoarox.Framework.Events
{
    public class EventArgsReturnable<TReturn> : EventArgsArguments
    {
        /*********
        ** Fields
        *********/
        private TReturn _Value;


        /*********
        ** Accessors
        *********/
        internal bool ReturnSet;

        public TReturn Value
        {
            set
            {
                this.ReturnSet = true;
                this._Value = value;
            }
            get => this._Value;
        }


        /*********
        ** Public methods
        *********/
        public EventArgsReturnable(object[] arguments = null)
            : base(arguments) { }
    }
}
