namespace FastMobile.FXamarin.Core
{
    public class FChangingObjectArgs<T> : FCancelArgs
    {
        public T Value { get; }

        public FChangingObjectArgs(T value) : base()
        {
            Value = value;
        }
    }
}