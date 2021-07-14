using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FMenuButtonReport : BindableObject
    {
        public static readonly BindableProperty ToolbarProperty = BindableProperty.Create("Toolbar", typeof(FToolbar), typeof(FMenuButtonReport));
        public static readonly BindableProperty ActionProperty = BindableProperty.Create("Action", typeof(Func<object, Task>), typeof(FMenuButtonReport));
        public static readonly BindableProperty VisibleProperty = BindableProperty.Create("Visible", typeof(bool), typeof(FMenuButtonReport));
        public static readonly BindableProperty EnableProperty = BindableProperty.Create("Enable", typeof(bool), typeof(FMenuButtonReport));

        public FToolbar Toolbar { get => (FToolbar)GetValue(ToolbarProperty); set => SetValue(ToolbarProperty, value); }
        public Func<object, Task> Action { get => (Func<object, Task>)GetValue(ActionProperty); set => SetValue(ActionProperty, value); }
        public bool Visible { get => (bool)GetValue(VisibleProperty); set => SetValue(VisibleProperty, value); }
        public bool Enable { get => (bool)GetValue(EnableProperty); set => SetValue(EnableProperty, value); }
        public ImageSource Icon { get => Toolbar.GetStringIcon()?.ToFontImageSource(Toolbar.GetIconColor(), FSetting.SizeIconMenu); }
        public string Title { get => Toolbar.Title; }
    }
}