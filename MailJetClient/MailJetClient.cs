using MailJet.Client.Response;
using Newtonsoft.Json;
using RestSharp;
using System;
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

        public SendResponse SendMessage(MailMessage Message)
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
                    request.AddFile("inline_attachments", x => item.ContentStream.CopyTo(x), item.ContentId);
            }

            if (Message.Sender != null && !String.IsNullOrWhiteSpace(Message.Sender.Address))
                throw new NotImplementedException("Sender Address not yet supported.");

            var response = WebClient.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw response.ErrorException;

            var data = JsonConvert.DeserializeObject<SendResponse>(response.Content);
            return data;
        }

        public MessageHistoryResponse GetMessageHistory(long MessageId)
        {
            var request = new RestRequest("REST/messagehistory/{id}", Method.GET);
            request.AddParameter("id", MessageId);
            var result = WebClient.Execute(request);
            var data = JsonConvert.DeserializeObject<MessageHistoryResponse>(result.Content);
            return data;
        }

        public MessageResponse GetMessage(long MessageId)
        {
            var request = new RestRequest("REST/message/{id}", Method.GET);
            request.AddParameter("id", MessageId);
            var result = WebClient.Execute(request);
            var data = JsonConvert.DeserializeObject<MessageResponse>(result.Content);
            return data;
        }

        public MessageResponse GetMessages(int? Limit = null)
        {
            var request = new RestRequest("REST/message", Method.GET);
            if (Limit.HasValue)
                request.AddParameter("limit", Limit.Value);

            var result = WebClient.Execute(request);
            var data = JsonConvert.DeserializeObject<MessageResponse>(result.Content);
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

