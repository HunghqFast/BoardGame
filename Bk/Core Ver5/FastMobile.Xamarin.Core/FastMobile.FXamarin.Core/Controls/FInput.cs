using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing;

namespace FastMobile.FXamarin.Core
{
    public class FInput : Grid
    {
        public delegate void OnChangeValueHanler(object sender, FInputChangeValueEventArgs e);

        public const string IsValidData = "FastMobile.FXamarin.Core.FInput.IsValidValue";
        public const char Seperate = 'þ';
        public const string IsModifierPropertyName = "IsModifier";

        public static readonly BindableProperty NotAllowsNullProperty = BindableProperty.Create("NotAllowsNull", typeof(bool), typeof(FInput), false);
        public static readonly BindableProperty IsShowTitleProperty = BindableProperty.Create("IsShowTitle", typeof(bool), typeof(FInput), true);
        public static readonly BindableProperty TitleHeightProperty = BindableProperty.Create("TitleHeight", typeof(double), typeof(FInput), -1d);
        public static readonly BindableProperty IsShowAnnotationProperty = BindableProperty.Create("IsShowAnnotation", typeof(bool), typeof(FInput), false);
        public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title", typeof(string), typeof(FInput), "");
        public static readonly BindableProperty TitleColorProperty = BindableProperty.Create("TitleColor", typeof(Color), typeof(FInput), FSetting.DisableColor);
        public static readonly BindableProperty ColorProperty = BindableProperty.Create("Color", typeof(Color), typeof(FInput), FSetting.TextColorContent);
        public static readonly BindableProperty TitleBackgroundColorProperty = BindableProperty.Create("TitleBackgroundColor", typeof(Color), typeof(FInput), Color.Transparent);
        public static readonly BindableProperty AnnotationProperty = BindableProperty.Create("Annotation", typeof(string), typeof(FInput), "");
        public static readonly BindableProperty AnnotationColorProperty = BindableProperty.Create("AnnotationColor", typeof(Color), typeof(FInput), FSetting.DisableColor);
        public static readonly BindableProperty DefaultValueProperty = BindableProperty.Create("DefaultValue", typeof(object), typeof(FInput), null);
        public static readonly BindableProperty CacheNameProperty = BindableProperty.Create("CacheName", typeof(string), typeof(FInput), "");
        public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create("IsReadOnly", typeof(bool), typeof(FInput), true);
        public static readonly BindableProperty DisableProperty = BindableProperty.Create("Disable", typeof(bool), typeof(FInput), false);
        public static readonly BindableProperty IsEditableProperty = BindableProperty.Create("IsEditable", typeof(bool), typeof(FInput), true);
        public static readonly BindableProperty HeightInputProperty = BindableProperty.Create("HeightInput", typeof(double), typeof(FInput), (double)FSetting.FilterInputHeight);
        public static readonly BindableProperty KeyboardProperty = BindableProperty.Create("Keyboard", typeof(Keyboard), typeof(FInput), InputView.KeyboardProperty.DefaultValue);
        public static readonly BindableProperty IsShowLineProperty = BindableProperty.Create("IsShowLine", typeof(bool), typeof(FInput), true);
        public static readonly BindableProperty IsValidValueProperty = BindableProperty.Create("IsValidValue", typeof(bool), typeof(FInput), true);
        public static readonly BindableProperty TextAlignmentProperty = BindableProperty.Create("TextAlignment", typeof(TextAlignment), typeof(FInput));
        public static readonly BindableProperty HasPaddingProperty = BindableProperty.Create("HasPadding", typeof(bool), typeof(FInput), true);

        public event OnChangeValueHanler ChangeValue;

        public event OnChangeValueHanler CompleteValue;

        public event EventHandler RequestStarted;

        public event EventHandler RequestEnded;

        public event EventHandler<FObjectPropertyArgs<bool>> IsVisibleChanged;

