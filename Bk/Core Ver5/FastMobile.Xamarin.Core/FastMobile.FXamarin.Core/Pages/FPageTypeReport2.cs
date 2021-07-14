using System.Data;

namespace FastMobile.FXamarin.Core
{
    public class FPageTypeReport2 : FPageTypeDefault
    {
        public FPageTypeReport2(string controller, DataTable resTable) : base(controller, true, resTable)
        {
            Target = "110";
        }
    }
}