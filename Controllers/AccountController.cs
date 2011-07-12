using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomizedBasic.Models;
using CustomizedBasic.Code;

namespace CustomizedBasic.Controllers
{
	public class AccountController : Controller
	{
		// this is only necessary if you are doing dependency injection
		IUserRepository _UserRepo;
		public AccountController(IUserRepository userRepo)
		{
			_UserRepo = userRepo;
		}
		// end DI code

		public ActionResult Authenticate()
		{
			return Json("This is open for unauthenticated access.", JsonRequestBehavior.AllowGet);
		}

		[CustomBasicAuthorize]
		public ActionResult Me()
		{
			return Json(_UserRepo.GetDetails(User.Identity.Name), JsonRequestBehavior.AllowGet);
		}

	}
}
