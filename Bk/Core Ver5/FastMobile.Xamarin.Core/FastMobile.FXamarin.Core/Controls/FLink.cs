using System;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FLink : Label
    {
        public event EventHandler<EventArgs> Clicked;

        private readonly TapGestureRecognizer Event;

        public FLink() : base()
        {
            Event = new TapGestureRecognizer();
            Event.Tapped += OnClicked;
            GestureRecognizers.Add(Event);
        }

        protected virtual void OnClicked(object sender, EventArgs e)
        {
            Clicked?.Invoke(sender, e);
        }
    }
}