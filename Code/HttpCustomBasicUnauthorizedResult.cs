using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomizedBasic.Code
{
	public class HttpCustomBasicUnauthorizedResult: HttpUnauthorizedResult
	{
		// the base class already assigns the 401.  
		// we bring these constructors with us to allow setting status text
		public HttpCustomBasicUnauthorizedResult() : base() { }
		public HttpCustomBasicUnauthorizedResult(string statusDescription) : base(statusDescription) { }

		public override void ExecuteResult(ControllerContext context)
		{
			if (context == null) throw new ArgumentNullException("context");

			// this is really the key to bringing up the basic authentication login prompt.
			// this header is what tells the client we need basic authentication
			context.HttpContext.Response.AddHeader("WWW-Authenticate", "Basic");
			base.ExecuteResult(context);
		}
	}
}