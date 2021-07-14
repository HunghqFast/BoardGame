using System.Data;

namespace FastMobile.FXamarin.Core
{
    public class FInputGridValue
    {
        public DataTable Table;
        public string Edited;

        public FInputGridValue(DataTable table, string edited)
        {
            Table = table;
            Edited = edited;
        }
    }
}