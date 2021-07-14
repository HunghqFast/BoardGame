using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputDate : FInput
    {
        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(DateTime), typeof(FInputDate), DateTime.Now);

        public FDatePicker Picker;
        public const string Format = "yyyyMMdd";
        public const string FormatText = "dd/MM/yyyy";
        public const string BaseValue = "  /  /    ";

        public DateTime Value { get => (DateTime)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        public override string Output => ((int)Value.ToOADate()).ToString();

        #region Public

        public FInputDate() : base()
        {
            Type = FieldType.DateTime;
            Picker.OKButton.Clicked += OKButtonClicked;
            Picker.CancelButton.Clicked += CancelButtonClicked;
            Children.Add(Picker);
        }

        public FInputDate(FField field) : base(field)
        {
            Type = FieldType.DateTime;
            Picker.OKButton.Clicked += OKButtonClicked;
            Picker.CancelButton.Clicked += CancelButtonClicked;
            Children.Add(Picker);
        }

        public override void InitValue(bool isRefresh = true)
        {
            base.InitValue(isRefresh);
            if (string.IsNullOrEmpty(CacheName))
            {
                if (DefaultValue == null) Value = DateTime.Now;
                else if (DefaultValue.ToString().Equals(BaseValue) || DefaultValue.ToString() == "null") Value = default;
                else Value = DateTime.ParseExact(DefaultValue.ToString(), FormatText, null);
            }
            else
            {
                try { Value = DateTime.ParseExact(GetCacheInput(DateTime.Now.ToString(Format)), Format, null); }
                catch { CacheName.RemoveCache(); Value = DateTime.Now; }
                if (isRefresh) ChangeValue += (s, e) => { SetCacheInput(Value.ToString(Format)); };
            }
        }

        public override void Clear(bool isCompleted = false)
        {
            base.Clear(isCompleted);
            InitValue(false);
        }

        public override void FocusInput()
        {
            base.FocusInput();
        }

        public override void UnFocusInput()
        {
            Picker.IsOpen = false;
            base.UnFocusInput();
        }

        public override bool IsEqual(object oldValue)
        {
            try
            {
                if (oldValue.Equals(BaseValue)) return Value == default;
                return ((DateTime)oldValue).ToString(Format) == Value.ToString(Format);
            }
            catch
            {
                return false;
            }
        }

        public static string GetRequestValue(object value)
        {
            if (value.ToString() == BaseValue) return string.Empty;
            if (DateTime.Parse(value.ToString()) != default) return DateTime.Parse(value.ToString()).ToString(Format);
            return string.Empty;
        }

        #endregion

        #region Protected

        protected override void Init()
        {
            base.Init();
            Picker = new FDatePicker();
        }

        protected override object ReturnValue(int mode)
        {
            return Value;
        }

        protected override void SetInput(List<string> value, bool isCompleted = false, bool isDisable = false)
        {
            if (Disable && isDisable) return;
            var isNull = value[0] == BaseValue || string.IsNullOrWhiteSpace(value[0]);
            Value = isNull ? default : DateTime.TryParse(value[0], out DateTime result) ? result.ToOADate() == 0 ? default : result : DateTime.Now;
            base.SetInput(value, isCompleted);
        }

        protected override void OnChangeValue(object sender, EventArgs e)
        {
            base.OnChangeValue(sender, new FInputChangeValueEventArgs(Value));
        }

        protected override void OnCompleteValue(object sender, EventArgs e)
        {
            base.OnCompleteValue(sender, new FInputChangeValueEventArgs(Value));
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(IsModifier):
                    OnPropertyChanged(ValueProperty.PropertyName);
                    break;
                default:
                    break;
            }
        }

        protected override View SetContentView()
        {
            var g = new Grid();
            var s = new StackLayout();
            var c = new ImageButton();
            var e = new FEntryBase();

            c.VerticalOptions = LayoutOptions.CenterAndExpand;
            c.WidthRequest = c.HeightRequest = 20;
            c.HorizontalOptions = LayoutOptions.EndAndExpand;
            c.Margin = new Thickness(1.5, 0);
            c.BackgroundColor = Color.Transparent;
            c.SetBinding(Image.SourceProperty, ValueProperty.PropertyName, BindingMode.Default, new FInputDateValueToIcon(DIcon, VIcon));
            c.Clicked += IconPicker;

            e.ReturnType = ReturnType.Done;
            e.HeightRequest = FSetting.FilterInputHeight;
            e.IsReadOnly = true;
            e.InputTransparent = FSetting.IsAndroid;
            e.SetBinding(Entry.TextProperty, ValueProperty.PropertyName, BindingMode.Default, new FInputDateValueToText(BaseValue, FormatText));
            e.SetBinding(Entry.HorizontalTextAlignmentProperty, TextAlignmentProperty.PropertyName);
            e.TextChanged += OnChangeValue;

            s.Margin = 0;
            s.Children.Add(e);
            s.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(OnOpenedPicker), CommandParameter = e });

            g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            g.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            g.Children.Add(s, 0, 0);
            g.Children.Add(c, 1, 0);
            return g;
        }

        protected override void InitPropertyByField(FField f)
        {
            CacheName = f.CacheName;
            base.InitPropertyByField(f);
        }

        #endregion

        #region Private

        private void IconPicker(object obj, EventArgs e)
        {
            if (!IsModifier) return;
            if (Value == default) OnOpenedPicker(obj);
            else
            {
                Value = default;
                OnCompleteValue(this, EventArgs.Empty);
            }
        }

        private void OnOpenedPicker(object obj)
        {
            if (!IsModifier) return;
            Picker.SetDataPicker((Value == default ? DateTime.Now : Value).ToString(Format));
            Picker.IsOpen = !Picker.IsOpen;
        }

        private void OKButtonClicked(object sender, EventArgs e)
        {
            var returnValue = Picker.GetDataPicker();
            if (Value.ToString(Format) != returnValue.ToString(Format))
            {
                Value = returnValue;
                OnCompleteValue(this, EventArgs.Empty);
            }
            Picker.IsOpen = false;
        }

        private void CancelButtonClicked(object sender, EventArgs e)
        {
            Picker.IsOpen = false;
        }

        private object DIcon()
        {
            return IsModifier ? FIcons.CalendarMonth.ToFontImageSource(FSetting.PrimaryColor, FSetting.SizeIconButton) : null;
        }

        private object VIcon()
        {
            return IsModifier ? FIcons.CalendarRemove.ToFontImageSource(FSetting.PrimaryColor, FSetting.SizeIconButton) : null;
        }

        #endregion
    }
}