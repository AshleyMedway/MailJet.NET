using MailJet.Client.Enum;

namespace MailJet.Client.Response.Data
{
    public class ContactMetadata : DataItem
    {
        public ContactMetadataDataType Datatype { get; set; }
        public string Name { get; set; }
        public ContactMetadataNameSpace NameSpace { get; set; }
    }
}
