using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextExtractor.Helpers.Models
{
	public class SmtpSettings
	{
		public string RelativityInstanceFromEmailAddress { get; set; }
		public string Server { get; set; }
		public int Port { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
	}
}
