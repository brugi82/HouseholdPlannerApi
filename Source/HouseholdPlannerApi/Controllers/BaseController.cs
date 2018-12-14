using HouseholdPlannerApi.Services.Security;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlannerApi.Controllers
{
    public class BaseController: ControllerBase
    {
        protected string GetCurrentUserId()
		{
			var idClaim = User.Claims.Single(c => c.Type == JwtConstants.JwtClaimIdentifiers.Id);
			var userId = idClaim.Value;

			return userId;
		}
    }
}
