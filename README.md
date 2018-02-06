# sp-adfs-whr 

Redirect users to their identity provider, using ADFS as the hub. 
Home realm discovery for SharePoint users via a whr http module.  

Required for partner federation with SharePoint not using the default setup to show list of partners. 
A "show list of partners" is considered unsecure and not user friendly.  

For now it adds a static WHR value. Will add  code that takes the users UPN (email) and insert the corresponding, dynamic, WHR. 

SiteURL = site.portima.contoso.com 
STS = ADFS Signing
ADFS =  adfs.contoso.com
PartnerIP = PI2/IP


SharePoint, anonymous, adds &whr and redirect to ADFS 
https://site.portima.contoso.com/

https://site.portima.contoso.com/_layouts/15/Authenticate.aspx?Source=%2F

https://site.portima.contoso.com/_login/default.aspx?ReturnUrl=%2f_layouts%2f15%2fAuthenticate.aspx%3fSource%3d%252F&Source=%2F

https://site.portima.contoso.com/_trust/default.aspx?trust=ADFS%20Signing&ReturnUrl=%2f_layouts%2f15%2fAuthenticate.aspx%3fSource%3d%252F&Source=%2F

https://site.portima.contoso.com/_trust/default.aspx?trust=ADFS+Signing&ReturnUrl=%2f_layouts%2f15%2fAuthenticate.aspx%3fSource%3d%252F&Source=%2f&whr=PI2/IP

SharePoint redirects to ADFS
https://adfs.contoso.com/adfs/ls/ 
?wa=wsignin1.0
&wtrealm=urn%3asite.contoso.com
&wctx=https%3a%2f%2fsite.portima.contoso.com%2f_layouts%2f15%2fAuthenticate.aspx%3fSource%3d%252F
&wreply=https%3a%2f%2fsite.portima.contoso.com%2f_trust%2fdefault.aspx
&whr=PI2%2fIP

ADFS redirects to IP, using whr to indicate partner 
https://saml-ip.partner.be/Pi2/Ip/css/TAuth-TGS_TicketGetSaml.aspx?SAMLRequest=7b0.....H47%2bHw%3d%3d&RelayState=89e1289e-8fbc-4256-9695-1bb7559b09a7

Use Fiddler, Textwizard to decode SAML 
<samlp:AuthnRequest ID="id-9bc8ea05-8ff5-4746-8646-30b14bfa88fe" Version="2.0" IssueInstant="2018-02-06T11:37:27.739Z" 
 Destination="https://saml-ip.partner.be/Pi2/Ip/css/TAuth-TGS_TicketGetSaml.aspx" 
 Consent="urn:oasis:names:tc:SAML:2.0:consent:unspecified" 
 xmlns:samlp="urn:oasis:names:tc:SAML:2.0:protocol"> 
 <Issuer xmlns="urn:oasis:names:tc:SAML:2.0:assertion">http://adfs.contoso.com/adfs/services/trust</Issuer>
 <samlp:NameIDPolicy Format="urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified" AllowCreate="true" />
</samlp:AuthnRequest>

IP redirects back to ADFS
https://adfsqual.asweb.portima.aginsurance.be/adfs/ls/

ADFS redirect back to SharePoint, now with valid token 
https://site.portima.contoso.com/_trust/default.aspx?
https://site.portima.contoso.com/_layouts/15/Authenticate.aspx?Source=%2F

SharePoint, authenticated, redirects to default homepage 
https://site.portima.contoso.com/
https://site.portima.contoso.com/Pages/default.aspx


## References 

Steve Peschka 
https://samlman.wordpress.com/2015/03/01/using-the-whr-parameter-with-sharepoint-2010-and-saml-auth/
In my case I wrote an HttpModule to append the WHR parameter.  Specifically here is how I did it:

Chris Keyser 
SharePoint 2010 claims and Home Realm Discover – passing whr on the url to SharePoint
https://blogs.msdn.microsoft.com/chriskeyser/2011/10/02/sharepoint-2010-claims-and-home-realm-discover-passing-whr-on-the-url-to-sharepoint/

Home Realm Discovery is a process to select the trusted identity provider in a federated claims scenario where there is more than one provider that can authenticate users.  The default experience is to have the user select which claims provider to use to authenticate.  Often organizations would like to avoid making their users perform this decision.  There are a few ways to automate this selection, one of which is through providing a hint on the url to the federation server (in the form of a whr parameter) that is then used by the federation server to pre-select the identity provider.  

# A Guide to Claims based Identity and Access Control 
https://msdn.microsoft.com/en-us/library/hh446526.aspx 
##Chapter 12 Federated Identity for SharePoint Applications 

## Remove session when closing the Browser
The default behavior for SharePoint is to use persistent session cookies. This enables a user to close the browser, re-open the browser, and re-visit a SharePoint web application without signing in again. Adatum wants users to always re-authenticate if they close the browser and then re-open it and revisit the a-Portal web application. To enforce this behavior, Adatum configured SharePoint to use an in-memory instead of a persistent session cookie. You can change this in the SPSecurityTokenServiceConfig (STS).  
the following PowerShell script to do this.

## Claims to enable role based access 
With multiple partners having access to the a-Portal SharePoint web application, Adatum wants to have the ability to restrict access to documents in the SharePoint document library based on the organization that the user belongs to. Adatum wants to be able to use the standard SharePoint groups mechanism for assigning and managing permissions, so it needs some way to identify the organization a user belongs to. Adatum achieves this objective by using claims. Adatum has configured ADFS to add an organization claim to the SAML token it issues based on the federated identity provider that originally authenticated the user. You should not rely on the identity provider to issue the organization claim because a malicious administrator at a partner organization could add an organization claim with another partner's value and gain access to confidential data. 

## Home Realm Discovery
If Adatum shares the a-Portal web application with multiple partners, each of those partners will have its own identity provider, as shown in Figure 2 earlier in this chapter. With multiple identity providers in place, there must be some mechanism for selecting the correct identity provider for a user to use for authentication, and that's the home-realm discovery process.
Adatum decided to customize the home-realm discovery page that ADFS provides. The default page in ADFS HomeRealmDiscovery.aspx displays a drop-down list of the claims provider trusts configured in ADFS (claims provider trusts represent identity providers in ADFS) for the user to select an identity provider. ADFS then redirects the user to the sign-in page at the identity provider. It's easy to customize this page with partner logos to make it easier for users to select the correct identity provider. In addition, this page in ADFS has access to the relying party identifier in the *&wtrealm* parameter so it can customize the list of identity providers based on the identity of the SharePoint relying party web application. After a user has selected an identity provider for the first time, ADFS can remember the choice so that in the future, the user bypasses the home-realm discovery page and redirects the browser directly to the identity provider's sign-in page.

