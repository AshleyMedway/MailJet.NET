using MailJet.Client.Response.Data;
using System;

namespace MailJet.Client
{
    public class DNSData : DataItem
    {
        public string DKIMRecordName { get; set; }
        public string DKIMRecordValue { get; set; }
        public string DKIMStatus { get; set; }
        public string Domain { get; set; }
        public bool IsCheckInProgress { get; set; }
        public DateTime LastCheckAt { get; set; }
        public string OwnerShipToken { get; set; }
        public string OwnerShipTokenRecordName { get; set; }
        public string SPFRecordValue { get; set; }
        public string SPFStatus { get; set; }
    }
}

