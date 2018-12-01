using HouseholdPlanner.Data.Models;
using HouseholdPlannerApi.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlannerApi.Services
{
    public class UserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task RegisterUser(RegistrationModel registrationModel)
        {
            var user = new ApplicationUser()
            {
                Email = registrationModel.Username,
                FirstName = registrationModel.FirstName,
                LastName = registrationModel.LastName
            };

            await _userManager.CreateAsync(user, registrationModel.Password);
        }
    }
}
