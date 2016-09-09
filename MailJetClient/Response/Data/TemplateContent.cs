
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MailJet.Client.Response.Data
{
    public class TemplateContent : DataItem
    {
        [JsonProperty(PropertyName = "Text-part")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "Html-part")]
        public string Html { get; set; }

        public Dictionary<string, object> MJMLContent { get; set; }

        public Dictionary<string, string> Headers { get; set; }
    } 
}
