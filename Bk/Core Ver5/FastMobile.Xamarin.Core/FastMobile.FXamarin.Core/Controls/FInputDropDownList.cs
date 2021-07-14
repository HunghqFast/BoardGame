using Syncfusion.DataSource.Extensions;
using Syncfusion.XForms.ComboBox;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputDropDownList : FInput
    {
        private bool isSetAuto;

        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(FItem), typeof(FInputDropDownList), null);
        public static readonly BindableProperty DataSourceProperty = BindableProperty.Create("DataSource", typeof(List<FItem>), typeof(FInputDropDownList), new List<FItem>());
        public static readonly BindableProperty ItemVisibleProperty = BindableProperty.Create("ItemVisible", typeof(List<FItem>), typeof(FInputDropDownList), new List<FItem>());

        public FItem Value { get => (FItem)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        public List<FItem> DataSource { get => (List<FItem>)GetValue(DataSourceProperty); set => SetValue(DataSourceProperty, value); }

        public List<FItem> ItemVisible { get => (List<FItem>)GetValue(ItemVisibleProperty); set => SetValue(ItemVisibleProperty, value); }

        public override string Output => $"'{Value.I}'";

        #region Public

        public FInputDropDownList(List<FItem> items) : base()
        {
            Type = FieldType.String;
            DataSource = items;
        }

        public FInputDropDownList(FField field) : base(field)
        {
            Type = FieldType.String;
            DataSource = field.Item;
        }

        public override void InitValue(bool isRefresh = true)
        {
            base.InitValue(isRefresh);
            if (DefaultValue == null) return;
            var value = FFunc.GetArrayString(DefaultValue.ToString(), Seperate);
            SetItemVisible(value);
            SetSelectedItem(value[0]);
        }

        public override void Clear(bool isCompleted = false)
        {
            base.Clear(isCompleted);
            InitValue(false);
        }

        public override bool IsEqual(object oldValue)
        {
            return oldValue == ReturnValue(0);
        }

        #endregion Public

        #region Protected

        protected override object ReturnValue(int mode)
        {
            return Value != null ? Value.I : string.Empty;
        }

        protected override void SetInput(List<string> value, bool isCompleted = false, bool isDisable = false)
        {
            if (Disable && isDisable) return;
            SetItemVisible(value);
            SetSelectedItem(value[0]);
            base.SetInput(value, isCompleted);
        }

        protected override void OnChangeValue(object sender, EventArgs e)
        {
            base.OnChangeValue(sender, new FInputChangeValueEventArgs(Value));
        }

        protected override void OnCompleteValue(object sender, EventArgs e)
        {
            base.OnCompleteValue(sender, new FInputChangeValueEventArgs(Value));
            isSetAuto = false;
        }

        protected override View SetContentView()
        {
            var c = new FSfComboBox();
            c.ShowBorder = false;
            c.DropDownTextSize = FSetting.FontSizeLabelContent;
            c.TextSize = FSetting.FontSizeLabelContent;
            c.TextColor = FSetting.TextColorContent;
            c.FontFamily = FSetting.FontText;
            c.HeightRequest = FSetting.FilterInputHeight;
            c.DropDownCornerRadius = 2;
            c.DisplayMemberPath = FItem.ItemValue;
            c.DropDownItemHeight = 40;
            c.MaximumDropDownHeight = 200;
            c.SetBinding(SfComboBox.DataSourceProperty, ItemVisibleProperty.PropertyName);
            c.SetBinding(SfComboBox.TextProperty, FItem.ItemValue);
            c.SetBinding(SfComboBox.SelectedItemProperty, ValueProperty.PropertyName, BindingMode.TwoWay);
            c.DropDownButtonSettings.SetBinding(DropDownButtonSettings.FontColorProperty, IsModifierPropertyName, converter: new FBoolToObject(FSetting.TextColorContent, Color.Transparent));
            c.ValueChanged += OnChangeValue;
            c.SelectionChanged += OnSelectedValue;
            c.DropDownOpen += OnOpenedCombobox;
            c.SetBinding(SfComboBox.IsEnabledProperty, IsModifierPropertyName);
            return c;
        }

        #endregion Protected

        #region Private

        private void SetItemVisible(List<string> value)
        {
            if (value.Count <= 1 || value[1].Equals(string.Empty)) ItemVisible = DataSource;
            else
            {
                List<string> list = FFunc.GetArrayString(value[1]);
                ItemVisible = DataSource.Where(s => list.Contains(s.I)).ToList();
            }
        }

        private void SetSelectedItem(object selected)
        {
            isSetAuto = false;
            Value = ItemVisible.Find(i => i.I == selected.ToString().Trim());
        }

        private void OnSelectedValue(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            if (isSetAuto) OnCompleteValue(this, EventArgs.Empty);
            OnChangeValue(this, EventArgs.Empty);
        }

        private void OnOpenedCombobox(object sender, EventArgs e)
        {
            isSetAuto = true;
        }

        #endregion Private
    }
}