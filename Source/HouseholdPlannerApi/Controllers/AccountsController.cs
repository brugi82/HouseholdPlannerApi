using HouseholdPlanner.Contracts.Security;
using HouseholdPlannerApi.Models;
using HouseholdPlannerApi.Services;
using HouseholdPlannerApi.Services.Account;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HouseholdPlannerApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("SiteCorsPolicy")]
    [ApiController]
    public class AccountsController:ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenFactory _tokenFactory;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IUserService userService, ITokenFactory tokenFactory, ILogger<AccountsController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tokenFactory = tokenFactory ?? throw new ArgumentNullException(nameof(tokenFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody]RegistrationModel registrationModel)
        {
			if (registrationModel == null)
				return BadRequest();

            try
            {
                await _userService.RegisterUser(registrationModel);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

		[HttpPost]
		public async Task<IActionResult> RegisterInvitedUser([FromBody]RegisterInvitationModel registerInvitationModel)
		{
			if (registerInvitationModel == null)
				return BadRequest();

			try
			{
				await _userService.RegisterInvitedUser(registerInvitationModel);

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return BadRequest();
			}
		}

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail([Bind(Prefix ="i")] string userId, [Bind(Prefix = "o")] string token)
        {
			try
			{
				await _userService.ConfirmEmail(userId, HttpUtility.UrlDecode(token));

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return BadRequest();
			}
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            try
            {
                var accessToken = await _userService.GetAccessToken(loginModel);

                return Ok(accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }
    }
}
