using System.Data;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FSpinWheelData : BindableObject
    {
        public const string dbID = "_ID_", dbName = "_NAME_", dbColor = "_COLOR_", dbStatus = "_STATUS_";

        public static readonly BindableProperty NameProperty = BindableProperty.Create("Name", typeof(string), typeof(FSpinWheelData));
        public static readonly BindableProperty ColorProperty = BindableProperty.Create("Color", typeof(Color), typeof(FSpinWheelData));
        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(double), typeof(FSpinWheelData));
        public static readonly BindableProperty StatusProperty = BindableProperty.Create("Status", typeof(bool), typeof(FSpinWheelData));

        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public bool Status
        {
            get => (bool)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public FSpinWheelData(DataRow row)
        {
            Name = row[dbName].ToString();
            Color = Color.FromHex(row[dbColor].ToString());
            Status = FFunc.StringToBoolean(row[dbStatus].ToString());
        }
    }
}