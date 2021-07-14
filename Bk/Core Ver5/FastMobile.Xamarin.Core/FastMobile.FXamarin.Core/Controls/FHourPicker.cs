using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FHourPicker : FBasePicker<string>
    {
        public override void SetDataPicker(string value)
        {
            SelectedItem = new ObservableCollection<object> { value.Substring(0, 2), value.Substring(3, 2) };
        }

        public override string GetDataPicker()
        {
            return $"{(SelectedItem as IList<object>)[0]}:{(SelectedItem as IList<object>)[1]}";
        }

        public FHourPicker()
        {
            HeaderText = FText.PickHour;
        }

        protected override ObservableCollection<string> InitHeader()
        {
            return new ObservableCollection<string>
            {
                FText.Hour,
                FText.Minute
            };
        }

        protected override void UpdateData(Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                SelectedItem = new ObservableCollection<object> { (e.NewValue as IList<object>)[0], (e.NewValue as IList<object>)[1] };
            });
        }

        protected override void PopulateDataCollection()
        {
            var hour = new ObservableCollection<object>();
            var minute = new ObservableCollection<object>();

            Enumerable.Range(0, 24).ForEach(i => hour.Add($"{(i < 10 ? "0" : string.Empty)}{i}"));
            Enumerable.Range(0, 60).ForEach(i => minute.Add($"{(i < 10 ? "0" : string.Empty)}{i}"));

            Data.Add(hour);
            Data.Add(minute);
        }
    }
}
