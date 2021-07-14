using Newtonsoft.Json.Linq;

namespace FastMobile.FXamarin.Core
{
    public class FKeys
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public FKeys()
        {
        }

        public FKeys(JObject obj)
        {
            Name = FFunc.GetStringValue(obj, "Name");
            Value = FFunc.GetStringValue(obj, "Value");
        }
    }
}