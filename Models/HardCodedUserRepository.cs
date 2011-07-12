using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomizedBasic.Models
{
	public class HardCodedUserRepository: IUserRepository
	{
		public UserBase Authenticate(string userName, string password)
		{
			// you would likely query the database here instead...
			if (userName.ToLowerInvariant() == "user" && password == "pass")
				return new UserBase { Id = "1", UserName = "user", FirstName = "Some", LastName = "User", Email = "user@user.com", Roles = new string[] { "Admin", "Manager" } };
			else
				return null;
		}

		public UserBase GetDetails(string userName)
		{
			// you would likely query the database here instead...
			if (userName.ToLowerInvariant() == "user")
				return new UserBase { Id = "1", UserName = "user", FirstName = "Some", LastName = "User", Email = "user@user.com", Roles = new string[] { "Admin", "Manager" } };
			else
				return null;
		}
	}
}