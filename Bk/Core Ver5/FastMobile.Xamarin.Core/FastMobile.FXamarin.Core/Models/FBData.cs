using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FBData : BindableObject
    {
        public object this[int index]
        {
            get
            {
                PropertyInfo property = GetType().GetProperty($"F{index}");
                return property.GetValue(this, null);
            }
        }

        public object this[string name]
        {
            get
            {
                PropertyInfo property = GetType().GetProperty(name);
                return property.GetValue(this, null);
            }
        }

        public object this[int index, FDFieldType type]
        {
            set
            {
                SetData(GetType().GetProperty($"F{index}"), value.ToString().TrimEnd(' '), type);
            }
        }

        public object this[string name, FDFieldType type]
        {
            set
            {
                SetData(GetType().GetProperty(name), value.ToString().TrimEnd(' '), type);
            }
        }

        private void SetData(PropertyInfo property, string result, FDFieldType type)
        {
            switch (type)
            {
                case FDFieldType.Decimal:
                    try
                    {
                        property.SetValue(this, Double.Parse(result));
                    }
                    catch
                    {
                        property.SetValue(this, 0);
                    }
                    break;

                case FDFieldType.NumberString:
                    try
                    {
                        property.SetValue(this, (Double.Parse(result) == 0) ? " " as object : Double.Parse(result));
                    }
                    catch
                    {
                        property.SetValue(this, " ");
                    }
                    break;

                case FDFieldType.DateTime:
                    try
                    {
                        property.SetValue(this, DateTime.Parse(result));
                    }
                    catch
                    {
                        property.SetValue(this, DateTime.Now);
                    }
                    break;

                case FDFieldType.Bool:
                    try
                    {
                        property.SetValue(this, result.Equals("1") ? true : Boolean.Parse(result));
                    }
                    catch
                    {
                        property.SetValue(this, false);
                    }
                    break;

                default:
                    property.SetValue(this, (result.Equals("")) ? " " : result);
                    break;
            }
        }

        static public FBData NewItem(System.Data.DataRow row, List<FDField> fields)
        {
            var item = New(fields.Count);

            fields.ForEach((x) =>
            {
                switch (x.Status)
                {
                    case FieldStatus.Default:
                        try
                        {
                            item[x.BindingName, x.Type] = row[x.Name];
                        }
                        catch
                        {
                            item[x.BindingName, x.Type] = x.DefaultValue ?? "";
                        }
                        break;

                    case FieldStatus.Internal:
                        item[x.BindingName, x.Type] = x.DefaultValue ?? "";
                        break;

                    default:
                        break;
                }
            });
            return item;
        }

        static public FBData New(int number)
        {
            return number switch
            {
                1 => new FD1(),
                2 => new FD2(),
                3 => new FD3(),
                4 => new FD4(),
                5 => new FD5(),
                6 => new FD6(),
                7 => new FD7(),
                8 => new FD8(),
                9 => new FD9(),
                10 => new FD10(),
                11 => new FD11(),
                12 => new FD12(),
                13 => new FD13(),
                14 => new FD14(),
                15 => new FD15(),
                16 => new FD16(),
                17 => new FD17(),
                18 => new FD18(),
                19 => new FD19(),
                20 => new FD20(),
                _ => new FD20(),
            };
        }
    }

    public class FD1 : FBData
    {
        public static readonly BindableProperty F1Property = BindableProperty.Create("F1", typeof(object), typeof(FD1), null);

        public object F1
        {
            get => (object)GetValue(F1Property);
            set => SetValue(F1Property, value);
        }
    }

    public class FD2 : FD1
    {
        public static readonly BindableProperty F2Property = BindableProperty.Create("F2", typeof(object), typeof(FD2), null);

        public object F2
        {
            get => (object)GetValue(F2Property);
            set => SetValue(F2Property, value);
        }
    }

    public class FD3 : FD2
    {
        public static readonly BindableProperty F3Property = BindableProperty.Create("F3", typeof(object), typeof(FD3), null);

        public object F3
        {
            get => (object)GetValue(F3Property);
            set => SetValue(F3Property, value);
        }
    }

    public class FD4 : FD3
    {
        public static readonly BindableProperty F4Property = BindableProperty.Create("F4", typeof(object), typeof(FD4), null);

        public object F4
        {
            get => (object)GetValue(F4Property);
            set => SetValue(F4Property, value);
        }
    }

    public class FD5 : FD4
    {
        public static readonly BindableProperty F5Property = BindableProperty.Create("F5", typeof(object), typeof(FD5), null);

        public object F5
        {
            get => (object)GetValue(F5Property);
            set => SetValue(F5Property, value);
        }
    }

    public class FD6 : FD5
    {
        public static readonly BindableProperty F6Property = BindableProperty.Create("F6", typeof(object), typeof(FD6), null);

        public object F6
        {
            get => (object)GetValue(F5Property);
            set => SetValue(F5Property, value);
        }
    }

    public class FD7 : FD6
    {
        public static readonly BindableProperty F7Property = BindableProperty.Create("F7", typeof(object), typeof(FD7), null);

        public object F7
        {
            get => (object)GetValue(F7Property);
            set => SetValue(F7Property, value);
        }
    }

    public class FD8 : FD7
    {
        public static readonly BindableProperty F8Property = BindableProperty.Create("F8", typeof(object), typeof(FD8), null);

        public object F8
        {
            get => (object)GetValue(F8Property);
            set => SetValue(F8Property, value);
        }
    }

    public class FD9 : FD8
    {
        public static readonly BindableProperty F9Property = BindableProperty.Create("F9", typeof(object), typeof(FD9), null);

        public object F9
        {
            get => (object)GetValue(F9Property);
            set => SetValue(F9Property, value);
        }
    }

    public class FD10 : FD9
    {
        public static readonly BindableProperty F10Property = BindableProperty.Create("F10", typeof(object), typeof(FD10), null);

        public object F10
        {
            get => (object)GetValue(F10Property);
            set => SetValue(F10Property, value);
        }
    }

    public class FD11 : FD9
    {
        public static readonly BindableProperty F11Property = BindableProperty.Create("F11", typeof(object), typeof(FD11), null);

        public object F11
        {
            get => (object)GetValue(F11Property);
            set => SetValue(F11Property, value);
        }
    }

    public class FD12 : FD9
    {
        public static readonly BindableProperty F12Property = BindableProperty.Create("F12", typeof(object), typeof(FD12), null);

        public object F12
        {
            get => (object)GetValue(F12Property);
            set => SetValue(F12Property, value);
        }
    }

    public class FD13 : FD9
    {
        public static readonly BindableProperty F13Property = BindableProperty.Create("F13", typeof(object), typeof(FD13), null);

        public object F13
        {
            get => (object)GetValue(F13Property);
            set => SetValue(F13Property, value);
        }
    }

    public class FD14 : FD9
    {
        public static readonly BindableProperty F14Property = BindableProperty.Create("F14", typeof(object), typeof(FD14), null);

        public object F14
        {
            get => (object)GetValue(F14Property);
            set => SetValue(F14Property, value);
        }
    }

    public class FD15 : FD10
    {
        public static readonly BindableProperty F15Property = BindableProperty.Create("F15", typeof(object), typeof(FD15), null);

        public object F15
        {
            get => (object)GetValue(F15Property);
            set => SetValue(F15Property, value);
        }
    }

    public class FD16 : FD10
    {
        public static readonly BindableProperty F16Property = BindableProperty.Create("F16", typeof(object), typeof(FD16), null);

        public object F16
        {
            get => (object)GetValue(F16Property);
            set => SetValue(F16Property, value);
        }
    }

    public class FD17 : FD10
    {
        public static readonly BindableProperty F17Property = BindableProperty.Create("F17", typeof(object), typeof(FD17), null);

        public object F17
        {
            get => (object)GetValue(F17Property);
            set => SetValue(F17Property, value);
        }
    }

    public class FD18 : FD10
    {
        public static readonly BindableProperty F18Property = BindableProperty.Create("F18", typeof(object), typeof(FD18), null);

        public object F18
        {
            get => (object)GetValue(F18Property);
            set => SetValue(F18Property, value);
        }
    }

    public class FD19 : FD10
    {
        public static readonly BindableProperty F19Property = BindableProperty.Create("F19", typeof(object), typeof(FD19), null);

        public object F19
        {
            get => (object)GetValue(F19Property);
            set => SetValue(F19Property, value);
        }
    }

    public class FD20 : FD15
    {
        public static readonly BindableProperty F20Property = BindableProperty.Create("F20", typeof(object), typeof(FD20), null);

        public object F20
        {
            get => (object)GetValue(F20Property);
            set => SetValue(F20Property, value);
        }
    }
}