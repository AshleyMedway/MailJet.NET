using MailJet.Client.Enum;
using System.Collections.Generic;

namespace MailJet.Client.Request
{
    public class Contact
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public CreateContactAction Action { get; set; }
        public Dictionary<string, string> Properties { get; set; }

        public Contact()
        {
            Properties = new Dictionary<string, string>();
        }

        public void AddProperty(string key, string value)
        {
            Properties.Add(key, value);
        }
    }
}
