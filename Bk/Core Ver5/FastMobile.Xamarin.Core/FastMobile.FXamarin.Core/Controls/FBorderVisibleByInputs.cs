using Syncfusion.XForms.Border;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FBorderVisibleByInputs : SfBorder, IViewVisible<View>
    {
        public static readonly BindableProperty ShowProperty = BindableProperty.Create("Show", typeof(bool), typeof(FPage), true);

        public bool Show
        {
            get => (bool)GetValue(ShowProperty);
            set => SetValue(ShowProperty, value);
        }

        public View View => this;

        private readonly List<FInput> Inputs;

        public FBorderVisibleByInputs() : base()
        {
            Inputs = new List<FInput>();
        }

        public void AddInput(FInput f)
        {
            if (Inputs.Contains(f))
                return;
            Inputs.Add(f);
            f.IsVisibleChanged += (s, e) => Hide(e.Value);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == IsVisibleProperty.PropertyName)
            {
                Hide(false);
                return;
            }

            if (propertyName == ShowProperty.PropertyName)
            {
                IsVisible = Show;
                return;
            }
        }

        private void Hide(bool value)
        {
            IsVisible = Show && !(!value && Inputs.Find(x => x.IsVisible) == null);
        }
    }
}