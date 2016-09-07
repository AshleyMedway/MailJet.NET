using MailJet.Client.Request;
using System.Collections.Generic;

namespace MailJet.Client.Response.Data
{
    public class ContactData : DataItem
    {
        public long ContactID { get; set; }
        public List<TKEYVALUELIST> Data { get; set; }
    }
}
