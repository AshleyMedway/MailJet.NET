using MailJet.Client.Response.Data;
using System.Collections.Generic;

namespace MailJet.Client.Response
{
    public class Response<T> where T : DataItem
    {
        public List<T> Data { get; set; }
        public int Count { get; set; }
        public int Total { get; set; }
    }
}
