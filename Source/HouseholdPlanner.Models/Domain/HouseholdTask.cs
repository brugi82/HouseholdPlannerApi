using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Models.Domain
{
    public class HouseholdTask
    {
		public string Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
		public string FamilyId { get; set; }
		public string AssignedToId { get; set; }
		public string CreatedById { get; set; }
		public string UpdatedById { get; set; }
	}
}
