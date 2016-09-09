using MailJet.Client.Enum;
using System.Collections.Generic;

namespace MailJet.Client.Response.Data
{
    public class TemplateData : DataItem
    {
        public string Author { get; set; }
        public List<string> Categories { get; set; }
        public string Copyright { get; set; }
        public string Description { get; set; }
        public int EditMode { get; set; }
        public bool IsStarred { get; set; }
        public string Name { get; set; }
        public long OwnerId { get; set; }
        public OwnerType OwnerType { get; set; }
        public string Presets { get; set; }
        public List<long> Previews { get; set; }
        public List<string> Purposes { get; set; }
    }
}
