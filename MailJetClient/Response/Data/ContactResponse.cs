using MailJet.Client.Enum;
using System.Collections.Generic;

namespace MailJet.Client.Response.Data
{
    public class ContactResponse : DataItem
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public CreateContactAction Action { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }
}
