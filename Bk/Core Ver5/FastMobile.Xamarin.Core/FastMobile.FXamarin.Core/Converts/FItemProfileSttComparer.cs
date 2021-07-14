using System.Collections.Generic;

namespace FastMobile.FXamarin.Core
{
    public class FItemProfileSttComparer : IComparer<object>
    {
        public static FItemProfileSttComparer Instance { get; }
        static FItemProfileSttComparer()
        {
            Instance = new FItemProfileSttComparer();
        }

        public int Compare(object x, object y)
        {
            var xItem = x as FItemProfile;
            var yItem = y as FItemProfile;

            if (xItem.Stt > yItem.Stt)
                return 1;
            if (xItem.Stt < yItem.Stt)
                return -1;
            return 0;
        }
    }
}
