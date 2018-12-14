using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Data.Models
{
    public class Invitation
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }

        public string MemberId { get; set; }
        public Member Member { get; set; }
        public string FamilyId { get; set; }
        public Family Family { get; set; }
    }
}
