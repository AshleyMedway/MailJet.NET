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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using MimeTypes;

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
            ExecuteRequest(request);
        }

        public void DeleteContactList(long ID)
        {
            var request = new RestRequest("REST/contactslist/{id}", Method.DELETE);
            request.AddParameter("id", ID, ParameterType.UrlSegment);
            ExecuteRequest(request);
        }

        public Response<ContactData> UpdateContactData(long ContactID, Dictionary<string, object> Data)
        {
            var request = new RestRequest("REST/contactdata/{id}", Method.PUT);
            request.AddParameter("id", ContactID, ParameterType.UrlSegment);

            var TData = TKEYVALUELIST.FromDictionary(Data);
            var d = new { Data = TData };
            request.AddJsonBody(d);
            return ExecuteRequest<ContactData>(request);
        }

        public Response<ContactData> UpdateContactData(string Email, Dictionary<string, object> Data)
        {
            var request = new RestRequest("REST/contactdata/{email}", Method.PUT);
            request.AddParameter("email", Email, ParameterType.UrlSegment);

            var TData = TKEYVALUELIST.FromDictionary(Data);
            var d = new { Data = TData };
            request.AddJsonBody(d);
            return ExecuteRequest<ContactData>(request);
        }


        public Response<ContactResponse> CreateContactForList(long ID, Contact contact)
        {
            var request = new RestRequest("REST/contactslist/{id}/managecontact", Method.POST);
            request.AddParameter("id", ID, ParameterType.UrlSegment);
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;
            JObject o = new JObject();
            o.Add("name", contact.Name);
            o.Add("email", contact.Email);
            o.Add("action", System.Enum.GetName(typeof(CreateContactAction), contact.Action));

            JObject p = new JObject();
            foreach (var i in contact.Properties)
            {
                p.Add(i.Name, JToken.FromObject(i.Value));
            }
            o.Add("properties", p);

            request.AddJsonBody(o);

            return ExecuteRequest<ContactResponse>(request);
        }

        public Response<ContactMetadata> GetContactMetaData()
        {
            var request = new RestRequest("REST/contactmetadata", Method.GET);
            return ExecuteRequest<ContactMetadata>(request);
        }

        public Response<ContactMetadata> GetContactMetaData(long ID)
        {
            var request = new RestRequest("REST/contactmetadata/{id}", Method.GET);
            request.AddParameter("id", ID, ParameterType.UrlSegment);
            return ExecuteRequest<ContactMetadata>(request);
        }

        public Response<ContactMetadata> GetContactMetaData(string Name, ContactMetadataNameSpace NameSpace)
        {
            var request = new RestRequest("REST/contactmetadata/{namespace}|{name}", Method.GET);
            request.AddParameter("name", Name, ParameterType.UrlSegment);
            var @namespace = System.Enum.GetName(typeof(ContactMetadataNameSpace), NameSpace);
            request.AddParameter("namespace", @namespace, ParameterType.UrlSegment);

            return ExecuteRequest<ContactMetadata>(request);
        }

        public Response<ContactMetadata> CreateContactMetaData(ContactMetadata ContactMetadata)
        {
            if (ContactMetadata.Name.Any(x => x == ' '))
            {
                throw new InvalidOperationException("Name cannot contain a space");
            }
            var request = new RestRequest("REST/contactmetadata", Method.POST);
            request.AddParameter("name", ContactMetadata.Name, ParameterType.GetOrPost);

            var @namespace = System.Enum.GetName(typeof(ContactMetadataNameSpace), ContactMetadata.NameSpace);
            request.AddParameter("namespace", @namespace, ParameterType.GetOrPost);

            var type = System.Enum.GetName(typeof(ContactMetadataDataType), ContactMetadata.Datatype);
            request.AddParameter("datatype", type, ParameterType.GetOrPost);

            return ExecuteRequest<ContactMetadata>(request);
        }

        public Response<ContactMetadata> UpdateContactMetaData(ContactMetadata ContactMetadata)
        {
            if (ContactMetadata.Name.Any(x => x == ' '))
            {
                throw new InvalidOperationException("Name cannot contain a space");
            }

            var request = new RestRequest("REST/contactmetadata/{id}", Method.PUT);
            request.AddParameter("id", ContactMetadata.ID, ParameterType.UrlSegment);
            request.AddParameter("name", ContactMetadata.Name, ParameterType.GetOrPost);

            var @namespace = System.Enum.GetName(typeof(ContactMetadataNameSpace), ContactMetadata.NameSpace);
            request.AddParameter("namespace", @namespace, ParameterType.GetOrPost);

            var type = System.Enum.GetName(typeof(ContactMetadataDataType), ContactMetadata.Datatype);
            request.AddParameter("datatype", type, ParameterType.GetOrPost);

            return ExecuteRequest<ContactMetadata>(request);
        }

        public void DeleteContactMetaData(long ID)
        {
            var request = new RestRequest("REST/contactmetadata/{id}", Method.DELETE);
            request.AddParameter("id", ID, ParameterType.UrlSegment);
            ExecuteRequest(request);
        }

        public Response<DataItem> SendTemplateMessage(long TemplateId, MailAddress To, MailAddress From, string Subject, Dictionary<string, object> Parameters = null, Dictionary<string, string> Properties = null)
        {
            return SendTemplateMessage(TemplateId, new MailAddress[] { To }, From, Subject, Parameters, Properties);
        }

        public String TemplateErrorReporting { get; set; }

        public Response<DataItem> SendTemplateMessage(long TemplateId, MailAddress[] To, MailAddress From, string Subject, Dictionary<string, object> Parameters = null, Dictionary<string, string> Properties = null)
        {
            if (To == null || To.Any(x => String.IsNullOrWhiteSpace(x.Address)))
                throw new ArgumentNullException("To", "You must specify the recipient address");

            if (From == null || String.IsNullOrWhiteSpace(From.Address))
                throw new ArgumentNullException("From", "You must specify the sender");

            if (String.IsNullOrWhiteSpace(Subject))
                throw new ArgumentNullException("Subject", "You must specify the Subject");

            var request = new RestRequest("send/message", Method.POST)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = NewtonsoftJsonSerializer.Default
            };
            JObject o = new JObject();

            o.Add("MJ-TemplateID", TemplateId);
            o.Add("MJ-TemplateLanguage", true);
            if (!String.IsNullOrWhiteSpace(TemplateErrorReporting))
                o.Add("MJ-TemplateErrorReporting", TemplateErrorReporting);

            o.Add("Subject", Subject);
            o.Add("FromEmail", From.Address);

            if (!String.IsNullOrWhiteSpace(From.DisplayName))
                o.Add("FromName", From.DisplayName);

            o.Add("Recipients", JToken.FromObject(To, NewtonsoftJsonSerializer.Default.Serializer));

            if (Properties != null && Properties.Any())
            {
                foreach (var p in Properties)
                {
                    o.Add(p.Key, p.Value);
                }
            }

            if (Parameters != null && Parameters.Any())
            {
                JObject p = new JObject();
                foreach (var i in Parameters)
                {
                    p.Add(i.Key, JToken.FromObject(i.Value));
                }
                o.Add("Vars", p);
            }

            request.AddJsonBody(o);

            return ExecuteRequest<DataItem>(request);
        }

        public SentMessageData SendMessage(MailMessage Message)
        {
            var request = new RestRequest("send/message", Method.POST)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = NewtonsoftJsonSerializer.Default
            };

            JObject message = new JObject();

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
            {
                message.Add("FromEmail", Message.From.Address);
            }
            else
            {
                message.Add("FromEmail", Message.From.Address);
                message.Add("FromName", Message.From.DisplayName);
            }

            message.Add("Subject", Message.Subject);

            string to = "";
            foreach (var address in Message.To)
            {
                if (String.IsNullOrWhiteSpace(address.DisplayName))
                {
                    to += address.Address + ",";
                }
                else
                {
                    to += String.Format("{0} <{1}>,", address.DisplayName, address.Address);
                }
            }
            message.Add("To", to);

            string cc = "";
            foreach (var address in Message.CC)
            {
                if (String.IsNullOrWhiteSpace(address.DisplayName))
                {
                    cc += address.Address + ",";
                }
                else
                {
                    cc += String.Format("{0} <{1}>,", address.DisplayName, address.Address);
                }
            }

            if (!String.IsNullOrWhiteSpace(cc))
                message.Add("CC", cc);

            string bcc = "";
            foreach (var address in Message.Bcc)
            {
                if (String.IsNullOrWhiteSpace(address.DisplayName))
                {
                    bcc += address.Address + ",";
                }
                else
                {
                    bcc += String.Format("{0} <{1}>,", address.DisplayName, address.Address);
                }
            }

            if (!String.IsNullOrWhiteSpace(bcc))
                message.Add("Bcc", bcc);

            if (Message.IsBodyHtml)
                message.Add("Html-part", Message.Body);
            else
                message.Add("Text-part", Message.Body);

            if (Message.Attachments.Any())
            {
                if (Message.Attachments.Sum(x => x.ContentStream.Length) > 15000000)
                    throw new InvalidOperationException("Attachments cannot exceed 15MB. http://dev.mailjet.com/guides/send-api-guide/");

                JArray attachments = new JArray();
                foreach (var item in Message.Attachments)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        item.ContentStream.CopyTo(ms);
                        JObject attachment = new JObject();
                        attachment.Add("Content-type", new JValue(MimeTypeMap.GetMimeType(Path.GetExtension(item.Name))));
                        attachment.Add("Filename", new JValue(item.Name));
                        string file = Convert.ToBase64String(ms.ToArray());
                        attachment.Add("content", new JValue(file));
                        attachments.Add(attachment);
                    }
                }
                message.Add("Attachments", attachments);
            }

            var view = Message.AlternateViews.FirstOrDefault();

            if (view != null && view.LinkedResources != null && view.LinkedResources.Any())
            {
                JArray attachments = new JArray();
                foreach (var item in view.LinkedResources)
                {
                    using (var ms = new MemoryStream())
                    {
                        item.ContentStream.CopyTo(ms);
                        JObject attachment = new JObject();
                        attachment.Add("Content-type", new JValue(MimeTypeMap.GetMimeType(Path.GetExtension(item.ContentId))));
                        attachment.Add("Filename", new JValue(item.ContentId));
                        string file = Convert.ToBase64String(ms.ToArray());
                        attachment.Add("content", new JValue(file));
                        attachments.Add(attachment);
                    }
                }
                message.Add("Inline_attachments", attachments);
            }

            if (Message.Sender != null && !String.IsNullOrWhiteSpace(Message.Sender.Address))
                throw new NotImplementedException("Sender Address not yet supported.");

            request.AddJsonBody(message);

            return SendMessage(request);
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

        /// <summary>
        /// Allows you to list and view the details of a Message (an e-mail) processed by Mailjet
        /// </summary>
        /// <param name="Limit">Limit the number of results, default is 10</param>
        /// <param name="ContactId">Only retrieve message resources for which Contact ID equals the specified value.</param>
        /// <param name="CampaignId">Only retrieve message resources for which Campaign ID equals the specified value.</param>
        /// <param name="DestinationId">Only retrieve message resources for which Destination ID equals the specified value.</param>
        /// <param name="MessageStateId">Only show messages with this state.</param>
        /// <param name="SenderId">Only show messages from this sender.</param>
        /// <returns></returns>
        public Response<MessageData> GetMessages(
            int? Limit = null,
            long? ContactId = null,
            long? CampaignId = null,
            long? DestinationId = null,
            long? MessageStateId = null,
            long? SenderId = null)
        {
            var request = new RestRequest("REST/message", Method.GET);

            if (ContactId.HasValue)
                request.AddQueryParameter("Contact", ContactId.Value.ToString());

            if (CampaignId.HasValue)
                request.AddQueryParameter("Campaign", CampaignId.Value.ToString());

            if (DestinationId.HasValue)
                request.AddQueryParameter("Destination", DestinationId.Value.ToString());

            if (MessageStateId.HasValue)
                request.AddQueryParameter("MessageState", MessageStateId.Value.ToString());

            if (SenderId.HasValue)
                request.AddQueryParameter("Sender", SenderId.Value.ToString());

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

        public Response<TemplateData> GetTemplate(long ID)
        {
            var request = new RestRequest("REST/template/{id}", Method.GET);
            request.AddParameter("id", ID, ParameterType.UrlSegment);
            return ExecuteRequest<TemplateData>(request);
        }

        public Response<TemplateData> GetTemplate(string name)
        {
            var request = new RestRequest("REST/template", Method.GET);
            request.AddParameter("name", name, ParameterType.QueryString);
            return ExecuteRequest<TemplateData>(request);
        }

        public Response<TemplateContent> GetTemplateContent(long ID)
        {
            var request = new RestRequest("REST/template/{id}/detailcontent", Method.GET);
            request.AddParameter("id", ID, ParameterType.UrlSegment);
            return ExecuteRequest<TemplateContent>(request);
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

        public Response<ContactData> GetContactData(long ID)
        {
            var request = new RestRequest("REST/contactdata/{id}", Method.GET);
            request.AddParameter("id", ID, ParameterType.UrlSegment);
            return ExecuteRequest<ContactData>(request);
        }

        /// <summary>
        /// Manage the relationship between a contact and a contactslists.
        /// </summary>
        /// <param name="IsActive">Retrieve only list recipients for which the IsActive property matches the specified value.</param>
        /// <param name="IsBlocked">Retrieve only list recipients for which the contact's IsBlocked property matches the specified value.</param>
        /// <param name="ContactId">Only retrieve listrecipient resources for which Contact ID equals the specified value.</param>
        /// <param name="ContactEmail">Retrieve only list recipients for which the contact's Email property matches the specified value.</param>
        /// <param name="ContactsListId">Retrieve only list recipients for the specified contact list.</param>
        /// <param name="IgnoreDeleted">Remove deleted contacts from the resultset.</param>
        /// <param name="LastActivityAt">Timestamp of last registered activity for this ListRecipient.</param>
        /// <param name="ListName">Retrieve only list recipients for the specified contact list.</param>
        /// <param name="IsOpened">Retrieve only list recipients for which the contact has at least an opened email.</param>
        /// <param name="Status">Retrieve only list recipients for the given status.</param>
        /// <param name="Unsub">Retrieve only list recipients for which the IsUnsubscribed property matches the specified value.</param>
        /// <returns></returns>
        public Response<ListRecipient> GetListRecipient(
            bool? IsActive = null,
            bool? IsBlocked = null,
            long? ContactId = null,
            string ContactEmail = null,
            long? ContactsListId = null,
            bool? IgnoreDeleted = null,
            DateTime? LastActivityAt = null,
            string ListName = null,
            bool? IsOpened = null,
            string Status = null,
            bool? Unsub = null)
        {
            var request = new RestRequest("REST/listrecipient", Method.GET);

            if (IsActive.HasValue)
                request.AddQueryParameter("Active", IsActive.Value.ToString());

            if (IsBlocked.HasValue)
                request.AddQueryParameter("Blocked", IsBlocked.Value.ToString());

            if (ContactId.HasValue)
                request.AddQueryParameter("Contact", ContactId.Value.ToString());

            if (!String.IsNullOrWhiteSpace(ContactEmail))
                request.AddQueryParameter("ContactEmail", ContactEmail);

            if (ContactsListId.HasValue)
                request.AddQueryParameter("ContactsList", ContactsListId.Value.ToString());

            if (IgnoreDeleted.HasValue)
                request.AddQueryParameter("IgnoreDeleted", IgnoreDeleted.Value.ToString());

            if (LastActivityAt.HasValue)
                request.AddQueryParameter("LastActivityAt", LastActivityAt.Value.ToString());

            if (!String.IsNullOrWhiteSpace(ListName))
                request.AddQueryParameter("ListName", ListName);

            if (IsOpened.HasValue)
                request.AddQueryParameter("Opened", IsOpened.Value.ToString());

            if (!String.IsNullOrWhiteSpace(Status))
                request.AddQueryParameter("Status", Status);

            if (Unsub.HasValue)
                request.AddQueryParameter("Unsub", Unsub.Value.ToString());

            return ExecuteRequest<ListRecipient>(request);
        }

        /// <summary>
        /// Get aggregate graph statistics available for this apikey.
        /// </summary>
        /// <param name="CampaignAggregateID">Only show statistics for this aggregation.</param>
        /// <param name="Range">The period of the aggregates (24 hours or 7 days).</param>
        /// <returns>Aggregated campaign statistics grouped over intervals.</returns>
        public Response<AggregateGraphStatistics> GetAggregateGraphStatistics(int? CampaignAggregateID = null, string Range = null)
        {
            var request = new RestRequest("REST/aggregategraphstatistics", Method.GET);

            if (CampaignAggregateID.HasValue)
                request.AddParameter("CampaignAggregateID", CampaignAggregateID.Value);

            if (!String.IsNullOrWhiteSpace(Range))
                request.AddParameter("Range", Range);

            return ExecuteRequest<AggregateGraphStatistics>(request);
        }

        private Response<T> ExecuteRequest<T>(RestRequest request) where T : DataItem
        {
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;

            var result = WebClient.Execute(request);

            if (result.ResponseStatus == ResponseStatus.Completed && (result.StatusCode == HttpStatusCode.NoContent))
                return null;

            var error = JsonConvert.DeserializeObject<ErrorResponse>(result.Content);
            if (!String.IsNullOrWhiteSpace(error.ErrorInfo) || !String.IsNullOrWhiteSpace(error.ErrorMessage))
                throw new Exception(String.Format("{0}\n{1}", error.ErrorMessage, error.ErrorMessage));

            var data = JsonConvert.DeserializeObject<Response<T>>(result.Content);
            return data;
        }

        private SentMessageData SendMessage(RestRequest request)
        {
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;

            var result = WebClient.Execute(request);

            if (result.ResponseStatus == ResponseStatus.Completed && (result.StatusCode == HttpStatusCode.NoContent))
                return null;

            var error = JsonConvert.DeserializeObject<ErrorResponse>(result.Content);
            if (!String.IsNullOrWhiteSpace(error.ErrorInfo) || !String.IsNullOrWhiteSpace(error.ErrorMessage))
                throw new Exception(String.Format("{0}\n{1}", error.ErrorMessage, error.ErrorMessage));

            var data = JsonConvert.DeserializeObject<SentMessageData>(result.Content);
            return data;
        }

        private void ExecuteRequest(RestRequest request)
        {
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;
            var result = WebClient.Execute(request);

            if (result.ResponseStatus == ResponseStatus.Completed && result.StatusCode == HttpStatusCode.NoContent)
                return;

            var error = JsonConvert.DeserializeObject<ErrorResponse>(result.Content);
            if (!String.IsNullOrWhiteSpace(error.ErrorInfo) || !String.IsNullOrWhiteSpace(error.ErrorMessage))
                throw new Exception(String.Format("{0}\n{1}", error.ErrorMessage, error.ErrorMessage));
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

