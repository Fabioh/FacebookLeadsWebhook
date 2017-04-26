using Microsoft.AspNet.WebHooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WebHookLeads.WebHooks
{
	public class GitHubHandler : WebHookHandler
	{

		public GitHubHandler()
		{
			Receiver		= GitHubWebHookReceiver.ReceiverName;
		}

		public override Task ExecuteAsync(string receiver, WebHookHandlerContext context)
		{
			string action	= context.Actions.First();
			JObject data	= context.GetDataOrDefault<JObject>();
			return Task.FromResult(true);
		}
	}
}