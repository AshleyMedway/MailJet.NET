using MailJet.Client.Enum;
using System;

namespace MailJet.Client.Response.Data
{
    public class MessageData : DataItem
    {
        public DateTime ArrivedAt { get; set; }
        public int AttachmentCount { get; set; }
        public int AttemptCount { get; set; }
        public long CampaignID { get; set; }
        public long ContactID { get; set; }
        public long Delay { get; set; }
        public int DestinationID { get; set; }
        public int FilterTime { get; set; }
        public long FromID { get; set; }
        public bool IsClickTracked { get; set; }
        public bool IsHtmlPartIncluded { get; set; }
        public bool IsOpenTracked { get; set; }
        public bool IsUnsubTracked { get; set; }
        public long MessageSize { get; set; }
        public decimal SpamassassinScore { get; set; }
        public long StateID { get; set; }
        public bool StatePermanent { get; set; }
        public MessageStatus Status { get; set; }
    }
}
