# sp-adfs-whr 

Automatic home realm discovery via whr module.  
Automticaly direct users to their identity provider. 
The default "show list of partners" is considered unsecure and not user friendly.  
Required for partner federation with SharePoint


# References 

Steve Peschka 
https://samlman.wordpress.com/2015/03/01/using-the-whr-parameter-with-sharepoint-2010-and-saml-auth/
In my case I wrote an HttpModule to append the WHR parameter.  Specifically here is how I did it:

Chris Keyser 
SharePoint 2010 claims and Home Realm Discover – passing whr on the url to SharePoint
https://blogs.msdn.microsoft.com/chriskeyser/2011/10/02/sharepoint-2010-claims-and-home-realm-discover-passing-whr-on-the-url-to-sharepoint/

Home Realm Discovery is a process to select the trusted identity provider in a federated claims scenario where there is more than one provider that can authenticate users.  The default experience is to have the user select which claims provider to use to authenticate.  Often organizations would like to avoid making their users perform this decision.  There are a few ways to automate this selection, one of which is through providing a hint on the url to the federation server (in the form of a whr parameter) that is then used by the federation server to pre-select the identity provider.  