        public string CacheInput => $"FastMobile.FXamarin.Core.FInput_{FString.ServiceName}_{FString.UserID}_{CacheName}";

        public IFPageFilter Root { get; set; }

        public IViewVisible<View> Expander { get; set; }

        public FieldType Type { get; protected set; }

        public bool IsValidValue { get => (bool)GetValue(IsValidValueProperty); set => SetValue(IsValidValueProperty, value); }

        public string Name { get; set; }

        public double HeightInput { get => (double)GetValue(HeightInputProperty); set => SetValue(HeightInputProperty, value); }

        public bool NotAllowsNull { get => (bool)GetValue(NotAllowsNullProperty); set => SetValue(NotAllowsNullProperty, value); }

        public bool IsShowTitle { get => (bool)GetValue(IsShowTitleProperty); set => SetValue(IsShowTitleProperty, value); }

        public double TitleHeight { get => (double)GetValue(TitleHeightProperty); set => SetValue(TitleHeightProperty, value); }

        public bool IsShowAnnotation { get => (bool)GetValue(IsShowAnnotationProperty); set => SetValue(IsShowAnnotationProperty, value); }

        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public Color TitleColor { get => (Color)GetValue(TitleColorProperty); set => SetValue(TitleColorProperty, value); }

        public Color Color { get => (Color)GetValue(ColorProperty); set => SetValue(ColorProperty, value); }

        public string Annotation { get => (string)GetValue(AnnotationProperty); set => SetValue(AnnotationProperty, value); }

        public Color AnnotationColor { get => (Color)GetValue(AnnotationColorProperty); set => SetValue(AnnotationColorProperty, value); }

        public Color TitleBackgroundColor { get => (Color)GetValue(TitleBackgroundColorProperty); set => SetValue(TitleBackgroundColorProperty, value); }

        public object DefaultValue { get => GetValue(DefaultValueProperty); set => SetValue(DefaultValueProperty, value); }

        public string CacheName { get => (string)GetValue(CacheNameProperty); set => SetValue(CacheNameProperty, value); }

        public bool IsReadOnly { get => (bool)GetValue(IsReadOnlyProperty); set { SetValue(IsReadOnlyProperty, value); OnPropertyChanged(IsModifierPropertyName); } }

        public bool BaseReadOnly { get; set; }

        public bool Disable { get => (bool)GetValue(DisableProperty); set => SetValue(DisableProperty, value); }

        public bool IsEditable { get => (bool)GetValue(IsEditableProperty); set { SetValue(IsEditableProperty, value); OnPropertyChanged(IsModifierPropertyName); } }

        public Keyboard Keyboard { get => (Keyboard)GetValue(KeyboardProperty); set => SetValue(KeyboardProperty, value); }

        public bool IsShowLine { get => (bool)GetValue(IsShowLineProperty); set => SetValue(IsShowLineProperty, value); }

        public TextAlignment TextAlignment { get => (TextAlignment)GetValue(TextAlignmentProperty); set => SetValue(TextAlignmentProperty, value); }

        public virtual bool HasPadding { get => (bool)GetValue(HasPaddingProperty); set => SetValue(HasPaddingProperty, value); }

        public new bool IsVisible
        {
            get => base.IsVisible;
            set
            {
                if (base.IsVisible != value)
                {
                    base.IsVisible = value;
                    IsVisibleChanged?.Invoke(this, new FObjectPropertyArgs<bool>(value));
                }
            }
        }

        public virtual double PaddingSize => 10;

        public List<string> HandleField { get; set; }

        public string HandleKey { get; set; }

        public List<string> RequestField { get; set; }

        public string RequestAction { get; set; }

        public List<string> ScriptReference { get; set; }

        public string ScriptDetail { get; set; }

        public bool ScriptCopy { get; set; }

        public string ScriptFocus { get; set; }

        public List<string> ScripScanner { get; set; }

        public string NoticeName { get; set; }

