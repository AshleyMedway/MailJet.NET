using System;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace MailJet.Client
{
    public static class RequestInterceptorExtensions
    {
        public static void ModifyBody(this RestRequest request, Func<JObject, JObject> bodyModification)
        {
            Parameter bodyParameter = request.Parameters.Find(param => param.Type == ParameterType.RequestBody);
            string body = bodyParameter.Value.ToString();
            JObject jObject = bodyModification(JObject.Parse(body));
            bodyParameter.Value = jObject.ToString();
        }
    }
}