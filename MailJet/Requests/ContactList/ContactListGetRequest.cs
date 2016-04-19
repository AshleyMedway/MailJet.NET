using MailJet.Http;

namespace MailJet.Requests.ContactList
{
    public class ContactListGetRequest : ApiRequest
    {
        public ContactListGetRequest() : base("/REST/contactslist", RequestType.GET) { }
    }
}