        public virtual bool IsModifier => IsReadOnly && IsEditable;

        public virtual string Output => "''";

        public FInput() : base()
        {
            Init();
            BindingContext = this;
        }

        public FInput(FField field) : base()
        {
            Init();
            InitPropertyByField(field);
            BindingContext = this;
        }

        public FInput(FField field, object extend) : base()
        {
            Init();
            InitPropertyByField(field, extend);
            BindingContext = this;
        }

        #region Protected

        protected virtual void OnChangeValue(object sender, EventArgs e)
        {
            ChangeValue?.Invoke(sender, e as FInputChangeValueEventArgs);
        }

        protected virtual void OnCompleteValue(object sender, EventArgs e)
        {
            CompleteValue?.Invoke(sender, e as FInputChangeValueEventArgs);
            if (Root != null && !Root.InputEdited.Contains(Name))
                Root.InputEdited.Add(Name);
        }

        protected virtual void OnScannedValue(object sender, Result e)
        {
            if (e == null) return;
            ScripScanner.ForEach(x => { _ = GetExpression(x, e.Text); });
        }

        protected virtual void Init()
        {
            IsValidValue = true;
        }

        protected virtual object ReturnValue(int mode)
        {
            return null;
        }

        protected virtual void SetInput(List<string> value, bool isCompleted = false, bool isDisable = false)
        {
            if (value.Count > 2) NotAllowsNull = FFunc.StringToBoolean(value[2]);
            if (isCompleted) OnCompleteValue(this, new FInputChangeValueEventArgs(value[0]));
        }

        protected virtual View SetContentView()
        {
            return new Label();
        }

        protected string GetCacheInput(string initValue)
        {
            var value = CacheInput.GetCache();
            if (!string.IsNullOrEmpty(value)) return value;
            if (DefaultValue == null) return initValue;
            return DefaultValue.ToString();
        }

        protected void SetCacheInput(string value)
        {
            value.SetCache(CacheInput);
        }

        protected async Task<bool> CheckScriptFocus()
        {
            try
            {
                return (string.IsNullOrEmpty(ScriptFocus)) || (bool)await GetExpressionAsync(ScriptFocus);
            }
            catch
            {
                return true;
            }
        }

        protected virtual void InitPropertyByField(FField f)
        {
            Name = f.Name;
            CacheName = f.CacheName;
            DefaultValue = f.DefaultValue;
            Title = f.Title;
            NotAllowsNull = f.AllowNulls;
            TextAlignment = f.TextAlignment;
            IsReadOnly = f.IsReadOnly;
            BaseReadOnly = f.IsReadOnly;
            Disable = f.Disable;
            HandleKey = f.HandleKey;
            HandleField = f.HandleField;
            RequestField = f.RequestField;
            RequestAction = f.RequestAction;
            ScriptReference = f.ScriptReference;
            ScriptDetail = f.ScriptDetail;
            ScriptCopy = f.ScriptCopy;
            ScriptFocus = f.ScriptFocus;
            ScripScanner = f.ScripScanner;
            if (!string.IsNullOrEmpty(f.TitleColor)) TitleColor = Color.FromHex(f.TitleColor);
            if (!string.IsNullOrEmpty(f.Color)) Color = Color.FromHex(f.Color);
            NoticeName = string.IsNullOrWhiteSpace(f.SubTitle) ? f.Title : f.SubTitle;
        }

        protected virtual void InitPropertyByField(FField f, object o)
        {
            InitPropertyByField(f);
        }

        #endregion Protected

        #region Event

        protected object GetExpression(string id, string input = null)
        {
            return Root.InvokeScript(id, ScriptDetail, input);
        }

        protected async Task<object> GetExpressionAsync(string id, string input = null)
        {
            return await Root.InvokeScriptAsync(id, ScriptDetail, input);
        }

