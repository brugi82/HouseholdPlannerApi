﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseholdPlannerApi.Models
{
	public class UserModel
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string Token { get; set; }
	}
}
