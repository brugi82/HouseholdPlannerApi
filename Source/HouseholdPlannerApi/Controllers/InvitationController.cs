using HouseholdPlanner.Contracts.Services;
using HouseholdPlannerApi.Models;
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
	public class InvitationController: BaseController
	{
		private readonly IInvitationService _invitationService;
		private readonly ILogger<InvitationController> _logger;

		public InvitationController(IInvitationService invitationService, ILogger<InvitationController> logger)
		{
			_invitationService = invitationService ?? throw new ArgumentNullException(nameof(invitationService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpPost]
		public async Task<IActionResult> InviteUser([FromBody]InvitationModel invitationModel)
		{
			if (invitationModel == null)
				return BadRequest();

			try
			{
				var userId = GetCurrentUserId();

				await _invitationService.SendInvitation(invitationModel.Email, invitationModel.Name, userId);

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
