using MailJet.Client.Converters;
using MailJet.Client.Enum;
using MailJet.Client.Request;
using MailJet.Client.Response;
using MailJet.Client.Response.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public MailJetClient(string publicKey, string privateKey)
        {
            _publicKey = publicKey;
            _privateKey = privateKey;
        }

        public Func<RestRequest, RestRequest> RequestInterceptor { get; set; }

        public Response<ContactListData> CreateContactList(string name)
        {
            RestRequest request = new RestRequest("REST/contactslist", Method.POST);
            request.AddParameter("name", name, ParameterType.GetOrPost);
            return ExecuteRequest<ContactListData>(request);
        }

        public Response<ContactListData> GetAllContactLists()
        {
            RestRequest request = new RestRequest("REST/contactslist", Method.GET);
            return ExecuteRequest<ContactListData>(request);
        }

        public Response<ContactListData> GetContactList(long id)
        {
            RestRequest request = new RestRequest("REST/contactslist/{id}", Method.GET);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            return ExecuteRequest<ContactListData>(request);
        }

        public Response<ContactListData> GetContactList(string address)
        {
            RestRequest request = new RestRequest("REST/contactslist/{Address}", Method.GET);
            request.AddParameter("Address", address, ParameterType.UrlSegment);
            return ExecuteRequest<ContactListData>(request);
        }

        public void DeleteContactList(string address)
        {
            RestRequest request = new RestRequest("REST/contactslist/{Address}", Method.DELETE);
            request.AddParameter("Address", address, ParameterType.UrlSegment);
            ExecuteRequest(request);
        }

        public void DeleteContactList(long id)
        {
            RestRequest request = new RestRequest("REST/contactslist/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            ExecuteRequest(request);
        }

        public Response<ContactData> UpdateContactData(long contactId, Dictionary<string, object> data)
        {
            RestRequest request = new RestRequest("REST/contactdata/{id}", Method.PUT);
            request.AddParameter("id", contactId, ParameterType.UrlSegment);

            List<TKEYVALUELIST> formattedData = TKEYVALUELIST.FromDictionary(data);
            var d = new { Data = formattedData };
            request.AddJsonBody(d);
            return ExecuteRequest<ContactData>(request);
        }

        public Response<ContactData> UpdateContactData(string email, Dictionary<string, object> data)
        {
            RestRequest request = new RestRequest("REST/contactdata/{email}", Method.PUT);
            request.AddParameter("email", email, ParameterType.UrlSegment);

            List<TKEYVALUELIST> formattedData = TKEYVALUELIST.FromDictionary(data);
            var d = new { Data = formattedData };
            request.AddJsonBody(d);
            return ExecuteRequest<ContactData>(request);
        }


        public Response<ContactResponse> CreateContactForList(long id, Contact contact)
        {
            RestRequest request = new RestRequest("REST/contactslist/{id}/managecontact", Method.POST);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;
            JObject o = new JObject
            {
                {"name", contact.Name},
                {"email", contact.Email},
                {"action", System.Enum.GetName(typeof(CreateContactAction), contact.Action)}
            };

            JObject p = new JObject();
            foreach (TKEYVALUELIST i in contact.Properties)
            {
                p.Add(i.Name, JToken.FromObject(i.Value));
            }
            o.Add("properties", p);

            request.AddJsonBody(o);

            return ExecuteRequest<ContactResponse>(request);
        }

        public Response<ContactMetadata> GetContactMetaData()
        {
            RestRequest request = new RestRequest("REST/contactmetadata", Method.GET);
            return ExecuteRequest<ContactMetadata>(request);
        }

        public Response<ContactMetadata> GetContactMetaData(long id)
        {
            RestRequest request = new RestRequest("REST/contactmetadata/{id}", Method.GET);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            return ExecuteRequest<ContactMetadata>(request);
        }

        public Response<ContactMetadata> GetContactMetaData(string name, ContactMetadataNameSpace nameSpace)
        {
            RestRequest request = new RestRequest("REST/contactmetadata/{namespace}|{name}", Method.GET);
            request.AddParameter("name", name, ParameterType.UrlSegment);
            string @namespace = System.Enum.GetName(typeof(ContactMetadataNameSpace), nameSpace);
            request.AddParameter("namespace", @namespace, ParameterType.UrlSegment);

            return ExecuteRequest<ContactMetadata>(request);
        }

        public Response<ContactMetadata> CreateContactMetaData(ContactMetadata contactMetadata)
        {
            if (contactMetadata.Name.Any(x => x == ' '))
            {
                throw new InvalidOperationException("Name cannot contain a space");
            }
            RestRequest request = new RestRequest("REST/contactmetadata", Method.POST);
            request.AddParameter("name", contactMetadata.Name, ParameterType.GetOrPost);

            string @namespace = System.Enum.GetName(typeof(ContactMetadataNameSpace), contactMetadata.NameSpace);
            request.AddParameter("namespace", @namespace, ParameterType.GetOrPost);

            string type = System.Enum.GetName(typeof(ContactMetadataDataType), contactMetadata.Datatype);
            request.AddParameter("datatype", type, ParameterType.GetOrPost);

            return ExecuteRequest<ContactMetadata>(request);
        }

        public Response<ContactMetadata> UpdateContactMetaData(ContactMetadata contactMetadata)
        {
            if (contactMetadata.Name.Any(x => x == ' '))
            {
                throw new InvalidOperationException("Name cannot contain a space");
            }

            RestRequest request = new RestRequest("REST/contactmetadata/{id}", Method.PUT);
            request.AddParameter("id", contactMetadata.ID, ParameterType.UrlSegment);
            request.AddParameter("name", contactMetadata.Name, ParameterType.GetOrPost);

            string @namespace = System.Enum.GetName(typeof(ContactMetadataNameSpace), contactMetadata.NameSpace);
            request.AddParameter("namespace", @namespace, ParameterType.GetOrPost);

            string type = System.Enum.GetName(typeof(ContactMetadataDataType), contactMetadata.Datatype);
            request.AddParameter("datatype", type, ParameterType.GetOrPost);

            return ExecuteRequest<ContactMetadata>(request);
        }

        public void DeleteContactMetaData(long id)
        {
            RestRequest request = new RestRequest("REST/contactmetadata/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            ExecuteRequest(request);
        }

        public Response<DataItem> SendTemplateMessage(long templateId, MailAddress to, MailAddress from, string subject, Dictionary<string, object> parameters = null)
        {
            return SendTemplateMessage(templateId, new[] { to }, from, subject, parameters);
        }

        public Response<DataItem> SendTemplateMessage(long templateId, MailAddress[] to, MailAddress from, string subject, Dictionary<string, object> parameters = null)
        {
            if (to == null || to.Any(x => string.IsNullOrWhiteSpace(x.Address)))
                throw new ArgumentNullException(nameof(to), "You must specify the recipient address");

            if (string.IsNullOrWhiteSpace(from?.Address))
                throw new ArgumentNullException(nameof(from), "You must specify the sender");

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentNullException(nameof(subject), "You must specify the Subject");

            RestRequest request = new RestRequest("send/message", Method.POST)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = NewtonsoftJsonSerializer.Default
            };
            JObject o = new JObject
            {
                {"MJ-TemplateID", templateId},
                {"MJ-TemplateLanguage", true},
                {"Subject", subject},
                {"FromEmail", from.Address}
            };


            if (!string.IsNullOrWhiteSpace(from.DisplayName))
                o.Add("FromName", from.DisplayName);

            o.Add("Recipients", JToken.FromObject(to, NewtonsoftJsonSerializer.Default.Serializer));

            if (parameters != null && parameters.Any())
            {
                JObject p = new JObject();
                foreach (KeyValuePair<string, object> i in parameters)
                {
                    p.Add(i.Key, JToken.FromObject(i.Value));
                }
                o.Add("Vars", p);
            }

            request.AddJsonBody(o);

            return ExecuteRequest<DataItem>(request);
        }

        public Response<DataItem> SendMessage(MailMessage message)
        {
            RestRequest request = new RestRequest("send/message", Method.POST);

            if (message.From == null)
                throw new InvalidOperationException("You must specify the from address. http://dev.mailjet.com/guides/send-api-guide/");

            if (string.IsNullOrWhiteSpace(message.Subject))
                throw new InvalidOperationException("You must specify the subject address. http://dev.mailjet.com/guides/send-api-guide/");

            if (message.Subject.Length > 255)
                throw new InvalidOperationException("The subject cannot be longer than 255 characters. http://dev.mailjet.com/guides/send-api-guide/");

            int recipientsCount = message.To.Count + message.CC.Count + message.Bcc.Count;

            if (recipientsCount == 0)
                throw new InvalidOperationException("Must have at least one recipient. http://dev.mailjet.com/guides/send-api-guide/");

            if (recipientsCount > 50)
                throw new InvalidOperationException("Max Recipients is 50. http://dev.mailjet.com/guides/send-api-guide/");

            request.AddParameter("from",
                string.IsNullOrWhiteSpace(message.From.DisplayName)
                    ? message.From.Address
                    : $"{message.From.DisplayName} <{message.From.Address}>");

            request.AddParameter("subject", message.Subject);

            foreach (MailAddress address in message.To)
            {
                request.AddParameter("to",
                    string.IsNullOrWhiteSpace(address.DisplayName)
                        ? address.Address
                        : $"\"{address.DisplayName}\" <{address.Address}>");
            }

            foreach (MailAddress address in message.CC)
            {
                request.AddParameter("cc",
                    string.IsNullOrWhiteSpace(address.DisplayName)
                        ? address.Address
                        : $"\"{address.DisplayName}\" <{address.Address}>");
            }

            foreach (MailAddress address in message.Bcc)
            {
                request.AddParameter("bcc",
                    string.IsNullOrWhiteSpace(address.DisplayName)
                        ? address.Address
                        : $"\"{address.DisplayName}\" <{address.Address}>");
            }

            request.AddParameter(message.IsBodyHtml ? "html" : "text", message.Body);

            if (message.Attachments.Any())
            {
                if (message.Attachments.Sum(x => x.ContentStream.Length) > 15000000)
                    throw new InvalidOperationException("Attachments cannot exceed 15MB. http://dev.mailjet.com/guides/send-api-guide/");

                foreach (Attachment item in message.Attachments)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        item.ContentStream.CopyTo(ms);
                        request.AddFile("attachment", ms.ToArray(), item.Name);
                    }
                }
            }

            AlternateView view = message.AlternateViews.FirstOrDefault();

            if (view?.LinkedResources != null && view.LinkedResources.Any())
            {
                foreach (LinkedResource item in view.LinkedResources)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        item.ContentStream.CopyTo(ms);
                        request.AddFile("inlineattachment", ms.ToArray(), item.ContentId, item.ContentType.MediaType);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(message.Sender?.Address))
                throw new NotImplementedException("Sender Address not yet supported.");

            return ExecuteRequest<DataItem>(request);
        }

        public Response<MessageData> GetMessageHistory(long messageId)
        {
            RestRequest request = new RestRequest("REST/messagehistory/{id}", Method.GET);
            request.AddParameter("id", messageId);
            return ExecuteRequest<MessageData>(request);
        }

        public Response<MessageData> GetMessage(long messageId)
        {
            RestRequest request = new RestRequest("REST/message/{id}", Method.GET);
            request.AddParameter("id", messageId);
            return ExecuteRequest<MessageData>(request);
        }

        /// <summary>
        /// Allows you to list and view the details of a Message (an e-mail) processed by Mailjet
        /// </summary>
        /// <param name="limit">Limit the number of results, default is 10</param>
        /// <param name="contactId">Only retrieve message resources for which Contact ID equals the specified value.</param>
        /// <param name="campaignId">Only retrieve message resources for which Campaign ID equals the specified value.</param>
        /// <param name="destinationId">Only retrieve message resources for which Destination ID equals the specified value.</param>
        /// <param name="messageStateId">Only show messages with this state.</param>
        /// <param name="senderId">Only show messages from this sender.</param>
        /// <returns></returns>
        public Response<MessageData> GetMessages(
            int? limit = null,
            long? contactId = null,
            long? campaignId = null,
            long? destinationId = null,
            long? messageStateId = null,
            long? senderId = null)
        {
            RestRequest request = new RestRequest("REST/message", Method.GET);

            if (contactId.HasValue)
                request.AddQueryParameter("Contact", contactId.Value.ToString());

            if (campaignId.HasValue)
                request.AddQueryParameter("Campaign", campaignId.Value.ToString());

            if (destinationId.HasValue)
                request.AddQueryParameter("Destination", destinationId.Value.ToString());

            if (messageStateId.HasValue)
                request.AddQueryParameter("MessageState", messageStateId.Value.ToString());

            if (senderId.HasValue)
                request.AddQueryParameter("Sender", senderId.Value.ToString());

            if (limit.HasValue)
                request.AddParameter("limit", limit.Value);

            return ExecuteRequest<MessageData>(request);
        }

        public Response<DNSData> GetDns(string domain)
        {
            RestRequest request = new RestRequest("REST/dns/{domain}", Method.GET);
            request.AddParameter("domain", domain, ParameterType.UrlSegment);
            return ExecuteRequest<DNSData>(request);
        }

        public Response<DNSData> GetDns(long recordId)
        {
            RestRequest request = new RestRequest("REST/dns/{id}", Method.GET);
            request.AddParameter("id", recordId);
            return ExecuteRequest<DNSData>(request);
        }

        public Response<DNSData> GetDns()
        {
            RestRequest request = new RestRequest("REST/dns", Method.GET);
            return ExecuteRequest<DNSData>(request);
        }

        public Response<DNSCheckData> ForceDnsRecheck(long recordId)
        {
            RestRequest request = new RestRequest("REST/dns/{id}/check", Method.POST);
            request.AddParameter("id", recordId, ParameterType.UrlSegment);
            return ExecuteRequest<DNSCheckData>(request);
        }

        public Response<MetaSenderData> GetMetaSender()
        {
            RestRequest request = new RestRequest("REST/metasender", Method.GET);
            return ExecuteRequest<MetaSenderData>(request);
        }

        public Response<MetaSenderData> GetMetaSender(long senderId)
        {
            RestRequest request = new RestRequest("REST/metasender/{id}", Method.GET);
            request.AddParameter("id", senderId);
            return ExecuteRequest<MetaSenderData>(request);
        }

        public Response<MetaSenderData> GetMetaSender(string email)
        {
            RestRequest request = new RestRequest("REST/metasender/{email}", Method.GET);
            request.AddParameter("email", email, ParameterType.UrlSegment);
            return ExecuteRequest<MetaSenderData>(request);
        }

        public Response<TemplateData> GetTemplate(long id)
        {
            RestRequest request = new RestRequest("REST/template/{id}", Method.GET);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            return ExecuteRequest<TemplateData>(request);
        }

        public Response<TemplateData> GetTemplate(string name)
        {
            RestRequest request = new RestRequest("REST/template", Method.GET);
            request.AddParameter("name", name, ParameterType.QueryString);
            return ExecuteRequest<TemplateData>(request);
        }

        public Response<TemplateContent> GetTemplateContent(long id)
        {
            RestRequest request = new RestRequest("REST/template/{id}/detailcontent", Method.GET);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            return ExecuteRequest<TemplateContent>(request);
        }

        public Response<MetaSenderData> CreateMetaSender(string email, string description = null)
        {
            RestRequest request = new RestRequest("REST/metasender", Method.POST);
            request.AddParameter("email", email);
            if (!string.IsNullOrWhiteSpace(description))
                request.AddParameter("description", description);

            return ExecuteRequest<MetaSenderData>(request);
        }

        public Response<MetaSenderData> UpdateMetaSender(long senderId, string email = null, bool? isEnabled = null, string description = null)
        {
            RestRequest request = new RestRequest("REST/metasender/{id}", Method.PUT);
            request.AddParameter("id", senderId, ParameterType.UrlSegment);
            if (!string.IsNullOrWhiteSpace(email))
                request.AddParameter("email", email);
            if (!string.IsNullOrWhiteSpace(description))
                request.AddParameter("description", description);
            if (isEnabled.HasValue)
                request.AddParameter("isEnabled", isEnabled.Value);

            return ExecuteRequest<MetaSenderData>(request);
        }

        public Response<ContactData> GetContactData(long id)
        {
            RestRequest request = new RestRequest("REST/contactdata/{id}", Method.GET);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            return ExecuteRequest<ContactData>(request);
        }

        /// <summary>
        /// Manage the relationship between a contact and a contactslists.
        /// </summary>
        /// <param name="isActive">Retrieve only list recipients for which the IsActive property matches the specified value.</param>
        /// <param name="isBlocked">Retrieve only list recipients for which the contact's IsBlocked property matches the specified value.</param>
        /// <param name="contactId">Only retrieve listrecipient resources for which Contact ID equals the specified value.</param>
        /// <param name="contactEmail">Retrieve only list recipients for which the contact's Email property matches the specified value.</param>
        /// <param name="contactsListId">Retrieve only list recipients for the specified contact list.</param>
        /// <param name="ignoreDeleted">Remove deleted contacts from the resultset.</param>
        /// <param name="lastActivityAt">Timestamp of last registered activity for this ListRecipient.</param>
        /// <param name="listName">Retrieve only list recipients for the specified contact list.</param>
        /// <param name="isOpened">Retrieve only list recipients for which the contact has at least an opened email.</param>
        /// <param name="status">Retrieve only list recipients for the given status.</param>
        /// <param name="unsub">Retrieve only list recipients for which the IsUnsubscribed property matches the specified value.</param>
        /// <returns></returns>
        public Response<ListRecipient> GetListRecipient(
            bool? isActive = null,
            bool? isBlocked = null,
            long? contactId = null,
            string contactEmail = null,
            long? contactsListId = null,
            bool? ignoreDeleted = null,
            DateTime? lastActivityAt = null,
            string listName = null,
            bool? isOpened = null,
            string status = null,
            bool? unsub = null)
        {
            RestRequest request = new RestRequest("REST/listrecipient", Method.GET);

            if (isActive.HasValue)
                request.AddQueryParameter("Active", isActive.Value.ToString());

            if (isBlocked.HasValue)
                request.AddQueryParameter("Blocked", isBlocked.Value.ToString());

            if (contactId.HasValue)
                request.AddQueryParameter("Contact", contactId.Value.ToString());

            if (!string.IsNullOrWhiteSpace(contactEmail))
                request.AddQueryParameter("ContactEmail", contactEmail);

            if (contactsListId.HasValue)
                request.AddQueryParameter("ContactsList", contactsListId.Value.ToString());

            if (ignoreDeleted.HasValue)
                request.AddQueryParameter("IgnoreDeleted", ignoreDeleted.Value.ToString());

            if (lastActivityAt.HasValue)
                request.AddQueryParameter("LastActivityAt", lastActivityAt.Value.ToString(CultureInfo.InvariantCulture));

            if (!string.IsNullOrWhiteSpace(listName))
                request.AddQueryParameter("ListName", listName);

            if (isOpened.HasValue)
                request.AddQueryParameter("Opened", isOpened.Value.ToString());

            if (!string.IsNullOrWhiteSpace(status))
                request.AddQueryParameter("Status", status);

            if (unsub.HasValue)
                request.AddQueryParameter("Unsub", unsub.Value.ToString());

            return ExecuteRequest<ListRecipient>(request);
        }

        /// <summary>
        /// Get aggregate graph statistics available for this apikey.
        /// </summary>
        /// <param name="campaignAggregateId">Only show statistics for this aggregation.</param>
        /// <param name="range">The period of the aggregates (24 hours or 7 days).</param>
        /// <returns>Aggregated campaign statistics grouped over intervals.</returns>
        public Response<AggregateGraphStatistics> GetAggregateGraphStatistics(int? campaignAggregateId = null, string range = null) {
          RestRequest request = new RestRequest("REST/aggregategraphstatistics", Method.GET);

          if (campaignAggregateId.HasValue)
            request.AddParameter("CampaignAggregateID", campaignAggregateId.Value);

          if (!string.IsNullOrWhiteSpace(range))
            request.AddParameter("Range", range);

          return ExecuteRequest<AggregateGraphStatistics>(request);
        }

        private Response<T> ExecuteRequest<T>(RestRequest request) where T : DataItem
        {
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;
            IRestResponse result = WebClient.Execute(request);

            if (result.ResponseStatus == ResponseStatus.Completed && (result.StatusCode == HttpStatusCode.NoContent))
                return null;

            ErrorResponse error = JsonConvert.DeserializeObject<ErrorResponse>(result.Content);
            if (!string.IsNullOrWhiteSpace(error.ErrorInfo) || !string.IsNullOrWhiteSpace(error.ErrorMessage))
                throw new Exception($"{error.ErrorMessage}\n{error.ErrorMessage}");

            Response<T> data = JsonConvert.DeserializeObject<Response<T>>(result.Content);
            return data;
        }

        private void ExecuteRequest(RestRequest request)
        {
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;
            if (RequestInterceptor != null)
                request = RequestInterceptor(request);
            IRestResponse result = WebClient.Execute(request);

            if (result.ResponseStatus == ResponseStatus.Completed && result.StatusCode == HttpStatusCode.NoContent)
                return;

            ErrorResponse error = JsonConvert.DeserializeObject<ErrorResponse>(result.Content);
            if (!string.IsNullOrWhiteSpace(error.ErrorInfo) || !string.IsNullOrWhiteSpace(error.ErrorMessage))
                throw new Exception($"{error.ErrorMessage}\n{error.ErrorMessage}");
        }

        private RestClient WebClient
        {
            get
            {
                RestClient client = new RestClient("https://api.mailjet.com/v3")
                {
                    Authenticator = new HttpBasicAuthenticator(_publicKey, _privateKey),
                    UserAgent = "MailJet.NET Client"
                };

                return client;
            }
        }
    }
}

