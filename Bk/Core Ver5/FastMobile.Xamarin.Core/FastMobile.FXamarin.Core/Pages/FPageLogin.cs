using System;
using System.Data;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace FastMobile.FXamarin.Core
{
    public partial class FPageLogin : FPage, IFGradientBackground
    {
        public static readonly BindableProperty StartColorProperty = BindableProperty.Create("StartColor", typeof(Color), typeof(FPageLogin), Color.Default);
        public static readonly BindableProperty EndColorProperty = BindableProperty.Create("EndColor", typeof(Color), typeof(FPageLogin), Color.Default);

        public Color StartColor
        {
            get => (Color)GetValue(StartColorProperty);
            set => SetValue(StartColorProperty, value);
        }

        public Color EndColor
        {
            get => (Color)GetValue(EndColorProperty);
            set => SetValue(EndColorProperty, value);
        }

        public readonly FLoginForm Form;
        public readonly Button LanguageButton, ProfileButton;

        public event EventHandler<EventArgs> LanguageChanging;

        public event EventHandler<EventArgs> LanguageChanged;

        public event EventHandler<EventArgs> ProfileButtonClicked;

        private readonly GradientStop Start, End;
        private readonly StackLayout B;
        private readonly Grid G;

        public FPageLogin(bool IsHasPullToRefresh, bool enalbleScrolling = false, bool hasBio = true) : base(IsHasPullToRefresh, enalbleScrolling)
        {
            FNavigationPage.SetHasNavigationBar(this, false);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
            StartColor = FSetting.StartColor;
            EndColor = FSetting.EndColor;
            Form = new FLoginForm(hasBio);
            B = new StackLayout();
            LanguageButton = new Button();
            ProfileButton = new Button();
            Start = new GradientStop();
            End = new GradientStop();
            G = new Grid();
            Content = G;
        }

        public static void SetUserInfo(DataTable data, string password)
        {
            FSetting.NetWorkKey = data.Rows[0]["network_key"].ToString().AESDecrypt(password.MD5());
            FString.UserID = data.Rows[0]["user_id"].ToString();
            FString.Admin = Convert.ToBoolean(data.Rows[0]["admin"]);
            FString.Comment = data.Rows[0]["comment"].ToString().Trim();
            FString.Comment2 = data.Rows[0]["comment2"].ToString().Trim();
            FString.DLanguage = data.Rows[0]["dlanguage"].ToString();
            FString.TimeOut = Convert.ToInt32(data.Rows[0]["dv_timeout"]);
            FServices.TicketExpireTime = DateTime.Now;
            if (data.Columns.Contains("expired"))
                FOptions.TicketMinute = Convert.ToInt32(data.Rows[0]["expired"]);
        }

        public override void Init()
        {
            base.Init();
            InitGradient();
            InitFormLogin();
            InitLanguage();
            InitProfileLabel();
            InitBottom();
            InitContent();
            Update();
        }

        public override FPage Init(bool setInit)
        {
            InitGradient();
            InitFormLogin();
            InitLanguage();
            InitProfileLabel();
            InitBottom();
            InitContent();
            Update();
            return base.Init(setInit);
        }

        public void FocusToPassword()
        {
            Form.Focus();
            Form.FocusPassword();
        }

        public void FocusToUsername()
        {
            Form.Focus();
            Form.FocusUsername();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == StartColorProperty.PropertyName || propertyName == EndColorProperty.PropertyName)
            {
                FInterface.IFAndroid?.SetCurentWindowBackground(StartColor, EndColor);
                return;
            }
        }

        public static void InitFirebase(string token, DataSet user)
        {
            if (string.IsNullOrEmpty(token) || FInterface.IFFirebase == null)
                return;

            "FastMobile.FXamarin.Core.FPageLogin.Firebase.TopicSubscribed".GetCache()?.Split((char)254)?.ForEach((x) => FInterface.IFFirebase?.UnSubscribe(x));

            if (user.Last().Columns.Contains("fcm_topics") && user.Last().Rows.Count > 0)
            {
                string listKey = "";
                user.Last().Rows.ForEach<DataRow>((x) =>
                {
                    listKey += $"{x["fcm_topics"].ToString().Trim()}{(char)254}";
                    if (FString.ServiceInternal == "1")
                        FInterface.IFFirebase?.Subscribe(x["fcm_topics"].ToString().Trim());
                });
                listKey.SetCache("FastMobile.FXamarin.Core.FPageLogin.Firebase.TopicSubscribed");
            }

            FInterface.IFFirebase?.Subscribe("fcmall");
            if (FString.ServiceInternal == "1")
            {
                FInterface.IFFirebase?.Subscribe(FSetting.IsAndroid ? "fcmandroid" : "fcmios");
                return;
            }
            FInterface.IFFirebase?.UnSubscribe("fcmandroid");
            FInterface.IFFirebase?.UnSubscribe("fcmios");
        }

        protected virtual void OnLanguageChanging(object sender, EventArgs e)
        {
            LanguageChanging?.Invoke(sender, e);
        }

        protected virtual void OnLanguageChanged(object sender, EventArgs e)
        {
            LanguageChanged?.Invoke(sender, e);
        }

        protected virtual void OnProfileButtonClicked(object sender, EventArgs e)
        {
            ProfileButtonClicked?.Invoke(sender, e);
        }

        private void InitContent()
        {
            G.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            G.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            G.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            G.Children.Add(Form, 0, 0);
            G.Children.Add(B, 0, 1);
        }

        private void InitFormLogin()
        {
            Form.HorizontalOptions = Form.VerticalOptions = LayoutOptions.CenterAndExpand;
            Form.WidthRequest = FSetting.ScreenWidth * 0.85;
        }

        private void InitGradient()
        {
            Start.BindingContext = End.BindingContext = G.BindingContext = this;
            Start.Offset = 0.0f;
            End.Offset = 1.0f;

            Start.SetBinding(GradientStop.ColorProperty, StartColorProperty.PropertyName);
            End.SetBinding(GradientStop.ColorProperty, EndColorProperty.PropertyName);

            BackgroundColor = Color.Transparent;
            Background = new LinearGradientBrush() { StartPoint = new Point(0, 0.5), EndPoint = new Point(1, 0.5), GradientStops = new GradientStopCollection { Start, End } };
        }

        private void InitProfileLabel()
        {
            ProfileButton.Text = string.IsNullOrEmpty(FString.ServiceName) ? FText.ProfileDeclare : FString.ServiceName;
            ProfileButton.Clicked += OnProfileButtonClicked;
            ProfileButton.TextColor = Color.White;
            ProfileButton.HorizontalOptions = LayoutOptions.EndAndExpand;
            SetButtonProperty(ProfileButton);
        }

        private void InitLanguage()
        {
            LanguageButton.Clicked += ChangedLanguage;
            LanguageButton.HorizontalOptions = LayoutOptions.Center;
            LanguageButton.TextColor = Color.White;
            SetButtonProperty(LanguageButton);
        }

        private void InitBottom()
        {
            B.Margin = FSetting.IsAndroid ? LanguageButton.Margin : new Thickness(0, 0, 0, 15);
            B.HorizontalOptions = LayoutOptions.Fill;
            B.Orientation = StackOrientation.Horizontal;
            B.Children.Add(LanguageButton);
            B.Children.Add(ProfileButton);
        }

        private void SetButtonProperty(Button btn)
        {
            btn.Padding = new Thickness(20);
            btn.BorderColor = Color.Transparent;
            btn.BackgroundColor = Color.Transparent;
            btn.VerticalOptions = LayoutOptions.Center;
            btn.FontSize = FSetting.FontSizeLabelContent;
            btn.FontFamily = FSetting.FontText;
        }

        private void ChangedLanguage(object sender, EventArgs e)
        {
            OnLanguageChanging(this, new EventArgs());
            FSetting.InternalLanguage = FSetting.V ? "E" : "V";
            Update();
            OnLanguageChanged(this, new EventArgs());
        }

        private void Update()
        {
            LanguageButton.Text = FText.AttributeString(FSetting.V ? "E" : "V", "Language");
            Form.UsernameInput.Hint = FText.UserNameHint;
            Form.PasswordInput.Hint = FText.PasswordHint;
            Form.Submiter.Text = FText.Login;
            Form.Recoverier.Text = FText.ForgotPassword;
        }
    }
}