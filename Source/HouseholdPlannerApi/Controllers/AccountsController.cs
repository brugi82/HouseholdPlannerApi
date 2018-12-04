using HouseholdPlannerApi.Models;
using HouseholdPlannerApi.Services;
using HouseholdPlannerApi.Services.Account;
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
    public class AccountsController:ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IUserService userService, ILogger<AccountsController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody]RegistrationModel registrationModel)
        {
            if (string.IsNullOrEmpty(registrationModel.FirstName) || string.IsNullOrEmpty(registrationModel.Username) ||
                string.IsNullOrEmpty(registrationModel.Password))
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
        public async Task<IActionResult> ConfirmEmail()
        {
            return Ok("Conf");
        }
    }
}
