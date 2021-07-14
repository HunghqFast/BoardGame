using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace FastMobile.FXamarin.Core
{
    public class FInputLocation : FInput
    {
        private ImageButton I;

        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(string), typeof(FInputLocation), string.Empty, BindingMode.TwoWay);
        public string Value { get => (string)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        public IFLocationPage LocationPage { get; private set; }
        public IFLocation Location { get; private set; }
        public bool HasAdress { get; set; }
        public bool HasNear { get; set; }
        public string MoveMode { get; set; }
        public string ApiKey { get; set; }

        private Label E;
        private StackLayout S;

        #region Public

        public FInputLocation() : base()
        {
            Type = FieldType.String;
        }

        public FInputLocation(FField field) : base(field)
        {
            Type = FieldType.String;
        }

        public override void InitValue(bool isRefresh = true)
        {
            base.InitValue(isRefresh);
            if (DefaultValue != null)
            {
                Value = (string)DefaultValue;
                UpdateAnnotation();
            }
        }

        public override void Clear(bool isCompleted = false)
        {
            base.Clear(isCompleted);
            InitValue(false);
        }

        public override bool IsEqual(object oldValue)
        {
            return oldValue.Equals(ReturnValue(0));
        }

        #endregion Public

        #region Protected

        protected override void Init()
        {
            I = new ImageButton();
            E = new Label();
            S = new StackLayout();

            Location = DependencyService.Resolve<IFLocation>(DependencyFetchTarget.NewInstance);
            if (Location is BindableObject location)
                location.PropertyChanged += OnLocationPropertyChanged;
            base.Init();
        }

        protected override View SetContentView()
        {
            E.MaxLines = 1;
            E.LineBreakMode = LineBreakMode.TailTruncation;
            E.FontSize = FSetting.FontSizeLabelContent;
            E.FontFamily = FSetting.FontText;
            E.HorizontalOptions = LayoutOptions.FillAndExpand;
            E.VerticalOptions = LayoutOptions.EndAndExpand;
            E.SetBinding(Label.TextProperty, ValueProperty.PropertyName, BindingMode.TwoWay);
            E.SetBinding(Label.TextColorProperty, ColorProperty.PropertyName);

            I.Margin = new Thickness(1.5, 0);
            I.WidthRequest = I.HeightRequest = 25;
            I.BackgroundColor = Color.Transparent;
            I.VerticalOptions = LayoutOptions.CenterAndExpand;
            I.HorizontalOptions = LayoutOptions.End;
            I.Source = FIcons.MapMarkerRadiusOutline.ToFontImageSource(FSetting.PrimaryColor, FSetting.SizeIconButton);
            I.SetBinding(Image.IsVisibleProperty, IsModifierPropertyName);
            I.Clicked += OpenLocationPage;

            S.Spacing = 0;
            S.Orientation = StackOrientation.Horizontal;
            S.HorizontalOptions = LayoutOptions.Fill;
            S.Children.Add(E);
            S.Children.Add(I);
            return S;
        }

        protected virtual async void OpenLocationPage(object sender, EventArgs e)
        {
            if (!Location.IsGpsEnable())
            {
                MessagingCenter.Send(new FMessageConfirm(OpenLocationSetting, 1, 1300, ""), FChannel.ALERT_BY_MESSAGE);
                return;
            }

            if (FSetting.IsAndroid && !await FPermissions.HasPermission<Permissions.LocationWhenInUse>(true))
            {
                FPermissions.ShowMessage();
                return;
            }

            if (!FUtility.HasNetwork)
            {
                MessagingCenter.Send(FMessage.FromFail(403), FChannel.ALERT_BY_MESSAGE);
                return;
            }

            await Root.SetBusy(true);

            if (LocationPage == null)
            {
                LocationPage = new FPageLocation() { Title = Title };
                LocationPage.Control.ApiKey = ApiKey;
                LocationPage.Control.HasAdress = HasAdress;
                LocationPage.Control.HasNear = HasNear;
                LocationPage.Location = Location;
                LocationPage.ItemClicked += OnLocationPageItemClicked;
                LocationPage.Refresh.Clicked += OnRefresh;
            }

            if (!Location.IsUpdating)
                await Updating(false);

            if (Navigation.NavigationStack.Contains(LocationPage as Page))
            {
                await Root.SetBusy(false);
                return;
            }

            await Navigation.PushAsync(LocationPage as Page);
            await Root.SetBusy(false);
        }

        protected override object ReturnValue(int mode)
        {
            return Value;
        }

        protected override void SetInput(List<string> value, bool isCompleted = false, bool isDisable = false)
        {
            if (Disable && isDisable) return;
            Value = value[0];
            UpdateAnnotation();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == AnnotationProperty.PropertyName)
            {
                IsShowAnnotation = !string.IsNullOrWhiteSpace(Annotation);
                S.HeightRequest = IsShowAnnotation ? FSetting.FilterInputHeight - FSetting.FontSizeLabelContent : FSetting.FilterInputHeight;
                return;
            }
        }

        protected override void InitPropertyByField(FField f)
        {
            MoveMode = f.ItemMoveMode;
            ApiKey = f.ApiKey;
            HasAdress = f.ItemAdress;
            HasNear = f.ItemNear;
            base.InitPropertyByField(f);
        }

        #endregion Protected

        #region Private

        private async void OnLocationPageItemClicked(object sender, FObjectPropertyArgs<FPlace> e)
        {
            if (!Location.IsGpsEnable())
            {
                MessagingCenter.Send(new FMessageConfirm(OpenLocationSetting, 1, 1300, ""), FChannel.ALERT_BY_MESSAGE);
                return;
            }

            UpdateText(e.Value.Position, false);
            Annotation = LocationPage.Places.Count > 0 && e.Value == LocationPage.Places[0] ? e.Value.PlaceName : e.Value.Address;
            if (FPageLookup.AutoComplete)
                await Navigation.PopAsync();
            LocationPage.CloseList();
        }

        private async void OnCurrentLocationChanged(object sender, FObjectPropertyArgs<(Position Position, double Accuracy)> e)
        {
            if (e.Value == default)
            {
                Value = string.Empty;
                Annotation = string.Empty;
                LocationPage.Places = new ObservableCollection<FPlace>();
                LocationPage.Control.PinsPlaces = new ObservableCollection<FPlace>();
                return;
            }
            await UpdateNearPlace(e.Value.Position, e.Value.Accuracy, true);
        }

        private async void OnRefresh(object sender, EventArgs e)
        {
            await Updating(true);
        }

        private async Task Updating(bool check)
        {
            if (check)
            {
                if (!Location.IsGpsEnable())
                {
                    MessagingCenter.Send(new FMessageConfirm(OpenLocationSetting, 1, 1300, ""), FChannel.ALERT_BY_MESSAGE);
                    return;
                }

                if (FSetting.IsAndroid && !await FPermissions.HasPermission<Permissions.LocationWhenInUse>(true))
                {
                    FPermissions.ShowMessage();
                    return;
                }

                if (!FUtility.HasNetwork)
                {
                    MessagingCenter.Send(FMessage.FromFail(403), FChannel.ALERT_BY_MESSAGE);
                    return;
                }
            }

            var position = await Location.GetCurrentPosition(FLocationAccuracy.Default, FUtility.TimeoutToken(CancellationToken.None, TimeSpan.FromSeconds(30)), !FSetting.IsAndroid);
            if (position == default)
            {
                FPermissions.ShowMessage();
                await Root.SetBusy(false);
                return;
            }
            await UpdateNearPlace(position.Position, position.Accuracy, false);

            Location.Updated = Updated;
            await Location.StartUpdating(FLocationAccuracy.Default, FUtility.TimeoutToken(CancellationToken.None, TimeSpan.FromSeconds(30)));
            void Updated(bool ok)
            {
                if (ok)
                {
                    Location.LocationChanged -= OnCurrentLocationChanged;
                    Location.LocationChanged += OnCurrentLocationChanged;
                }
            }
        }

        private async Task UpdateNearPlace(Position position, double accuracy, bool setValue)
        {
            accuracy = accuracy <= 0 ? 1 : accuracy;
            var anno = HasAdress ? await PositionAddress(position) : string.Format(FText.LocationAnnotation, Convert.ToInt32(accuracy).ToString());
            var (places, _) = await LocationPage.Control.GetNearPlaces(position, 100);
            places.Insert(0, LocationPage.Control.NewCurrentPlace(position, anno));
            LocationPage.Places = places;
            LocationPage.Control.PinsPlaces = new ObservableCollection<FPlace>
            {
                LocationPage.Control.NewCurrentPlace(position, anno)
            };
            LocationPage.Control.GoTo(position, new Distance(100));
            if (setValue) OnMoveMode(position);
        }

        private void UpdateText(Position position, bool getDetail)
        {
            if (position == default)
            {
                Annotation = string.Empty;
                if (string.IsNullOrWhiteSpace(Value))
                    return;
                Value = string.Empty;
                OnCompleteValue(this, new FInputChangeValueEventArgs(Value));
                return;
            }

            Value = FUtility.ToString(position);
            OnCompleteValue(this, new FInputChangeValueEventArgs(Value));
            if (getDetail) UpdateAnnotation();
        }

        private async void UpdateAnnotation()
        {
            Annotation = await PositionAddress(FUtility.ToPosition(Value));
        }

        private async Task<string> PositionAddress(Position position)
        {
            if (position == default)
                return string.Empty;
            if (!HasAdress) return FText.LocationCurrent;

            var message = await FMapService.Instance.PlaceDetail(ApiKey, position.Latitude, position.Longitude, FMapService.Instance.LanguageText(FSetting.V), true);
            if (message.OK.Success != 1 || message.results.Length == 0)
                return FText.LocationUnknown;

            return message.results[0].formatted_address;
        }

        private void OnLocationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsUpdating" && LocationPage != null)
            {
                if (Location.IsUpdating)
                {
                    LocationPage.ToolbarItems.Remove(LocationPage.Refresh);
                    return;
                }
                LocationPage.ToolbarItems.Insert(0, LocationPage.Refresh);
                return;
            }
        }

        private void OnMoveMode(Position position)
        {
            switch (MoveMode)
            {
                case "Update":
                    UpdateText(position, true);
                    break;

                case "Clear":
                    UpdateText(default, false);
                    break;

                default:
                    break;
            }
        }

        private void OpenLocationSetting(bool value)
        {
            if (value) Location.OpenSetting();
        }
        #endregion
    }
}