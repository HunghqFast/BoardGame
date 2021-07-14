using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FData : BindableObject, ICloneable
    {
        public const string BindingPropertyName = "Data";
        public const string CheckStatusName = "isCheck";
        public const string LineNumberRowName = "line_nbr";
        public const string OrdinalNumberName = "stt_rec0";

        public Dictionary<string, object> Data { get; private set; }

        public object this[string name] => Data.TryGetValue(name, out object value) ? value : null;

        public object this[string name, FieldType type]
        {
            set
            {
                if (Data.TryGetValue(name, out _)) Data[name] = ConvertData(value, type);
                else Data.Add(name, ConvertData(value, type));
                OnPropertyChanged(BindingPropertyName);
            }
        }

        public int LineNumberRow { get => Convert.ToInt32(Data[LineNumberRowName]); set => this[LineNumberRowName, FieldType.Number] = value; }

        public bool IsCheck { get => FFunc.StringToBoolean(Data[CheckStatusName].ToString()); set => this[CheckStatusName, FieldType.Bool] = value; }

        public object SttRec0 { get => Data[OrdinalNumberName]; set => this[OrdinalNumberName, FieldType.String] = value; }

        public bool CheckName(string name) => Data.TryGetValue(name, out _);

        public FData()
        {
            Data = new Dictionary<string, object>
            {
                { CheckStatusName, false },
                { LineNumberRowName, null },
                { OrdinalNumberName, null }
            };
        }

        public int GetIndex()
        {
            return Convert.ToInt32(Data[LineNumberRowName]);
        }

        private object ConvertData(object result, FieldType type)
        {
            switch (type)
            {
                case FieldType.Number:
                    return result is null || result.ToString().Trim().Length == 0 ? 0 : decimal.Parse(result.ToString());

                case FieldType.NumberString:
                    return result is null || decimal.Parse(result.ToString()) == 0 ? " " as object : decimal.Parse(result.ToString());

                case FieldType.DateTime:
                    try
                    {
                        if (result is null || result.ToString() == string.Empty || result.ToString() == "null" || result.ToString() == FInputDate.BaseValue || result == DBNull.Value || (result is DateTime dateTime && dateTime == default))
                            return FInputDate.BaseValue;
                        if (DateTime.TryParse(result.ToString(), out DateTime date))
                            return date;
                        return DateTime.ParseExact(result.ToString(), FInputDate.Format, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        return default;
                    }
                case FieldType.Bool:
                    return result is not null && FFunc.StringToBoolean(result.ToString());

                case FieldType.Table:
                    return result;

                case FieldType.Char:
                    return result is null || result == DBNull.Value ? string.Empty : result.ToString().Trim();

                default:
                    return result is null || result == DBNull.Value || result.ToString().Equals(string.Empty) ? " " : result.ToString();
            }
        }

        public static FData NewItem(System.Data.DataRow row, List<FField> fields, bool checkDisable = false)
        {
            FData item = new FData();
            int i = 0;
            fields.ForEach((x) =>
            {
                bool disable = !checkDisable || !x.Disable;
                bool con = x.FieldStatus == FieldStatus.Default && row != null && row.Table.Columns.Contains(x.Name) && disable;
                item[x.Name, x.FieldType] = con ? row[x.Name] : x.DefaultValue ?? string.Empty;
                i++;
            });
            return item;
        }

        public static string GetBindingName(string name)
        {
            return $"{BindingPropertyName}[{name}]";
        }

        public static string FDataToString(FData data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public static FData StringToFData(string json)
        {
            return JsonConvert.DeserializeObject<FData>(json);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}