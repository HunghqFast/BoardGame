using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Page = Xamarin.Forms.Page;

namespace FastMobile.FXamarin.Core
{
    public class FPageReport : FPageSearchForms
    {
        private bool isLock;
        private readonly FViewPage inputGrid;
        private readonly ContentView subTitle, master, footer, tableTitle, pagingTitle, loadmoreTitle, attachment, newRecord, comment;

        public static readonly BindableProperty SubTitleViewProperty = BindableProperty.Create("SubTitleView", typeof(View), typeof(FPageReport), new StackLayout());
        public static readonly BindableProperty MasterViewProperty = BindableProperty.Create("MasterView", typeof(View), typeof(FPageReport), new StackLayout());
        public static readonly BindableProperty TitleTableViewProperty = BindableProperty.Create("TitleTableView", typeof(View), typeof(FPageReport), new StackLayout());
        public static readonly BindableProperty PagingTitleViewProperty = BindableProperty.Create("PagingTitleView", typeof(View), typeof(FPageReport), new StackLayout());
        public static readonly BindableProperty LoadmoreTitleViewProperty = BindableProperty.Create("LoadmoreTitleView", typeof(View), typeof(FPageReport), new StackLayout());
        public static readonly BindableProperty FooterViewProperty = BindableProperty.Create("FooterView", typeof(View), typeof(FPageReport), new StackLayout());
        public static readonly BindableProperty NewRecordViewProperty = BindableProperty.Create("NewRecordView", typeof(View), typeof(FPageReport), new StackLayout());
        public static readonly BindableProperty AttachmentViewProperty = BindableProperty.Create("AttachmentView", typeof(View), typeof(FPageReport), new StackLayout());
        public static readonly BindableProperty CommentViewProperty = BindableProperty.Create("CommentView", typeof(View), typeof(FPageReport), new StackLayout());
        public static readonly BindableProperty IsVisibleSubTitleViewProperty = BindableProperty.Create("IsVisibleSubTitleView", typeof(bool), typeof(FPageReport), true);
        public static readonly BindableProperty IsVisibleMasterViewProperty = BindableProperty.Create("IsVisibleMasterView", typeof(bool), typeof(FPageReport), true);
        public static readonly BindableProperty IsVisibleTitleTableViewProperty = BindableProperty.Create("IsVisibleTitleTableView", typeof(bool), typeof(FPageReport), true);
        public static readonly BindableProperty IsVisiblePagingTitleViewProperty = BindableProperty.Create("IsVisiblePagingTitleView", typeof(bool), typeof(FPageReport), true);
        public static readonly BindableProperty IsVisibleLoadmoreTitleViewProperty = BindableProperty.Create("IsVisibleLoadmoreTitleView", typeof(bool), typeof(FPageReport), true);
        public static readonly BindableProperty IsVisibleFooterViewProperty = BindableProperty.Create("IsVisibleFooterView", typeof(bool), typeof(FPageReport), true);
        public static readonly BindableProperty IsVisibleAttachmentViewProperty = BindableProperty.Create("IsVisibleAttachmentView", typeof(bool), typeof(FPageReport));
        public static readonly BindableProperty IsVisibleNewRecordViewProperty = BindableProperty.Create("IsVisibleNewRecordView", typeof(bool), typeof(FPageReport), true);
        public static readonly BindableProperty IsVisibleCommentViewProperty = BindableProperty.Create("IsVisibleCommentView", typeof(bool), typeof(FPageReport), true);
        public static readonly BindableProperty IsLoadingViewProperty = BindableProperty.Create("IsLoading", typeof(bool), typeof(FPageReport), true);
        public static readonly BindableProperty ReportStyleProperty = BindableProperty.Create("ReportStyle", typeof(FReportStyle), typeof(FPageReport), new FReportStyle());
        public static readonly BindableProperty SubtitleTextProperty = BindableProperty.Create("SubtitleText", typeof(string), typeof(FPageReport), "");
        public static readonly BindableProperty MenuButtonProperty = BindableProperty.Create("MenuButton", typeof(ObservableCollection<FMenuButtonReport>), typeof(FPageReport));
        public static readonly BindableProperty AttachmentProperty = BindableProperty.Create("Attachment", typeof(ObservableCollection<FMenuButtonReport>), typeof(FPageReport));

