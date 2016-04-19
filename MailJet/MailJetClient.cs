using MailJet.Http;
using MailJet.Requests;
using MailJet.Requests.ContactList;
using MailJet.Response;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MailJet
{
    public class MailJetClient : IDisposable
    {
        private readonly HttpClient _client;

        public MailJetClient(string publicKey, string privateKey)
        {
            _client = new HttpClient();
            ConfigureHttpClient(publicKey, privateKey);
        }

        public MailJetClient(string publicKey, string privateKey, HttpClient client)
        {
            _client = client;
            ConfigureHttpClient(publicKey, privateKey);
        }

        private void ConfigureHttpClient(string publicKey, string privateKey)
        {
            _client.DefaultRequestHeaders.Authorization = new ApiAuthHeader(publicKey, privateKey);
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("MailJet.NET V2");
            _client.BaseAddress = new Uri("https://api.mailjet.com/v3");
        }

        public async Task<List<ContactListData>> ExecuteRequest(ContactListGetRequest request)
        {
            return await ExecuteRequest<List<ContactListData>>(request);
        }

        public async Task<ContactListData> ExecuteRequest(ContactListGetByIdRequest request)
        {
            return await ExecuteRequest<ContactListData>(request);
        }

        public async Task<T> ExecuteRequest<T>(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request.RequestType == RequestType.GET)
            {
                response = await _client.GetAsync(request.Url);
            }
            else if (request.RequestType == RequestType.POST)
            {
                throw new NotImplementedException();
            }
            else if (request.RequestType == RequestType.PUT)
            {
                throw new NotImplementedException();
            }
            else if (request.RequestType == RequestType.DELETE)
            {
                throw new NotImplementedException();
            }


            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (_client != null)
                _client.Dispose();
        }
    }
}
