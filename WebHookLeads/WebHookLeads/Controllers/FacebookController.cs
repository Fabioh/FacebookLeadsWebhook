using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebHookLeads.Models.Facebook;

namespace WebHookLeads.Controllers
{
	public class FacebookController : Controller
	{
		private JObject _leadsUpdate;
		private AuthResponseVm _authResponseVm;

		[HttpGet]
		public Task<string> Leads()
		{
			var hub_challenge		= Request.QueryString["hub.challenge"];
			var hub_verify_token	= Request.QueryString["hub.verify_token"];
			var hub_mode			= Request.QueryString["hub.mode"];

			if (!string.IsNullOrEmpty(hub_mode))
			{
				if (hub_mode.ToLower() == "subscribe")
				{
					if (!string.IsNullOrEmpty(hub_verify_token)
								&& hub_verify_token.ToLower() == ConfigurationManager.AppSettings["facebook_webhook_sub"])
					{
						//System.Diagnostics.Trace.TraceError($"Faceboo reconheceu com sucesso o webhook hub_challenge: {hub_challenge}");
						System.Diagnostics.Trace.TraceInformation($"Faceboo reconheceu com sucesso o webhook hub_challenge: {hub_challenge}");
						return Task.FromResult(hub_challenge);
					}
					else
					{
						return Task.FromResult(string.Empty);
					}
				}
				else
				{
					return Task.FromResult(string.Empty);
				}
			}
			else
			{
				return Task.FromResult(string.Empty);
			}
		}

		[HttpPost, ActionName("Leads")]
		public Task<HttpStatusCodeResult> LeadsPost()
		{
			var req					= Request.InputStream;
			req.Seek(0, System.IO.SeekOrigin.Begin);
			string json				= new StreamReader(req).ReadToEnd();
			_leadsUpdate			= JObject.Parse(json);
			System.Diagnostics.Trace.TraceInformation($"JSON recebido do Facebook: {json}");

			GetLeadInfo();

			var ret					= new HttpStatusCodeResult(HttpStatusCode.OK);
			return Task.FromResult<HttpStatusCodeResult>(ret);
		}

		[HttpPost]
		public JsonResult StoreLoginResponse(AuthResponseVm authResponseVm)
		{
			// do the logic to save the acces token 
			_authResponseVm = authResponseVm;
			generateLongLivedToken();
			var ret = base.Json(new { result = new { sucess = true } });
			return ret;
		}

		private void GetLeadInfo()
		{
			var client			= new System.Net.Http.HttpClient();
			var apiversion		= ConfigurationManager.AppSettings["facebook_api_version"];
			var leadsIds		= _leadsUpdate["entry"].Select(a => a["changes"]).Children().ToList().Where(x => x["field"].ToString() == "leadgen").Select(k => k["value"]["leadgen_id"].ToString());

			var baseAddress		= $"https://graph.facebook.com/{apiversion}/";
			foreach (var item in leadsIds)
			{
				// TODO: retrieve this token from database based on form_id field, passed by webhook.
				var token		= base.HttpContext.Application["longLivedToken"].ToString();
				var a			= client.GetAsync(baseAddress + item + $"?access_token={token}");
				if (a.Result.IsSuccessStatusCode)
				{
					var leadinfo =  a.Result.Content.ReadAsStringAsync();
					System.Diagnostics.Trace.TraceInformation("Informações do lead json " + leadinfo.Result);
				}
				else
				{
					System.Diagnostics.Trace.TraceError("Deu ruim na hoara de ler os dados do lead");
				}
			}
			
			
		}

		private void generateLongLivedToken()
		{
			try
			{
				var client			= new System.Net.Http.HttpClient();
				var appId			= ConfigurationManager.AppSettings["facebook_app_id"];
				var appSecret		= ConfigurationManager.AppSettings["facebook_app_secret"];
				var shortLivedToken = _authResponseVm.accessToken;
				var urlRequest		= $"https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&client_id={appId}&client_secret={appSecret}&fb_exchange_token={shortLivedToken}";
				System.Diagnostics.Trace.TraceInformation($"urlRequest: {urlRequest}");
				var ret				= client.GetStringAsync(urlRequest);
				var res				= ret.Result;
				var obj				= JObject.Parse(res);
				
				// TODO: the correct way is save this token in database
				System.Diagnostics.Trace.TraceInformation($"userId: {_authResponseVm.userID}");
				base.HttpContext.Application.Add($"longLivedToken", obj["access_token"].Value<string>());
				System.Diagnostics.Trace.TraceInformation($"Json longlived token {res}");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.TraceError($"Exceção ao gerar long lived token: {ex.Message} {Environment.NewLine} StackTrace: {ex.StackTrace}");
				throw;
			}
		}
	}
}