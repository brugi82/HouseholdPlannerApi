using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlannerApi.Models
{
    public class RegisterInvitationModel: RegistrationModel
    {
		public string InvitationId { get; set; }
	}
}
