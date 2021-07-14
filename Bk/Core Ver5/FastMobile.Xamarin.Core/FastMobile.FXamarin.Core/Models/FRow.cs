using Newtonsoft.Json.Linq;

namespace FastMobile.FXamarin.Core
{
    public class FRow
    {
        public string Value { get; set; }
        public string Width { get; set; }
        public string Align { get; set; }
        public string VAlign { get; set; }
        public string Style { get; set; }
        public string Color { get; set; }
        public string FontSize { get; set; }
        public double Height { get; set; }
        public int Line { get; set; }
        public string Text { get; set; }
        public string Hidden { get; set; }

        public FRow()
        {
        }

        public FRow(JObject obj)
        {
            Width = FFunc.GetStringValue(obj, "Width");
            Align = FFunc.GetStringValue(obj, "Align");
            VAlign = FFunc.GetStringValue(obj, "VAlign");
            Style = FFunc.GetStringValue(obj, "Style");
            Color = FFunc.GetStringValue(obj, "Color");
            FontSize = FFunc.GetStringValue(obj, "FontSize");
            Height = FFunc.GetNumberValue(obj, "Height", 25);
            Value = FFunc.GetStringValue(obj, "Value");
            Line = (int)FFunc.GetNumberValue(obj, "Line", 1);
            Text = FFunc.GetStringValue(obj, "Text");
            Hidden = FFunc.GetStringValue(obj, "Hidden");
        }
    }
}