using System;

namespace MailJet.Client.Response.Data
{
    public class ListRecipient : DataItem
    {
        public long ContactID { get; set; }
        public bool IsActive { get; set; }
        public bool IsUnsubscribed { get; set; }
        public long ListID { get; set; }
        public DateTime? UnsubscribedAt { get; set; }
    }
}
