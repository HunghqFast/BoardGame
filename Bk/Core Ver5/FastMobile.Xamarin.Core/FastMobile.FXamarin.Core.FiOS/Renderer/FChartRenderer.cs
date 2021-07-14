using CoreGraphics;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FiOS;
using Syncfusion.SfChart.XForms;
using Syncfusion.SfChart.XForms.iOS;
using Syncfusion.SfChart.XForms.iOS.Renderers;
using System;
using System.Reflection;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Native = Syncfusion.SfChart.iOS;

[assembly: ExportRenderer(typeof(FSfChart), typeof(FChartRenderer))]

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FChartRenderer : SfChartRenderer
    {
        private FSfChart FormsChart => Element as FSfChart;

        protected override void OnElementChanged(ElementChangedEventArgs<SfChart> e)
        {
            base.OnElementChanged(e);

            if (FormsChart == null || FormsChart.Type == FChartType.Bar)
                return;
            if (!(Control != null && Control is Native.SFChart))
                return;
            for (int i = 0; i < FormsChart.ChartBehaviors.Count; i++)
            {
                if (FormsChart.ChartBehaviors[i] is ChartTooltipBehavior)
                {
                    ChartTooltipBehavior formsTooltip = FormsChart.ChartBehaviors[i] as ChartTooltipBehavior;
                    FTooltipBehavior nativeTooltip = new FTooltipBehavior();
                    nativeTooltip.FormsBehavior = formsTooltip;
                    var properties = SfChartRenderer.GetPropertiesChanged(typeof(ChartTooltipBehavior), formsTooltip);

                    foreach (var name in properties)
                        ChartTooltipBehaviorMapping.OnChartTooltipBehaviorPropertiesChanged(name, formsTooltip, nativeTooltip);

                    SfChartRenderer.SetNativeObject(typeof(ChartTooltipBehavior), formsTooltip, nativeTooltip);
                    Control.Behaviors.RemoveAt(i);
                    Control.Behaviors.Insert(i, nativeTooltip);
                }
            }
        }
    }

    public class FTooltipBehavior : ChartTooltipBehaviorHelper
    {
        public override void DrawRect(CGRect rect)
        {
            PropertyInfo propertyInfo = typeof(Native.SFChart).GetProperty("TooltipView", BindingFlags.NonPublic | BindingFlags.Instance);
            UIView value = (UIView)propertyInfo.GetValue(this.Chart, null);
            UIView tooltipView = value;
            CGRect seriesClipRect = Chart.SeriesClipRect;
            nfloat width = value.Frame.Width;
            nfloat height = value.Frame.Height;

            if (value.Frame.X < 0)
                tooltipView.Frame = new CGRect(seriesClipRect.Left, seriesClipRect.GetMidY(), width, height);
            else if (value.Frame.Y > seriesClipRect.Height)
                tooltipView.Frame = new CGRect(value.Frame.X, seriesClipRect.GetMidY(), width, height);
            else if (value.Frame.X > seriesClipRect.GetMidX())
                tooltipView.Frame = new CGRect(seriesClipRect.Left, seriesClipRect.Top, width, height);

            propertyInfo.SetValue(Chart, tooltipView);
            base.DrawRect(rect);
        }
    }
}