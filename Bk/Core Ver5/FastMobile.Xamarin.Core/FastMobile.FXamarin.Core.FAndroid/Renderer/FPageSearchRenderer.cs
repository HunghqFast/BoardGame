using Android.Content;
using Android.Runtime;
using AndroidX.AppCompat.Widget;
using Android.Views;
using Android.Views.InputMethods;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FAndroid;
using System;
using System.ComponentModel;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using EditText = Android.Widget.EditText;
using Platform = Xamarin.Essentials.Platform;

[assembly: ExportRenderer(typeof(FPageSearch), typeof(FPageSearchRenderer))]

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FPageSearchRenderer : FPageRenderer
    {
        public static Type TypeMenu { get; }

        protected static int IDMenu, IDSearch;
        protected SearchView Search;
        protected EditText EditText;
        protected Toolbar Toolbar;
        private FPageSearch Current => Element as FPageSearch;

        static FPageSearchRenderer()
        {
            TypeMenu = Assembly.GetCallingAssembly().TypeByAssemply("Resource", "Menu");
        }

        public FPageSearchRenderer(Context context) : base(context)
        {
            IDSearch = Resources.GetIdentifier("ActionSearch", "id", Context.PackageName);
            Search = new SearchView(context);
        }


        public static void Init(string searchMenu = "SearchMenu")
        {
            try
            {
                IDMenu = Convert.ToInt32(TypeMenu.GetStaticFieldValue(searchMenu));
            }
            catch { }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            if (Current != null && Current.Parent is NavigationPage)
            {
                AddSearchToToolbar();
            }
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            if (Current != null && Current.Parent is NavigationPage nav)
            {
                nav.Popped -= HandleNavigationPagePopped;
                nav.PoppedToRoot -= HandleNavigationPagePopped;
                nav.Popped += HandleNavigationPagePopped;
                nav.PoppedToRoot += HandleNavigationPagePopped;
            }
        }
        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            if (Current != null && Current.Parent is NavigationPage)
            {
                AddSearchToToolbar();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (Toolbar != null)
                Toolbar.Menu?.RemoveItem(IDMenu);
            base.Dispose(disposing);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == FPageSearch.SearchBackgroundColorProperty.PropertyName)
            {
                Search?.SetBackgroundColor(Current.SearchBackgroundColor.ToAndroid());
                return;
            }
            if (e.PropertyName == FPageSearch.SearchPlaceHolderProperty.PropertyName)
            {
                UpdatePlaceHolder();
                return;
            }
            if (e.PropertyName == FPageSearch.SearchTextColorProperty.PropertyName)
            {
                UpdateSearchTextColor();
                return;
            }
            if (e.PropertyName == FPageSearch.SearchFontProperty.PropertyName)
            {
                UpdateFont();
                return;
            }
            if (e.PropertyName == FPageSearch.SearchPlaceHolderColorProperty.PropertyName)
            {
                UpdatePlaceHolderColor();
                return;
            }
            if (e.PropertyName == FPageSearch.SearchBoxColorProperty.PropertyName)
            {
                UpdateBoxBackground();
                return;
            }
            if (e.PropertyName == FPageSearch.TurnOnSearchProperty.PropertyName)
            {
                UpdateTurnOn();
                return;
            }
        }

        protected virtual void AddSearchToToolbar()
        {
            Toolbar ??= GetToolbar();

            if (Toolbar == null)
                return;

            if (Toolbar.Menu?.FindItem(IDSearch)?.ActionView?.JavaCast<SearchView>()?.GetType() == typeof(SearchView))
                return;

            if (Current == null)
                return;

            Toolbar.Title = Current.Title;
            Toolbar.SetBackgroundColor(Color.Transparent.ToAndroid());
            InflateMenu();
            Search = Toolbar.Menu?.FindItem(IDSearch)?.ActionView?.JavaCast<SearchView>();

            if (Search == null)
                return;
            EditText = Search.FindViewById<EditText>(Search.Context.Resources.GetIdentifier("search_src_text", "id", Context.PackageName));
            if (EditText != null)
                EditText.Text = Current.SearchText;

            if (!string.IsNullOrEmpty(Current.SearchText))
            {
                Search.Iconified = false;
                SClearFocus();
            }

            Search.ImeOptions = (int)ImeAction.Search;
            Search.MaxWidth = int.MaxValue;
            Search.QueryTextChange += SearchQueryTextChange;
            Search.Close += SearchClose;

            EditText.ImeOptions = ImeAction.Search;
            EditText.EditorAction += OnEdittorAction;
            EditText.SetSelection(EditText.Text.Length);
            UpdateFont();
            UpdateSearchTextColor();
            UpdatePlaceHolder();
            UpdatePlaceHolderColor();
            UpdateBackgroundSearch();
            UpdateBoxBackground();
            UpdateTurnOn();
        }

        protected virtual void InflateMenu()
        {
            Toolbar.InflateMenu(IDMenu);
        }

        private void HandleNavigationPagePopped(object sender, NavigationEventArgs e)
        {
            if (sender is NavigationPage navigationPage && navigationPage.CurrentPage is IFSearch)
            {
                AddSearchToToolbar();
            }
        }

        private void UpdateTurnOn()
        {
            Toolbar.Menu?.FindItem(IDSearch)?.SetVisible(Current.TurnOnSearch);
        }

        private void SearchClose(object sender, SearchView.CloseEventArgs e)
        {
            Search.Iconified = false;
            Search.ClearFocus();
            Search.OnActionViewCollapsed();
            Toolbar.Menu?.FindItem(IDSearch)?.CollapseActionView();
            Search.ImeOptions = (int)ImeAction.Search;
            EditText.ImeOptions = ImeAction.Search;
        }

        private void OnEdittorAction(object sender, Android.Widget.TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Search)
            {
                Current?.OnSearchSubmit(Current, new FSearchEventArgs(EditText.Text));
                SClearFocus();
            }
        }

        private void SearchQueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            Current.SearchText = e.NewText;
            Current.OnSearchChanged(Current, new FSearchEventArgs(e.NewText));
        }

        private void UpdateSearchTextColor()
        {
            EditText?.SetTextColor(Current.SearchTextColor.ToAndroid());
        }

        private void UpdateFont()
        {
            if (EditText == null)
                return;
            EditText.Typeface = Current.SearchFont.ToTypeface();
        }

        private void UpdatePlaceHolder()
        {
            if (EditText == null || Search == null)
                return;
            EditText.Hint = Current.SearchPlaceHolder;
            Search.QueryHint = Current.SearchPlaceHolder;
        }

        private void UpdatePlaceHolderColor()
        {
            EditText?.SetHintTextColor(Current.SearchPlaceHolderColor.ToAndroid());
        }

        private void UpdateBackgroundSearch()
        {
            Search?.SetBackgroundColor(Current.SearchBackgroundColor.ToAndroid());
        }

        private void UpdateBoxBackground()
        {
            EditText?.SetBackgroundColor(Current.SearchBoxColor.ToAndroid());
        }

        private void SClearFocus()
        {
            Search?.ClearFocus();
            EditText?.ClearFocus();
        }

        private Toolbar GetToolbar()
        {
            return Platform.CurrentActivity.FindViewById<Toolbar>(Resources.GetIdentifier("toolbar", "id", Context.PackageName));
        }
    }
}