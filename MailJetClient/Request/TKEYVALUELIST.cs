using System.Collections.Generic;
using System.Linq;

namespace MailJet.Client.Request
{
    public struct TKEYVALUELIST
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public static List<TKEYVALUELIST> FromDictionary(Dictionary<string, object> dictionary)
        {
            return dictionary.Select(x => new TKEYVALUELIST { Name = x.Key, Value = x.Value }).ToList();
        }
    }
}
