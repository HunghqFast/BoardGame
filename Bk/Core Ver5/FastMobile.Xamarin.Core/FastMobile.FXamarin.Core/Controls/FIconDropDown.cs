using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FIconDropDown : View
    {
        /// <summary>
        /// Đang phát triển
        /// </summary>
        public FIconDropDown() : base()
        {
            Unfocused += FIconDropDown_Unfocused;
            iconDropdown = new Image();
            iconDropdown.BindingContext = this;
            iconDropdown.SetBinding(Image.SourceProperty, "IconDropdownSource");
        }

        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create("SelectedIndex", typeof(int), typeof(FIconDropDown), -1);
        public static readonly BindableProperty SelectedValueProperty = BindableProperty.Create("SelectedValue", typeof(string), typeof(FIconDropDown), "");
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create("SelectedItem", typeof(object), typeof(FIconDropDown), null);
        public static readonly BindableProperty IconDropdownSourceProperty = BindableProperty.Create("IconDropdownSource", typeof(ImageSource), typeof(FIconDropDown), "");
        public static readonly BindableProperty SelectedBackgroundCorlorProperty = BindableProperty.Create("SelectedBackgroundCorlor", typeof(Color), typeof(FIconDropDown), Color.LightGray);
        public static readonly BindableProperty SelectedTextColorProperty = BindableProperty.Create("SelectedTextColor", typeof(Color), typeof(FIconDropDown), Color.Black);
        public static readonly BindableProperty UnSelectedBackgroundCorlorProperty = BindableProperty.Create("UnSelectedBackgroundCorlor", typeof(Color), typeof(FIconDropDown), Color.Transparent);
        public static readonly BindableProperty UnSelectedTextCorlorProperty = BindableProperty.Create("UnSelectedTextCorlor", typeof(Color), typeof(FIconDropDown), Color.Black);
        public static readonly BindableProperty DropdownTextSizeProperty = BindableProperty.Create("DropdownTextSize", typeof(int), typeof(FIconDropDown), 12);
        public static readonly BindableProperty DropdownFontFamilyProperty = BindableProperty.Create("DropdownFontFamily", typeof(string), typeof(FIconDropDown), new Font().FontFamily);

        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }
            set
            {
                SetValue(SelectedIndexProperty, value);
            }
        }

        public string SelectedValue
        {
            get
            {
                return (string)GetValue(SelectedValueProperty);
            }
            set
            {
                SetValue(SelectedValueProperty, value);
            }
        }

        public object SelectedItem
        {
            get
            {
                return (object)GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        public ImageSource IconDropdownSource
        {
            get
            {
                return (ImageSource)GetValue(IconDropdownSourceProperty);
            }
            set
            {
                SetValue(IconDropdownSourceProperty, value);
            }
        }

        public Color SelectedBackgroundCorlor
        {
            get
            {
                return (Color)GetValue(SelectedBackgroundCorlorProperty);
            }
            set
            {
                SetValue(SelectedBackgroundCorlorProperty, value);
            }
        }

        public Color SelectedTextColor
        {
            get
            {
                return (Color)GetValue(SelectedTextColorProperty);
            }
            set
            {
                SetValue(SelectedTextColorProperty, value);
            }
        }

        public Color UnSelectedBackgroundCorlor
        {
            get
            {
                return (Color)GetValue(UnSelectedBackgroundCorlorProperty);
            }
            set
            {
                SetValue(UnSelectedBackgroundCorlorProperty, value);
            }
        }

        public Color UnSelectedTextCorlor
        {
            get
            {
                return (Color)GetValue(UnSelectedTextCorlorProperty);
            }
            set
            {
                SetValue(UnSelectedTextCorlorProperty, value);
            }
        }

        public int DropdownTextSize
        {
            get
            {
                return (int)GetValue(DropdownTextSizeProperty);
            }
            set
            {
                SetValue(DropdownTextSizeProperty, value);
            }
        }

        public string DropdownFontFamily
        {
            get
            {
                return (string)GetValue(DropdownFontFamilyProperty);
            }
            set
            {
                SetValue(DropdownFontFamilyProperty, value);
            }
        }

        public List<string> ItemSource;

        public event EventHandler<SelectedItemChangedEventArgs> SelectedChanged;

        protected virtual void OnSelectedChanged(SelectedItemChangedEventArgs e)
        {
            EventHandler<SelectedItemChangedEventArgs> handler = SelectedChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #region PrivateVariable

        private Image iconDropdown;

        #endregion PrivateVariable

        #region PrivateMethod

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            switch (propertyName)
            {
                case nameof(SelectedIndex):
                    ExecuteSelectedChanged();
                    ChangedLayout();
                    break;

                case nameof(SelectedValue):
                    ExecuteSelectedChanged();
                    ChangedLayout();
                    break;

                case nameof(SelectedItem):
                    ExecuteSelectedChanged();
                    ChangedLayout();
                    break;

                case nameof(IconDropdownSource):
                    break;

                default:
                    break;
            }
        }

        private void FIconDropDown_Unfocused(object sender, FocusEventArgs e)
        {
            //dropdown close
        }

        private void ExecuteSelectedChanged()
        {
            SelectedChanged?.Invoke(this, new SelectedItemChangedEventArgs(ItemSource[SelectedIndex], SelectedIndex));
        }

        private void ChangedLayout()
        {
        }

        #endregion PrivateMethod
    }
}