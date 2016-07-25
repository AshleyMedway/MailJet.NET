using System;

namespace MailJet.Client.Response.Data
{
    public class Campaign : DataItem
    {
        public int CampaignType { get; set; }
        public int ClickTracked { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomValue { get; set; }
        public long FirstMessageID { get; set; }
        public string FromEmail { get; set; }
        public int FromID { get; set; }
        public string FromName { get; set; }
        public int HasHtmlCount { get; set; }
        public int HasTxtCount { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsStarred { get; set; }
        public int NewsLetterID { get; set; }
        public int OpenTracked { get; set; }
        public DateTime SendEndAt { get; set; }
        public DateTime SendStartAt { get; set; }
        public int SpamassScore { get; set; }
        public int Status { get; set; }
        public string Subject { get; set; }
        public int UnsubscribeTrackedCount { get; set; }
    }

    

}