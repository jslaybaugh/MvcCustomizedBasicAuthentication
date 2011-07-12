using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;
using System.Security.Principal;
using System.Text;
using CustomizedBasic.Models;

namespace CustomizedBasic.Code
{
	public class CustomBasicAuthorizeAttribute: AuthorizeAttribute
	{
		bool _RequireSsl = true;
		public bool RequireSsl
		{
			get { return _RequireSsl; }
			set { _RequireSsl = value; }
		}

		// These lines of code are only required when using Ninject for
		// constructor dependency injection with a parameterless constructor
		// of this attribute.
		[Inject]
		public IUserRepository Repository { get; set; }
		public CustomBasicAuthorizeAttribute()
		{
			// NinjectHelper is a static helper class that just exposes the current kernel object
			// check the code in NinjectHttpApplicationModule.cs to see where the kernel is initialized
			NinjectHelper.Kernel.Inject(this);
		}
		// end DI code

		private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
		{
			validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
		}

		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext == null) throw new ArgumentNullException("filterContext");
			
			if (!Authenticate(filterContext.HttpContext))
			{
				// HttpCustomBasicUnauthorizedResult inherits from HttpUnauthorizedResult and does the 
				// work of displaying the basic authentication prompt to the client
				filterContext.Result = new HttpCustomBasicUnauthorizedResult();
			}
			else
			{
				// AuthorizeCore is in the base class and does the work of checking if we have
				// specified users or roles when we use our attribute
				if (AuthorizeCore(filterContext.HttpContext))
				{
					HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
					cachePolicy.SetProxyMaxAge(new TimeSpan(0));
					cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
				}
				else
				{
					// auth failed, display login

					// HttpCustomBasicUnauthorizedResult inherits from HttpUnauthorizedResult and does the 
					// work of displaying the basic authentication prompt to the client
					filterContext.Result = new HttpCustomBasicUnauthorizedResult();
				}
			}
		}

		// from here on are private methods to do the grunt work of parsing/verifying the credentials

		private bool Authenticate(HttpContextBase context)
		{
			if (_RequireSsl && !context.Request.IsSecureConnection && !context.Request.IsLocal) return false;

			if (!context.Request.Headers.AllKeys.Contains("Authorization")) return false;

			string authHeader = context.Request.Headers["Authorization"];

			IPrincipal principal;
			if (TryGetPrincipal(authHeader, out principal))
			{
				HttpContext.Current.User = principal;
				return true;
			}
			return false;
		}

		private bool TryGetPrincipal(string authHeader, out IPrincipal principal)
		{
			var creds = ParseAuthHeader(authHeader);
			if (creds != null)
			{
				if (TryGetPrincipal(creds[0], creds[1], out principal)) return true;
			}

			principal = null;
			return false;
		}

		private string[] ParseAuthHeader(string authHeader)
		{
			// Check this is a Basic Auth header 
			if (authHeader == null || authHeader.Length == 0 || !authHeader.StartsWith("Basic")) return null;

			// Pull out the Credentials with are seperated by ':' and Base64 encoded 
			string base64Credentials = authHeader.Substring(6);
			string[] credentials = Encoding.ASCII.GetString(Convert.FromBase64String(base64Credentials)).Split(new char[] { ':' });

			if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[0])) return null;

			// Okay this is the credentials 
			return credentials;
		}

		private bool TryGetPrincipal(string userName, string password, out IPrincipal principal)
		{
			// this is the method that authenticates against my repository (in this case, hard coded)
			// you can replace this with whatever logic you'd use, but proper separation would put the
			// data access in a repository or separate layer/library.
			UserBase user = Repository.Authenticate(userName, password);

			if (user != null)
			{
				// once the user is verified, assign it to an IPrincipal with the identity name and applicable roles
				principal = new GenericPrincipal(new GenericIdentity(user.UserName), user.Roles);
				return true;
			}
			else
			{
				principal = null;
				return false;
			}
		}
	}
}