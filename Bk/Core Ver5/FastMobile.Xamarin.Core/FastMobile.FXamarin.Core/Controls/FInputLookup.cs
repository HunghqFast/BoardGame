using Syncfusion.XForms.ComboBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using SelectionChangedEventArgs = Syncfusion.XForms.ComboBox.SelectionChangedEventArgs;

namespace FastMobile.FXamarin.Core
{
    public class FInputLookup : FInput
    {
        private bool isWait;
        private bool isOpen;
        private bool isLookup;
        private bool isAllowUnfocus;
        private bool isFast;
        private DateTime time;

        protected FPageLookup L;
        protected FSfComboBox C;
        protected ImageButton I;
        protected FEntryBase E;

        public bool HasValid => IsModifier && (Root.FormType != FFormType.Filter || NotAllowsNull || OnDemand);

        public const int FilterDelayTime = 1000;
        public const int DropDownHeight = 200;
        public const int RequestDelayTime = 50;
        public const int RequestWaitTime = 1000;
        public const string IsValidLookupPropertyName = "IsValidLookup";

        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(FItem), typeof(FInputLookup), FItem.Empty);
        public static readonly BindableProperty ListInputProperty = BindableProperty.Create("ListInput", typeof(List<string>), typeof(FInputLookup), new List<string>());
        public static readonly BindableProperty HasAnnotationProperty = BindableProperty.Create("HasAnnotation", typeof(bool), typeof(FInputLookup), true);
        public static readonly BindableProperty OnDemandProperty = BindableProperty.Create("OnDemand", typeof(bool), typeof(FInputLookup), false);
        public static readonly BindableProperty FilterCharacterProperty = BindableProperty.Create("FilterCharacter", typeof(int), typeof(FInputLookup));
        public static readonly BindableProperty SuggestionProperty = BindableProperty.Create("Suggestion", typeof(ObservableCollection<FItem>), typeof(FInputLookup));
        public static readonly BindableProperty NormalProperty = BindableProperty.Create("Normal", typeof(bool), typeof(FInputLookup));

        public List<string> Reference { get; set; }

        public bool IsCommpleted { get; set; }

        public string TargetName { get; set; }

        public bool AllowFilter { get; set; }

        public FItem Value { get => (FItem)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        public int FilterCharacter { get => (int)GetValue(FilterCharacterProperty); set => SetValue(FilterCharacterProperty, value); }

        public bool HasAnnotation { get => (bool)GetValue(HasAnnotationProperty); set => SetValue(HasAnnotationProperty, value); }

        public bool OnDemand { get => (bool)GetValue(OnDemandProperty); set => SetValue(OnDemandProperty, value); }

        public List<string> ListInput { get => (List<string>)GetValue(ListInputProperty); set => SetValue(ListInputProperty, value); }

        public ObservableCollection<FItem> Suggestion { get => (ObservableCollection<FItem>)GetValue(SuggestionProperty); set => SetValue(SuggestionProperty, value); }

        public bool Normal { get => (bool)GetValue(NormalProperty); set => SetValue(NormalProperty, value); }

        public bool IsValidLookup => IsValidValue || !HasValid;

        public override string Output => $"'{Value.I}'";

        #region Protected

        protected async void OpenLookup(object sender, EventArgs e)
        {
            if (IsModifier && await CheckScriptFocus())
            {
                lock (sender) { if (isOpen) return; isOpen = true; }
                if (Navigation.NavigationStack.Contains(L)) return;
                await Root.SetBusy(true);
                if (isLookup) L.RefreshView();
                else
                {
                    isLookup = true;
                    await L.ResetSelected(string.Empty);
                }
                await Navigation.PushAsync(L, true);
                await Root.SetBusy(false);
                isOpen = false;
            }
        }

        protected void ClearValue(bool isCompleted = false)
        {
            if (!IsModifier) return;
            Value = FItem.Empty;
            IsValidValue = true;
            if (isCompleted) _ = L.ResetSelected();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(Value):
                    E.TextChanged -= ValueChangedInput;
                    E.Text = Value.I;
                    E.TextChanged += ValueChangedInput;
                    Annotation = Value.V;
                    break;

                case nameof(Annotation):
                    IsShowAnnotation = HasAnnotation && Annotation.TrimEnd().Length != 0;
                    HeightInput = IsShowAnnotation ? FSetting.FilterInputHeight - FSetting.FontSizeLabelContent : FSetting.FilterInputHeight;
                    break;

                case nameof(IsModifier):
                    if (!IsModifier) IsValidValue = true;
                    break;

                case nameof(IsValidValue):
                    if (!IsValidValue) Annotation = string.Empty;
                    OnPropertyChanged(IsValidLookupPropertyName);
                    break;

                default:
                    break;
            }
        }

