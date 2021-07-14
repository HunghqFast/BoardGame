using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace FastMobile.FXamarin.Core
{
    public class FScript
    {
        public string Id { get; set; }
        public string ReturnValue { get; set; }
        public List<string> Field { get; set; }
        public string Method { get; set; }
        public string Condition { get; set; }

        public FScript()
        {
        }

        public FScript(JObject obj)
        {
            Id = FFunc.GetStringValue(obj, "Id");
            ReturnValue = FFunc.GetStringValue(obj, "ReturnValue");
            Field = FFunc.GetArrayString(FFunc.GetStringValue(obj, "Field"));
            Method = FFunc.GetStringValue(obj, "Method");
            Condition = FFunc.GetStringValue(obj, "Condition");
        }
    }
}