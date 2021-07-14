using System;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageProfileInput : FPageInput
    {
        public readonly FInputText Link, Name, Database;

        public FPageProfileInput(bool IsHasPullToRefresh = false, bool enableScroll = true) : base(IsHasPullToRefresh, enableScroll)
        {
            Link = new FInputText();
            Name = new FInputText();
            Database = new FInputText();
            Closer.Clicked += OnClose;
            Submiter.Clicked += Submit;
            InitForm();
            Update(FSetting.V);
        }

        public override void Update(bool v)
        {
            Title = FText.ProfileDeclare;
            Link.Title = FText.Link;
            Name.Title = FText.Alias;
            Database.Title = FText.CompanyCode;
            Submiter.Text = FText.Save;
            Closer.Text = FText.Cancel;
        }

        public void UpdateIcon(bool isEdit)
        {
        }

        private void InitForm()
        {
            Name.NotAllowsNull = true;
            Name.MaxLength = 32;
            Name.Rendering();

            Link.NotAllowsNull = true;
            Link.Keyboard = Keyboard.Url;
            Link.Rendering();

            Database.NotAllowsNull = true;
            Database.IsVisible = FSetting.AppMode == FAppMode.FBO;
            Database.Rendering();

            Form.RowSpacing = 0;
            Form.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            Form.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Form.RowDefinitions.Add(new RowDefinition { Height = 1 });
            Form.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Form.RowDefinitions.Add(new RowDefinition { Height = 1 });
            Form.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Form.RowDefinitions.Add(new RowDefinition { Height = 1 });

            Form.RowSpacing = 0;
            Form.Children.Add(Name, 0, 0);
            Form.Children.Add(new FLine(), 0, 1);
            Form.Children.Add(Link, 0, 2);
            Form.Children.Add(new FLine(), 0, 3);
            Form.Children.Add(Database, 0, 4);
            Form.Children.Add(new FLine() { BindingContext = Database }, 0, 5);
            Form.Children[^1].SetBinding(View.IsVisibleProperty, View.IsVisibleProperty.PropertyName);
        }

        private async void OnClose(object sender, EventArgs e)
        {
            await Navigation.PopAsync(true);
        }

        private void Submit(object sender, EventArgs e)
        {
            Link.UnFocusInput();
            Name.UnFocusInput();
            Database.UnFocusInput();
        }
    }
}