        protected async void OnHandleFieldChange()
        {
            var handle = HandleKey;
            HandleField.ForEach(h =>
            {
                if (Root.Input.TryGetValue(h, out var value))
                {
                    handle = handle.Replace($"[{h}]", value.Output);
                    handle = FFunc.Compute(Root, handle).ToString();
                    if (bool.TryParse(handle, out bool result))
                    {
                        if (!result) Clear(!Root.IsOpening);
                        IsReadOnly = result && BaseReadOnly;
                    }
                }
            });
            await Task.CompletedTask;
        }

        protected async void OnRequestFieldComplete()
        {
            RequestStarted?.Invoke(this, EventArgs.Empty);
            await FServices.ForAllAsync(FFunc.GetArrayString(RequestAction), (x) => Root.Request(x, RequestField));
            RequestEnded?.Invoke(this, EventArgs.Empty);
        }

        #endregion Event

        #region SetEvent

        protected async void SetScriptChangeEvent()
        {
            ScriptReference.ForEach(id => CompleteValue += (s, e) => { if (ScriptReference.Contains(id)) _ = GetExpression(id); });
            await Task.CompletedTask;
        }

        protected async void SetHandleChangeEvent()
        {
            foreach (var name in HandleField) if (Root.Input.TryGetValue(name, out var value)) value.ChangeValue += (s, e) => { OnHandleFieldChange(); };
            await Task.CompletedTask;
        }

        protected async void SetRequestCompletedEvent()
        {
            if (!RequestAction.Equals("")) CompleteValue += (s, e) => { OnRequestFieldComplete(); };
            await Task.CompletedTask;
        }

        #endregion SetEvent

        #region Public

        public void Rendering()
        {
            var check = ScripScanner != null && ScripScanner.Count > 0;
            var titleLabel = new Label();
            var titleText = new Span();
            var isRequired = new Span();
            var textAnnotation = new Label();
            var content = SetContentView();

            titleText.FontSize = FSetting.FontSizeLabelHint;
            titleText.FontFamily = FSetting.FontText;
            titleText.SetBinding(Span.TextProperty, TitleProperty.PropertyName);
            titleText.SetBinding(Span.TextColorProperty, TitleColorProperty.PropertyName);

            isRequired.FontSize = FSetting.FontSizeLabelHint;
            isRequired.TextColor = FSetting.DangerColor;
            isRequired.FontFamily = FSetting.FontText;
            isRequired.SetBinding(Span.TextProperty, NotAllowsNullProperty.PropertyName, converter: new FBoolToObject(" * ", ""));

            titleLabel.FormattedText = new FormattedString { Spans = { titleText, isRequired } };
            titleLabel.SetBinding(Label.IsVisibleProperty, IsShowTitleProperty.PropertyName);
            titleLabel.SetBinding(Label.BackgroundColorProperty, TitleBackgroundColorProperty.PropertyName);

            content.SetBinding(View.HeightRequestProperty, HeightInputProperty.PropertyName);

            textAnnotation.FontSize = FSetting.FontSizeLabelContent - 1;
            textAnnotation.FontFamily = FSetting.FontTextItalic;
            textAnnotation.VerticalTextAlignment = TextAlignment.Center;
            textAnnotation.MaxLines = 1;
            textAnnotation.LineBreakMode = LineBreakMode.TailTruncation;
            textAnnotation.Padding = new Thickness(0, 0, 0, PaddingSize);
            textAnnotation.SetBinding(Label.TextProperty, AnnotationProperty.PropertyName);
            textAnnotation.SetBinding(Label.TextColorProperty, AnnotationColorProperty.PropertyName);
            textAnnotation.SetBinding(View.IsVisibleProperty, IsShowAnnotationProperty.PropertyName);

            var col1 = new ColumnDefinition { BindingContext = this };
            var col2 = new ColumnDefinition { BindingContext = this };
            col1.SetBinding(ColumnDefinition.WidthProperty, HasPaddingProperty.PropertyName, converter: new FBoolToFuncObject(() => this.PaddingSize, () => 0));
            col2.SetBinding(ColumnDefinition.WidthProperty, HasPaddingProperty.PropertyName, converter: new FBoolToFuncObject(() => this.PaddingSize, () => 0));

            ColumnDefinitions.Add(col1);
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            ColumnDefinitions.Add(col2);

            RowDefinitions.Add(new RowDefinition { BindingContext = this });
            RowDefinitions.Add(new RowDefinition { BindingContext = this });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RowDefinitions[0].SetBinding(RowDefinition.HeightProperty, IsShowTitleProperty.PropertyName, converter: new FBoolToObject(PaddingSize, 0));
            RowDefinitions[1].SetBinding(RowDefinition.HeightProperty, TitleHeightProperty.PropertyName, converter: new FDoubleToGridLength());

            ColumnSpacing = RowSpacing = 0;
            Children.Add(titleLabel, 1, 1);
            Children.Add(content, 1, 2);
            Children.Add(textAnnotation, 1, 3);
            if (check)
            {
                ColumnDefinitions.Insert(2, new ColumnDefinition { Width = GridLength.Auto });
                var sc = new FScanner(40, 40, FIcons.QrcodeScan.ToFontImageSource(FSetting.DisableColor, FSetting.SizeIconButton));
                sc.OnFscannerCommpleted += OnScannedValue;
                Children.Add(sc, 2, 2);
                SetColumnSpan(titleLabel, 2);
            }
            this.SetBinding(AnnotationColorProperty, IsValidValueProperty.PropertyName, converter: new FBoolToObject(FSetting.TextColorContent, FSetting.DangerColor));
        }

