using MailJet.Client.Enum;
using System.Collections.Generic;

namespace MailJet.Client.Request
{
    public class Contact
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public CreateContactAction Action { get; set; }
        public List<TKEYVALUELIST> Properties { get; set; }

        public Contact()
        {
            Properties = new List<TKEYVALUELIST>();
        }

        public void AddProperty(string key, string value)
        {
            Properties.Add(new TKEYVALUELIST { Name = key, Value = value });
        }
    }
}
