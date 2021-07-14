using Syncfusion.SfNumericTextBox.XForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputNumber : FInput
    {
        private bool isChanged;
        private bool haveMaxValue;

        protected FSfNumericTextBox N;

        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(object), typeof(FInputNumber), "");
        public static readonly BindableProperty FormatProperty = BindableProperty.Create("Format", typeof(string), typeof(FInputNumber));
        public static readonly BindableProperty MaxValueProperty = BindableProperty.Create("MaxValue", typeof(object), typeof(FInputNumber));
        public static readonly BindableProperty MinValueProperty = BindableProperty.Create("MinValue", typeof(object), typeof(FInputNumber));
        public static readonly BindableProperty MaxDigitsProperty = BindableProperty.Create("MaxDigits", typeof(int), typeof(FInputNumber), 0);

        public object Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        public string Format { get => (string)GetValue(FormatProperty); set => SetValue(FormatProperty, value); }

        public object MaxValue { get => GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }

        public object MinValue { get => GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }

        public int MaxDigits { get => (int)GetValue(MaxDigitsProperty); set => SetValue(MaxDigitsProperty, value); }

        public string GroupSeparate { get; set; }

        public string DecimalSeparate { get; set; }

        public int MaxLength { get; set; }

        public int[] NumberGroupSizes => Format?.Split(DecimalSeparate)[0].Split(GroupSeparate).Select(x => x.Length > 9 ? 9 : x.Length).Reverse().ToArray();

        public override string Output => Value.ToString();

        #region Public

        public FInputNumber() : base()
        {
            N = new FSfNumericTextBox();
            N.Culture.NumberFormat.NumberGroupSeparator = GroupSeparate;
        }

        public FInputNumber(FField field) : base(field)
        {
            N = new FSfNumericTextBox();
            N.Culture.NumberFormat.NumberGroupSeparator = GroupSeparate;
        }

        public override void InitValue(bool isRefresh = true)
        {
            base.InitValue(isRefresh);
            if (string.IsNullOrEmpty(CacheName)) SetInput(DefaultValue ?? "0");
            else
            {
                SetInput(GetCacheInput("0"));
                if (isRefresh) ChangeValue += (s, e) => { SetCacheInput(Value.ToString()); };
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
            Type = FieldType.Number;
            GroupSeparate = " ";
            DecimalSeparate = ".";
            isChanged = false;
            base.Init();
        }

        protected override object ReturnValue(int mode)
        {
            return ObjectToDecimal(Value, MaxDigits);
        }

        protected override void SetInput(List<string> value, bool isCompleted = false, bool isDisable = false)
        {
            if (Disable && isDisable) return;
            Value = ObjectToDecimal(value[0], MaxDigits);
            base.SetInput(value, isCompleted);
        }

        protected override void OnChangeValue(object sender, EventArgs e)
        {
            isChanged = true;
            UpdateMaxMinValue((e as ValueEventArgs).OldValue);
            base.OnChangeValue(sender, new FInputChangeValueEventArgs(Value));
        }

        protected override void OnCompleteValue(object sender, EventArgs e)
        {
            isChanged = false;
            base.OnCompleteValue(sender, new FInputChangeValueEventArgs(Value));
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(Format):
                    MaxDigits = Format.Contains(DecimalSeparate) ? Format.Length - Format.IndexOf(DecimalSeparate) - 1 : 0;
                    MaxLength = Format.Remove(DecimalSeparate).Remove(GroupSeparate).Length - MaxDigits;
                    if (MaxValue == null || MaxValue.Equals(double.NaN)) MaxValue = null;
                    else haveMaxValue = true;
                    if (MinValue == null || MinValue.Equals(double.NaN)) MinValue = null;
                    break;

                default:
                    break;
            }
        }

        protected override View SetContentView()
        {
            var t = new FInputLayoutDashed();
            if (FSetting.IsAndroid)
            {
                N.GroupSeparatorMode = GroupSeparatorMode.LostFocus;
                N.EnableGroupSeparator = false;
            }
            else if (NumberGroupSizes.Length > 1)
            {
                N.GroupSeparatorMode = GroupSeparatorMode.Always;
                N.EnableGroupSeparator = true;
            }
            N.ValueChangeMode = ValueChangeMode.OnKeyFocus;
            N.SetBinding(SfNumericTextBox.ValueProperty, ValueProperty.PropertyName);
            N.SetBinding(SfNumericTextBox.MaximumProperty, MaxValueProperty.PropertyName);
            N.SetBinding(SfNumericTextBox.MinimumProperty, MinValueProperty.PropertyName);
            N.SetBinding(SfNumericTextBox.MaximumNumberDecimalDigitsProperty, MaxDigitsProperty.PropertyName);
            N.SetBinding(SfNumericTextBox.TextAlignmentProperty, TextAlignmentProperty.PropertyName);
            N.SetBinding(SfNumericTextBox.FormatStringProperty, FormatProperty.PropertyName);
            N.SetBinding(SfNumericTextBox.IsEnabledProperty, IsModifierPropertyName);
            N.ValueChanged += OnChangeValue;
            N.Completed += OnCompleteValue;
            N.Focused += OnFocused;
            N.Unfocused += OnUnFocused;

            t.InputViewPadding = new Thickness(0, (FSetting.FilterInputHeight - FSetting.FontSizeLabelContent) * 0.5 - 2, 0, 1);
            t.HeightRequest = FSetting.FilterInputHeight;
            t.ContainerBackgroundColor = Color.Transparent;
            t.InputView = N;
            return t;
        }

        protected override void InitPropertyByField(FField f)
        {
            MaxValue = f.Max;
            MinValue = f.Min;
            Format = f.DataFormatString;
            base.InitPropertyByField(f);
        }

        #endregion Protected

        #region Private

        private void UpdateMaxMinValue(object oldValue)
        {
            if (haveMaxValue) return;
            var value0 = ObjectToDecimal(Value, MaxDigits);
            var value0_old = ObjectToDecimal(oldValue, MaxDigits);
            if (value0 < 0)
            {
                if (Math.Truncate(Math.Abs(value0)).ToString().Length > MaxLength)
                {
                    Value = value0_old;
                }
                return;
            }

            value0 = Math.Abs(value0);
            value0_old = Math.Abs(value0_old);

            var value1 = Math.Truncate(value0);
            var value1_old = Math.Truncate(value0_old);
            var isTypeInDecimal = value1 >= value1_old;
            var value2 = value0 - value1;
            var cDecimal = value1.ToString().Length >= MaxLength;
            var cDigit = (value2.ToString().Length - (value2 == 0 ? 1 : 2)) >= MaxDigits;
            MaxValue = cDecimal ? value1 + (isTypeInDecimal || cDigit ? value2 : 1 - 1 / (decimal)Math.Pow(10, MaxDigits)) : null;
        }

        private decimal ObjectToDecimal(object obj, int digit)
        {
            if (obj == null) return 0;
            return Math.Round(decimal.TryParse(obj.ToString(), NumberStyles.Float, null, out decimal result) ? result : 0, digit);
        }

        private void RemoveMaxMinValue()
        {
            if (!haveMaxValue) MaxValue = null;
        }

        private void UpdateNumberGroupSizes()
        {
            N.Culture.NumberFormat.NumberGroupSizes = NumberGroupSizes;
        }

        private async void OnFocused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            UpdateNumberGroupSizes();
            UpdateMaxMinValue(Value);
            if (Root == null || (!Root.IsBusy && await CheckScriptFocus())) return;
            N.Unfocus();
        }

        private void OnUnFocused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            RemoveMaxMinValue();
            if (isChanged) OnCompleteValue(sender, EventArgs.Empty);
        }

        #endregion Private
    }
}