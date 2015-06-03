using MailJet.Client.Response.Data;
using System.Collections.Generic;

namespace MailJet.Client.Response
{
    public class MessageHistoryResponse : BaseResponse
    {
        public List<MessageHistoryData> Data { get; set; }
    }
}
