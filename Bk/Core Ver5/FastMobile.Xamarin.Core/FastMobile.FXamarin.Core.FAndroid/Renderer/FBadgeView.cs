using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FBadgeView : TextView
    {
        public static int TextSizeDip = 9;

        private const int DefaultLrPaddingDip = 4;
        private const int DefaultCornerRadiusDip = 7;
        private static Animation fadeInAnimation, fadeOutAnimation;
        private Context context;
        private readonly Color defaultBadgeColor = Color.ParseColor("#CCFF0000");
        private ShapeDrawable backgroundShape;
        private FBadgePosition position;
        private int badgeMarginL, badgeMarginR, badgeMarginT, badgeMarginB;
        private bool hasWrappedLayout;

        public View Target { get; set; }

        public FBadgePosition Postion
        {
            get => position;
            set
            {
                if (position == value)
                {
                    return;
                }
                position = value;
                ApplyLayoutParams();
            }
        }

        public Color BadgeColor
        {
            get => backgroundShape.Paint.Color;
            set
            {
                backgroundShape.Paint.Color = value;
                Background.InvalidateSelf();
            }
        }

        public Color TextColor
        {
            get => new Color(CurrentTextColor);
            set => SetTextColor(value);
        }

        public void SetMargins(float left, float top, float right, float bottom)
        {
            badgeMarginL = DipToPixels(left);
            badgeMarginT = DipToPixels(top);
            badgeMarginR = DipToPixels(right);
            badgeMarginB = DipToPixels(bottom);
            ApplyLayoutParams();
        }

        public static FBadgeView ForTarget(Context context, View target)
        {
            var FBadgeView = new FBadgeView(context, null, Android.Resource.Attribute.TextViewStyle);
            FBadgeView.WrapTargetWithLayout(target);
            return FBadgeView;
        }

        public static FBadgeView ForTargetLayout(Context context, View target)
        {
            var FBadgeView = new FBadgeView(context, null, Android.Resource.Attribute.TextViewStyle);
            FBadgeView.AddToTargetLayout(target);
            return FBadgeView;
        }

        public new string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                switch (Visibility)
                {
                    case ViewStates.Visible when string.IsNullOrEmpty(value):
                        Hide(true);
                        break;

                    case ViewStates.Gone when !string.IsNullOrEmpty(value):
                        Show(true);
                        break;
                }
            }
        }

        public void Show()
        {
            Show(false, null);
        }

        public void Show(bool animate)
        {
            Show(animate, fadeInAnimation);
        }

        public void Hide(bool animate)
        {
            Hide(animate, fadeOutAnimation);
        }

        private FBadgeView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Init(context);
        }

        private void Init(Context context)
        {
            this.context = context;

            var paddingPixels = DipToPixels(DefaultLrPaddingDip);
            SetPadding(paddingPixels, 0, paddingPixels, 0);
            SetTextColor(Color.White);
            SetTextSize(ComplexUnitType.Dip, TextSizeDip);

            fadeInAnimation = new AlphaAnimation(0, 1)
            {
                Interpolator = new DecelerateInterpolator(),
                Duration = 200
            };
            fadeOutAnimation = new AlphaAnimation(1, 0)
            {
                Interpolator = new AccelerateInterpolator(),
                Duration = 200
            };

            backgroundShape = CreateBackgroundShape();
            ViewCompat.SetBackground(this, backgroundShape);
            BadgeColor = defaultBadgeColor;

            Visibility = ViewStates.Gone;
        }

        private ShapeDrawable CreateBackgroundShape()
        {
            var radius = DipToPixels(DefaultCornerRadiusDip);
            var outerR = new float[] { radius, radius, radius, radius, radius, radius, radius, radius };
            return new ShapeDrawable(new RoundRectShape(outerR, null, null));
        }

        private void AddToTargetLayout(View target)
        {
            var layout = target.Parent as ViewGroup;
            if (layout == null)
                return;
            layout.SetClipChildren(false);
            layout.SetClipToPadding(false);
            layout.AddView(this);
            Target = target;
        }

        private void WrapTargetWithLayout(View target)
        {
            var lp = target.LayoutParameters;
            var parent = target.Parent;
            var group = parent as ViewGroup;
            if (group == null)
                return;
            group.SetClipChildren(false);
            group.SetClipToPadding(false);

            var container = new FrameLayout(context);
            var index = group.IndexOfChild(target);
            group.RemoveView(target);
            group.AddView(container, index, lp);

            container.AddView(target);
            group.Invalidate();
            container.AddView(this);

            Target = target;
            hasWrappedLayout = true;
        }

        private void Show(bool animate, Animation anim)
        {
            ApplyLayoutParams();
            if (animate)
            {
                StartAnimation(anim);
            }
            Visibility = ViewStates.Visible;
        }

        private void Hide(bool animate, Animation anim)
        {
            Visibility = ViewStates.Gone;
            if (animate)
            {
                StartAnimation(anim);
            }
        }

        private void ApplyLayoutParams()
        {
            var layoutParameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            if (!hasWrappedLayout)
            {
                var targetParams = ((FrameLayout.LayoutParams)Target.LayoutParameters);
                var w = targetParams.Width / 2;
                var h = targetParams.Height / 2;
                layoutParameters.Gravity = GravityFlags.Center;
                switch (Postion)
                {
                    case FBadgePosition.TopLeft:
                        layoutParameters.SetMargins(badgeMarginL - w, badgeMarginT - h, 0, 0);
                        break;

                    case FBadgePosition.TopRight:
                        layoutParameters.SetMargins(0, badgeMarginT - h, badgeMarginR - w, 0);
                        break;

                    case FBadgePosition.BottomLeft:
                        layoutParameters.SetMargins(badgeMarginL - w, 0, 0, 0 + badgeMarginB - h);
                        break;

                    case FBadgePosition.BottomRight:
                        layoutParameters.SetMargins(0, 0, badgeMarginR - w, 0 + badgeMarginB - h);
                        break;

                    case FBadgePosition.Center:
                        layoutParameters.SetMargins(badgeMarginL, badgeMarginT, badgeMarginR, badgeMarginB);
                        break;

                    case FBadgePosition.TopCenter:
                        layoutParameters.SetMargins(0, 0 + badgeMarginT - h, 0, 0);
                        break;

                    case FBadgePosition.BottomCenter:
                        layoutParameters.SetMargins(0, 0, 0, 0 + badgeMarginB - h);
                        break;

                    case FBadgePosition.LeftCenter:
                        layoutParameters.SetMargins(badgeMarginL - w, 0, 0, 0);
                        break;

                    case FBadgePosition.RightCenter:
                        layoutParameters.SetMargins(0, 0, badgeMarginR - w, 0);
                        break;
                }
            }
            else
            {
                switch (Postion)
                {
                    case FBadgePosition.TopLeft:
                        layoutParameters.Gravity = GravityFlags.Left | GravityFlags.Top;
                        layoutParameters.SetMargins(badgeMarginL, badgeMarginT, 0, 0);
                        break;

                    case FBadgePosition.TopRight:
                        layoutParameters.Gravity = GravityFlags.Right | GravityFlags.Top;
                        layoutParameters.SetMargins(0, badgeMarginT, badgeMarginR, 0);
                        break;

                    case FBadgePosition.BottomLeft:
                        layoutParameters.Gravity = GravityFlags.Left | GravityFlags.Bottom;
                        layoutParameters.SetMargins(badgeMarginL, 0, 0, badgeMarginB);
                        break;

                    case FBadgePosition.BottomRight:
                        layoutParameters.Gravity = GravityFlags.Right | GravityFlags.Bottom;
                        layoutParameters.SetMargins(0, 0, badgeMarginR, badgeMarginB);
                        break;

                    case FBadgePosition.Center:
                        layoutParameters.Gravity = GravityFlags.Center;
                        layoutParameters.SetMargins(0, 0, 0, 0);
                        break;

                    case FBadgePosition.TopCenter:
                        layoutParameters.Gravity = GravityFlags.Center | GravityFlags.Top;
                        layoutParameters.SetMargins(0, badgeMarginT, 0, 0);
                        break;

                    case FBadgePosition.BottomCenter:
                        layoutParameters.Gravity = GravityFlags.Center | GravityFlags.Bottom;
                        layoutParameters.SetMargins(0, 0, 0, badgeMarginB);
                        break;

                    case FBadgePosition.LeftCenter:
                        layoutParameters.Gravity = GravityFlags.Left | GravityFlags.Center;
                        layoutParameters.SetMargins(badgeMarginL, 0, 0, 0);
                        break;

                    case FBadgePosition.RightCenter:
                        layoutParameters.Gravity = GravityFlags.Right | GravityFlags.Center;
                        layoutParameters.SetMargins(0, 0, badgeMarginR, 0);
                        break;
                }
            }

            LayoutParameters = layoutParameters;
        }

        private int DipToPixels(float dip)
        {
            return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dip, Resources.DisplayMetrics);
        }
    }
}