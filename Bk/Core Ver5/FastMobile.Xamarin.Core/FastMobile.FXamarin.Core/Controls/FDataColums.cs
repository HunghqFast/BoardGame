using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    internal class FHeaderColumn : Label
    {
        public FHeaderColumn()
        {
            TextColor = FSetting.DisableColor;
            FontFamily = FSetting.FontText;
            FontSize = FSetting.FontSizeLabelContent - 1;
            HorizontalTextAlignment = TextAlignment.Center;
            VerticalOptions = LayoutOptions.Center;
            LineBreakMode = LineBreakMode.TailTruncation;
        }
    }

    public class FNumericColumn : GridNumericColumn
    {
        public FNumericColumn(string text)
        {
            RecordFont = FSetting.FontText;
            CellTextSize = FSetting.FontSizeLabelContent;
            TextAlignment = TextAlignment.End;
            LineBreakMode = LineBreakMode.TailTruncation;
            LoadUIView = true;
            ColumnSizer = ColumnSizer.None;
            Padding = new Thickness(8, 0);
            MinimumWidth = 40;
            HeaderTemplate = new DataTemplate(() =>
            {
                return new FHeaderColumn { Text = text };
            });
        }
    }

    public class FTextColumn : GridTextColumn
    {
        public FTextColumn(string text)
        {
            RecordFont = FSetting.FontText;
            CellTextSize = FSetting.FontSizeLabelContent;
            TextAlignment = TextAlignment.Start;
            LineBreakMode = LineBreakMode.TailTruncation;
            LoadUIView = true;
            ColumnSizer = ColumnSizer.None;
            Padding = new Thickness(8, 0);
            MinimumWidth = 40;
            HeaderTemplate = new DataTemplate(() =>
            {
                return new FHeaderColumn { Text = text };
            });
        }
    }

    public class FDateColumn : GridDateTimeColumn
    {
        public FDateColumn(string text)
        {
            RecordFont = FSetting.FontText;
            CellTextSize = FSetting.FontSizeLabelContent;
            TextAlignment = TextAlignment.Center;
            LineBreakMode = LineBreakMode.TailTruncation;
            LoadUIView = true;
            ColumnSizer = ColumnSizer.None;
            Padding = new Thickness(8, 0);
            MinimumWidth = 40;
            HeaderTemplate = new DataTemplate(() =>
            {
                return new FHeaderColumn { Text = text };
            });
        }
    }

    public class FCheckBoxColumn : GridTemplateColumn
    {
        public FCheckBoxColumn(string text, bool editable = true)
        {
            LoadUIView = true;
            ColumnSizer = ColumnSizer.None;
            HeaderTemplate = new DataTemplate(() =>
            {
                return new FHeaderColumn { Text = text };
            });
            CellTemplate = new DataTemplate(() =>
            {
                var cell = new StackLayout();
                var check = new FCheckBox(true);

                check.SetBinding(FCheckBox.IsCheckedProperty, MappingName);
                check.IsEnabled = editable;

                cell.Padding = new Thickness(8, 0);
                cell.VerticalOptions = LayoutOptions.CenterAndExpand;
                cell.HorizontalOptions = LayoutOptions.CenterAndExpand;
                cell.Children.Add(check);
                return cell;
            });
        }
    }

    public class FCustomColumn : GridTemplateColumn
    {
        public FCustomColumn()
        {
            LoadUIView = true;
            ColumnSizer = ColumnSizer.None;
            HeaderTemplate = new DataTemplate(() =>
            {
                return new FHeaderColumn { Text = "" };
            });
        }
    }

    public class FGridSettings : DataGridStyle
    {
        public override Color GetBorderColor()
        {
            return FSetting.LineBoxReportColor;
        }

        public override float GetBorderWidth()
        {
            return 1;
        }

        public override Color GetAlternatingRowBackgroundColor()
        {
            return FSetting.BackgroundAlternatingRow;
        }

        public override Color GetDataGridBackgroundColor()
        {
            return FSetting.BackgroundMain;
        }

        public override Color GetHeaderBackgroundColor()
        {
            return FSetting.BackgroundMain;
        }

        public override Color GetHeaderBorderColor()
        {
            return FSetting.LineBoxReportColor;
        }

        public override Color GetSelectionBackgroundColor()
        {
            return Color.FromHex("#d7edf4");
        }

        public override Color GetSelectionForegroundColor()
        {
            return FSetting.TextColorContent;
        }

        public override float GetHeaderBorderWidth()
        {
            return 1;
        }

        public override GridLinesVisibility GetGridLinesVisibility()
        {
            return GridLinesVisibility.Both;
        }

        public override Color GetFrozenIndicatorColor()
        {
            return FSetting.LineBoxReportColor;
        }
    }
}