using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebHookLeads.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}


		//[HttpGet]
		//public Task<string> FaceBookCheck()
		//{
		//	var hub_challenge		= Request.QueryString["hub.challenge"];
		//	var hub_verify_token	= Request.QueryString["hub.verify_token"];
		//	var hub_mode			= Request.QueryString["hub.mode"];

		//	if (!string.IsNullOrEmpty(hub_mode))
		//	{
		//		if (hub_mode.ToLower() == "subscribe")
		//		{
		//			if (!string.IsNullOrEmpty(hub_verify_token)
		//						&& hub_verify_token.ToLower() == ConfigurationManager.AppSettings["facebook_webhook_sub"])
		//			{
		//				return Task.FromResult(hub_challenge);
		//			}
		//			else
		//			{
		//				return Task.FromResult(string.Empty);
		//			} 
		//		}
		//		else
		//		{
		//			return Task.FromResult(string.Empty);
		//		}
		//	}
		//	else
		//	{
		//		return Task.FromResult(string.Empty);
		//	}
		//}

		//[HttpPost, ActionName("FaceBookCheck")]
		//public Task<HttpStatusCodeResult> FaceBookPost()
		//{
		//	var req = Request.InputStream;
		//	req.Seek(0, System.IO.SeekOrigin.Begin);

		//	string json = new StreamReader(req).ReadToEnd();

		//	System.IO.File.AppendAllText(@"D:\log.txt", json);

		//	var ret = new HttpStatusCodeResult(HttpStatusCode.OK);
		//	return Task.FromResult<HttpStatusCodeResult>(ret);
		//}
	}
}