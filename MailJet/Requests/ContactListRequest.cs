using MailJet.Requests.ContactList;

namespace MailJet.Requests
{
    public class ContactListRequest
    {
        public static ContactListGetRequest GET()
        {
            return new ContactListGetRequest();
        }

        public static ContactListGetByIdRequest GET(long id)
        {
            return new ContactListGetByIdRequest(id);
        }
    }
}
