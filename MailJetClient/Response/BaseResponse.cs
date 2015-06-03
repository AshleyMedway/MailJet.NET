namespace MailJet.Client.Response
{
    public abstract class BaseResponse
    {
        public int Count { get; set; }
        public int Total { get; set; }
    }
}
