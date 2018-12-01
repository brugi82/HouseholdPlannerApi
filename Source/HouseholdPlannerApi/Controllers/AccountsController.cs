using Microsoft.AspNetCore.Mvc;
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
        public AccountsController()
        {

        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] RegistrationModel registrationModel)
        {
            return Ok("Reg");
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail()
        {
            return Ok("Conf");
        }
    }
}
