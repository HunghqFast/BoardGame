using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FItemNotify : BindableObject, IFNotifyElement
    {
        public static readonly BindableProperty IsSeenProperty = BindableProperty.Create("IsSeen", typeof(bool), typeof(FItemNotify), false);
        public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title", typeof(string), typeof(FItemNotify));
        public static readonly BindableProperty ContentProperty = BindableProperty.Create("Content", typeof(string), typeof(FItemNotify));
        public static readonly BindableProperty TimeProperty = BindableProperty.Create("Time", typeof(string), typeof(FItemNotify));
        public static readonly BindableProperty CodeProperty = BindableProperty.Create("Code", typeof(string), typeof(FItemNotify));
        public static readonly BindableProperty FontProperty = BindableProperty.Create("Font", typeof(string), typeof(FItemNotify));
        public static readonly BindableProperty CheckTextProperty = BindableProperty.Create("CheckText", typeof(string), typeof(FItemNotify));
        public static readonly BindableProperty RemoveTextProperty = BindableProperty.Create("RemoveText", typeof(string), typeof(FItemNotify));
        public static readonly BindableProperty CheckIconProperty = BindableProperty.Create("CheckIcon", typeof(ImageSource), typeof(FItemNotify));
        public static readonly BindableProperty RemoveIconProperty = BindableProperty.Create("RemoveIcon", typeof(ImageSource), typeof(FItemNotify));
        public static readonly BindableProperty IconProperty = BindableProperty.Create("Icon", typeof(ImageSource), typeof(FItemNotify));
        public static readonly BindableProperty ColorContentProperty = BindableProperty.Create("ColorContent", typeof(Color), typeof(FItemNotify), Color.Default);
        public static readonly BindableProperty ColorTimeProperty = BindableProperty.Create("ColorTime", typeof(Color), typeof(FItemNotify), Color.Default);
        public static readonly BindableProperty TypeProperty = BindableProperty.Create("Type", typeof(FWebViewType), typeof(FItemNotify), FWebViewType.Default);

        public bool IsSeen
        {
            get => (bool)GetValue(IsSeenProperty);
            set => SetValue(IsSeenProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Content
        {
            get => (string)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public string Time
        {
            get => (string)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        public string Code
        {
            get => (string)GetValue(CodeProperty);
            set => SetValue(CodeProperty, value);
        }

        public string Font
        {
            get => (string)GetValue(FontProperty);
            set => SetValue(FontProperty, value);
        }

        public string CheckText
        {
            get => (string)GetValue(CheckTextProperty);
            set => SetValue(CheckTextProperty, value);
        }

        public string RemoveText
        {
            get => (string)GetValue(RemoveTextProperty);
            set => SetValue(RemoveTextProperty, value);
        }

        public ImageSource CheckIcon
        {
            get => (ImageSource)GetValue(CheckIconProperty);
            set => SetValue(CheckIconProperty, value);
        }

        public ImageSource RemoveIcon
        {
            get => (ImageSource)GetValue(RemoveIconProperty);
            set => SetValue(RemoveIconProperty, value);
        }

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public Color ColorContent
        {
            get => (Color)GetValue(ColorContentProperty);
            set => SetValue(ColorContentProperty, value);
        }

        public Color ColorTime
        {
            get => (Color)GetValue(ColorTimeProperty);
            set => SetValue(ColorTimeProperty, value);
        }

        public FWebViewType Type
        {
            get => (FWebViewType)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public FItemNotify()
        {
            RemoveText = FText.Delete;
            RemoveIcon = FIcons.TrashCanOutline.ToFontImageSource(Color.White, FSetting.SizeIconButton);
        }

        public void Read()
        {
            IsSeen = true;
            Font = FSetting.FontText;
            ColorTime = ColorContent = FSetting.DisableColor;
            CheckText = FText.MarkUnread;
            CheckIcon = FIcons.Close.ToFontImageSource(Color.White, FSetting.SizeIconButton);
        }

        public void UnRead()
        {
            IsSeen = false;
            Font = FSetting.FontTextMedium;
            ColorContent = FSetting.TextColorTitle;
            ColorTime = FSetting.ColorTime;
            CheckText = FText.MarkRead;
            CheckIcon = FIcons.Check.ToFontImageSource(Color.White, FSetting.SizeIconButton);
        }

        public FItemNotify Refresh()
        {
            if (IsSeen)
            {
                CheckText = FText.MarkUnread;
                CheckIcon = FIcons.Close.ToFontImageSource(Color.White, FSetting.SizeIconButton);
            }
            else
            {
                CheckText = FText.MarkRead;
                CheckIcon = FIcons.Check.ToFontImageSource(Color.White, FSetting.SizeIconButton);
            }
            return this;
        }
    }
}