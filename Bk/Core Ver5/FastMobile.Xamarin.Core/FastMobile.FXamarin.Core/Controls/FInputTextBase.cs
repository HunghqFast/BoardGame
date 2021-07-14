using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public abstract class FInputTextBase : FInput
    {
        private bool isChanged;
        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(string), typeof(FInputText), "");
        public static readonly BindableProperty PlaceHolderProperty = BindableProperty.Create("PlaceHolder", typeof(string), typeof(FInputText));
        public static readonly BindableProperty IsTextUpperProperty = BindableProperty.Create("IsTextUpper", typeof(bool), typeof(FInputText), false);
        public static readonly BindableProperty IsTextLowerProperty = BindableProperty.Create("IsTextUpper", typeof(bool), typeof(FInputText), false);
        public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create("IsPassword", typeof(bool), typeof(FInputText), false);
        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create("MaxLength", typeof(int), typeof(FInputText), Int32.MaxValue);

        public string Value { get => (string)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        public string PlaceHolder { get => (string)GetValue(PlaceHolderProperty); set => SetValue(PlaceHolderProperty, value); }

        public bool IsTextUpper { get => (bool)GetValue(IsTextUpperProperty); set => SetValue(IsTextUpperProperty, value); }

        public bool IsTextLower { get => (bool)GetValue(IsTextLowerProperty); set => SetValue(IsTextLowerProperty, value); }

        public bool IsPassword { get => (bool)GetValue(IsPasswordProperty); set => SetValue(IsPasswordProperty, value); }

        public int MaxLength { get => (int)GetValue(MaxLengthProperty); set => SetValue(MaxLengthProperty, value); }

        public override bool IsModifier => !base.IsModifier;

        public override string Output => $"'{Value}'";

        public EventHandler<TextChangedEventArgs> TextChanged;

        #region Protected

        protected FEntryBase E;

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(Value):
                    if (Value == null) break;
                    isChanged = true;
                    break;

                case nameof(Color):
                    E.TextColor = Color;
                    break;

                default:
                    break;
            }
        }

        protected override void Init()
        {
            E = new FEntryBase();
            E.TextChanged += OnTextChanged;
            isChanged = false;
            base.Init();
        }

        protected override object ReturnValue(int mode)
        {
            return Value;
        }

        protected override void SetInput(List<string> value, bool isCompleted = false, bool isDisable = false)
        {
            if (Disable && isDisable) return;
            Value = value[0].TrimEnd();
            isChanged = isCompleted;
            base.SetInput(value, isCompleted);
        }

        protected override void OnChangeValue(object sender, EventArgs e)
        {
            base.OnChangeValue(sender, new FInputChangeValueEventArgs((e as TextChangedEventArgs).NewTextValue));
        }

        protected override void OnCompleteValue(object sender, EventArgs e)
        {
            isChanged = false;
            base.OnCompleteValue(sender, new FInputChangeValueEventArgs(Value));
        }

        protected override View SetContentView()
        {
            E.ReturnType = ReturnType.Done;
            E.ClearButtonVisibility = ClearButtonVisibility.WhileEditing;
            E.SetBinding(FEntryBase.KeyboardProperty, KeyboardProperty.PropertyName);
            E.SetBinding(FEntryBase.TextProperty, ValueProperty.PropertyName, BindingMode.TwoWay);
            E.SetBinding(FEntryBase.PlaceholderProperty, PlaceHolderProperty.PropertyName);
            E.SetBinding(FEntryBase.MaxLengthProperty, MaxLengthProperty.PropertyName);
            E.SetBinding(FEntryBase.IsPasswordProperty, IsPasswordProperty.PropertyName);
            E.SetBinding(FEntryBase.HorizontalTextAlignmentProperty, TextAlignmentProperty.PropertyName);
            E.SetBinding(FEntryBase.IsReadOnlyProperty, IsModifierPropertyName);
            if (IsTextUpper) E.TextTransform = TextTransform.Uppercase;
            if (IsTextLower) E.TextTransform = TextTransform.Lowercase;
            E.TextChanged += OnChangeValue;
            E.Focused += OnFocused;
            E.Unfocused += OnUnfocused;
            return E;
        }

        protected override void InitPropertyByField(FField f)
        {
            IsTextUpper = f.DataFormatString == "U" || f.DataFormatString == "X";
            IsTextLower = f.DataFormatString == "u" || f.DataFormatString == "x";
            MaxLength = f.MaxLength;
            PlaceHolder = f.PlaceHolder;
            base.InitPropertyByField(f);
        }

        protected virtual async void OnFocused(object sender, FocusEventArgs e)
        {
            if (Root == null || (!Root.IsBusy && await CheckScriptFocus())) return;
            if (!FSetting.IsAndroid || Root.FormType != FFormType.Filter) E.Unfocus();
        }

        protected virtual void OnUnfocused(object sender, FocusEventArgs e)
        {
            if (isChanged) OnCompleteValue(sender, e);
        }

        protected virtual void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(this, e);
        }

        #endregion Protected

        #region Public

        public FInputTextBase() : base()
        {
            Type = FieldType.String;
        }

        public FInputTextBase(FField field) : base(field)
        {
            Type = FieldType.String;
        }

        public override void InitValue(bool isRefresh = true)
        {
            base.InitValue(isRefresh);
            if (string.IsNullOrEmpty(CacheName)) SetInput(DefaultValue ?? string.Empty);
            else
            {
                SetInput(GetCacheInput(string.Empty));
                if (isRefresh) ChangeValue += (s, e) => { SetCacheInput(Value); };
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
            E.Unfocus();
            base.UnFocusInput();
        }

        public override bool IsEqual(object oldValue)
        {
            return oldValue.Equals(ReturnValue(0));
        }

        #endregion Public
    }
}