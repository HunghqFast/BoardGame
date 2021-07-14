using System;
using System.Collections.Generic;

namespace FastMobile.FXamarin.Core
{
    public class FMessageOptions : FMessage
    {
        public Action<string> Action { get; set; }

        public IEnumerable<FItemCustom> Source { get; set; }

        public FMessageOptions() : base()
        {
        }

        public FMessageOptions(FMessage m, Action<string> action, IEnumerable<FItemCustom> json) : base(m.Success, m.Code, m.Message)
        {
            Action = action;
            Source = json;
        }
    }
}