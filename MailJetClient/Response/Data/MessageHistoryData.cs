using MailJet.Client.Enum;

namespace MailJet.Client.Response.Data
{
    public class MessageHistoryData
    {
        public string Comment { get; set; }
        public long EventAt { get; set; }
        public MessageStatus EventType { get; set; }
        public string State { get; set; }
        public string Useragent { get; set; }
    }
}
