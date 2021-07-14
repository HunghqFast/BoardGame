using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace FastMobile.FXamarin.Core
{
    public class FDataObservation : ObservableCollection<FData>
    {
        private bool increase;
        private int maxLnr;

        public FDataObservation(bool increase = false) : base()
        {
            this.increase = increase;
            maxLnr = 0;
        }

        public FDataObservation(IEnumerable<FData> collection, bool increase = false) : base(collection)
        {
            this.increase = increase;
        }

        public FDataObservation(List<FData> list, bool increase = false) : base(list)
        {
            this.increase = increase;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    if (increase)
                    {
                        if (this[Count - 1].LineNumberRow == 0)
                        {
                            maxLnr++;
                            this[Count - 1].LineNumberRow = maxLnr;
                        }
                        else
                            maxLnr = this[Count - 1].LineNumberRow;
                    }
                    else this[Count - 1].LineNumberRow = Count;
                    if (this[Count - 1].SttRec0 != null && this[Count - 1].SttRec0.Equals(" "))
                    {
                        if (Count == 1) this[Count - 1].SttRec0 = "001";
                        else
                        {
                            long maxLine = this.Where(s => IndexOf(s) < Count - 1).Max(s => Base36ToNumber(s.SttRec0.ToString()));
                            this[Count - 1].SttRec0 = IncreaseNumber(maxLine + 1);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (!increase) this[e.NewStartingIndex].LineNumberRow = e.OldStartingIndex + 1;
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (!increase) for (int i = e.OldStartingIndex; i < Count; i++) this[i].LineNumberRow = i + 1;
                    break;
            };
        }

        public void UnCheckAll()
        {
            this.ToList().ForEach(x => x.IsCheck = false);
        }

        public void CheckAll()
        {
            this.ToList().ForEach(x => x.IsCheck = true);
        }

        public List<FData> SelectedsItem => this.ToList().FindAll(s => FFunc.StringToBoolean(s.IsCheck.ToString())).ToList();

        public void CheckItem(int index)
        {
            this[index].IsCheck = !this[index].IsCheck;
        }

        private string IncreaseNumber(long nDec)
        {
            if (nDec <= 999) return nDec.ToString("000");
            nDec += 10998;

            long nQuot = nDec;
            string sHex = string.Empty;
            long nRem;
            char cHex;

            while (nQuot > 0)
            {
                nRem = nQuot % 36;
                nQuot /= (long)36;
                cHex = nRem < 10 ? char.Parse(nRem.ToString()) : Convert.ToChar(nRem + 55);
                sHex = cHex + sHex;
            }
            return sHex;
        }

        private long Base36ToNumber(string base36)
        {
            long result = 0;
            for (int i = 0; i < 3; i++)
            {
                if (int.TryParse(base36[i].ToString(), out int value)) result += (long)Math.Pow(10, 2 - i) * value;
                else result += (long)Math.Pow(10, 2 - i) * Convert.ToChar(base36[i] - 55);
            }
            return result;
        }
    }
}