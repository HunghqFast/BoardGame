using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FSwitch : Switch
    {
        public static readonly BindableProperty OffColorProperty = BindableProperty.Create(nameof(OffColor), typeof(Color), typeof(FSwitch), Color.Default);
        public static readonly BindableProperty OnThumbColorProperty = BindableProperty.Create(nameof(OnThumbColor), typeof(Color), typeof(FSwitch), Color.Default);
        public static readonly BindableProperty OffThumbColorProperty = BindableProperty.Create(nameof(OffThumbColor), typeof(Color), typeof(FSwitch), Color.Default);

        public Color OffColor
        {
            get { return (Color)GetValue(OffColorProperty); }
            set { SetValue(OffColorProperty, value); }
        }

        public Color OnThumbColor
        {
            get { return (Color)GetValue(OnThumbColorProperty); }
            set { SetValue(OnThumbColorProperty, value); }
        }

        public Color OffThumbColor
        {
            get { return (Color)GetValue(OffThumbColorProperty); }
            set { SetValue(OffThumbColorProperty, value); }
        }

        public FSwitch() : base()
        {
            Base();

            //if (FSetting.IsAndroid)
            //{
            //    S.SetBinding(FSwitch.OnThumbColorProperty, IsModifierPropertyName, converter: new FBoolToObject(FSetting.PrimaryColor.ColorTrans("90"), FSetting.PrimaryColor.ColorTrans("60")));
            //    S.SetBinding(FSwitch.OffThumbColorProperty, IsModifierPropertyName, converter: new FBoolToObject(FSetting.DisableColor.ColorTrans("80"), FSetting.DisableColor.ColorTrans("60")));
            //    S.SetBinding(FSwitch.OnColorProperty, IsModifierPropertyName, converter: new FBoolToObject(FSetting.PrimaryColor.ColorTrans("70"), FSetting.PrimaryColor.ColorTrans("70")));
            //    S.SetBinding(FSwitch.OffColorProperty, IsModifierPropertyName, converter: new FBoolToObject(FSetting.DisableColor.ColorTrans("50"), FSetting.DisableColor.ColorTrans("40")));
            //}
            //else
            //{
            //    S.OnThumbColor = Color.White;
            //    S.OffThumbColor = Color.White;
            //    S.SetBinding(FSwitch.OnColorProperty, IsModifierPropertyName, converter: new FBoolToObject(FSetting.PrimaryColor, Color.FromHex("#87b6f7")));
            //    S.SetBinding(FSwitch.OffColorProperty, IsModifierPropertyName, converter: new FBoolToObject(Color.FromHex("#c8c8c8"), Color.FromHex("#e9e9e9")));
            //}
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == IsEnabledProperty.PropertyName || propertyName == IsToggledProperty.PropertyName)
            {
                Base();
                return;
            }
        }

        protected virtual void Base()
        {
            if (IsEnabled && IsToggled)
            {
                UpdateTurnOn();
                return;
            }

            if (IsEnabled && !IsToggled)
            {
                UpdateTurnOff();
                return;
            }

            if (!IsEnabled && IsToggled)
            {
                UpdateTurnOnDisable();
                return;
            }
            UpdateTurnOffDisable();
        }

        protected virtual void UpdateTurnOn()
        {
            if (FSetting.IsAndroid)
            {
                OnColor = FSetting.PrimaryColor.ColorTrans("80");
                OnThumbColor = Color.FromHex("#2c7ef2");
                return;
            }

            OnColor = FSetting.PrimaryColor;
            OnThumbColor = Color.White;
        }

        protected virtual void UpdateTurnOff()
        {
            if (FSetting.IsAndroid)
            {
                OffColor = Color.FromHex("#d4d4d4");
                OffThumbColor = FSetting.DisableColor.ColorTrans("75");
                return;
            }

            OffColor = Color.FromHex("#c8c8c8");
            OffThumbColor = Color.White;
        }

        protected virtual void UpdateTurnOnDisable()
        {
            if (FSetting.IsAndroid)
            {
                OnColor = FSetting.PrimaryColor.ColorTrans("40");
                OnThumbColor = FSetting.PrimaryColor.ColorTrans("60");
                return;
            }
            OnColor = Color.FromHex("#87b6f7");
            OnThumbColor = Color.White;
        }

        protected virtual void UpdateTurnOffDisable()
        {
            if (FSetting.IsAndroid)
            {
                OffColor = FSetting.DisableColor.ColorTrans("40");
                OffThumbColor = FSetting.DisableColor.ColorTrans("60");
                return;
            }
            OffColor = Color.FromHex("#e9e9e9");
            OffThumbColor = Color.White;
        }
    }
}