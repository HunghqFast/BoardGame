using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FSpinWheelListData : ObservableCollection<FSpinWheelData>
    {
        public FSpinWheelData FailData { private set; get; }

        public FSpinWheelListData()
        {
        }

        public FSpinWheelListData(IEnumerable<FSpinWheelData> collection) : base(collection)
        {
        }

        public void UpdateValue()
        {
            if (Count == 0) return;
            double value = 360d / Count;
            this.ToList().ForEach(x => { x.Value = value; if (x.Status) FailData = x; });
        }

        public IList<Color> GetColor()
        {
            return this.ToList().Select(x => x.Color).ToList();
        }

        public FSpinWheelData GetItem(string ID)
        {
            return this.ToList().Find(x => x.Name.Equals(ID)) ?? FailData;
        }
    }
}