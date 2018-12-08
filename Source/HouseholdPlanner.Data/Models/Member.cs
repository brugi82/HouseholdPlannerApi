using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Data.Models
{
    public class Member
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

		public string FamilyId { get; set; }
        public Family Family { get; set; }
    }
}
