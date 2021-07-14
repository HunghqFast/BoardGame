using Syncfusion.SfChart.XForms;
using System;

namespace FastMobile.FXamarin.Core
{
    public class FNumericalAxis : NumericalAxis
    {
        protected override void OnCreateLabels()
        {
            base.OnCreateLabels();
            if (Convert.ToDouble(VisibleLabels[^1].LabelContent) < 10 && Convert.ToDouble(VisibleLabels[0].LabelContent) > -10)
            {
                for (int i = 0; i < VisibleLabels.Count; i++)
                {
                    if (Convert.ToDouble(VisibleLabels[i].LabelContent) % 1 != 0)
                        VisibleLabels[i].LabelContent = " ";
                    else
                        VisibleLabels[i].LabelContent = Convert.ToDouble(VisibleLabels[i].LabelContent).ToString(FXyChart.FormatNum).TrimStart();
                }
                return;
            }

            for (int i = 0; i < VisibleLabels.Count; i++)
                VisibleLabels[i].LabelContent = Convert.ToDouble(VisibleLabels[i].LabelContent).ToString(FXyChart.FormatNum).TrimStart();
        }
    }
}