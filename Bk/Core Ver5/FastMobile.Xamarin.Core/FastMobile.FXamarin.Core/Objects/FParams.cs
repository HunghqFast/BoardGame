namespace FastMobile.FXamarin.Core
{
    public class FParam
    {
        public string ParamValue { get; set; }
        public string Name { get; set; }

        public FParam(string sName, string sValue)
        {
            ParamValue = sValue;
            Name = sName;
        }
    }
}