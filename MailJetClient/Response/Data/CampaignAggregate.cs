using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailJet.Client.Response.Data
{
    public class CampaignAggregate: DataItem
    {
        public string CampaignIDS { get; set; }
        public int ContactFilterID { get; set; }
        public int ContactsListID { get; set; }
        public bool Final { get; set; }
        public string FromDate { get; set; }
        public string Keyword { get; set; }
        public string Name { get; set; }
        public int SenderID { get; set; }
        public string ToDate { get; set; }
    }

    

}
