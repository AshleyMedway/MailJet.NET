using System;
using System.Net.Mail;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Linq;

namespace MailJet.Client
{
	public class MailJetClient
	{
		private readonly string _publicKey;
		private readonly string _privateKey;

		public MailJetClient (string PublicKey, string PrivateKey)
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

				foreach (var address in Message.To)
					data.Add(new KeyValuePair<string, string>("to", address.Address));

				if (Message.IsBodyHtml)
					data.Add(new KeyValuePair<string, string>("html", Message.Body));
				else
					data.Add(new KeyValuePair<string, string>("text", Message.Body));

				if (Message.Attachments.Any())
					throw new InvalidOperationException("Attachments not yet supported.");

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

