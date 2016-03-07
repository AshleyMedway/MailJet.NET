using MailJet.Client.Enum;
using MailJet.Client.Request;
using MailJet.Client.Response;
using MailJet.Client.Response.Data;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace MailJet.Client
{
    public class MailJetClient
    {
        private readonly string _publicKey;
        private readonly string _privateKey;

        public MailJetClient(string PublicKey, string PrivateKey)
        {
            _publicKey = PublicKey;
            _privateKey = PrivateKey;
        }

        public Response<ContactListData> CreateContactList(string Name)
        {
            var request = new RestRequest("REST/contactslist", Method.POST);
            request.AddParameter("name", Name, ParameterType.GetOrPost);
            return ExecuteRequest<ContactListData>(request);
        }

        public Response<ContactListData> GetAllContactLists()
        {
            var request = new RestRequest("REST/contactslist", Method.GET);
            return ExecuteRequest<ContactListData>(request);
        }

        public Response<ContactListData> GetContactList(long ID)
        {
            var request = new RestRequest("REST/contactslist/{id}", Method.GET);
            request.AddParameter("id", ID, ParameterType.UrlSegment);
            return ExecuteRequest<ContactListData>(request);
        }

        public Response<ContactListData> GetContactList(string Address)
        {
            var request = new RestRequest("REST/contactslist/{Address}", Method.GET);
            request.AddParameter("Address", Address, ParameterType.UrlSegment);
            return ExecuteRequest<ContactListData>(request);
        }

        public void DeleteContactList(string Address)
        {
            var request = new RestRequest("REST/contactslist/{Address}", Method.DELETE);
            request.AddParameter("Address", Address, ParameterType.UrlSegment);
        }

        public void DeleteContactList(long ID)
        {
            var request = new RestRequest("REST/contactslist/{id}", Method.DELETE);
            request.AddParameter("id", ID, ParameterType.UrlSegment);
        }

        public Response<ContactListData> CreateContactForList(long ID, Contact contact)
        {
            var request = new RestRequest("REST/contactslist/{id}/managecontact", Method.POST);
            request.AddParameter("id", ID, ParameterType.UrlSegment);
            request.AddParameter("name", contact.Name, ParameterType.GetOrPost);
            request.AddParameter("email", contact.Email, ParameterType.GetOrPost);
            request.AddParameter("properties", contact.Properties, ParameterType.GetOrPost);
            request.AddParameter("action", System.Enum.GetName(typeof(CreateContactAction), contact.Action), ParameterType.GetOrPost);

            return ExecuteRequest<ContactListData>(request);
        }

        public Response<DataItem> SendMessage(MailMessage Message)
        {
            var request = new RestRequest("send/message", Method.POST);

            if (Message.From == null)
                throw new InvalidOperationException("You must specify the from address. http://dev.mailjet.com/guides/send-api-guide/");

            if (String.IsNullOrWhiteSpace(Message.Subject))
                throw new InvalidOperationException("You must specify the subject address. http://dev.mailjet.com/guides/send-api-guide/");

            if (Message.Subject.Length > 255)
                throw new InvalidOperationException("The subject cannot be longer than 255 characters. http://dev.mailjet.com/guides/send-api-guide/");

            var recipientsCount = Message.To.Count + Message.CC.Count + Message.Bcc.Count;

            if (recipientsCount == 0)
                throw new InvalidOperationException("Must have at least one recipient. http://dev.mailjet.com/guides/send-api-guide/");

            if (recipientsCount > 50)
                throw new InvalidOperationException("Max Recipients is 50. http://dev.mailjet.com/guides/send-api-guide/");

            if (String.IsNullOrWhiteSpace(Message.From.DisplayName))
                request.AddParameter("from", Message.From.Address);
            else
                request.AddParameter("from", String.Format("{0} <{1}>", Message.From.DisplayName, Message.From.Address));

            request.AddParameter("subject", Message.Subject);

            foreach (var address in Message.To)
            {
                if (String.IsNullOrWhiteSpace(address.DisplayName))
                    request.AddParameter("to", address.Address);
                else
                    request.AddParameter("to", String.Format("\"{0}\" <{1}>", address.DisplayName, address.Address));
            }

            foreach (var address in Message.CC)
            {
                if (String.IsNullOrWhiteSpace(address.DisplayName))
                    request.AddParameter("cc", address.Address);
                else
                    request.AddParameter("cc", String.Format("\"{0}\" <{1}>", address.DisplayName, address.Address));
            }

            foreach (var address in Message.Bcc)
            {
                if (String.IsNullOrWhiteSpace(address.DisplayName))
                    request.AddParameter("bcc", address.Address);
                else
                    request.AddParameter("bcc", String.Format("\"{0}\" <{1}>", address.DisplayName, address.Address));
            }

            if (Message.IsBodyHtml)
                request.AddParameter("html", Message.Body);
            else
                request.AddParameter("text", Message.Body);

            if (Message.Attachments.Any())
            {
                if (Message.Attachments.Sum(x => x.ContentStream.Length) > 15000000)
                    throw new InvalidOperationException("Attachments cannot exceed 15MB. http://dev.mailjet.com/guides/send-api-guide/");

                foreach (var item in Message.Attachments)
                    request.AddFile("attachment", x => item.ContentStream.CopyTo(x), item.Name);
            }

            var view = Message.AlternateViews.FirstOrDefault();

            if (view != null && view.LinkedResources != null && view.LinkedResources.Any())
            {
                foreach (var item in view.LinkedResources)
                {
                    using (var ms = new MemoryStream())
                    {
                        item.ContentStream.CopyTo(ms);
                        request.AddFile("inlineattachment", ms.ToArray(), item.ContentId, item.ContentType.MediaType);
                    }
                }
            }

            if (Message.Sender != null && !String.IsNullOrWhiteSpace(Message.Sender.Address))
                throw new NotImplementedException("Sender Address not yet supported.");

            var response = WebClient.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw response.ErrorException ?? new Exception(response.StatusDescription);

            return ExecuteRequest<DataItem>(request);
        }

        public Response<MessageData> GetMessageHistory(long MessageId)
        {
            var request = new RestRequest("REST/messagehistory/{id}", Method.GET);
            request.AddParameter("id", MessageId);
            return ExecuteRequest<MessageData>(request);
        }

        public Response<MessageData> GetMessage(long MessageId)
        {
            var request = new RestRequest("REST/message/{id}", Method.GET);
            request.AddParameter("id", MessageId);
            return ExecuteRequest<MessageData>(request);
        }

        public Response<MessageData> GetMessages(int? Limit = null)
        {
            var request = new RestRequest("REST/message", Method.GET);
            if (Limit.HasValue)
                request.AddParameter("limit", Limit.Value);

            return ExecuteRequest<MessageData>(request);
        }

        public Response<DNSData> GetDNS(string Domain)
        {
            var request = new RestRequest("REST/dns/{domain}", Method.GET);
            request.AddParameter("domain", Domain, ParameterType.UrlSegment);
            return ExecuteRequest<DNSData>(request);
        }

        public Response<DNSData> GetDNS(long RecordId)
        {
            var request = new RestRequest("REST/dns/{id}", Method.GET);
            request.AddParameter("id", RecordId);
            return ExecuteRequest<DNSData>(request);
        }

        public Response<DNSData> GetDNS()
        {
            var request = new RestRequest("REST/dns", Method.GET);
            return ExecuteRequest<DNSData>(request);
        }

        public Response<DNSCheckData> ForceDNSRecheck(long RecordId)
        {
            var request = new RestRequest("REST/dns/{id}/check", Method.POST);
            request.AddParameter("id", RecordId, ParameterType.UrlSegment);
            return ExecuteRequest<DNSCheckData>(request);
        }

        public Response<MetaSenderData> GetMetaSender()
        {
            var request = new RestRequest("REST/metasender", Method.GET);
            return ExecuteRequest<MetaSenderData>(request);
        }

        public Response<MetaSenderData> GetMetaSender(long SenderId)
        {
            var request = new RestRequest("REST/metasender/{id}", Method.GET);
            request.AddParameter("id", SenderId);
            return ExecuteRequest<MetaSenderData>(request);
        }

        public Response<MetaSenderData> GetMetaSender(string Email)
        {
            var request = new RestRequest("REST/metasender/{email}", Method.GET);
            request.AddParameter("email", Email, ParameterType.UrlSegment);
            return ExecuteRequest<MetaSenderData>(request);
        }

        public Response<MetaSenderData> CreateMetaSender(string Email, string Description = null)
        {
            var request = new RestRequest("REST/metasender", Method.POST);
            request.AddParameter("email", Email);
            if (!String.IsNullOrWhiteSpace(Description))
                request.AddParameter("description", Description);

            return ExecuteRequest<MetaSenderData>(request);
        }

        public Response<MetaSenderData> UpdateMetaSender(long SenderId, string Email = null, bool? IsEnabled = null, string Description = null)
        {
            var request = new RestRequest("REST/metasender/{id}", Method.PUT);
            request.AddParameter("id", SenderId, ParameterType.UrlSegment);
            if (!String.IsNullOrWhiteSpace(Email))
                request.AddParameter("email", Email);
            if (!String.IsNullOrWhiteSpace(Description))
                request.AddParameter("description", Description);
            if (IsEnabled.HasValue)
                request.AddParameter("isEnabled", IsEnabled.Value);

            return ExecuteRequest<MetaSenderData>(request);
        }

        private Response<T> ExecuteRequest<T>(RestRequest request) where T : DataItem
        {
            request.RequestFormat = DataFormat.Json;
            var result = WebClient.Execute(request);

            if (result.ResponseStatus == ResponseStatus.Completed && result.StatusCode == HttpStatusCode.NoContent)
                return null;

            var error = JsonConvert.DeserializeObject<ErrorResponse>(result.Content);
            if (!String.IsNullOrWhiteSpace(error.ErrorInfo) || !String.IsNullOrWhiteSpace(error.ErrorMessage))
                throw new Exception(String.Format("{0}\n{1}", error.ErrorMessage, error.ErrorMessage));

            var data = JsonConvert.DeserializeObject<Response<T>>(result.Content);
            return data;
        }

        private RestClient WebClient
        {
            get
            {
                var client = new RestClient("https://api.mailjet.com/v3")
                {
                    Authenticator = new HttpBasicAuthenticator(_publicKey, _privateKey),
                    UserAgent = "MailJet.NET Client"
                };

                return client;
            }
        }
    }
}

