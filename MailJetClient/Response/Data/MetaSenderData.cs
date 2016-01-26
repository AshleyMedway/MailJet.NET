using System;

namespace MailJet.Client.Response.Data
{
    public class MetaSenderData : DataItem
    {
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Filename { get; set; }
        public bool IsEnabled { get; set; }
    }
}
