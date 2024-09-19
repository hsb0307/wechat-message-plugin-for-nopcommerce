using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.WeChatMessage.Domain.Api
{
    public partial class BalanceData
    {
        // {"MobileNumber": "18753686528","Money": "178","Balance": "2,109.1","Date": "2024-08-19 10:29:51"}
        public string MobileNumber { get; set; }
        public string Money { get; set; }

        public string Balance { get; set; }
        public string Date { get; set; }

    }
}
