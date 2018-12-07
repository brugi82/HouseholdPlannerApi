using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Data.Models
{
    public class Family
    {
		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public ICollection<ApplicationUser> Members { get; set; }
	}
}
