using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FDatePicker : FBasePicker<DateTime>
    {
        public override void SetDataPicker(string value)
        {
            SelectedItem = new ObservableCollection<object> { value.Substring(6, 2), value.Substring(4, 2), value.Substring(0, 4) };
        }

        public override DateTime GetDataPicker()
        {
            var s = SelectedItem;
            var d = int.Parse((s as IList<object>)[0].ToString());
            var m = int.Parse((s as IList<object>)[1].ToString());
            var y = int.Parse((s as IList<object>)[2].ToString());

            if (d > DateTime.DaysInMonth(y, m)) d = DateTime.DaysInMonth(y, m);
            return new DateTime(y, m, d);
        }

        public FDatePicker()
        {
            HeaderText = FText.PickDay;
        }

        protected override ObservableCollection<string> InitHeader()
        {
            return new ObservableCollection<string>
            {
                FText.Day,
                FText.Month,
                FText.Year
            };
        }

        protected override void UpdateData(Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                int d = int.Parse((e.NewValue as IList<object>)[0].ToString());
                int m = int.Parse((e.NewValue as IList<object>)[1].ToString());
                int y = int.Parse((e.NewValue as IList<object>)[2].ToString());
                if (d > DateTime.DaysInMonth(y, m))
                {
                    SelectedItem = new ObservableCollection<object>
                    {
                        DateTime.DaysInMonth(y, m).ToString(),
                        (e.NewValue as IList<object>)[1],
                        (e.NewValue as IList<object>)[2]
                    };
                }
            });
        }

        protected override void PopulateDataCollection()
        {
            var day = new ObservableCollection<object>();
            var month = new ObservableCollection<object>();
            var year = new ObservableCollection<object>();

            // Populate year
            for (var i = 1900; i <= 2078; i++) year.Add(i.ToString());
            // Populate months
            for (var i = 1; i <= 12; i++) month.Add(i < 10 ? $"0{i}" : i.ToString());
            // Populate Days
            for (var i = 1; i <= 31; i++) day.Add(i < 10 ? $"0{i}" : i.ToString());

            Data.Add(day);
            Data.Add(month);
            Data.Add(year);
        }
    }
}