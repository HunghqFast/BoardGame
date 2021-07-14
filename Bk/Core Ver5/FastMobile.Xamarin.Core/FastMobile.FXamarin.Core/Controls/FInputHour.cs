using Syncfusion.XForms.MaskedEdit;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputHour : FInput
    {
        private bool isEdit;
        public static string BaseValue = "  :  ";
        public static string BaseFormat = "00:00";

        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(string), typeof(FInputHour), BaseValue);

        public FHourPicker Picker;

        public string Value { get => (string)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        public override string Output => $"'{Value}'";

        #region Public

        public FInputHour() : base()
        {
            isEdit = false;
            Type = FieldType.String;
            Picker.OKButton.Clicked += OKButtonClicked;
            Picker.CancelButton.Clicked += CancelButtonClicked;
            Children.Add(Picker);
        }

        public FInputHour(FField field) : base(field)
        {
            isEdit = false;
            Type = FieldType.String;
            Picker.OKButton.Clicked += OKButtonClicked;
            Picker.CancelButton.Clicked += CancelButtonClicked;
            Children.Add(Picker);
        }

        public override void InitValue(bool isRefresh = true)
        {
            base.InitValue(isRefresh);
            if (string.IsNullOrEmpty(CacheName)) SetInput(DefaultValue ?? BaseValue);
            else
            {
                SetInput(GetCacheInput(BaseValue));
                if (isRefresh) ChangeValue += (s, e) => { SetCacheInput(Value); };
            }
        }

        public override void Clear(bool isCompleted = false)
        {
            base.Clear(isCompleted);
            InitValue(false);
        }

        public override bool IsEqual(object oldValue)
        {
            return oldValue.Equals(ReturnValue(0));
        }

        #endregion Public

        #region Protected

        protected override void Init()
        {
            base.Init();
            Picker = new FHourPicker();
        }

        protected override object ReturnValue(int mode)
        {
            return Value == BaseValue ? string.Empty : Value;
        }

        protected override void SetInput(List<string> value, bool isCompleted = false, bool isDisable = false)
        {
            if (Disable && isDisable) return;
            Value = value.Count == 0 ? BaseValue : string.IsNullOrWhiteSpace(value[0]) ? BaseValue : value[0];
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
            var c = new ImageButton();
            var m = new SfMaskedEdit();
            var t = new FInputLayoutDashed();

            c.VerticalOptions = LayoutOptions.CenterAndExpand;
            c.WidthRequest = c.HeightRequest = 20;
            c.HorizontalOptions = LayoutOptions.EndAndExpand;
            c.Margin = new Thickness(1.5, 0);
            c.BackgroundColor = Color.Transparent;
            c.SetBinding(Image.SourceProperty, ValueProperty.PropertyName, BindingMode.Default, new FInputHourValueToIcon(DIcon, VIcon));
            c.Clicked += IconPicker;

            m.FontSize = FSetting.FontSizeLabelContent;
            m.TextColor = FSetting.TextColorContent;
            m.FontFamily = FSetting.FontText;
            m.MaskType = MaskType.Text;
            m.Mask = BaseFormat;
            m.HidePromptOnLeave = true;
            m.Keyboard = Keyboard.Numeric;
            m.ValidationMode = InputValidationMode.LostFocus;
            m.ValueMaskFormat = MaskFormat.IncludePromptAndLiterals;
            m.ReturnType = ReturnType.Done;
            m.SetBinding(SfMaskedEdit.ValueProperty, ValueProperty.PropertyName);
            m.SetBinding(SfMaskedEdit.HorizontalTextAlignmentProperty, TextAlignmentProperty.PropertyName);
            m.SetBinding(SfMaskedEdit.IsEnabledProperty, IsModifierPropertyName);
            m.ValueChanged += OnChangeValue;
            m.ValueChanged += OnValueChanged;
            m.Focused += OnFocused;
            m.Effects.Add(Effect.Resolve($"Core.FPressedEffect"));
            FPressedEffect.SetLongTapCommand(m, new Command(() => IconPicker(c, EventArgs.Empty)));

            t.InputViewPadding = new Thickness(0, (FSetting.FilterInputHeight - FSetting.FontSizeLabelContent) * 0.5 - 2, 0, 1);
            t.HeightRequest = FSetting.FilterInputHeight;
            t.ContainerBackgroundColor = Color.Transparent;
            t.InputView = m;
            t.Effects.Add(Effect.Resolve($"Core.FPressedEffect"));
            FPressedEffect.SetLongTapCommand(t, new Command(() => IconPicker(c, EventArgs.Empty)));

            g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            g.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            g.Children.Add(t, 0, 0);
            g.Children.Add(c, 1, 0);
            return g;
        }

        protected override void InitPropertyByField(FField f)
        {
            CacheName = f.CacheName;
            base.InitPropertyByField(f);
        }

        #endregion Protected

        #region Private

        private void IconPicker(object obj, EventArgs e)
        {
            if (!IsModifier) return;
            if (Value == BaseValue) OnOpenedPicker(obj);
            else
            {
                Value = BaseValue;
                OnCompleteValue(this, EventArgs.Empty);
            }
        }

        private void OnOpenedPicker(object obj)
        {
            Picker.SetDataPicker(Value == BaseValue ? BaseFormat : Value);
            Picker.IsOpen = !Picker.IsOpen;
        }

        private void OKButtonClicked(object sender, EventArgs e)
        {
            var returnValue = Picker.GetDataPicker();
            if (Value != returnValue)
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

        private async void OnFocused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            if (Root == null || (!Root.IsBusy && await CheckScriptFocus()))
            {
                isEdit = true;
                return;
            }
            (sender as SfMaskedEdit).Unfocus();
        }

        private void OnValueChanged(object sender, Syncfusion.XForms.MaskedEdit.ValueChangedEventArgs eventArgs)
        {
            var validValue = ValidValue(Value);
            var isChange = Value != validValue;

            Value = validValue;
            if (isEdit && validValue == (sender as SfMaskedEdit).Value.ToString()) OnCompleteValue(sender, eventArgs);
            isEdit = isChange;
        }

        private string ValidValue(object obj)
        {
            if (obj.Equals("") || obj.Equals(BaseValue)) return BaseValue;
            var value = obj.ToString().Replace("_", "0");
            var hour = double.Parse(value.Substring(0, 2));
            var mimute = double.Parse(value.Substring(3, 2));

            if (mimute > 60)
            {
                hour++;
                mimute -= 60;
            }
            if (hour >= 24) hour = 0;
            return $"{(hour < 10 ? "0" : "")}{hour}:{(mimute < 10 ? "0" : "")}{mimute}";
        }

        private object DIcon()
        {
            return IsModifier ? FIcons.CalendarClock.ToFontImageSource(FSetting.PrimaryColor, FSetting.SizeIconButton) : null;
        }

        private object VIcon()
        {
            return IsModifier ? FIcons.CalendarRemove.ToFontImageSource(FSetting.PrimaryColor, FSetting.SizeIconButton) : null;
        }

        #endregion Private
    }
}