        public object InvokeScripts(string id)
        {
            return GetExpression(id);
        }

        public async Task<object> InvokeScriptsAsync(string id)
        {
            return await GetExpressionAsync(id);
        }

        public void Completed()
        {
            this.OnCompleteValue(this, EventArgs.Empty);
        }

        public void Handle()
        {
            OnHandleFieldChange();
        }

        public virtual void InitValue(bool isRefresh = true)
        {
            if (isRefresh)
            {
                SetHandleChangeEvent();
                SetRequestCompletedEvent();
                SetScriptChangeEvent();
            }
        }

        public virtual void Clear(bool isCompleted = false)
        {
            if (isCompleted) OnCompleteValue(this, new FInputChangeValueEventArgs(null));
        }

        public void SetInput(object value, bool isCompleted = false, bool isDisable = false)
        {
            var values = FFunc.GetArrayString(value?.ToString(), Seperate, false);
            if (values.Count == 0) values.Add(string.Empty);
            SetInput(values, isCompleted, isDisable);
        }

        public object GetInput(int mode)
        {
            return mode == 0 || IsValidValue ? ReturnValue(mode) : IsValidData;
        }

        public virtual void FocusInput()
        {
            try
            {
                _ = Root.InputView.ScrollToAsync(this, ScrollToPosition.Center, true);
            }
            catch
            {
                _ = Root.InputView.ScrollToAsync(Expander.View, ScrollToPosition.Center, true);
            }
        }

        public virtual void UnFocusInput()
        {
        }

        public virtual void AddScript(IEnumerable<string> scripts)
        {
            scripts.ForEach(x =>
            {
                if (!ScriptReference.Contains(x))
                {
                    ScriptReference.Add(x);
                    CompleteValue += (s, e) => { if (ScriptReference.Contains(x)) _ = GetExpression(x); };
                }
            });
        }

        public virtual void RemoveScript(IEnumerable<string> scripts)
        {
            scripts.ForEach(x => ScriptReference.Remove(x));
        }

        public virtual bool IsEqual(object oldValue)
        {
            return true;
        }

        public static bool IsRootParam(string param)
        {
            return param.Contains("$");
        }

        public static string RootParam(string param)
        {
            return param.Remove("$");
        }

        #endregion Public
    }
}