using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputSwitch : FInput
    {
        private readonly FSwitch S;

        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(bool), typeof(FInputSwitch), false);
        public static readonly BindableProperty ContentProperty = BindableProperty.Create("Content", typeof(string), typeof(FInputSwitch), "");

        public bool Value { get => (bool)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        public string Content { get => $"<span>{(string)GetValue(ContentProperty)}</span>"; set => SetValue(ContentProperty, value); }

        public override string Output => Value.ToString().ToLower();

        #region Public

        public FInputSwitch() : base()
        {
            Type = FieldType.Bool;
            S = new FSwitch();
        }

        public FInputSwitch(FField field) : base(field)
        {
            Type = FieldType.Bool;
            S = new FSwitch();
        }

        public override void InitValue(bool isRefresh = true)
        {
            base.InitValue(isRefresh);
            if (DefaultValue != null) Value = FFunc.StringToBoolean(DefaultValue.ToString());
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

        #endregion

        #region Protected

        protected override void OnChangeValue(object sender, EventArgs e)
        {
            base.OnChangeValue(sender, new FInputChangeValueEventArgs((e as ToggledEventArgs).Value));
        }

        protected override void OnCompleteValue(object sender, EventArgs e)
        {
            base.OnCompleteValue(sender, new FInputChangeValueEventArgs(Value));
        }

        protected override object ReturnValue(int mode)
        {
            return Value;
        }

        protected override void SetInput(List<string> value, bool isCompleted = false, bool isDisable = false)
        {
            if (Disable && isDisable) return;
            Value = FFunc.StringToBoolean(value[0]);
            base.SetInput(value, isCompleted);
            S.Toggled -= OnCompleteValue;
            S.Toggled += OnCompleteValue;
        }

        protected override View SetContentView()
        {
            var t = new Label();
            var g = new Grid();

            t.FontSize = FSetting.FontSizeLabelContent;
            t.FontFamily = FSetting.FontText;
            t.HorizontalOptions = LayoutOptions.StartAndExpand;
            t.VerticalOptions = LayoutOptions.CenterAndExpand;
            t.VerticalTextAlignment = TextAlignment.Center;
            t.TextType = TextType.Html;
            t.BackgroundColor = Color.Transparent;
            t.SetBinding(Label.TextProperty, ContentProperty.PropertyName);
            t.SetBinding(Label.TextColorProperty, ColorProperty.PropertyName);


            S.VerticalOptions = LayoutOptions.CenterAndExpand;
            S.HorizontalOptions = LayoutOptions.EndAndExpand;
            S.BackgroundColor = Color.Transparent;
            S.SetBinding(FSwitch.IsToggledProperty, ValueProperty.PropertyName);
            S.SetBinding(FSwitch.IsEnabledProperty, IsModifierPropertyName);
            S.Toggled += OnChangeValue;

            g.HorizontalOptions = LayoutOptions.FillAndExpand;
            g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(FSetting.FilterInputHeight) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(FSetting.FilterInputHeight + 30) });
            g.Children.Add(t, 0, 0);
            g.Children.Add(S, 1, 0);
            return g;
        }

        protected override void InitPropertyByField(FField f)
        {
            Content = f.Title;
            IsShowTitle = false;
            base.InitPropertyByField(f);
        }

        #endregion
    }
}