        public string SubtitleText { get => (string)GetValue(SubtitleTextProperty); set => SetValue(SubtitleTextProperty, value); }

        public View SubTitleView { get => (View)GetValue(SubTitleViewProperty); set => SetValue(SubTitleViewProperty, value); }

        public View MasterView { get => (View)GetValue(MasterViewProperty); set => SetValue(MasterViewProperty, value); }

        public View FooterView { get => (View)GetValue(FooterViewProperty); set => SetValue(FooterViewProperty, value); }

        public View TitleTableView { get => (View)GetValue(TitleTableViewProperty); set => SetValue(TitleTableViewProperty, value); }

        public View PagingTitleView { get => (View)GetValue(PagingTitleViewProperty); set => SetValue(PagingTitleViewProperty, value); }

        public View LoadmoreTitleView { get => (View)GetValue(LoadmoreTitleViewProperty); set => SetValue(LoadmoreTitleViewProperty, value); }

        public View AttachmentView { get => (View)GetValue(AttachmentViewProperty); set => SetValue(AttachmentViewProperty, value); }

        public View NewRecordView { get => (View)GetValue(NewRecordViewProperty); set => SetValue(NewRecordViewProperty, value); }

        public View CommentView { get => (View)GetValue(CommentViewProperty); set => SetValue(CommentViewProperty, value); }

        public bool IsVisibleTitleTableView { get => (bool)GetValue(IsVisibleTitleTableViewProperty); set => SetValue(IsVisibleTitleTableViewProperty, value); }

        public bool IsVisibleMasterView { get => (bool)GetValue(IsVisibleMasterViewProperty); set => SetValue(IsVisibleMasterViewProperty, value); }

        public bool IsVisibleSubTitleView { get => (bool)GetValue(IsVisibleSubTitleViewProperty); set => SetValue(IsVisibleSubTitleViewProperty, value); }

        public bool IsVisiblePagingTitleView { get => (bool)GetValue(IsVisiblePagingTitleViewProperty); set { SetValue(IsVisiblePagingTitleViewProperty, value); OnPropertyChanged("IsVisibleLoadMoreAndPaging"); } }

        public bool IsVisibleLoadMoreAndPaging
        {
            get
            {
                if (IsVisibleLoadmoreTitleView || IsVisiblePagingTitleView)
                    return (!IsVisibleTitleTableView || (IsVisibleTitleTableView && (IsVisibleLoadmoreTitleView || IsVisiblePagingTitleView))) && Grid.GridType != GridType.GridView;
                return false;
            }
        }

        public bool IsVisibleLoadmoreTitleView { get => (bool)GetValue(IsVisibleLoadmoreTitleViewProperty); set { SetValue(IsVisibleLoadmoreTitleViewProperty, value); OnPropertyChanged("IsVisibleLoadMoreAndPaging"); } }

        public bool IsVisibleFooterView { get => (bool)GetValue(IsVisibleFooterViewProperty); set => SetValue(IsVisibleFooterViewProperty, value); }

        public bool IsVisibleAttachmentView { get => (bool)GetValue(IsVisibleAttachmentViewProperty); set => SetValue(IsVisibleAttachmentViewProperty, value); }

        public bool IsVisibleNewRecordView { get => (bool)GetValue(IsVisibleNewRecordViewProperty); set => SetValue(IsVisibleNewRecordViewProperty, value); }

        public bool IsVisibleCommentView { get => (bool)GetValue(IsVisibleCommentViewProperty); set => SetValue(IsVisibleCommentViewProperty, value); }

        public bool IsLoading { get => (bool)GetValue(IsLoadingViewProperty); set => SetValue(IsLoadingViewProperty, value); }

        public FReportStyle ReportStyle { get => (FReportStyle)GetValue(ReportStyleProperty); set => SetValue(ReportStyleProperty, value); }

