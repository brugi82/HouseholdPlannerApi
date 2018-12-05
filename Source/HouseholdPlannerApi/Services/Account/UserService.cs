using HouseholdPlanner.Contracts.Notification;
using HouseholdPlanner.Contracts.Security;
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
        private readonly ITokenFactory _tokenFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
		private readonly ApplicationSettings _applicationSettings;

		public UserService(UserManager<ApplicationUser> userManager, IEmailService emailService, ITokenFactory tokenFactory,
            ApplicationSettings applicationSettings, ILogger<UserService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenFactory = tokenFactory ?? throw new ArgumentNullException(nameof(tokenFactory));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
			_applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
        }

        public Task RegisterUser(RegistrationModel registrationModel)
        {
			if (string.IsNullOrEmpty(registrationModel.FirstName))
				throw new ArgumentNullException(nameof(registrationModel.FirstName));
			if(string.IsNullOrEmpty(registrationModel.Username))
				throw new ArgumentNullException(nameof(registrationModel.Username));
			if(string.IsNullOrEmpty(registrationModel.Password))
				throw new ArgumentNullException(nameof(registrationModel.Password));

			return RegisterUserAsync(registrationModel);
        }

		private async Task RegisterUserAsync(RegistrationModel registrationModel)
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

		public Task ConfirmEmail(string userId, string token)
		{
			if (string.IsNullOrEmpty(userId))
				throw new ArgumentNullException(nameof(userId));
			if (string.IsNullOrEmpty(token))
				throw new ArgumentNullException(nameof(token));

			return ConfirmEmailAsync(userId, token);
		}

		private async Task ConfirmEmailAsync(string userId, string token)
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

        public Task<string> GetAccessToken(LoginModel loginModel)
        {
            if (loginModel == null)
                throw new ArgumentNullException(nameof(loginModel));
            if (string.IsNullOrEmpty(loginModel.Username))
                throw new ArgumentNullException(nameof(loginModel.Username));
            if (string.IsNullOrEmpty(loginModel.Password))
                throw new ArgumentNullException(nameof(loginModel.Password));

			return GetAccessTokenAsync(loginModel);
        }

		private async Task<string> GetAccessTokenAsync(LoginModel loginModel)
		{
			var user = await _userManager.FindByEmailAsync(loginModel.Username);
			if (user != null)
			{
				var validCredentials = await _userManager.CheckPasswordAsync(user, loginModel.Password);
				if (validCredentials)
					return _tokenFactory.GenerateJwtToken(user.Email, user.Id);
				else
					throw new ArgumentException("Invalid username or password.");
			}
			else
				throw new ArgumentException("Invalid username or password.");
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
