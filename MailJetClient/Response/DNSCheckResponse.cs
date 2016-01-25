using System;
using System.Collections.Generic;

namespace MailJet.Client
{
	public class DNSCheckResponse
	{
		public int Count { get; set; }
		public List<DNSCheckData> Data { get; set; }
	}
}

