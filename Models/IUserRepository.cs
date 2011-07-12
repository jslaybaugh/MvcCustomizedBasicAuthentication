using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomizedBasic.Models
{
	public interface IUserRepository
	{
		UserBase Authenticate(string userName, string password);
		UserBase GetDetails(string userName);
	}
}
