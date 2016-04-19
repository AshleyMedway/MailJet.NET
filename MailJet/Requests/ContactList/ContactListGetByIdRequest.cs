using MailJet.Http;
using System;

namespace MailJet.Requests.ContactList
{
    public class ContactListGetByIdRequest : ApiRequest
    {
        public ContactListGetByIdRequest(long id) : base(String.Format("/REST/contactslist/{0}", id), RequestType.GET) { }
    }
}
