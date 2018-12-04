using HouseholdPlanner.Contracts.Notification;
using HouseholdPlanner.Data.Models;
using HouseholdPlannerApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlannerApi.Services.Account
{
    public class UserService: IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public UserService(UserManager<ApplicationUser> userManager, IEmailService emailService, ILogger<UserService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task RegisterUser(RegistrationModel registrationModel)
        {
            var newUser = MapToNewUser(registrationModel);

            var createResult = await _userManager.CreateAsync(newUser, registrationModel.Password);

            if (createResult.Succeeded)
                await SendConfirmationEmail(newUser);
            else
                ProcessErrors(createResult);
        }

        private ApplicationUser MapToNewUser(RegistrationModel registrationModel)
        {
            return new ApplicationUser()
            {
                Email = registrationModel.Username,
                FirstName = registrationModel.FirstName,
                LastName = registrationModel.LastName
            };
        }

        private async Task SendConfirmationEmail(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmEmailCallback = $"/{token}";
            await _emailService.SendRegistrationEmail(user.UserName, confirmEmailCallback);
        }

        private void ProcessErrors(IdentityResult createResult)
        {
            foreach (var error in createResult.Errors)
                _logger.LogError($"Failed to create user. Code:{error.Code} Description:{error.Description}");

            throw new InvalidOperationException("Unable to create user.");
        }
    }
}
