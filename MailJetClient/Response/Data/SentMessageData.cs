namespace MailJet.Client.Response.Data
{
    public class SentMessageData
    {
        public Sent[] Sent { get; set; }
    }

    public class Sent
    {
        public string Email { get; set; }
        public long MessageID { get; set; }
    }
}
