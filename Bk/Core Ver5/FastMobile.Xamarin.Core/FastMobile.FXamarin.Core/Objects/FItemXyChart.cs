using System;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FItemXyChart : BindableObject
    {
        public static readonly BindableProperty NameProperty = BindableProperty.Create("Name", typeof(string), typeof(FItemNotify));
        public static readonly BindableProperty Value1Property = BindableProperty.Create("Value1", typeof(double), typeof(FItemNotify), default(double));
        public static readonly BindableProperty Value2Property = BindableProperty.Create("Value2", typeof(double), typeof(FItemNotify), default(double));
        public static readonly BindableProperty Value3Property = BindableProperty.Create("Value3", typeof(double), typeof(FItemNotify), default(double));
        public static readonly BindableProperty Value4Property = BindableProperty.Create("Value4", typeof(double), typeof(FItemNotify), default(double));
        public static readonly BindableProperty Value5Property = BindableProperty.Create("Value5", typeof(double), typeof(FItemNotify), default(double));
        public static readonly BindableProperty StringValue1Property = BindableProperty.Create("StringValue1", typeof(string), typeof(FItemNotify));
        public static readonly BindableProperty StringValue2Property = BindableProperty.Create("StringValue2", typeof(string), typeof(FItemNotify));
        public static readonly BindableProperty StringValue3Property = BindableProperty.Create("StringValue3", typeof(string), typeof(FItemNotify));
        public static readonly BindableProperty StringValue4Property = BindableProperty.Create("StringValue4", typeof(string), typeof(FItemNotify));
        public static readonly BindableProperty StringValue5Property = BindableProperty.Create("StringValue5", typeof(string), typeof(FItemNotify));

        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public double Value1
        {
            get => (double)GetValue(Value1Property);
            set => SetValue(Value1Property, value);
        }

        public double Value2
        {
            get => (double)GetValue(Value2Property);
            set => SetValue(Value2Property, value);
        }

        public double Value3
        {
            get => (double)GetValue(Value3Property);
            set => SetValue(Value3Property, value);
        }

        public double Value4
        {
            get => (double)GetValue(Value4Property);
            set => SetValue(Value4Property, value);
        }

        public double Value5
        {
            get => (double)GetValue(Value5Property);
            set => SetValue(Value5Property, value);
        }

        public string StringValue1
        {
            get => (string)GetValue(StringValue1Property);
            set => SetValue(StringValue1Property, value);
        }

        public string StringValue2
        {
            get => (string)GetValue(StringValue2Property);
            set => SetValue(StringValue2Property, value);
        }

        public string StringValue3
        {
            get => (string)GetValue(StringValue3Property);
            set => SetValue(StringValue3Property, value);
        }

        public string StringValue4
        {
            get => (string)GetValue(StringValue4Property);
            set => SetValue(StringValue4Property, value);
        }

        public string StringValue5
        {
            get => (string)GetValue(StringValue5Property);
            set => SetValue(StringValue5Property, value);
        }

        public static Type Type { get; }

        static FItemXyChart()
        {
            Type = typeof(FItemXyChart);
        }
    }
}