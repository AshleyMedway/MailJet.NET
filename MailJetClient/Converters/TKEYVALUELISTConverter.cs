using MailJet.Client.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MailJet.Client.Converters
{
    public class TKEYVALUELISTConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(List<TKEYVALUELIST>) == objectType;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var data = (List<TKEYVALUELIST>)value;

            writer.WriteStartObject();
            foreach (TKEYVALUELIST item in data)
            {
                writer.WritePropertyName(item.Name);
                writer.WriteValue(item.Value);
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
