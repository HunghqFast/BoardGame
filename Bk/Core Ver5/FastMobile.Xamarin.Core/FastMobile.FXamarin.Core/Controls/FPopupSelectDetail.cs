using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Data;
using Xamarin.Forms;
using SDataRow = System.Data.DataRow;
using SfSelectionMode = Syncfusion.SfDataGrid.XForms.SelectionMode;

namespace FastMobile.FXamarin.Core
{
    public class FPopupSelectDetail : FPopupInput, IFSelectDetail
    {
        public static readonly BindableProperty SourceProperty = BindableProperty.Create("Source", typeof(FDataObservation), typeof(FPopupSelectDetail));

        public FDataObservation Source { get => (FDataObservation)GetValue(SourceProperty); set => SetValue(SourceProperty, value); }

        public event EventHandler<(FInputGridValue Checks, FInputGridValue UnChecks)> AcceptClicked;

        public event EventHandler<EventArgs> CancelClicked;

        public readonly SfDataGrid Grid;
        public readonly FButton Check;
        public readonly FPageFilter Detail;

        public FPopupSelectDetail(FPageFilter detail, DataTable souces) : base()
        {
            Check = new FButton(FText.Select, FIcons.FormatListChecks);
            Grid = FGridStyle.Instance.InitGridView();
            Detail = detail;
            Base(souces);
        }

        public void UpdateSource(DataTable source, bool checkDisable, bool isNew)
        {
            var s = isNew ? new FDataObservation() : new FDataObservation(Source);
            source.Rows.ForEach<SDataRow>(v => { s.Add(FData.NewItem(v, Detail.Settings.Fields, checkDisable)); });
            Source = s;
        }

        public void Open()
        {
            try
            {
                var maxHeight = FSetting.ScreenHeight * 0.80;
                var height = FSetting.HeightRowGrid * (Source.Count > 15 ? 15d : (Source.Count + 3));
                if (height >= maxHeight) height = maxHeight;
                PopupView.HeightRequest = height;
                Show(false);
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
        }

        public void Close()
        {
            IsOpen = false;
        }

        private void Base(DataTable souces)
        {
            UpdateSource(souces, false, true);
            Submiter.Text = FText.Accept;
            Closer.Text = FText.Cancel;

            InitColumn();
            Grid.BindingContext = this;
            Grid.SelectionMode = SfSelectionMode.Multiple;
            Grid.VerticalOptions = Grid.HorizontalOptions = LayoutOptions.Fill;
            Grid.SetBinding(SfDataGrid.ItemsSourceProperty, SourceProperty.PropertyName);
            V.Content = Grid;

            B.HeightRequest = FSetting.HeightRowGrid;
            B.Children.Clear();
            B.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            B.Children.Add(Submiter, 0, 0);
            B.Children.Add(Check, 1, 0);
            B.Children.Add(Closer, 2, 0);
            Closer.Clicked += OnClose;
            Submiter.Clicked += OnSubmitClicked;
            Check.Clicked += OnCheckClicked;
        }

        private void OnCheckClicked(object sender, EventArgs e)
        {
            foreach (var item in Source)
            {
                if (Grid.SelectedItems.Contains(item))
                    Grid.SelectedItems.Remove(item);
                else Grid.SelectedItems.Add(item);
            }
        }

        private void OnClose(object sender, EventArgs e)
        {
            CancelClicked?.Invoke(this, e);
        }

        private void OnSubmitClicked(object sender, EventArgs e)
        {
            var check = new FInputGridValue(new DataTable(), "1");
            var unCheck = new FInputGridValue(new DataTable(), "1");
            UpdateSelectTable(check.Table, Grid.SelectedItems);
            var outSource = new List<FData>();
            Source.ForEach(x =>
            {
                if (!Grid.SelectedItems.Contains(x))
                    outSource.Add(x.Clone() as FData);
            });
            UpdateSelectTable(unCheck.Table, outSource);
            AcceptClicked?.Invoke(this, (check, unCheck));
        }

        private void InitColumn()
        {
            Grid.Columns.Clear();
            Detail.Settings.Fields.ForEach(f => { if (!f.Hidden) Grid.Columns.Add(FGridStyle.Instance.GridColumnView(Detail.Settings.Details, f)); });
        }

        private void UpdateSelectTable(DataTable dt, IEnumerable<object> datas)
        {
            dt.Clear();
            var fs = Detail.Settings?.Fields;
            if (fs == null || fs.Count == 0 || Source.Count == 0)
                return;
            FFunc.AddColumnTable(dt, fs);
            datas.ForIndex((x, i) => FFunc.AddRowTable(dt, x as FData, fs, true, i));
        }
    }
}