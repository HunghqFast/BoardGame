using Syncfusion.DataSource;
using Syncfusion.ListView.XForms;
using System;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FGridMenu : FListView
    {
        public FGridMenu(FPageMenu root) : base()
        {
            SelectionBackgroundColor = Color.Transparent;
            DataSource.GroupDescriptors.Add(new GroupDescriptor(FItemMenu.GroupNameProperty.PropertyName)
            {
                KeySelector = (object obj1) => ((FItemMenu)obj1).GroupName
            });
            ItemSpacing = 0;
            AutoFitMode = AutoFitMode.DynamicHeight;
            DataSource.LiveDataUpdateMode = LiveDataUpdateMode.AllowDataShaping;
            GroupHeaderTemplate = new DataTemplate(() => new FTLGroupHeaderMenu(root, this));
        }

        public void InitMenu(FMenuViewType type, Action<object, IFDataEvent> invoke = null)
        {
            ItemTemplate = new DataTemplate(() => new FTLItemMenu(type, this, invoke));
            LayoutManager = new GridLayout { SpanCount = type == FMenuViewType.Grid ? 3 : 1 };
        }

        public void InitSorting(string sortKey, ListSortDirection sortDirection)
        {
            DataSource.SortDescriptors.Clear();
            DataSource.SortDescriptors.Add(new SortDescriptor()
            {
                PropertyName = FItemMenu.SortImportanceProperty.PropertyName,
                Direction = sortDirection
            });
            DataSource.SortDescriptors.Add(new SortDescriptor()
            {
                PropertyName = sortKey,
                Direction = sortDirection
            });
            DataSource.SortDescriptors.Add(new SortDescriptor()
            {
                PropertyName = FItemMenu.WMenuIdProperty.PropertyName,
                Direction = sortDirection
            });
        }
    }
}