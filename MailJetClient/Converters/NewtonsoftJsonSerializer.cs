using Newtonsoft.Json;
using RestSharp.Serializers;
using System.IO;

namespace MailJet.Client.Converters
{
    public class NewtonsoftJsonSerializer : ISerializer
    {
        public NewtonsoftJsonSerializer(Newtonsoft.Json.JsonSerializer serializer)
        {
            Serializer = serializer;
        }

        public string ContentType
        {
            get { return "application/json"; } // Probably used for Serialization?
            set { }
        }

        public Newtonsoft.Json.JsonSerializer Serializer { get; }

        public string DateFormat { get; set; }

        public string Namespace { get; set; }

        public string RootElement { get; set; }

        public string Serialize(object obj)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    Serializer.Serialize(jsonTextWriter, obj);

                    return stringWriter.ToString();
                }
            }
        }

        public T Deserialize<T>(RestSharp.IRestResponse response)
        {
            string content = response.Content;

            using (StringReader stringReader = new StringReader(content))
            {
                using (JsonTextReader jsonTextReader = new JsonTextReader(stringReader))
                {
                    return Serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }

        public static NewtonsoftJsonSerializer Default
        {
            get
            {
                Newtonsoft.Json.JsonSerializer json = new Newtonsoft.Json.JsonSerializer()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                };
                json.Converters.Insert(0, new TKEYVALUELISTConverter());
                json.Converters.Insert(0, new MailAddressArrayConverter());
                NewtonsoftJsonSerializer serializer = new NewtonsoftJsonSerializer(json);
                return serializer;
            }
        }
    }
}