        public ObservableCollection<FMenuButtonReport> MenuButton { get => (ObservableCollection<FMenuButtonReport>)GetValue(MenuButtonProperty); set => SetValue(MenuButtonProperty, value); }

        public ObservableCollection<FMenuButtonReport> Attachment { get => (ObservableCollection<FMenuButtonReport>)GetValue(AttachmentProperty); set => SetValue(AttachmentProperty, value); }

        public FPageFilter Filter { get; set; }
        public FPageFilter Dir { get; set; }
        public FGridBase Grid { get; set; }
        public FPageReport BeforePage { get; set; }

        public string Target, Controller;
        public DataSet InputData;
        public DataTable DetailData;
        public FTargetType DataTarget;
        public bool IsFirst, IsFilter;

        public FPageReport(string controller, bool isFilter = true, FPageReport before = null, DataSet data = null, FViewPage grid = null, FTargetType target = FTargetType.Filter, DataTable resTable = null) : base(false, false)
        {
            isLock = false;
            inputGrid = grid;

            Controller = controller;
            InputData = data;
            DataTarget = target;
            BeforePage = before;
            IsFilter = isFilter;
            IsFirst = true;
            DetailData = resTable;

            ReportStyle.InitBaseView(ref subTitle, this, "SubTitleView", "IsVisibleSubTitleView");
            ReportStyle.InitBaseView(ref tableTitle, this, "TitleTableView", "IsVisibleTitleTableView");
            ReportStyle.InitBaseView(ref master, this, "MasterView", "IsVisibleMasterView");
            ReportStyle.InitBaseView(ref pagingTitle, this, "PagingTitleView", "IsVisiblePagingTitleView");
            ReportStyle.InitBaseView(ref loadmoreTitle, this, "LoadmoreTitleView", "IsVisibleLoadmoreTitleView");
            ReportStyle.InitBaseView(ref footer, this, "FooterView", "IsVisibleFooterView");
            ReportStyle.InitBaseView(ref comment, this, "CommentView", "IsVisibleCommentView");
            ReportStyle.InitBaseView(ref attachment, this, "AttachmentView", "IsVisibleAttachmentView");
            ReportStyle.InitBaseView(ref newRecord, this, "NewRecordView", "IsVisibleNewRecordView");

            attachment.Padding = new Thickness(0, 3);
        }

        #region Public

        public static Task SetReportByAction(Page parent, string action, string controller, FPageReport before = null, DataSet data = null, FViewPage grid = null, FTargetType target = FTargetType.Filter, bool openDetail = false, DataTable resTable = null)
        {
            return action switch
            {
                "" => new FPageTypeDefault(controller, true, resTable).Run(parent, openDetail),
                "List" => new FPageTypeDefault(controller, false, resTable).Run(parent, openDetail),
                "List2" => new FPageTypeReport(controller, true, resTable).Run(parent, openDetail),
                "Report" => new FPageTypeReport(controller, false, resTable).Run(parent, openDetail),
                "Report2" => new FPageTypeReport2(controller, resTable).Run(parent, openDetail),
                "Approval" => new FPageTypeApproval(controller, resTable).Run(parent, openDetail),
                "Account" => new FPageReportTypeAccount(controller).Run(parent),
                "Inquiry" => new FPageTypeInquiry(controller, before, data, grid, target).Run(parent),
                "Filter" => new FPageTypeFilter(controller).Run(parent),
                _ => new FPageTypeDefault(controller, true, resTable).Run(parent, openDetail),
            };
        }

        public async Task<bool> InitSetting()
        {
            InitFilter();
            InitGridBase();
            InitDir();

            if (await InitAll())
            {
                await Task.WhenAll(InitGridView(), InitFilterView(), InitDirView(), InitAnotherView(), InitSearchBarView()).ConfigureAwait(false);
                return await CheckStatus();
            }
            return false;
        }

