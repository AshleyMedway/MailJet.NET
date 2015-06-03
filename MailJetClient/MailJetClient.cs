using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

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

        public bool SendMessage(MailMessage Message)
        {
            using (var client = WebClient)
            {
                var data = new List<KeyValuePair<string, string>>();
                data.Add(new KeyValuePair<string, string>("from", Message.From.Address));
                data.Add(new KeyValuePair<string, string>("subject", Message.Subject));

                var recipientsCount = Message.To.Count;
                recipientsCount += Message.CC.Count;
                recipientsCount += Message.Bcc.Count;

                if (recipientsCount == 0)
                    throw new InvalidOperationException("Must have at least one recipient. http://dev.mailjet.com/guides/send-api-guide/");

                if (recipientsCount > 50)
                    throw new InvalidOperationException("Max Recipients is 50. http://dev.mailjet.com/guides/send-api-guide/");

                foreach (var address in Message.To)
                    data.Add(new KeyValuePair<string, string>("to", address.Address));

                foreach (var address in Message.CC)
                    data.Add(new KeyValuePair<string, string>("cc", address.Address));

                foreach (var address in Message.Bcc)
                    data.Add(new KeyValuePair<string, string>("bcc", address.Address));

                if (Message.IsBodyHtml)
                    data.Add(new KeyValuePair<string, string>("html", Message.Body));
                else
                    data.Add(new KeyValuePair<string, string>("text", Message.Body));

                if (Message.Attachments.Any())
                    throw new NotImplementedException("Attachments not yet supported.");

                if (Message.Sender != null && !String.IsNullOrWhiteSpace(Message.Sender.Address))
                    throw new NotImplementedException("Sender Address not yet supported.");

                var content = new FormUrlEncodedContent(data);
                var task = client.PostAsync("https://api.mailjet.com/v3/send/message", content);
                var result = Task.Run(() => task).Result;
                return result.StatusCode == HttpStatusCode.OK;
            }
        }

        private HttpClient WebClient
        {
            get
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(String.Format("{0}:{1}", _publicKey, _privateKey))));
                return client;
            }
        }
    }
}

