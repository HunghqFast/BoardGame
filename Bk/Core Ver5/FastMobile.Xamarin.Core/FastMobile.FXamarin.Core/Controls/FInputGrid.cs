using Newtonsoft.Json.Linq;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputGrid : FInputTable
    {
        public override bool HasPadding => false;

        public override FViewPage Settings => Detail.Settings;

        public override string Controller => Detail.Controller;

        public override string Target => "Grid";

        public override FInputGridValue Value
        {
            get
            {
                var table = new DataTable();
                var field = Settings.Fields;
                if (field == null || field.Count == 0) return new FInputGridValue(table, IsEdited);
                FFunc.AddColumnTable(table, field);
                Source.ForEach(s => FFunc.AddRowTable(table, s, field, true, Source.IndexOf(s)));
                table.TableName = Name;
                return new FInputGridValue(table, IsEdited);
            }
        }

        public FPageFilter Detail;
        public IFSelectDetail SelectionView { get; set; }

        public FInputGrid(string controller, JObject setting) : base()
        {
            Type = FieldType.Table;
            Detail = new FPageFilter(setting)
            {
                Controller = controller,
                Target = FFormTarget.Grid,
                EditCommand = EditCommand,
                NewCommand = NewCommand,
                IsMaster = false,
                Root = this
            };
            Detail.CancelClick += OnCanceled;
            Detail.CloseClick += OnCanceled;
            Detail.OkClick += OnSuccessed;
            Detail.SaveClick += OnSuccessed;
            Detail.BackButtonClicked += OnCanceled;
            Detail.NewClick += (s, e) => { Detail.ClearAll(); };
            IsEdited = "0";
            InitGridView();
        }

        public FInputGrid(FField field) : base(field)
        {
            Type = FieldType.Table;
            Detail = new FPageFilter(field.ItemTemplate)
            {
                Controller = field.ItemController,
                Target = FFormTarget.Grid,
                EditCommand = EditCommand,
                NewCommand = NewCommand,
                IsMaster = false,
                Root = this
            };
            Detail.CancelClick += OnCanceled;
            Detail.CloseClick += OnCanceled;
            Detail.OkClick += OnSuccessed;
            Detail.SaveClick += OnSuccessed;
            Detail.BackButtonClicked += OnCanceled;
            Detail.NewClick += (s, e) => { Detail.ClearAll(); };
            IsEdited = "0";
            InitGridView();
        }

        #region Public

        public void SetDetailColumn(string column, List<string> fields, string expression, string condition)
        {
            Source.Select(s =>
            {
                string e = expression, c = condition;
                var b = Settings.Fields.Find(x => x.Name == column);
                foreach (var n in fields)
                {
                    var v = $"{(IsRootParam(n) ? Root.Input.TryGetValue(RootParam(n), out FInput i) ? i.GetInput(0) : n : s[n])}";
                    e = e.Replace($"[{n}]", v);
                    c = c.Replace($"[{n}]", v);
                }
                if (b != null && (c.Equals("") || FFunc.StringToBoolean(FFunc.Compute(Root, c).ToString()))) s[column, b.FieldType] = FFunc.Compute(Root, e);
                return s;
            }).ToList();
        }

        public decimal Sum(string expression, string condition)
        {
            if (Source.Count == 0) return 0;
            var r = new Regex("\\[(.+?)\\]");
            return Source.Sum(s =>
            {
                string e = expression, c = condition;
                r.Matches(expression).ForEach(m => e = e.Replace($"[{m.Groups[1].Value}]", s[m.Groups[1].Value].ToString()));
                if (!string.IsNullOrWhiteSpace(c)) r.Matches(c).ForEach(m => c = c.Replace($"[{m.Groups[1].Value}]", s[m.Groups[1].Value].ToString()));
                return string.IsNullOrWhiteSpace(c) || FFunc.StringToBoolean(FFunc.Compute(null, c).ToString()) ? decimal.Parse(FFunc.Compute(null, e).ToString(), NumberStyles.Float) : 0;
            });
        }

        public override void InitColumn()
        {
            base.InitColumn();
            var m = new List<FToolbar> { new FToolbar { Command = "Delete" }, new FToolbar { Command = "Edit" } };
            var a = new List<Func<object, FData, Task>> { OnDeleteItem, OnEditToolbar };
            Grid.Columns.Insert(0, new FCustomColumn
            {
                MappingName = FData.GetBindingName(FData.CheckStatusName),
                CellTemplate = GridStyle.TemplateGridSwipe(m.ToArray(), a.ToArray(), false),
                Width = m.Count * FSetting.HeightRowGrid,
                MinimumWidth = m.Count * FSetting.HeightRowGrid,
                MaximumWidth = m.Count * FSetting.HeightRowGrid
            });
        }

        public override bool IsEqual(object oldValue)
        {
            try
            {
                var oldTable = (oldValue as FInputGridValue).Table;
                var newTable = Value.Table;
                if (oldTable.Rows.Count != newTable.Rows.Count) return false;
                for (int i = 0; i < oldTable.Rows.Count; i++)
                    for (int c = 0; c < oldTable.Columns.Count; c++)
                    {
                        var cname = oldTable.Columns[c].ColumnName;
                        if (cname != FData.OrdinalNumberName && !Detail.Input[cname].IsVisible) continue;
                        var a = oldTable.Rows[i][c];
                        var b = newTable.Rows[i][c];
                        if (!Equals(oldTable.Rows[i][c], newTable.Rows[i][c])) return false;
                    }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ShowSelectionPage(DataTable table, Action<FInputGridValue, FInputGridValue> acceptClicked, Action cancelClicked, bool alwaysShow, string title = null)
        {
            try
            {
                if (!alwaysShow && (table == null || table.Rows.Count == 0))
                    return;

                SelectionView = new FPopupSelectDetail(Detail, table) { Title = title ?? FText.SelectRecord };
                SelectionView.AcceptClicked += SelectionClicked;
                SelectionView.CancelClicked += OnClose;
                SelectionView.Open();
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
            }

            void SelectionClicked(object sender, (FInputGridValue Checks, FInputGridValue UnChecks) e)
            {
                try
                {
                    SelectionView.Close();
                    acceptClicked?.Invoke(e.Checks, e.UnChecks);
                    IsEdited = "1";
                }
                catch { }
            }

            void OnClose(object sender, EventArgs e)
            {
                try
                {
                    SelectionView.Close();
                    cancelClicked?.Invoke();
                }
                catch { }
            }
        }

        #endregion

        #region Protected

        protected virtual async Task<bool> NewCommand(FData data)
        {
            return await Task.FromResult(true);
        }

        protected virtual async Task<bool> EditCommand(FData data)
        {
            return await Task.FromResult(true);
        }

        protected virtual async Task OnNewItem(object obj)
        {
            if (await InitDirForm(FFormType.New, null) is Page detail) await Navigation.PushAsync(detail, true);
        }

        protected virtual async Task OnEditItem(object obj, FData data)
        {
            Grid.SelectedIndex = data.GetIndex();
            if (await InitDirForm(FFormType.Edit, Source[Grid.SelectedIndex - 1]) is Page detail) await Navigation.PushAsync(detail, true);
        }

        protected virtual async Task OnSaveItem(FFormType type, FData data)
        {
            if (type == FFormType.Edit) await UpdateItem(data);
            else await AddNewItem(data);
            await Navigation.PopAsync();
        }

        protected virtual async Task OnDeleteItem(object obj, FData data)
        {
            Grid.SelectedIndex = data.GetIndex();
            await Root.SetBusy(true);
            if (IsModifier && (Settings.ClearMode || await FAlertHelper.Confirm("805")))
            {
                await Task.Delay(100);
                await DeleteItem();
            }
            await Root.SetBusy(false);
        }

        protected override async Task UpdateView()
        {
            await base.UpdateView();
            if (Root == null) return;
            InputKey?.ForEach(x => { if (Root.Input.TryGetValue(x, out FInput input)) input.IsReadOnly = Source.Count == 0; });
        }

        protected override View SetContentView()
        {
            var c = new Grid();
            var g = new RowDefinition();
            var a = new FMenuButtonReport { Toolbar = new FToolbar { Command = "New", Title = FText.NewRecord }, Action = OnNewToolbar, Visible = true, BindingContext = this };
            var d = new FMenuButtonReport { Toolbar = new FToolbar { Command = "ClearAll", Title = FText.ClearAll }, Action = OnClearItem, BindingContext = this };
            var m = new ObservableCollection<FMenuButtonReport> { a, d };

            Detail.Settings.Toolbars.ForEach(x =>
            {
                var r = new FMenuButtonReport { Toolbar = x, Action = OnCustomCommand, BindingContext = this, Enable = true, Visible = true };
                m.Add(r);
            });

            a.Enable = d.Enable = true;
            d.SetBinding(FMenuButtonReport.VisibleProperty, ClearModeProperty.PropertyName);
            g.Height = new GridLength(FSetting.HeightRowGrid * 4);
            g.SetBinding(RowDefinition.HeightProperty, HeightGridViewProperty.PropertyName);

            c.RowSpacing = 0;
            c.VerticalOptions = LayoutOptions.StartAndExpand;
            c.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            c.RowDefinitions.Add(g);
            c.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            c.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            c.Children.Add(Grid, 0, 0);
            c.Children.Add(new FLine(), 0, 1);
            c.Children.Add(new FReportStyle().InitToolbarView(m, FSetting.HeightRowGrid, -1, FSetting.SizeIconMenu, true, true, false), 0, 2);
            return c;
        }

        protected virtual async Task OnViewItem(object sender, GridTappedEventArgs e)
        {
            Grid.SelectedIndex = e.RowColumnIndex.RowIndex;
            if (await InitDirForm(IsModifier ? FFormType.View : FFormType.ViewDetail, e.RowData as FData) is Page detail)
                await Navigation.PushAsync(detail, true);
        }

        protected override async void GridTapped(object sender, GridTappedEventArgs e)
        {
            if (e.RowColumnIndex.RowIndex <= 0) return;
            await Lock(this, async () =>
            {
                await Root.SetBusy(true);
                await OnViewItem(sender, e);
                await Root.SetBusy(false);
            });
        }

        protected async Task<FPageFilter> InitDirForm(FFormType type, FData input)
        {
            Detail.Content = null;
            Detail.InputView.Content = null;
            Detail.FormType = type;
            Detail.InputData = input;
            CheckFieldScript(input);
            Detail.Input.Clear();
            await Detail.SetBusy(true);
            Detail.Title = $"{Detail.Action} {Settings.Title}";
            await Detail.InitBySetting();
            await Detail.SetBusy(false);
            return Detail;
        }

        #endregion

        #region private

        private async Task OnNewToolbar(object obj)
        {
            if (!IsModifier) return;
            await Lock(this, async () =>
            {
                await Root.SetBusy(true);
                _ = OnNewItem(obj);
                await Root.SetBusy(false);
                await Task.CompletedTask;
            });
        }

        private async Task OnEditToolbar(object obj, FData data)
        {
            if (!IsModifier) return;
            await Lock(this, async () =>
            {
                await Root.SetBusy(true);
                _ = OnEditItem(obj, data);
                await Root.SetBusy(false);
                await Task.CompletedTask;
            });
        }

        private async Task OnCustomCommand(object obj)
        {
            if (!IsModifier) return;
            var toolbar = obj as FToolbar;

            toolbar.CommandArgument = toolbar.MenuItem.Count > 0 ? await new FAlertOptions().ShowOptions(toolbar.Title, FText.Accept, FText.Cancel, toolbar.GetMenuItem, FItem.ItemID, FItem.ItemValue) : null;
            if (toolbar.CommandArgument == "") return;
            if (!string.IsNullOrEmpty(toolbar.CommandSuccessScript) && !string.IsNullOrEmpty(toolbar.CommandArgument))
            {
                toolbar.CommandSuccess = toolbar.CommandSuccessScript.Replace("[commandArgument]", toolbar.CommandArgument);
                toolbar.CommandSuccess = (string)FFunc.Compute(Root, toolbar.CommandSuccessScript.Replace("[commandArgument]", toolbar.CommandArgument));
            }
            else toolbar.CommandSuccess = null;
            toolbar.ShowForm = toolbar.ShowForm?.Replace("[commandSuccess]", toolbar.CommandSuccess);
            if (!string.IsNullOrEmpty(toolbar.ShowForm))
            {
                await ShowForm(toolbar);
                return;
            }
        }

        private async void OnSuccessed(object sender, EventArgs e)
        {
            await Lock(sender, async () =>
            {
                await Root.SetBusy(true);
                await OnSaveItem(Detail.FormType, Detail.FDataDirForm());
                await Root.SetBusy(false);
            });
        }

        private void CheckFieldScript(FData input)
        {
            if (input != null && input.CheckName("script"))
            {
                Detail.InitData = new DataSet().AddTable(new DataTable().AddRowValue(0, "script", input["script"]).Copy());
                Settings.HasInit = true;
            }
            else
            {
                Detail.InitData = null;
                Settings.HasInit = false;
            }
        }

        private async Task ShowForm(FToolbar toolbar)
        {
            var form = new FPageFilter(toolbar.ShowForm)
            {
                Target = FFormTarget.Filter,
                FormType = FFormType.Filter,
                Root = Root,
                //InputData = Root.FDataDirForm(0)
            };
            form.CancelClick += async (s, e) => { await Root.Page.Navigation.PopAsync(); };
            form.BackButtonClicked += async (s, e) => { await Root.Page.Navigation.PopAsync(); };
            form.InputView.Content = null;
            await form.SetBusy(true);
            await Root.Page.Navigation.PushAsync(form, true);
            await form.InitByController();
            await form.SetBusy(false);
            form.OkClick += async (s, e) => { await Root.Page.Navigation.PopAsync(); };
        }

        #endregion
    }
}