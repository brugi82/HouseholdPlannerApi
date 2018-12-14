using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Models.Domain
{
    public class Invitation
    {
		public string Id { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public bool Used { get; set; }

		public string InviterId { get; set; }
		public string FamilyId { get; set; }
	}
}
