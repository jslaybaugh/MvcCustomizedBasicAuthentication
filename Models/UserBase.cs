using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomizedBasic.Models
{
	public class UserBase
	{
		public string Id { get; set; }
		public string UserName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string[] Roles { get; set; }
	}
}