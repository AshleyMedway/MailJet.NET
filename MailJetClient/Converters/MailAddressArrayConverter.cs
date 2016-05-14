using MailJet.Client.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace MailJet.Client.Converters
{
    public class MailAddressArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(MailAddress[]) == objectType;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var data = (MailAddress[])value;

            writer.WriteStartArray();
            foreach (MailAddress item in data)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Email");
                writer.WriteValue(item.Address);
                if (!String.IsNullOrWhiteSpace(item.DisplayName))
                {
                    writer.WritePropertyName("Name");
                    writer.WriteValue(item.DisplayName);
                }
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
