using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace FastMobile.FXamarin.Core
{
    public class FViews
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Hidden { get; set; }
        public string IsExpand { get; set; }
        public string Type { get; set; }
        public bool Line { get; set; }
        public List<FRow> Row { get; set; }
        public FViewPage Reference { get; set; }
        public string Controller { get; set; }
        public string Bio { get; set; }
        public string BioPassword { get; set; }
        public string Password { get; set; }

        public FViews()
        {
        }

        public FViews(JObject obj)
        {
            Id = FFunc.GetStringValue(obj, "ID");
            Title = FFunc.GetStringValue(obj, "Title");
            Hidden = FFunc.GetStringValue(obj, "Hidden");
            IsExpand = FFunc.GetStringValue(obj, "IsExpand");
            Type = FFunc.GetStringValue(obj, "Type");
            Line = FFunc.GetBooleanValue(obj, "Line");
            Row = FFunc<FRow>.GetFListObject((JArray)obj.SelectToken("Row"));
            Controller = FFunc.GetStringValue(obj, "Controller");
            Bio = FFunc.GetStringValue(obj, "Bio");
            BioPassword = FFunc.GetStringValue(obj, "BioPassword");
            Password = FFunc.GetStringValue(obj, "Password");
            Reference = obj["Reference"].Type == JTokenType.Null ? null : new FViewPage((JObject)obj.SelectToken("Reference"));
        }
    }
}