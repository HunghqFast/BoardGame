using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputTable : FInput
    {
        private bool isLock;

        public static readonly BindableProperty SourceProperty = BindableProperty.Create("Source", typeof(FDataObservation), typeof(FInputTable));
        public static readonly BindableProperty HeightGridViewProperty = BindableProperty.Create("HeightGridView", typeof(double), typeof(FInputTable), FSetting.HeightRowGrid * 2d);
        public static readonly BindableProperty MaxRecordProperty = BindableProperty.Create("MaxRecord", typeof(int), typeof(FInputTable), 5);
        public static readonly BindableProperty ClearModeProperty = BindableProperty.Create("ClearMode", typeof(bool), typeof(FInputTable));

        public int MaxRecord { get => (int)GetValue(MaxRecordProperty); set => SetValue(MaxRecordProperty, value); }

        public bool ClearMode { get => (bool)GetValue(ClearModeProperty); set => SetValue(ClearModeProperty, value); }

        public FDataObservation Source { get => (FDataObservation)GetValue(SourceProperty); set => SetValue(SourceProperty, value); }

        public double HeightGridView { get => (double)GetValue(HeightGridViewProperty); set => SetValue(HeightGridViewProperty, value); }

        public List<string> InputKey { get; set; }

        public string IsEdited { get; protected set; }

        public SfDataGrid Grid { get; set; }

        public FGridStyle GridStyle { get; set; }

        public virtual FViewPage Settings { get; }

        public virtual string Controller { get; }

        public virtual string Target { get; }

        public virtual bool IncreaseLnr => false;

        public virtual FInputGridValue Value
        {
            get
            {
                return new FInputGridValue(new DataTable(), IsEdited);
            }
        }

        public override string Output => IsEdited;

        public FInputTable() : base()
        {
            Source = new FDataObservation(IncreaseLnr);
        }

        public FInputTable(FField field) : base(field)
        {
            Source = new FDataObservation(IncreaseLnr);
        }

        #region Public

        public virtual void InitColumn()
        {
            Grid.Columns.Clear();
            Settings.Fields.ForEach(f => { if (!f.Hidden) Grid.Columns.Add(GridStyle.GridColumnView(Settings.Details, f)); });
        }

        public virtual async Task InitData()
        {
            Clear();
            if (Root.InputData == null) return;
            var i = Newtonsoft.Json.JsonConvert.DeserializeObject<FInputGridValue>(Root.InputData[Name].ToString());
            if (i?.Table?.Rows != null)
            {
                await UpdateSource(i.Table.Select(), true, true);
                IsEdited = "1";
                return;
            }

            if (Root.FormType == FFormType.New)
            {
                UpdateDetail(Root.InputData);
                return;
            }

            var m = await Loading();
            if (m.Success == 1)
            {
                var d = m.ToDataSet();
                var v = d.Tables[Settings.TableData]?.Select();
                if (v == null) return;

                if (Settings.TableDetails > -1 && Settings.TableDetails < d.Tables.Count) UpdateDetail(d.Tables[Settings.TableDetails]);
                await UpdateSource(v, false, true);
            }
            else MessagingCenter.Send(new FMessage(0, m.Code, m.Message), FChannel.ALERT_BY_MESSAGE);
        }

        public virtual async void InsertDetail(DataTable table, bool checkDisable, bool isNew)
        {
            await UpdateSource(table.Select(), checkDisable, isNew);
            IsEdited = "1";
        }

        public virtual async Task ClearDetail()
        {
            await Root.SetBusy(true);
            if (Source.Count > 0) IsEdited = "1";
            Source.Clear();
            await UpdateView();
            await Root.SetBusy(false);
        }

        public override void Clear(bool isCompleted = false)
        {
            base.Clear(isCompleted);
            Source.Clear();
            _ = UpdateView();
        }

        #endregion Public

        #region Protected

        protected async Task Lock(object sender, Func<Task> task)
        {
            lock (sender) { if (isLock) return; isLock = true; }
            await task.Invoke();
            isLock = false;
        }

        protected async Task ChangedView()
        {
            IsEdited = "1";
            OnChangeValue(this, EventArgs.Empty);
            OnCompleteValue(this, EventArgs.Empty);
            await UpdateView();
        }

        protected async void InitGridView()
        {
            Clear();
            Grid.FrozenColumnsCount = 1;
            InitColumn();
            Grid.GridTapped += GridTapped;
            Grid.SetBinding(SfDataGrid.ItemsSourceProperty, SourceProperty.PropertyName);
            await Task.CompletedTask;
        }

        protected async Task UpdateSource(System.Data.DataRow[] row, bool checkDisable, bool isNew)
        {
            var s = isNew ? new FDataObservation(IncreaseLnr) : new FDataObservation(Source, IncreaseLnr);
            row.ForEach(v => { s.Add(FData.NewItem(v, Settings.Fields, checkDisable)); });
            Source = s;
            await UpdateView();
        }

        protected async Task AddNewItem(FData data)
        {
            Source.Add(data);
            await ChangedView();
        }

        protected async Task DeleteItem()
        {
            Source.RemoveAt(Grid.SelectedIndex - 1);
            await ChangedView();
        }

        protected async Task UpdateItem(FData data)
        {
            Source[Grid.SelectedIndex - 1] = data;
            Grid.Refresh();
            IsEdited = "1";
            OnChangeValue(this, EventArgs.Empty);
            OnCompleteValue(this, EventArgs.Empty);
            await Task.CompletedTask;
        }

        protected async void OnCanceled(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        protected async Task OnClearItem(object obj)
        {
            if (!IsModifier || Source.Count == 0) return;
            await ClearDetail();
        }

        protected override object ReturnValue(int mode)
        {
            return Value;
        }

        protected override void Init()
        {
            base.Init();
            GridStyle = new FGridStyle();
            Grid = GridStyle.InitGridView();
        }

        protected override void SetInput(List<string> value, bool isCompleted = false, bool isDisable = false)
        {
            if (Disable && isDisable) return;
            base.SetInput(value);
        }

        protected override void OnChangeValue(object sender, EventArgs e)
        {
            base.OnChangeValue(sender, new FInputChangeValueEventArgs(Value));
        }

        protected override void OnCompleteValue(object sender, EventArgs e)
        {
            base.OnCompleteValue(sender, new FInputChangeValueEventArgs(Value));
        }

        protected override void InitPropertyByField(FField f)
        {
            IsShowTitle = false;
            InputKey = f.ItemInput;
            ClearMode = f.ItemClearMode;
            MaxRecord = f.MaxLength == 256 ? 5 : f.MaxLength;
            base.InitPropertyByField(f);
        }

        protected virtual async Task UpdateView()
        {
            HeightGridView = (Source.Count <= MaxRecord ? Source.Count + 2 : MaxRecord + 2) * FSetting.HeightRowGrid;
            HeightInput = HeightGridView + FSetting.HeightRowGrid;
            await Task.CompletedTask;
        }

        protected virtual async void GridTapped(object sender, GridTappedEventArgs e)
        {
            if (e.RowColumnIndex.RowIndex <= 0) return;
            Grid.SelectedIndex = e.RowColumnIndex.RowIndex;
            await Task.CompletedTask;
        }

        protected virtual void UpdateDetail(DataTable table)
        {
        }

        protected virtual void UpdateDetail(FData data)
        {
        }

        protected virtual async Task<FMessage> Loading()
        {
            var ds = new DataSet();
            FFunc.AddTypeAction(ref ds, "command", Target, controllerParent: Root.Controller);
            FFunc.AddFieldInput(ref ds, new DataTable(), Settings.Code, Root.InputData, true, Settings.Reference);
            FMessage result = await FServices.ExecuteCommand("Loading", Controller, ds, "300", null, true);
            ds.Dispose();
            return result;
        }

        #endregion Protected
    }
}