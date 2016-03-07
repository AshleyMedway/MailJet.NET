using MailJet.Client.Response.Data;
using System.Collections.Generic;

namespace MailJet.Client
{
    public class DNSCheckData : DataItem
    {
        public string DKIMStatus { get; set; }
        public List<string> DKIMErrors { get; set; }
        public string DKIMRecordCurrentValue { get; set; }
        public string SPFStatus { get; set; }
        public List<string> SPFErrors { get; set; }
        public List<string> SPFRecordsCurrentValues { get; set; }
    }
}

