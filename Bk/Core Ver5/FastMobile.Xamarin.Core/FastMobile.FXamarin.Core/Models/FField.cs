using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FField : ICloneable
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string PlaceHolder { get; set; }
        public double Width { get; set; }
        public int MaxLength { get; set; }
        public object Max { get; set; }
        public object Min { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool Hidden { get; set; }
        public bool IsReadOnly { get; set; }
        public bool AllowNulls { get; set; }
        public bool AllowRequest { get; set; }
        public bool AllowFilter { get; set; }
        public bool Push { get; set; }
        public bool Disable { get; set; }
        public TextAlignment Align { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public FieldType FieldType { get; set; }
        public FieldStatus FieldStatus { get; set; }
        public object DefaultValue { get; set; }
        public string DataFormatString { get; set; }
        public bool OnFilter { get; set; }
        public bool OnDemand { get; set; }
        public string Condition { get; set; }
        public string Check { get; set; }
        public string HiddenByKey { get; set; }
        public string CacheName { get; set; }
        public string ItemReference { get; set; }
        public string ItemController { get; set; }
        public List<string> ItemInput { get; set; }
        public FItemStyle ItemStyle { get; set; }
        public string ItemTableName { get; set; }
        public string ItemTargetName { get; set; }
        public string ApiKey { get; set; }
        public string ItemMoveMode { get; set; }
        public bool ItemAdress { get; set; }
        public bool ItemNear { get; set; }
        public bool ItemAnnotation { get; set; }
        public bool ItemClearMode { get; set; }
        public bool ItemAllowNulls { get; set; }
        public bool ItemNormal { get; set; }
        public bool ItemAllowMedia { get; set; }
        public int FilterCharacter { get; set; }
        public List<string> ScriptReference { get; set; }
        public string ScriptDetail { get; set; }
        public bool ScriptCopy { get; set; }
        public string ScriptFocus { get; set; }
        public string HandleKey { get; set; }
        public List<string> HandleField { get; set; }
        public string RequestAction { get; set; }
        public List<string> RequestField { get; set; }
        public List<string> ScripScanner { get; set; }
        public JObject ItemTemplate { get; set; }
        public List<FItem> Item { get; set; }
        public Dictionary<string, string> Keys { get; set; }
        public string Color { get; set; }
        public string TitleColor { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public FField()
        {
        }

        public FField(JObject obj)
        {
            var p = FFunc.GetStringValue(obj, "Type");
            var a = FFunc.GetStringValue(obj, "Align").ToLower();
            var l = FFunc.GetStringValue(obj, "TextAlignment").ToLower();
            var c = FFunc.GetStringValue(obj, "ClientDefault");
            var i = FFunc.GetStringValue(obj, "ItemStyle");
            var m = FFunc.GetNumberValue(obj, "Max", double.NaN);
            var n = FFunc.GetNumberValue(obj, "Min", double.NaN);
            var t = obj.SelectToken("ItemTemplate");
            var o = FFunc.GetStringValue(obj, "OnFilter");

            FieldType = p switch
            {
                "Numeric" => FieldType.NumberString,
                "Decimal" => FieldType.Number,
                "Bool" => FieldType.Bool,
                "DateTime" => FieldType.DateTime,
                "Table" => FieldType.Table,
                "Char" => FieldType.Char,
                "Map" => FieldType.Map,
                "Media" => FieldType.Media,
                _ => FieldType.String
            };
            Align = a switch { "right" => TextAlignment.End, "center" => TextAlignment.Center, _ => TextAlignment.Start };
            DefaultValue = c switch
            {
                "" => null,
                "Today" => DateTime.Now,
                "Yesterday" => DateTime.Now.AddDays(-1),
                "Tomorrow" => DateTime.Now.AddDays(1),
                "ThisMonth" => DateTime.Now.Month,
                "LastMonth" => DateTime.Now.Month - 1,
                "NextMonth" => DateTime.Now.Month + 1,
                "ThisYear" => DateTime.Now.Year,
                "LastYear" => DateTime.Now.Year - 1,
                "NextYear" => DateTime.Now.Year + 1,
                _ => c.Contains("getDate") || c.Contains("getMonth") || c.Contains("getYear") ? FJSInvoke.Invoke(null, c, null) : c,
            };
            ItemStyle = i switch
            {
                "AutoComplete" => FItemStyle.AutoComplete,
                "Lookup" => FItemStyle.Lookup,
                "DropDownList" => FItemStyle.DropDownList,
                "Toggle" => FItemStyle.Toggle,
                "Grid" => FItemStyle.Grid,
                "Dir" => FItemStyle.Dir,
                "Tax" => FItemStyle.Tax,
                "Password" => FItemStyle.Password,
                "Email" => FItemStyle.Email,
                "Captcha" => FItemStyle.CaptCha,
                "TextUnderLine" => FItemStyle.TextUnderLine,
                "Hour" => FItemStyle.Hour,
                "FileUpload" => FItemStyle.File,
                "BioPassword" => FItemStyle.BioPassword,
                "Script" => FItemStyle.ScriptText,
                _ => FieldType switch
                {
                    FieldType.NumberString => FItemStyle.NumberEdit,
                    FieldType.Number => FItemStyle.NumberEdit,
                    FieldType.Bool => FItemStyle.Toggle,
                    FieldType.DateTime => FItemStyle.DateTimePicker,
                    FieldType.Char => FItemStyle.TextCode,
                    FieldType.Map => i switch
                    {
                        "Location" => FItemStyle.Location,
                        _ => FItemStyle.Location
                    },
                    FieldType.Media => i switch
                    {
                        "Image" => FItemStyle.Image,
                        "Video" => FItemStyle.Video,
                        _ => FItemStyle.Image
                    },
                    _ => FItemStyle.TextEdit,
                }
            };
            if (FieldType == FieldType.Number || ItemStyle == FItemStyle.Hour) TextAlignment = l switch { "start" => TextAlignment.Start, "center" => TextAlignment.Center, _ => TextAlignment.End };
            else TextAlignment = l switch { "end" => TextAlignment.End, "center" => TextAlignment.Center, _ => TextAlignment.Start };
            if (ItemStyle == FItemStyle.Toggle && FieldType != FieldType.Bool) FieldType = FieldType.Bool;

            Name = FFunc.GetStringValue(obj, "Name");
            Title = FFunc.GetStringValue(obj, "Header");
            SubTitle = FFunc.GetStringValue(obj, "Footer");
            PlaceHolder = FFunc.GetStringValue(obj, "PlaceHolder");
            DataFormatString = FFunc.GetStringValue(obj, "DataFormatString");
            ItemReference = FFunc.GetStringValue(obj, "ItemReference");
            ItemInput = FFunc.GetArrayString(FFunc.GetStringValue(obj, "ItemInput"));
            ItemTableName = FFunc.GetStringValue(obj, "ItemTableName");
            ItemTargetName = FFunc.GetStringValue(obj, "ItemTargetName");
            ItemController = FFunc.GetStringValue(obj, "ItemController");
            ItemMoveMode = FFunc.GetStringValue(obj, "ItemMoveMode");
            ItemAdress = FFunc.GetBooleanValue(obj, "ItemAdress");
            ItemNear = FFunc.GetBooleanValue(obj, "ItemNear");
            CacheName = FFunc.GetStringValue(obj, "CacheName");
            Condition = FFunc.GetStringValue(obj, "Condition");
            Check = FFunc.GetStringValue(obj, "Check");
            HiddenByKey = FFunc.GetStringValue(obj, "HiddenByKey");
            ApiKey = FFunc.GetStringValue(obj, "ApiKey");
            Color = FFunc.GetStringValue(obj, "Color");
            TitleColor = FFunc.GetStringValue(obj, "TitleColor");

            AllowNulls = !FFunc.GetBooleanValue(obj, "AllowNulls");
            AllowRequest = FFunc.GetBooleanValue(obj, "AllowRequest");
            AllowFilter = FFunc.GetBooleanValue(obj, "AllowFilter");
            Push = FFunc.GetBooleanValue(obj, "Push");
            OnFilter = string.IsNullOrEmpty(o) || FFunc.StringToBoolean(o);
            OnDemand = FFunc.GetBooleanValue(obj, "OnDemand");
            Hidden = FFunc.GetBooleanValue(obj, "Hidden");
            IsReadOnly = !FFunc.GetBooleanValue(obj, "IsReadOnly");
            FieldStatus = FFunc.GetBooleanValue(obj, "Internal") ? FieldStatus.Internal : FieldStatus.Default;
            IsPrimaryKey = FFunc.GetBooleanValue(obj, "IsPrimaryKey");
            Disable = FFunc.GetBooleanValue(obj, "Disable");
            ItemAnnotation = FFunc.GetBooleanValue(obj, "ItemAnnotation");
            ItemClearMode = FFunc.GetBooleanValue(obj, "ItemClearMode");
            ItemAllowNulls = FFunc.GetBooleanValue(obj, "ItemAllowNulls");
            ItemNormal = FFunc.GetBooleanValue(obj, "ItemNormal");
            ItemAllowMedia = FFunc.GetBooleanValue(obj, "ItemAllowMedia");

            Width = FFunc.GetNumberValue(obj, "Width");
            FilterCharacter = (int)FFunc.GetNumberValue(obj, "FilterCharacter", 3);
            MaxLength = (int)FFunc.GetNumberValue(obj, "MaxLength", 256);
            Max = m == double.NaN ? null : m;
            Min = n == double.NaN ? null : n;

            ItemTemplate = t == null || t.Type == JTokenType.Null ? null : (JObject)t;
            Item = FFunc<FItem>.GetFListObject((JArray)obj.SelectToken("Item"));
            ScriptReference = FFunc.GetArrayString(FFunc.GetStringValue(obj, "ScriptReference"));
            ScriptDetail = FFunc.GetStringValue(obj, "ScriptDetail");
            ScriptCopy = FFunc.GetBooleanValue(obj, "ScriptCopy");
            ScriptFocus = FFunc.GetStringValue(obj, "ScripFocus").Trim();
            HandleField = FFunc.GetArrayString(FFunc.GetStringValue(obj, "HandleField"));
            HandleKey = FFunc.GetStringValue(obj, "HandleKey");
            RequestField = FFunc.GetArrayString(FFunc.GetStringValue(obj, "RequestField"));
            RequestAction = FFunc.GetStringValue(obj, "RequestAction");
            ScripScanner = FFunc.GetArrayString(FFunc.GetStringValue(obj, "ScripScanner"));
            Keys = FFunc<FKeys>.GetFListObject((JArray)obj.SelectToken("Keys")).Distinct().ToDictionary(x => x.Name, x => x.Value);
        }
    }
}