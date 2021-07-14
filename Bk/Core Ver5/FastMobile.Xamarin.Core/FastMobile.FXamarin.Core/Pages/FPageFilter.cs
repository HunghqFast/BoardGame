using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FastMobile.FXamarin.Core
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class FPageFilter : FPage, IFPageFilter, IFStatus
    {
        public static ImageSource SaveFileIcon = FIcons.ContentSaveEditOutline.ToFontImageSource(Color.White, FSetting.SizeIconToolbar);
        public static ImageSource DownFileIcon = FIcons.FileDownloadOutline.ToFontImageSource(Color.White, FSetting.SizeIconToolbar);
        public static ImageSource TipsIcon = FIcons.CommentQuestionOutline.ToFontImageSource(Color.White, FSetting.SizeIconToolbar);
        public static ImageSource ClearIcon = FIcons.Broom.ToFontImageSource(Color.White, FSetting.SizeIconToolbar);

        public static FButton SaveButton => new(FText.Save, FIcons.Check);
        public static FButton OkButton => new(FText.Accept, FIcons.Check);
        public static FButton CancelButton => new(FText.Cancel, FIcons.Close);
        public static FButton NewButton => new(FText.NewAction, FIcons.Plus);
        public static FButton EditButton => new(FText.Edit, FIcons.PencilOutline);
        public static FButton CloseButton => new(FText.Close, FIcons.ChevronLeft);

        public static readonly BindableProperty FormTypeProperty = BindableProperty.Create("FormType", typeof(FFormType), typeof(FPageFilter), FFormType.Filter);
        public static readonly BindableProperty IsMasterProperty = BindableProperty.Create("IsMaster", typeof(bool), typeof(FPageFilter), true);

        public string Controller { get; set; }

        public bool Success { get; set; }

        public bool IsOpening { get; set; }

        public string ClientScript { get; set; }

        public DataSet InitData { get; set; }

        public ContentView ButtonView { get; set; }

        public ScrollView InputView { get; set; }

        public Grid Form { get => Content as Grid; private set => Content = value; }

        public FPage Page { get; set; }

        public FFormType FormType { get => (FFormType)GetValue(FormTypeProperty); set { SetValue(FormTypeProperty, value); OnPropertyChanged("IsEditable"); } }

        public bool IsMaster { get => (bool)GetValue(IsMasterProperty); set => SetValue(IsMasterProperty, value); }

        public Dictionary<string, FInput> Input { get; set; }

        public List<string> InputEdited { get; set; }

        public FViewPage Settings { get; set; }

        public FFormTarget Target { get; set; }

        public string SettingsDeviceID => $"FastMobile.FXamarin.Core.FPageFilter.device_settings_{Target}_{Controller}";

        public string DataSetDeviceID => $"FastMobile.FXamarin.Core.FPageFilter.device_dataset_{Target}_{Controller}";

        public string SaveToolbarDeviceID => $"FastMobile.FXamarin.Core.FPageFilter.device_save_toolbar_{Target}_{Controller}_{FString.ServiceName}_{FString.UserID}";

        public string ExpanderStatus => $"FastMobile.FXamarin.Core.FPageFilter.form_expander_status_{Target}_{Controller}_{FString.ServiceName}_{FString.UserID}_[index]";

        public bool IsEditable => FormType == FFormType.Edit || FormType == FFormType.New || FormType == FFormType.Filter;

        public string Action => FormType switch { FFormType.Filter => "", FFormType.New => FSetting.V ? "Thêm" : "Add", FFormType.Edit => FSetting.V ? "Sửa" : "Edit", FFormType.View => FSetting.V ? "Xem" : "View", FFormType.ViewDetail => FSetting.V ? "Xem" : "View", _ => FSetting.V ? "Thêm" : "Add" };

        public FData InputData { get; set; }

        public FData GridData { get; set; }

        public FData OldData { get; set; }

        public Action Script { get; set; }

        public object Root { get; set; }

        public IFPageFilter.CheckCommand EditCommand { get; set; }

        public IFPageFilter.CheckCommand NewCommand { get; set; }

        public FPageFilterStyle FormStyle { get; set; }

        public FPageFilterMethod Method { get; set; }

        public event EventHandler OkClick;

        public event EventHandler SaveClick;

        public event EventHandler NewClick;

        public event EventHandler EditClick;

        public event EventHandler CancelClick;

        public event EventHandler CloseClick;

        public event EventHandler BackButtonClicked;

        public FPageFilter(string controller) : base(false, false)
        {
            Controller = controller;
            InitDefault();
        }

        public FPageFilter(FViewPage settings) : base(false, false)
        {
            Settings = settings;
            InitDefault();
        }

        public FPageFilter(JObject settings) : base(false, false)
        {
            Settings = new FViewPage(SettingsDeviceID, settings);
            InitDefault();
        }

        #region Protected

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(FormType):
                    switch (FormType)
                    {
                        case FFormType.Filter:
                            ButtonView.Content = FormStyle.InitButtonsView(new List<FButton> { OkButton, CancelButton }, new List<EventHandler<EventArgs>> { OnOkButtonClicked, OnCancelButtonClicked });
                            break;

                        case FFormType.New:
                            ButtonView.Content = IsMaster ? FormStyle.InitButtonsView(new List<FButton> { SaveButton, CancelButton }, new List<EventHandler<EventArgs>> { OnSaveButtonClicked, OnCloseButtonClicked }) : FormStyle.InitButtonsView(new List<FButton> { OkButton, CancelButton }, new List<EventHandler<EventArgs>> { OnSaveButtonClicked, OnCancelButtonClicked });
                            break;

                        case FFormType.Copy:
                        case FFormType.Edit:
                            ButtonView.Content = IsMaster ? FormStyle.InitButtonsView(new List<FButton> { SaveButton, CancelButton }, new List<EventHandler<EventArgs>> { OnSaveButtonClicked, OnCloseButtonClicked }) : FormStyle.InitButtonsView(new List<FButton> { OkButton, CancelButton }, new List<EventHandler<EventArgs>> { OnSaveButtonClicked, OnCancelButtonClicked });
                            break;

                        case FFormType.View:
                            var isNew = false;
                            if (Root is FPageReport report) report.Grid.Settings.Toolbars.ForEach(x => { if (x.Command == "New") { isNew = true; return; } });
                            if (isNew) ButtonView.Content = FormStyle.InitButtonsView(new List<FButton> { NewButton, EditButton, CloseButton }, new List<EventHandler<EventArgs>> { OnNewButtonClicked, OnEditButtonClicked, OnCloseButtonClicked });
                            else ButtonView.Content = FormStyle.InitButtonsView(new List<FButton> { EditButton, CloseButton }, new List<EventHandler<EventArgs>> { OnEditButtonClicked, OnCloseButtonClicked });
                            break;

                        case FFormType.ViewDetail:
                            ButtonView.Content = FormStyle.InitButtonsView(new List<FButton> { CloseButton }, new List<EventHandler<EventArgs>> { OnCloseButtonClicked });
                            break;
                    }
                    InitToobar();
                    break;

                case nameof(IsEditable):
                    Input.ForEach(i => i.Value.IsEditable = IsEditable);
                    break;

                default:
                    break;
            }
        }

        #endregion Protected

        #region Button

        private async void OnOkButtonClicked(object sender, EventArgs e)
        {
            await SetBusy(true);
            await Task.Delay(200);
            if (await Method.CheckFault()) FFunc.CreateEventArgs(this, OkClick, e);
            else await SetBusy(false);
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            await SetBusy(true);
            await Task.Delay(200);
            if (!await Method.CheckEdited())
            {
                await SetBusy(false);
                await Navigation.PopAsync();
            }
            else if (await Method.CheckFault())
            {
                await Method.InvokeExtendAction();
                FFunc.CreateEventArgs(this, SaveClick, e);
            }
            else await SetBusy(false);
        }

        private async void OnNewButtonClicked(object sender, EventArgs e)
        {
            await SetBusy(true);
            if (await NewCommand(FDataDirForm()))
            {
                FormType = FFormType.New;
                Title = $"{Action} {Settings.Title}";
                FFunc.CreateEventArgs(this, NewClick, e);
            }
            await SetBusy(false);
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            await SetBusy(true);
            if (await EditCommand(FDataDirForm()))
            {
                FormType = FFormType.Edit;
                Title = $"{Action} {Settings.Title}";
                FFunc.CreateEventArgs(this, EditClick, e);
            }
            await SetBusy(false);
        }

        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            FFunc.CreateEventArgs(this, CancelClick, e);
        }

        private void OnCloseButtonClicked(object sender, EventArgs e)
        {
            FFunc.CreateEventArgs(this, CloseClick, e);
        }

        protected override bool OnBackButtonPressed()
        {
            FFunc.CreateEventArgs(this, BackButtonClicked, EventArgs.Empty);
            return true;
        }

        #endregion Button

        #region Public

        public async Task InitByController()
        {
            await Method.InitSettings(false);
        }

        public async Task InitBySetting()
        {
            await Method.InitSettings(true);
        }

        public async Task InitContent()
        {
            if (FormType != FFormType.Filter) InitForm();
            await Task.CompletedTask;
        }

        public async Task ExcuteButton()
        {
            await SetBusy(true);
            if (FormType == FFormType.Filter)
            {
                FFunc.CreateEventArgs(this, OkClick, EventArgs.Empty);
                await SetBusy(false);
                return;
            }

            if (FormType != FFormType.View && FormType != FFormType.ViewDetail)
            {
                FFunc.CreateEventArgs(this, SaveClick, EventArgs.Empty);
                await SetBusy(false);
                return;
            }
            await SetBusy(false);
        }

        public async Task Request(string requestAction, List<string> requestField)
        {
            await SetBusy(true);
            var request = await Method.Requesting(requestAction, requestField);
            if (request.Success != 1)
            {
                await SetBusy(false);
                return;
            }
            var ds = request.ToDataSet();
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0 || ds.Tables[0].Rows[0] is not DataRow dr)
            {
                await SetBusy(false);
                return;
            }
            var co = ds.Tables[0].Columns;
            var fs = Settings.Fields.FindAll(f => co.Contains(f.Name));
            fs.ForEach(fi => { if (Input.TryGetValue(fi.Name, out FInput va)) va.SetInput(dr[fi.Name], true); });
            FFunc.CatchScriptMethod(this, ds);
            await SetBusy(false);
        }

        public object InvokeScript(string id, string scriptDetail, string input = null)
        {
            bool isReturn = false;
            string me = string.Empty;
            FInput rt = null;

            DeclareVariable(ref isReturn, ref me, ref rt, id, input, scriptDetail);
            if (isReturn) return null;
            var rs = FFunc.Compute(this, me, null, rt != null && rt.Type == FieldType.Number);
            if (rt != null) rt.SetInput(rs);
            return rs ?? false;
        }

        public async Task<object> InvokeScriptAsync(string id, string scriptDetail, string input = null)
        {
            bool isReturn = false;
            string me = string.Empty;
            FInput rt = null;

            DeclareVariable(ref isReturn, ref me, ref rt, id, input, scriptDetail);
            if (isReturn) return null;

            var rs = await FFunc.ComputeAsync(this, me, null, rt != null && rt.Type == FieldType.Number);
            if (rt != null) rt.SetInput(rs);
            return rs ?? false;
        }

        public async void ClearView()
        {
            Form.Children.Clear();
            await Task.CompletedTask;
        }

        public async void ClearAll()
        {
            Input.Values.ForEach(ip => ip.Clear(false));
            await Task.CompletedTask;
        }

        public async void FillAll(bool isDisable)
        {
            await SetBusy(true);
            await Method.ClearExtendFunc();
            Input.Values.ForEach(ip => Method.FillValue(ip, isDisable));
            await Method.CheckExtendFunc();
            await SetBusy(false);
        }

        public FData FDataDirForm(int mode = 0)
        {
            var it = new FData();
            Settings.Fields.ForEach(f => it[f.Name, Input[f.Name].Type] = Input[f.Name].GetInput(mode));
            return it;
        }

        #endregion Public

        #region Private

        private void DeclareVariable(ref bool isReturn, ref string me, ref FInput rt, string id, string input, string scriptDetail)
        {
            var ro = this as IFPageFilter;
            var de = false;

            if (!scriptDetail.Equals(string.Empty))
            {
                if (id.Contains(scriptDetail))
                {
                    id = id.Remove($"{scriptDetail}.");
                    ro = (Input[scriptDetail] as FInputGrid).Detail;
                    de = true;
                }
            }
            else if (FInput.IsRootParam(id))
            {
                id = FInput.RootParam(id);
                ro = (Root as FInputGrid).Root;
            }

            var sc = ro?.Settings.Scripts.Find(s => s.Id == id);
            if (sc == null)
            {
                isReturn = true;
                return;
            }

            var rn = sc.ReturnValue;
            var fs = sc.Field;
            var co = sc.Condition;
            me = sc.Method.Replace("@@scanner", $"'{input ?? string.Empty}'");

            if (de)
            {
                (Input[scriptDetail] as FInputGrid).SetDetailColumn(rn, fs, me, co);
                isReturn = true;
                return;
            }
            me = ReplaceVariable(me, co, fs, ro);
            me = me.Replace("@@copy", Method.IsCopy.ToString().ToLower());
            rt = ro.Input.ContainsKey(rn) ? ro.Input[rn] : null;
        }

        private string ReplaceVariable(string method, string condition, List<string> field, IFPageFilter root)
        {
            foreach (var n in field)
            {
                var fi = FInput.IsRootParam(n) ? (root.Root as FInputGrid).Root.Input : root.Input;
                if (fi.TryGetValue(FInput.RootParam(n), out var ip))
                {
                    if (method.Contains($"[{n}].Title")) method = method.Replace($"[{n}].Title", ReplaceSpecial(ip.Title));
                    if (method.Contains($"[{n}].Footer")) method = method.Replace($"[{n}].Footer", ReplaceSpecial(ip.NoticeName));
                    if (method.Contains($"[{n}].Annotation")) method = method.Replace($"[{n}].Annotation", ReplaceSpecial(ip.Annotation));
                    if (method.Contains($"[{n}].AllowNulls")) method = method.Replace($"[{n}].AllowNulls", (!ip.NotAllowsNull).ToString().ToLower());
                    if (method.Contains($"[{n}].IsReadonly")) method = method.Replace($"[{n}].IsReadonly", (!ip.IsReadOnly).ToString().ToLower());
                    switch (ip.Type)
                    {
                        case FieldType.DateTime:
                        case FieldType.Bool:
                        case FieldType.Number:
                        case FieldType.NumberString:
                            method = method.Replace($"[{n}]", ip.Output);
                            break;

                        case FieldType.Table:
                            var detail = ip as FInputGrid;
                            if (method.Contains($"[{n}].Sum"))
                            {
                                var re = new Regex($"\\[{n}\\].Sum\\((.+?)\\)");
                                re.Matches(method).ForEach(ma => method = method.Replace($"[{n}].Sum({ma.Groups[1].Value})", detail.Sum(ma.Groups[1].Value, condition).ToString()));
                            }
                            break;

                        default:
                            method = method.Replace($"[{n}]", ReplaceSpecial(ip.GetInput(0).ToString()));
                            break;
                    }
                }
            }
            return method;

            string ReplaceSpecial(string str)
            {
                return str.Replace("\\", "\\\\").Replace("'", "\\'");
            }
        }

        private Task AddInput(FField field)
        {
            FField f = field.Clone() as FField;
            FInput input = f.ItemStyle switch
            {
                FItemStyle.AutoComplete => new FInputLookup(f, Controller, FLookupType.None, Target),
                FItemStyle.Lookup => new FInputLookup(f, Controller, FLookupType.Multi, Target),
                FItemStyle.DropDownList => new FInputDropDownList(f),
                FItemStyle.Toggle => new FInputSwitch(f),
                FItemStyle.DateTimePicker => new FInputDate(f),
                FItemStyle.Grid => new FInputGrid(f),
                FItemStyle.Dir => new FInputDir(f),
                FItemStyle.File => new FInputFile(f),
                FItemStyle.NumberEdit => new FInputNumber(f),
                FItemStyle.Password => new FInputPassword(f),
                FItemStyle.CaptCha => new FInputCaptcha(f),
                FItemStyle.Hour => new FInputHour(f),
                FItemStyle.ScriptText => new FInputScripts(f),
                FItemStyle.Email => new FInputEmail(f),
                FItemStyle.TextCode or FItemStyle.TextUnderLine => new FInputTextUnderline(f),
                FItemStyle.Location => new FInputLocation(f),
                FItemStyle.Image => new FInputMediaImage(f),
                FItemStyle.Video => new FInputMediaVideo(f),
                _ => new FInputText(f)
            };
            input.Root = this;
            input.IsEditable = IsEditable;
            input.Rendering();
            Input.Add(f.Name, input);
            return Task.CompletedTask;
        }

        #endregion Private

        #region Created

        private void InitDefault()
        {
            InitData = null;
            GridData = null;
            Page = this;
            FormStyle = new FPageFilterStyle(this);
            Method = new FPageFilterMethod(this, this);
            ButtonView = new ContentView
            {
                Content = FormStyle.InitButtonsView(new List<FButton> { OkButton, CancelButton }, new List<EventHandler<EventArgs>> { OnOkButtonClicked, OnCancelButtonClicked })
            };
            InputView = new ScrollView { VerticalScrollBarVisibility = ScrollBarVisibility.Never };
            InputData = null;
            Root = null;
            ClientScript = string.Empty;
            Input = new Dictionary<string, FInput>();
            InputEdited = new List<string>();
            InitForm();
            FNavigationPage.SetBackButtonTitle(this, string.Empty);
            NavigationPage.SetHasBackButton(this, false);
        }

        public void InitForm()
        {
            Form ??= new Grid
            {
                ColumnDefinitions = { new ColumnDefinition { Width = GridLength.Star } },
                RowDefinitions = { new RowDefinition { Height = GridLength.Star }, new RowDefinition { Height = 1 }, new RowDefinition { Height = GridLength.Auto } },
                RowSpacing = 0
            };
            Form.Children.Add(InputView, 0, 0);
            Form.Children.Add(new FLine(), 0, 1);
            Form.Children.Add(ButtonView, 0, 2);
        }

        public async void InitToobar()
        {
            if (Settings == null) return;
            if (FormType == FFormType.Filter)
            {
                ToolbarItems.Clear();
                if (Settings.Tips != null) ToolbarItems.Add(new ToolbarItem { IconImageSource = TipsIcon, Command = new Command(Method.OnTipsToolbarClicked) });
                if (Settings.ClearMode) ToolbarItems.Add(new ToolbarItem { IconImageSource = ClearIcon, Command = new Command(ClearAll) });
                return;
            }
            if (FormType == FFormType.New || FormType == FFormType.View || FormType == FFormType.Edit)
            {
                ToolbarItems.Clear();
                if (!Settings.CopyMode) return;
                if (!string.IsNullOrEmpty(SaveToolbarDeviceID.GetCache()) && FormType != FFormType.View)
                {
                    ToolbarItems.Add(new ToolbarItem { IconImageSource = DownFileIcon, Command = new Command(Method.OnDownFileToobarClicked), BindingContext = this });
                    ToolbarItems[^1].SetBinding(ToolbarItem.IsEnabledProperty, IsBusyProperty.PropertyName, converter: FInvertBool.Instance);
                }
                ToolbarItems.Add(new ToolbarItem { IconImageSource = SaveFileIcon, Command = new Command(Method.OnSaveToobarClicked), BindingContext = this });
                ToolbarItems[^1].SetBinding(ToolbarItem.IsEnabledProperty, IsBusyProperty.PropertyName, converter: FInvertBool.Instance);
            }
            await Task.CompletedTask;
        }

        public async Task InitInput()
        {
            if (Input.Count > 0) return;
            await Settings.Fields.ForAllAsync(x => AddInput(x));
            FormStyle.InitInputView();
        }

        public static void SwapInput(FInput A, FInput B, bool isSwapTitle)
        {
            FFunc.SwapObject(A, B, "NotAllowsNull");
            FFunc.SwapObject(A, B, "DefaultValue");
            FFunc.SwapObject(A, B, "IsReadOnly");
            FFunc.SwapObject(A, B, "ScriptFocus");
            FFunc.SwapObject(A, B, "TextAlignment");
            FFunc.SwapObject(A, B, "Color");
            FFunc.SwapObject(A, B, "TitleColor");

            if (isSwapTitle)
            {
                FFunc.SwapObject(A, B, "Title");
                FFunc.SwapObject(A, B, "NoticeName");
            }

            if (A is FInputNumber a && B is FInputNumber b)
            {
                FFunc.SwapObject(a, b, "Format");
                FFunc.SwapObject(a, b, "MaxValue");
                FFunc.SwapObject(a, b, "MinValue");
            }
            else if (A is FInputTextBase c && B is FInputTextBase d)
            {
                FFunc.SwapObject(c, d, "MaxLength");
                FFunc.SwapObject(c, d, "MaxValue");
                FFunc.SwapObject(c, d, "MinValue");
                FFunc.SwapObject(c, d, "PlaceHolder");
                FFunc.SwapObject(c, d, "IsTextUpper");
                FFunc.SwapObject(c, d, "IsTextLower");
            }
        }

        public static void SwapFields(FField A, FField B)
        {
            FFunc.SwapObject(A, B, "Title");
            FFunc.SwapObject(A, B, "SubTitle");
            FFunc.SwapObject(A, B, "AllowNulls");
            FFunc.SwapObject(A, B, "DefaultValue");
            FFunc.SwapObject(A, B, "IsReadOnly");
            FFunc.SwapObject(A, B, "Max");
            FFunc.SwapObject(A, B, "Min");
            FFunc.SwapObject(A, B, "MaxLength");
            FFunc.SwapObject(A, B, "DataFormatString");
            FFunc.SwapObject(A, B, "ScriptFocus");
            FFunc.SwapObject(A, B, "TextAlignment");
            FFunc.SwapObject(A, B, "Color");
            FFunc.SwapObject(A, B, "TitleColor");
        }

        #endregion Created
    }
}