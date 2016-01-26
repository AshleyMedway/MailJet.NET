namespace MailJet.Client.Response
{
    public class ErrorResponse
    {
        public string ErrorInfo { get; set; }
        public string ErrorMessage { get; set; }
        public int StatusCode { get; set; }
    }
}
