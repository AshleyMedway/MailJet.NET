using System.Collections.Generic;
using System.Linq;

namespace MailJet.Client.Request
{
    public struct TKEYVALUELIST
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public static List<TKEYVALUELIST> FromDictionary(Dictionary<string, string> dictionary)
        {
            return dictionary.Select(x => new TKEYVALUELIST { Name = x.Key, Value = x.Value }).ToList();
        }
    }
}
