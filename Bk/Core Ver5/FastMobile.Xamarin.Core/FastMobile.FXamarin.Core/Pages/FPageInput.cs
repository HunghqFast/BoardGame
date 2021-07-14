using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageInput : FPage
    {
        public readonly FButton Submiter, Closer;

        protected readonly Grid Form;
        private readonly Grid G, B;
        private readonly ScrollView S;

        public FPageInput(bool pull, bool scroll) : base(pull, scroll)
        {
            Submiter = new FButton(string.Empty, FIcons.Check);
            Closer = new FButton(string.Empty, FIcons.Close);
            G = new Grid();
            B = new Grid();
            Form = new Grid();
            S = new ScrollView() { Content = Form };
            Content = G;
            Base();
        }

        public virtual void Update(bool v)
        {
        }

        private void Base()
        {
            B.ColumnSpacing = FSetting.SpacingButtons;
            B.Padding = new Thickness(10, 0);

            B.RowDefinitions.Add(new RowDefinition { Height = 49 });
            B.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            B.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            B.Children.Add(Submiter, 0, 0);
            B.Children.Add(Closer, 1, 0);

            G.RowSpacing = 0;
            G.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            G.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            G.RowDefinitions.Add(new RowDefinition { Height = 1 });
            G.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            G.Children.Add(S, 0, 0);
            G.Children.Add(new FLine(), 0, 1);
            G.Children.Add(B, 0, 2);
        }
    }
}