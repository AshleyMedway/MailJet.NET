using MailJet.Http;

namespace MailJet.Requests
{
    public class ApiRequest
    {
        public string Url { get; set; }
        public RequestType RequestType { get; set; }

        public ApiRequest(string url, RequestType requestType)
        {
            Url = url;
            RequestType = requestType;
        }
    }
}
