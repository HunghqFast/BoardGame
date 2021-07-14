using System;
using System.Data;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FItemMenu : BindableObject
    {
        public static readonly BindableProperty IDProperty = BindableProperty.Create("ID", typeof(int), typeof(FItemMenu), default(int));
        public static readonly BindableProperty SortTappedProperty = BindableProperty.Create("SortTapped", typeof(string), typeof(FItemMenu));
        public static readonly BindableProperty SortImportanceProperty = BindableProperty.Create("SortImportance", typeof(string), typeof(FItemMenu));
        public static readonly BindableProperty WMenuIdProperty = BindableProperty.Create("WMenuId", typeof(string), typeof(FItemMenu));
        public static readonly BindableProperty WMenuId0Property = BindableProperty.Create("WMenuId0", typeof(string), typeof(FItemMenu));
        public static readonly BindableProperty BarProperty = BindableProperty.Create("Bar", typeof(string), typeof(FItemMenu));
        public static readonly BindableProperty ControllerProperty = BindableProperty.Create("Controller", typeof(string), typeof(FItemMenu));
        public static readonly BindableProperty ActionProperty = BindableProperty.Create("Action", typeof(string), typeof(FItemMenu));
        public static readonly BindableProperty BadgeTextProperty = BindableProperty.Create("BadgeText", typeof(string), typeof(FItemMenu));
        public static readonly BindableProperty XTypeProperty = BindableProperty.Create("XType", typeof(string), typeof(FItemMenu));
        public static readonly BindableProperty GroupNameProperty = BindableProperty.Create("GroupName", typeof(string), typeof(FItemMenu));
        public static readonly BindableProperty IconUrlProperty = BindableProperty.Create("IconUrl", typeof(ImageSource), typeof(FItemMenu));

        //Append
        public static readonly BindableProperty BadgeColorProperty = BindableProperty.Create("BadgeColor", typeof(Color), typeof(FItemMenu), FSetting.DangerColor);

        public int ID
        {
            get => (int)GetValue(IDProperty);
            set => SetValue(IDProperty, value);
        }

        public string SortTapped
        {
            get => (string)GetValue(SortTappedProperty);
            set => SetValue(SortTappedProperty, value);
        }

        public string SortImportance
        {
            get => (string)GetValue(SortImportanceProperty);
            set => SetValue(SortImportanceProperty, value);
        }

        public string WMenuId
        {
            get => (string)GetValue(WMenuIdProperty);
            set => SetValue(WMenuIdProperty, value);
        }

        public string WMenuId0
        {
            get => (string)GetValue(WMenuId0Property);
            set => SetValue(WMenuId0Property, value);
        }

        public string Bar
        {
            get => (string)GetValue(BarProperty);
            set => SetValue(BarProperty, value);
        }

        public string Controller
        {
            get => (string)GetValue(ControllerProperty);
            set => SetValue(ControllerProperty, value);
        }

        public string Action
        {
            get => (string)GetValue(ActionProperty);
            set => SetValue(ActionProperty, value);
        }

        public string BadgeText
        {
            get => (string)GetValue(BadgeTextProperty);
            set => SetValue(BadgeTextProperty, value);
        }

        public string XType
        {
            get => (string)GetValue(XTypeProperty);
            set => SetValue(XTypeProperty, value);
        }

        public string GroupName
        {
            get => (string)GetValue(GroupNameProperty);
            set => SetValue(GroupNameProperty, value);
        }

        public ImageSource IconUrl
        {
            get => (ImageSource)GetValue(IconUrlProperty);
            set => SetValue(IconUrlProperty, value);
        }

        public Color BadgeColor
        {
            get => (Color)GetValue(BadgeColorProperty);
            set => SetValue(BadgeColorProperty, value);
        }

        public FItemMenu(string wmenuID, string wmenuID0, string bar, string controller, string action, string xtype, string groupname, ImageSource source, string badge, Color badgeColor, string sortedTapped, string sortImportance)
        {
            ID = new Random().Next();
            SortTapped = sortedTapped;
            SortImportance = sortImportance;
            WMenuId = wmenuID;
            WMenuId0 = wmenuID;
            Bar = bar;
            Controller = controller;
            Action = action;
            BadgeText = badge;
            XType = xtype;
            GroupName = groupname;
            IconUrl = source;
            BadgeColor = badgeColor;
        }

        public void SetBadgeText(string badge)
        {
            BadgeText = badge;
        }

        public async void RefreshBadge()
        {
            var message = await FServices.ExecuteCommand("GetBadge", "System", new DataSet().AddTable(new DataTable().AddRowValue("controller", Controller)), "0", null);
            if (!message.OK)
                return;
            var ds = message.ToDataSet();
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0 || !ds.Tables[0].Columns.Contains("badge"))
                return;
            var badge = ds.Tables[0].Rows[0]["badge"].ToString();
            Device.BeginInvokeOnMainThread(() => SetBadgeText(badge));
        }
    }
}