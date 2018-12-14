using HouseholdPlanner.Contracts.Services;
using HouseholdPlannerApi.Models;
using HouseholdPlannerApi.Services.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlannerApi.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class FamilyController: ControllerBase
    {
		private readonly IFamilyService _familyService;
		private readonly ILogger<FamilyController> _logger;

		public FamilyController(IFamilyService familyService, ILogger<FamilyController> logger)
		{
			_familyService = familyService ?? throw new ArgumentNullException(nameof(familyService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpPost]
		public async Task<IActionResult> CreateFamily([FromBody]FamilyModel familyModel)
		{
			if (familyModel == null)
				return BadRequest();

			try
			{
				var idClaim = User.Claims.Single(c => c.Type == JwtConstants.JwtClaimIdentifiers.Id);
				var userId = idClaim.Value;

				await _familyService.Add(userId, familyModel.Name, familyModel.Description);

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return BadRequest();
			}
		}
	}
}
