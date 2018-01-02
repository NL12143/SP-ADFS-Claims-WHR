using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Diagnostics;


namespace AdfsHrd
{
	public class RedirModule : IHttpModule
	{

		//default endpoint
		private const string DEFAULT_REALM = "http://geneva.vbtoys.com/adfs/services/trust";
		

		#region required interfaces
		public void Dispose()
		{
			//throw new NotImplementedException();
		}
		#endregion


		public void Init(HttpApplication context)
		{
			try
			{
				context.BeginRequest += new EventHandler(context_BeginRequest);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		void context_BeginRequest(object sender, EventArgs e)
		{
			try
			{
				//get the app and context sources
				HttpApplication application = (HttpApplication)sender;
				HttpContext context = application.Context;

				Uri rqst = context.Request.Url;

				//look for the /_trust/default.aspx page that does not have whr param yet
				if (
					(rqst.ToString().ToLower().Contains("_trust/default.aspx")) &&
					(!context.Request.QueryString.AllKeys.Contains("whr"))
					)
				{
					StringBuilder sb = new StringBuilder(1024);

					//enumerate all query string values
					foreach (string key in context.Request.QueryString.Keys)
					{
						sb.Append(key + "=" + context.Request.QueryString[key] + "&");
					}

					//THIS IS MY SIMPLE REDIRECTION FOR HLD; INSERT YOUR BUSINESS LOGIC
					//HERE TO SEND THE USER TO THE LOCATION THEY SHOULD GO
					//append the whr parameter
					sb.Append("whr=" + DEFAULT_REALM);

					//redirect with the new set of query string values
					string redir = rqst.Scheme + "://" + rqst.Host + ":" +
						rqst.Port.ToString() + rqst.AbsolutePath;
					context.Response.Redirect(redir + "?" + sb.ToString());
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}


	}
}
