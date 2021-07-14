using Newtonsoft.Json.Linq;

namespace FastMobile.FXamarin.Core
{
    public class FItem
    {
        public static FItem Empty => new(string.Empty, string.Empty);
        public const string ItemID = "I", ItemValue = "V";

        public string I { get; set; }
        public string V { get; set; }

        public FItem(object id, object value)
        {
            I = id.ToString().TrimEnd();
            V = value.ToString().TrimEnd();
        }

        public FItem(string id, string value)
        {
            I = id.TrimEnd();
            V = value.TrimEnd();
        }

        public FItem(JObject obj)
        {
            I = FFunc.GetStringValue(obj, ItemID).TrimEnd();
            V = FFunc.GetStringValue(obj, ItemValue).TrimEnd();
        }
    }
}