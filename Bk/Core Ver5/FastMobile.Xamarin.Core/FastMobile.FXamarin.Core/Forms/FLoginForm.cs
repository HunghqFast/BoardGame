using Syncfusion.XForms.Border;
using Syncfusion.XForms.Buttons;
using System;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FLoginForm : Grid
    {
        public readonly Image Logo;
        public readonly FEntry UsernameInput, PasswordInput;
        public readonly SfButton Submiter;
        public readonly FLink Recoverier;
        public readonly ImageButton Biometric;
        public readonly SfBorder BioParent;
        public readonly StackLayout ViewButtons;
        private readonly bool HasBio;

        public FLoginForm(bool hasBio)
        {
            HasBio = hasBio;
            Logo = new Image();
            UsernameInput = new FEntry();
            PasswordInput = new FEntry();
            Submiter = new SfButton();
            Recoverier = new FLink();
            Biometric = new ImageButton();
            BioParent = new SfBorder { Content = Biometric };
            ViewButtons = new StackLayout();
            Init();
        }

        public void FocusPassword()
        {
            PasswordInput.InputView.Focus();
        }

        public void FocusUsername()
        {
            UsernameInput.InputView.Focus();
        }

        public void SaveInfo()
        {
            FString.Username = UsernameInput.InputView.Text;
        }

        private void Init()
        {
            InitLogo();
            InitUsername();
            InitPassword();
            InitRecoveryButton();
            InitSubmit();
            InitContent();
            if (HasBio) InitBiometricButton();
        }

        private void InitContent()
        {
            ViewButtons.Orientation = StackOrientation.Horizontal;
            ViewButtons.HorizontalOptions = LayoutOptions.Fill;
            ViewButtons.Spacing = 10;

            RowSpacing = 0;
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            Children.Add(Logo, 0, 0);
            Children.Add(UsernameInput, 0, 1);
            Children.Add(PasswordInput, 0, 2);
            if (HasBio)
            {
                ViewButtons.Children.Add(Submiter);
                ViewButtons.Children.Add(BioParent);
                Children.Add(ViewButtons, 0, 3);
            }
            else Children.Add(Submiter, 0, 3);
            Children.Add(Recoverier, 0, 4);
        }

        private void InitLogo()
        {
            Logo.BindingContext = this;
            Logo.Margin = new Thickness(0, 0, 0, 20);
            Logo.HorizontalOptions = LayoutOptions.CenterAndExpand;
            Logo.VerticalOptions = LayoutOptions.Start;
        }

        private void InitUsername()
        {
            InitInputText(UsernameInput);
            UsernameInput.InputView.Text = FString.Username;
            if (FSetting.AppMode == FAppMode.FBO)
            {
                UsernameInput.InputView.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeCharacter);
                return;
            }
            UsernameInput.InputView.Keyboard = Keyboard.Email;
        }

        private void InitPassword()
        {
            InitInputText(PasswordInput);
            PasswordInput.InputView.IsPassword = true;
        }

        private void InitInputText(FEntry input)
        {
            input.InputView.TextColor = input.FocusedColor = input.UnfocusedColor = Color.White;
            input.InputView.ClearButtonVisibility = ClearButtonVisibility.Never;
        }

        private void InitSubmit()
        {
            Submiter.HorizontalOptions = LayoutOptions.FillAndExpand;
            Submiter.VerticalOptions = LayoutOptions.End;
            Submiter.CornerRadius = 50;
            Submiter.Margin = new Thickness(0, 15, 0, 20);
            Submiter.Padding = new Thickness(0, 10);
            Submiter.FontSize = FSetting.FontSizeButton + 2;
            Submiter.FontFamily = FSetting.FontText;
            Submiter.BackgroundColor = Color.White;
            Submiter.ShadowColor = Color.Gray;
            Submiter.HasShadow = true;
            Submiter.TextColor = FSetting.TextColorTitle;
            Submiter.Clicked += OnLogin;
        }

        private void InitBiometricButton()
        {
            BioParent.BindingContext = Submiter;
            BioParent.Margin = new Thickness(0, 15, 0, 20);
            BioParent.CornerRadius = 50;
            BioParent.HorizontalOptions = LayoutOptions.Center;
            BioParent.VerticalOptions = LayoutOptions.Center;
            BioParent.SetBinding(View.HeightRequestProperty, View.HeightProperty.PropertyName);
            BioParent.SetBinding(View.WidthRequestProperty, View.HeightProperty.PropertyName);

            Biometric.BindingContext = this;
            Biometric.BackgroundColor = Color.Transparent;
            Biometric.Clicked += OnBiometricClicked;
        }

        private void InitRecoveryButton()
        {
            Recoverier.BindingContext = this;
            Recoverier.Padding = new Thickness(0, 5);
            Recoverier.FontSize = FSetting.FontSizeLabelContent;
            Recoverier.FontFamily = FSetting.FontText;
            Recoverier.BackgroundColor = Color.Transparent;
            Recoverier.HorizontalTextAlignment = TextAlignment.End;
            Recoverier.HorizontalOptions = LayoutOptions.EndAndExpand;
            Recoverier.Clicked += OnRecoverClicked;
            Recoverier.TextColor = Color.White;
        }

        private void OnRecoverClicked(object sender, EventArgs e)
        {
            UnFocus();
        }

        private void OnBiometricClicked(object sender, EventArgs e)
        {
            UnFocus();
        }

        private void OnLogin(object sender, EventArgs e)
        {
            UnFocus();
        }

        private void UnFocus()
        {
            PasswordInput.InputView.Unfocus();
            UsernameInput.InputView.Unfocus();
        }
    }
}