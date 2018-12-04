using HouseholdPlanner.Contracts.Notification;
using HouseholdPlanner.Data.Models;
using HouseholdPlanner.Models.Options;
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
		private readonly ApplicationSettings _applicationSettings;

		public UserService(UserManager<ApplicationUser> userManager, IEmailService emailService, ApplicationSettings applicationSettings,
			ILogger<UserService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
			_applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
        }

        public async Task RegisterUser(RegistrationModel registrationModel)
        {
			var createResult = IdentityResult.Success;
			var user = await _userManager.FindByEmailAsync(registrationModel.Username);
			if (user == null)
			{
				user = MapToNewUser(registrationModel);
				createResult = await _userManager.CreateAsync(user, registrationModel.Password);
			}

            if (createResult.Succeeded)
                await SendConfirmationEmail(user);
            else
                ProcessErrors(nameof(RegisterUser), createResult);
        }

		public async Task ConfirmEmail(string userId, string token)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user != null)
			{
				var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, token);
				if (confirmEmailResult.Succeeded)
					await _emailService.SendWelcomeEmail(user.Email, user.FirstName);
				else
					ProcessErrors(nameof(ConfirmEmail), confirmEmailResult);
			}
			else
				ProcessErrors("", IdentityResult.Failed(new IdentityError() { Description = $"Unable to confirm email. User with id {userId} not found." }));
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
            var confirmEmailUrl = $"{_applicationSettings.ApplicationUrl}api/accounts/ConfirmEmail?i={user.Id}&o={token}";
            await _emailService.SendRegistrationEmail(user.UserName, confirmEmailUrl);
        }

        private void ProcessErrors(string description, IdentityResult createResult)
        {
            foreach (var error in createResult.Errors)
                _logger.LogError($"{description} Code:{error.Code} Description:{error.Description}");

            throw new InvalidOperationException(description);
        }
    }
}
