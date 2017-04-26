using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHookLeads.Models.Facebook
{
	public class AuthResponseVm
	{
		public string accessToken { get; set; }
		public string expiresIn { get; set; }
		public string signedRequest { get; set; }
		public string userID { get; set; }

	}
}