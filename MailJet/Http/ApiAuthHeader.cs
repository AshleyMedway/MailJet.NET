using System;
using System.Net.Http.Headers;
using System.Text;

namespace MailJet.Http
{
    internal class ApiAuthHeader : AuthenticationHeaderValue
    {
        internal ApiAuthHeader(string publicKey, string privateKey) : base("basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", publicKey, privateKey))))
        {

        }
    }
}