        protected override void OnChangeValue(object sender, EventArgs e)
        {
            base.OnChangeValue(sender, e);
        }

        protected override void OnCompleteValue(object sender, EventArgs e)
        {
            OnChangeValue(sender, new FInputChangeValueEventArgs(Value));
            base.OnCompleteValue(sender, new FInputChangeValueEventArgs(Value));
        }

        protected override object ReturnValue(int mode)
        {
            return mode == 0 ? Value.I : Value.I + Seperate + Value.V;
        }

        protected override void SetInput(List<string> value, bool isCompleted = false, bool isDisable = false)
        {
            if (Disable && isDisable) return;
            Value = new FItem(value[0], value.Count > 1 && HasAnnotation ? value[1] : string.Empty);
            isAllowUnfocus = false;
            base.SetInput(value, isCompleted);
        }

        protected override View SetContentView()
        {
            I.VerticalOptions = LayoutOptions.CenterAndExpand;
            I.WidthRequest = I.HeightRequest = 20;
            I.HorizontalOptions = LayoutOptions.EndAndExpand;
            I.Margin = new Thickness(1.5, 0);
            I.BackgroundColor = Color.Transparent;
            I.Source = FIcons.TableSearch.ToFontImageSource(FSetting.PrimaryColor, FSetting.SizeIconButton);
            I.Clicked += OpenLookup;
            I.SetBinding(IsVisibleProperty, IsModifierPropertyName);

            E.ReturnType = ReturnType.Search;
            E.HeightRequest = FSetting.FilterInputHeight - 5;
            E.SetBinding(InputView.IsReadOnlyProperty, IsModifierPropertyName, converter: new FInvertBool());
            E.SetBinding(InputView.TextColorProperty, IsValidLookupPropertyName, converter: new FBoolToObject(FSetting.TextColorContent, FSetting.DangerColor));
            E.SetBinding(InputView.KeyboardProperty, KeyboardProperty.PropertyName);

            C.ShowBorder = false;
            C.DropDownTextSize = FSetting.FontSizeLabelContent;
            C.TextSize = FSetting.FontSizeLabelContent;
            C.FontFamily = FSetting.FontText;
            C.DropDownItemHeight = 40;
            C.DropDownCornerRadius = 2;
            C.DropDownButtonSettings.Width = 0;
            C.MaximumDropDownHeight = 0;
            C.DisplayMemberPath = FItem.ItemID;
            C.MaximumSuggestion = 4;
            C.LoadMoreText = FText.ViewMore;

            if (L.LookupType == FLookupType.None && AllowFilter)
            {
                E.Unfocused += UnfocusedInput;
                E.Focused += FocusedInput;
                E.TextChanged += ValueChangedInput;

                C.MaximumDropDownHeight = DropDownHeight;
                C.SelectionChanged += SelectionChangedInput;
            }
            else
            {
                E.InputTransparent = true;
            }
            C.ItemTemplate = new DataTemplate(() =>
            {
                var g = new Grid();
                var i = new Label();
                var n = new Label();
                var h = new FLine();
                var v = new FLine();

                i.VerticalOptions = n.VerticalOptions = LayoutOptions.CenterAndExpand;
                i.VerticalTextAlignment = n.VerticalTextAlignment = TextAlignment.Center;
                i.FontSize = n.FontSize = FSetting.FontSizeLabelContent;
                i.TextColor = n.TextColor = FSetting.TextColorContent;
                i.FontFamily = n.FontFamily = FSetting.FontText;
                i.LineBreakMode = n.LineBreakMode = LineBreakMode.TailTruncation;
                i.Margin = n.Margin = 10;

                i.SetBinding(Label.TextProperty, FItem.ItemID);
                n.SetBinding(Label.TextProperty, FItem.ItemValue, converter: new FStringNoNullConvert());

                g.RowSpacing = g.RowSpacing = 0;
                g.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                g.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = 120 });
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = 1 });
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                g.Children.Add(h, 0, 0);
                g.Children.Add(i, 0, 1);
                g.Children.Add(v, 1, 1);
                g.Children.Add(n, 2, 1);
                SetColumnSpan(h, 3);
                return g;
            });
            C.SetBinding(SfComboBox.DropDownWidthProperty, WidthProperty.PropertyName, converter: new FWidthDropdownConvert());
            C.SetBinding(SfComboBox.DataSourceProperty, SuggestionProperty.PropertyName);

            var g = new Grid { ColumnSpacing = 0 };
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = 0 });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            g.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            g.Children.Add(C, 0, 0);
            g.Children.Add(E, 1, 0);
            g.Children.Add(I, 2, 0);
            return g;
        }

        protected override void InitPropertyByField(FField f)
        {
            ListInput = f.ItemInput;
            Normal = f.ItemNormal;
            TargetName = string.IsNullOrEmpty(f.ItemTargetName.Trim()) ? f.Name : f.ItemTargetName;
            base.InitPropertyByField(f);
        }

        protected override void InitPropertyByField(FField f, object o)
        {
            if ((bool)o)
            {
                FilterCharacter = f.FilterCharacter;
                OnDemand = f.OnDemand;
                HasAnnotation = f.ItemAnnotation;
                AllowFilter = f.AllowFilter;
            }
            else
            {
                AllowFilter = false;
            }
            base.InitPropertyByField(f, o);
        }

        #endregion Protected

        #region Public

        public FInputLookup(string controller, string value, string reference, FLookupType type, FFormTarget target) : base()
        {
            Base(controller, value, reference, type, target);
        }

        public FInputLookup(FField field, string controller, FLookupType type, FFormTarget target) : base(field, type == FLookupType.None)
        {
            Base(controller, field.Name, field.ItemReference, type, target);
        }

        public override void InitValue(bool isRefresh = true)
        {
            base.InitValue(isRefresh);
            if (string.IsNullOrEmpty(CacheName)) SetInput(DefaultValue ?? string.Empty);
            else
            {
                SetInput(GetCacheInput(string.Empty));
                if (isRefresh) CompleteValue += (s, e) => { SetCacheInput(ReturnValue(1).ToString()); };
            }
            foreach (var name in ListInput)
            {
                var root = IsRootParam(name) ? (Root.Root as FInputGrid).Root.Input : Root.Input;
                if (root.TryGetValue(RootParam(name), out var input))
                {
                    input.RequestStarted += InputRequestStarted;
                    input.CompleteValue += ListInputChanged;
                    input.RequestEnded += InputRequestEnded;
                }
            }
        }

        public override void Clear(bool isCompleted = false)
        {
            ClearValue(isCompleted);
            base.Clear(isCompleted);
        }

        public override void FocusInput()
        {
            base.FocusInput();
        }

        public override void UnFocusInput()
        {
            E.Unfocus();
            base.UnFocusInput();
        }

        public override bool IsEqual(object oldValue)
        {
            return Value.I.ClearString() == oldValue.ClearString();
        }

        public void SetReference(FData data)
        {
            if (L.LookupType != FLookupType.None || data == null) return;
            Reference.ForEach(key =>
            {
                if (Root.Input.ContainsKey(key)) Root.Input[key].SetInput(data[key], true);
            });
        }

        public void SetReference(DataRow data)
        {
            if (L.LookupType != FLookupType.None) return;
            Reference.ForEach(key =>
            {
                if (Root.Input.ContainsKey(key)) Root.Input[key].SetInput(data[Reference.IndexOf(key) + 1], true);
            });
        }

        public Task SetReference()
        {
            if (Reference.Count == 0) return Task.CompletedTask;
            if (Root.Input.TryGetValue(Reference[0], out FInput input))
            {
                var ann = input.GetInput(0).ToString();
                if (!string.IsNullOrWhiteSpace(ann)) Annotation = ann;
            }
            return Task.CompletedTask;
        }

        #endregion Public

        #region Private

        private void Base(string controller, string value, string reference, FLookupType type, FFormTarget target)
        {
            Type = FieldType.String;
            Reference = FFunc.GetArrayString(reference);
            C = new FSfComboBox();
            I = new ImageButton();
            E = new FEntryBase();
            L = new FPageLookup(controller, value, target);
            time = default;
            isAllowUnfocus = true;
            L.Root = this;
            L.TableData = 1;
            L.LookupType = type;
            L.Disappeared += LookupDisappeared;
            if (L.LookupType == FLookupType.None) L.SourceChanged += DataSourceChanged;
            Suggestion = new ObservableCollection<FItem>();
        }

        private async void ValueChangedInput(object sender, TextChangedEventArgs e)
        {
            time = DateTime.Now;
            isFast = true;
            isLookup = false;
            DropDownOff();
            await Task.Delay(FilterDelayTime);
            if (Compare(Value.I, e.NewTextValue) && !E.IsFocused)
            {
                isFast = false;
                return;
            }
            if (DateTime.Now <= time.AddMilliseconds(FilterDelayTime)) return;
            if (E.IsFocused && !CheckEmtyText())
            {
                await Root.SetBusy(true);
                time = DateTime.Now;
                await WaitCheckValue(E.Text.Length < FilterCharacter);
                DropDownUpdate();
                await Root.SetBusy(false);
            }
        }

        private async void FocusedInput(object sender, FocusEventArgs e)
        {
            if (await CheckScriptFocus())
            {
                isAllowUnfocus = true;
                return;
            }
            E.Unfocus();
        }

        private void UnfocusedInput(object sender, FocusEventArgs e)
        {
            var oldValue = Value;
            if (CheckEmtyText())
            {
                Value = FItem.Empty;
                IsValidValue = true;
                L.SelectedsItem.Clear();
                CheckCompleted(sender, oldValue);
                return;
            }
            if (isFast)
            {
                _ = WaitCheckValue(true);
                return;
            }
            if (!isAllowUnfocus) return;
            if (Compare(Value.I, E.Text))
            {
                if (!IsValidValue)
                {
                    _ = WaitCheckValue(true);
                    return;
                }
                if (!Normal) E.Text = Value.I.ToUpper();
                return;
            }
            if (IsDropDownSelected())
            {
                Value = C.SelectedItem as FItem;
                IsValidValue = true;
                CheckCompleted(sender, oldValue);
                return;
            }
            _ = WaitCheckValue(true);
            L.SelectedsItem.Clear();
            return;
        }

        private void SelectionChangedInput(object sender, SelectionChangedEventArgs e)
        {
            if (e.Value != null && !string.IsNullOrEmpty(e.Value.ToString()))
            {
                E.Unfocus();
            }
        }

        private void DataSourceChanged(object sender, EventArgs e)
        {
            FData result = null;
            var suggestion = new ObservableCollection<FItem>();
            L.DataSource.ForEach(x =>
            {
                if (Compare(x[TargetName], E.Text)) result = x;
                suggestion.Add(new FItem(x[TargetName], x[Reference[0]]));
            });
            if (FSetting.IsAndroid) C.SelectedItem = string.Empty;
            Suggestion = suggestion;
            IsValidValue = !IsModifier || string.IsNullOrWhiteSpace(E.Text) || (E.Text.ClearString() == Value.I.ClearString() && IsValidValue) || result != null;
        }

        private void LookupDisappeared(object sender, EventArgs e)
        {
            if (IsCommpleted) OnCompleteValue(this, EventArgs.Empty);
        }

        private async void ListInputChanged(object sender, EventArgs e)
        {
            if (CheckEmtyText() || !HasValid)
            {
                IsValidValue = true;
                isLookup = false;
                return;
            }
            IsValidValue = false;
            await WaitRequest();
            await UpdateValidInput(Value.I);
            isLookup = false;
        }

        private async Task UpdateValidInput(string text)
        {
            if (await L.CheckExist(text))
            {
                IsValidValue = true;
                OnCompleteValue(this, EventArgs.Empty);
            }
            else IsValidValue = false;
        }

        private async Task WaitRequest()
        {
            var time = DateTime.Now;
            while (isWait || Root.IsBusy)
            {
                await Task.Delay(RequestDelayTime);
                if (DateTime.Now >= time.AddMilliseconds(RequestWaitTime)) return;
            }
        }

        private async Task WaitCheckValue(bool check)
        {
            if (check) await UpdateValidInput(E.Text.Trim());
            else await L.ResetSelected(E.Text.Trim());
            isAllowUnfocus = !check;
            isFast = false;
        }

        private void InputRequestStarted(object sender, EventArgs e)
        {
            isWait = true;
        }

        private void InputRequestEnded(object sender, EventArgs e)
        {
            isWait = false;
        }

        private void DropDownOff()
        {
            C.MaximumDropDownHeight = 0;
            if (C.IsDropDownOpen) C.IsDropDownOpen = false;
        }

        private void DropDownUpdate()
        {
            if (C.IsDropDownOpen) C.IsDropDownOpen = false;
            C.MaximumDropDownHeight = E.Text.Length >= FilterCharacter ? Suggestion.Count > 4 ? DropDownHeight : 40 * Suggestion.Count : 0;
            if (C.MaximumDropDownHeight > 0) C.IsDropDownOpen = true;
        }

        private bool Compare(object a, object b)
        {
            return a.ClearString() == b.ClearString();
        }

        private bool CheckEmtyText()
        {
            return E.Text.TrimEnd().Length == 0;
        }

        private bool IsDropDownSelected()
        {
            return C.SelectedItem != null && !string.IsNullOrEmpty(C.SelectedItem.ToString());
        }

        private void CheckCompleted(object sender, FItem oldValue)
        {
            OnPropertyChanged(ValueProperty.PropertyName);
            if (!Compare(oldValue.I, Value.I) && IsValidValue)
            {
                OnCompleteValue(sender, EventArgs.Empty);
            }
            isAllowUnfocus = false;
            isFast = false;
        }

        #endregion Private
    }
}