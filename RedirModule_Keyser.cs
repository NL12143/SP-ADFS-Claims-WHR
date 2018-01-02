using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Diagnostics;

namespace AdfsHrd
{
    public class RedirModule : iHttpModule
    {
	    public void Dispose()
	    {
		    //throw new NotImplementedException();
	    }
	    public void Init(HttpApplication context)
	    {
		    try
		    {
                context.EndRequest += new EventHandler(EndRequest);
		    }
		    catch (Exception ex)
		    {
			    Debug.WriteLine(ex.Message);
		    }
	    }

        public void EndRequest(object sender, EventArgs e)
        {
            try
            {
                //get the app and context sources
                HttpApplication application = (HttpApplication)sender;
                HttpContext context = application.Context;

                if (context.Response.IsRequestBeingRedirected == true &&
                    context.Request.QueryString.AllKeys.Contains("whr"))
                {
                    string whr = "whr=" + context.Request.QueryString["whr"];
                    string redir = context.Response.RedirectLocation;
                    if (!redir.Contains(whr))
                    {
                        redir = redir + (redir.Contains("?") ? "&" + whr : "?" + whr);
                        context.Response.RedirectLocation = redir;
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
