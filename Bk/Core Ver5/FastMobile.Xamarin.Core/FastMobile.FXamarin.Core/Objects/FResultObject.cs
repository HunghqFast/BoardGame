namespace FastMobile.FXamarin.Core
{
    public class FResultObject<T>
    {
        public FResultObject(bool ok, T value)
        {
            OK = ok;
            Value = value;
        }

        public bool OK { get; }
        public T Value { get; }
    }
}