using System.Collections.Generic;

namespace FastMobile.FXamarin.Core
{
    public class FBDashboard
    {
        public string Title { get; set; }
        public int Table { get; set; }
        public double[] Margin { get; set; }
        public int LoadingTable { get; set; }
        public int ChangingTable { get; set; }
        public string DefaultData { get; set; }
        public List<FBParam> Params { get; set; }
        public FBHeader Header { get; set; }
        public BFooter2 Footer { get; set; }
    }
}