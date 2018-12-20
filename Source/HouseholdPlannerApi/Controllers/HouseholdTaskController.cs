using HouseholdPlanner.Contracts.Services;
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
	public class HouseholdTaskController: BaseController
    {
		private readonly IHouseholdTaskService _householdTaskService;
		private readonly ILogger<HouseholdTaskController> _logger;

		public HouseholdTaskController(IHouseholdTaskService householdTaskService, ILogger<HouseholdTaskController> logger)
		{
			_householdTaskService = householdTaskService ?? throw new ArgumentNullException(nameof(householdTaskService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpPost]
		public async Task<IActionResult> CreateHouseholdTask(string title, string description)
		{
			if (string.IsNullOrEmpty(title))
				return BadRequest();

			try
			{
				var userId = GetCurrentUserId();

				await _householdTaskService.Add(userId, title, description);

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return BadRequest();
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetFamilyTasks()
		{
			try
			{
				var userId = GetCurrentUserId();

				var tasks = await _householdTaskService.GetUsersFamilyTasks(userId);

				return Ok(tasks);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return BadRequest();
			}
		}
	}
}
