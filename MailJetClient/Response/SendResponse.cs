using System.Collections.Generic;

namespace MailJet.Client.Response
{
    public class SendResponse
    {
        public int Count { get; set; }
        public int Total { get; set; }
        public List<DataItem> Data { get; set; }
    }
}