        public virtual void SetPageView()
        {
            var ab = new AbsoluteLayout { BackgroundColor = Color.White, VerticalOptions = LayoutOptions.StartAndExpand, HorizontalOptions = LayoutOptions.StartAndExpand };
            var st = new StackLayout { Margin = 0, Spacing = 0, BackgroundColor = Color.White };
            var lo = new BoxView { BackgroundColor = Color.White, HeightRequest = FSetting.ScreenHeight, WidthRequest = FSetting.ScreenWidth };
            var ls = new FLine { BindingContext = this };
            var lm = new FLine { BindingContext = this };
            var lp = new FLine { BindingContext = this };
            var lt = new FLine { BindingContext = this };
            var la = new FLine { BindingContext = this };
            var ln = new FLine { BindingContext = this };
            var lf = new FLine { BindingContext = this };
            var lc = new FLine { BindingContext = this };

            ls.SetBinding(FLine.IsVisibleProperty, "IsVisibleSubTitleView");
            lm.SetBinding(FLine.IsVisibleProperty, "IsVisibleMasterView");
            lp.SetBinding(FLine.IsVisibleProperty, "IsVisibleLoadMoreAndPaging");
            lt.SetBinding(FLine.IsVisibleProperty, "IsVisibleTitleTableView");
            la.SetBinding(FLine.IsVisibleProperty, "IsVisibleAttachmentView");
            ln.SetBinding(FLine.IsVisibleProperty, "IsVisibleNewRecordView");
            lf.SetBinding(FLine.IsVisibleProperty, "IsVisibleFooterView");
            lc.SetBinding(FLine.IsVisibleProperty, "IsVisibleCommentView");

            st.Children.Add(subTitle);
            st.Children.Add(ls);
            st.Children.Add(master);
            st.Children.Add(lm);
            st.Children.Add(pagingTitle);
            st.Children.Add(loadmoreTitle);
            st.Children.Add(lp);
            st.Children.Add(tableTitle);
            st.Children.Add(lt);

            st.Children.Add(Grid);
            st.Children.Add(lc);
            st.Children.Add(comment);

            st.Children.Add(la);
            st.Children.Add(attachment);
            st.Children.Add(ln);
            st.Children.Add(newRecord);
            st.Children.Add(lf);
            st.Children.Add(footer);

            AbsoluteLayout.SetLayoutBounds(lo, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(lo, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(st, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(st, AbsoluteLayoutFlags.All);

            lo.SetBinding(BoxView.IsVisibleProperty, IsLoadingViewProperty.PropertyName);
            lo.BindingContext = this;

            ab.Children.Add(st);
            ab.Children.Add(lo);

            SearchContent = ab;
        }

        public virtual async void SetView()
        {
            if (IsVisiblePagingTitleView || IsVisibleLoadmoreTitleView)
            {
                IsVisibleLoadmoreTitleView = Grid.Settings.ReportType == ReportType.Loadmore && Grid.GridType == GridType.ListView;
                IsVisiblePagingTitleView = !IsVisibleLoadmoreTitleView;
            }
            await UpdateSubTitle();
        }

        public void Dispose()
        {
            Grid.CacheDeviceID.RemoveCache();
        }

        public void RefreshMenuView()
        {
            NewRecordView = ReportStyle.InitToolbarView(MenuButton, 50, -1, FSetting.SizeIconMenu, true, true);
        }

        public async Task OptionalViewType()
        {
            if (Filter.Input.TryGetValue("view_type", out FInput input))
            {
                var viewType = bool.Parse(input.GetInput(0).ToString());
                IsVisibleTitleTableView = !viewType && Grid.Settings.Views.Find(x => x.Id == "Header") != null;
                Grid.GridType = !viewType ? GridType.ListView : GridType.GridView;
            }
            await Task.CompletedTask;
        }

        public async Task UpdateSubTitle()
        {
            if (!IsVisibleSubTitleView) return;
            var value = $"<span>{Grid.Settings.SubTitle}</span>";
            Grid.ExtData.ForEach(k => value = value.Replace($"%{ k.Key}", $"<font color='#000080'>{k.Value}</font>"));
            SubtitleText = value;
            await Task.CompletedTask;
        }

        public async Task UpdateMasterView()
        {
            if (!IsVisibleMasterView) return;
            MasterView.BindingContext = Grid.Details;
            await Task.CompletedTask;
        }

        public async Task UpdateFooterView()
        {
            if (!IsVisibleFooterView) return;
            FooterView.BindingContext = Grid.Details;
            await Task.CompletedTask;
        }

        public async void ShowFilter(object sender)
        {
            await Lock(sender, async () => { await FilterCustom(); });
        }

        public async Task FilterCustom(Page page = null)
        {
            var content = Filter.InputView.Content;
            Filter.InputView.Content = null;
            await Filter.SetBusy(true);
            if (page == null) await Navigation.PushAsync(Filter, true);
            else await page.Navigation.PushAsync(Filter, true);
            if (content == null) await Filter.InitBySetting();
            else Filter.InputView.Content = content;
            await Filter.SetBusy(false);
        }

        public async Task DirCustom(FFormType type, FData input, FData data, Action action)
        {
            Dir.ClearView();
            Dir.FormType = type;
            Dir.InputData = input;
            Dir.GridData = data;
            Dir.Script = action;
            Dir.Input.Clear();
            Dir.InputEdited.Clear();
            await Dir.SetBusy(true);
            Dir.Title = $"{Dir.Action} {Dir.Settings.Title}";
            await Navigation.PushAsync(Dir, true);
            await Dir.InitBySetting();
            await Dir.SetBusy(false);
        }

        public async void SetMenuButton()
        {
            ToolbarItems.Clear();
            if (IsFilter) ToolbarItems.Add(new ToolbarItem { IconImageSource = FIcons.FilterMenuOutline.ToFontImageSource(Color.White, FSetting.SizeIconToolbar), Command = new Command(ShowFilter), CommandParameter = this });
            await Task.CompletedTask;
        }

        #endregion Public

        #region Private

        private async Task InitGridView()
        {
            Title = Grid.Settings.Title;
            Grid.ItemPerPage = Grid.Settings.PageItems;
            Grid.IsAlowSelect = Grid.Settings.SelectMode;
            await Grid.InitView();
            UpdateToobarButtonBioProperty();
            if (Grid.Settings.SelectMode) Grid.GridView.Columns.Insert(0, new FCheckBoxColumn(string.Empty) { MappingName = FData.GetBindingName(FData.CheckStatusName), Width = 46, MaximumWidth = 46, MinimumWidth = 46 });
        }

        private async Task InitFilterView()
        {
            if (Filter?.Settings == null) return;
            Filter.Title = Filter.Settings.Title;
            Filter.Success = true;
            await Task.CompletedTask;
        }

        private async Task InitDirView()
        {
            if (Dir?.Settings == null) return;
            Dir.Success = true;
            await Task.CompletedTask;
        }

        private async Task InitSearchBarView()
        {
            TurnOnSearch = Grid.Settings.IsSearchBar;
            Placeholder = Grid.Settings.PlacHolderSearchBar;
            SearchBarTextSubmit += FPageReportSearchBarTextSubmit;
            await Task.CompletedTask;
        }

        private async Task InitAnotherView()
        {
            var hd = Grid.Settings.Views.Find(x => x.Id == "Header");
            var mt = Grid.Settings.Views.Find(x => x.Id == "Master");
            var ft = Grid.Settings.Views.Find(x => x.Id == "Footer");

            if (hd != null) TitleTableView = Grid.GridStyle.GridCustomView(Grid.Settings.Fields, hd.Row, FSetting.ScreenWidth);
            else IsVisibleTitleTableView = false;

            if (mt != null) MasterView = Grid.GridStyle.GridCustomView(Grid.Settings.Details, mt.Row, FSetting.ScreenWidth);
            else IsVisibleMasterView = false;

            if (ft != null) FooterView = Grid.GridStyle.GridCustomView(Grid.Settings.Details, ft.Row, FSetting.ScreenWidth);
            else IsVisibleFooterView = false;

            if (!string.IsNullOrWhiteSpace(Grid.Settings.SubTitle)) SubTitleView = ReportStyle.InitHtmlView(this, "SubtitleText");
            else IsVisibleSubTitleView = false;

            PagingTitleView = ReportStyle.InitPagingDefaultView(Grid);
            LoadmoreTitleView = ReportStyle.InitPagingLoadmoreView(Grid);

            SetMenuButton();
            InitToolbar();
            SetView();
            SetPageView();
            FFunc.CatchScriptMethod(this, InputData);
            await Task.CompletedTask;
        }

        private async Task Lock(object sender, Func<Task> task)
        {
            lock (sender) { if (isLock) return; isLock = true; }
            await task.Invoke();
            isLock = false;
        }

        private async Task Warning()
        {
            if (Dir.FormType == FFormType.New || Dir.FormType == FFormType.Edit)
            {
                if (await FAlertHelper.Confirm("807")) await Navigation.PopAsync(true);
            }
            else await Navigation.PopAsync(true);
        }

        private async Task<bool> CheckSettings()
        {
            if (Target[0] == '1')
            {
                var f = await FViewPage.InitSettingsFromDevice(Filter.SettingsDeviceID);
                if (f == null) return false;
                Filter.Settings = new FViewPage(f);
            }

            if (Target[1] == '1')
            {
                var g = await FViewPage.InitSettingsFromDevice(Grid.SettingsDeviceID);
                if (g == null) return false;
                Grid.Settings = new FViewPage(g);
            }

            if (Target[2] == '1')
            {
                var d = await FViewPage.InitSettingsFromDevice(Dir.SettingsDeviceID);
                if (d == null) return false;
                Dir.Settings = new FViewPage(d);
            }
            return true;
        }

        private async Task<bool> CheckStatus()
        {
            var status = true;
            if (Target[0] == '1') status = status && Filter.Success;
            if (Target[1] == '1') status = status && Grid.Success;
            if (Target[2] == '1') status = status && Dir.Success;
            return await Task.FromResult(status);
        }

        private async void FPageReportSearchBarTextSubmit(object sender, FSearchEventArgs e)
        {
            await SetBusy(true);
            await Grid.LoadingGrid(FTargetType.SearchBar);
            await SetBusy(false);
        }

        private void UpdateToobarButtonBioProperty()
        {
            if (BeforePage == null) return;
            var detail = BeforePage.Grid.DetailView;
            if (detail == null) return;
            Grid.Settings.Toolbars.ForEach(t =>
            {
                if (t.Bio.Equals(FToolbar.ParentPreferenceName)) t.Bio = detail.Bio;
                if (t.Password.Equals(FToolbar.ParentPreferenceName)) t.Password = detail.Password;
                if (t.BioPassword.Equals(FToolbar.ParentPreferenceName)) t.BioPassword = detail.BioPassword;
            });
        }

        #endregion Private

        #region Protected

        protected virtual async void InitGridBase()
        {
            Grid = new FGridBase(Controller)
            {
                Root = this
            };
            await Task.CompletedTask;
        }

        protected virtual async void InitDir()
        {
            if (Target[2] == '0') return;
            Dir = new FPageFilter(Controller)
            {
                Target = FFormTarget.Dir,
                FormType = FFormType.New,
                InputData = null,
                EditCommand = Grid.EditInDirItem,
                NewCommand = Grid.NewInDirItem,
                Root = this
            };
            Dir.SaveClick += async (s, e) => { await Lock(s, async () => { await DirSuccessed(); await Dir.SetBusy(false); }); };
            Dir.CloseClick += async (s, e) => { await Warning(); };
            Dir.BackButtonClicked += async (s, e) => { await Warning(); };
            await Task.CompletedTask;
        }

        protected virtual async void InitFilter()
        {
            if (!IsFilter) return;
            Filter = new FPageFilter(Controller)
            {
                Target = FFormTarget.Filter,
                FormType = FFormType.Filter,
                Root = this
            };
            Filter.OkClick += async (s, e) => { await Lock(s, async () => await FilterSuccessed()); };
            Filter.CancelClick += async (s, e) => { await FilterCanceled(); };
            Filter.BackButtonClicked += async (s, e) => { await FilterBackButton(); };
            await Task.CompletedTask;
        }

        protected virtual async void InitToolbar()
        {
            var nr = Grid.Settings.Toolbars.FindAll(x => x.IsToolbar);
            if (nr == null || nr.Count == 0)
            {
                IsVisibleNewRecordView = false;
                return;
            }
            MenuButton = new ObservableCollection<FMenuButtonReport>();
            nr.ForEach(x =>
            {
                MenuButton.Add(new FMenuButtonReport
                {
                    Action = x.Command switch
                    {
                        "New" => Grid.NewItem,
                        "Release" => Grid.ReleaseItem,
                        "Cancel" => Grid.CancelItem,
                        "Select" => Grid.SelectButton,
                        "AcceptApproval" => Grid.AcceptApprovalItem,
                        "CancelApproval" => Grid.CancelApprovalItem,
                        "UndoApproval" => Grid.UndoApprovalItem,
                        "Next" => Grid.NextToolbarItem,
                        "Back" => Grid.BackToolbarItem,
                        "Close" => async (o) => { await Navigation.PopAsync(); }
                        ,
                        _ => Grid.CustomToolbarItem,
                    },
                    Toolbar = x,
                    Visible = true,
                    Enable = x.Command switch
                    {
                        "Next" => Grid.CheckNextBackToobarEnable(true),
                        "Back" => Grid.CheckNextBackToobarEnable(false),
                        _ => true
                    }
                });
            });
            RefreshMenuView();
            await Task.CompletedTask;
        }

        protected virtual async Task FilterSuccessed()
        {
            IsLoading = true;
            Grid.DataFilter = Filter.FDataDirForm();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                MessagingCenter.Send(FMessage.FromFail(403), FChannel.ALERT_BY_MESSAGE);
                await Filter.SetBusy(false);
                return;
            }
            await OptionalViewType();
            await Filter.SetBusy(false);
            await Navigation.PopAsync(true);
            await Grid.LoadingGrid(FTargetType.Filter);
            await UpdateSubTitle();
            await UpdateMasterView();
            await UpdateFooterView();
            IsLoading = false;
            IsFirst = false;
        }

        protected virtual async Task DirSuccessed()
        {
            await Grid.UpdateItem(Dir.FormType);
        }

        protected virtual async Task FilterCanceled()
        {
            await Navigation.PopAsync(true);
        }

        protected virtual async Task FilterBackButton()
        {
            await Navigation.PopAsync(true);
        }

        protected override bool OnBackButtonPressed()
        {
            Dispose();
            return base.OnBackButtonPressed();
        }

        protected virtual async Task<bool> InitAll()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Controller)) { Grid.Settings = inputGrid; return true; }
                if (await CheckSettings()) return true;
                var m = await GetSettings();
                if (m.Success == 1)
                {
                    var r = JObject.Parse(m.Message.AESDecrypt(FSetting.NetWorkKey));
                    if (Target[0] == '1') Filter.Settings = FViewPage.CheckNull(r["Filter"]) ? new FViewPage(Filter.SettingsDeviceID, (JObject)r["Filter"]["ViewPage"]) : null;
                    if (Target[1] == '1') Grid.Settings = FViewPage.CheckNull(r["Grid"]) ? new FViewPage(Grid.SettingsDeviceID, (JObject)r["Grid"]["ViewPage"]) : null;
                    if (Target[2] == '1') Dir.Settings = FViewPage.CheckNull(r["Dir"]) ? new FViewPage(Dir.SettingsDeviceID, (JObject)r["Dir"]["ViewPage"]) : null;
                    return true;
                }
                else
                {
                    MessagingCenter.Send(m, FChannel.ALERT_BY_MESSAGE);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(0, 310, ex.Message), FChannel.ALERT_BY_MESSAGE);
                return false;
            }
        }

        protected virtual async Task<FMessage> GetSettings()
        {
            return await FServices.ExecuteCommand("Report", Controller, new DataSet().AddTable(new DataTable().AddRowValue("type", "report").AddRowValue(0, "target", Target)), "300", FExtensionParam.New(true, Controller, FAction.Initialize), true);
        }

        protected virtual async Task Run(Page parent, bool openDetail = false)
        {
            await Task.CompletedTask;
        }

        #endregion Protected
    }
}