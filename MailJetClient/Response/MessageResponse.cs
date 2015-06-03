using MailJet.Client.Response.Data;
using System.Collections.Generic;

namespace MailJet.Client.Response
{
    public class MessageResponse : BaseResponse
    {
        public List<MessageData> Data { get; set; }
    }
}
