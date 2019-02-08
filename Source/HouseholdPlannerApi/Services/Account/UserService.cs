using HouseholdPlanner.Contracts.Notification;
using HouseholdPlanner.Contracts.Security;
using HouseholdPlanner.Contracts.Services;
using HouseholdPlanner.Data.EntityFramework.Models;
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
		private readonly IInvitationService _invitationService;
		private readonly ApplicationSettings _applicationSettings;
		private readonly IMemberService _memberService;

		public UserService(UserManager<ApplicationUser> userManager, IEmailService emailService, ITokenFactory tokenFactory,
            IMemberService memberService, IInvitationService invitationService, ApplicationSettings applicationSettings, 
			ILogger<UserService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenFactory = tokenFactory ?? throw new ArgumentNullException(nameof(tokenFactory));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
			_invitationService = invitationService ?? throw new ArgumentNullException(nameof(invitationService));
			_applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
			_memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
        }

        public Task RegisterUser(RegistrationModel registrationModel)
        {
			if (registrationModel == null)
				throw new ArgumentNullException(nameof(registrationModel));
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
			var newUser = await CreateNewUser(registrationModel);
			await SendConfirmationEmail(newUser);
		}

		private async Task<ApplicationUser> CreateNewUser(RegistrationModel registrationModel)
		{
			var user = await _userManager.FindByEmailAsync(registrationModel.Username);
			if (user == null)
			{
				user = MapToNewUser(registrationModel);
				var createResult = await _userManager.CreateAsync(user, registrationModel.Password);
				if (!createResult.Succeeded)
					ProcessErrors("Creating new user failed.", createResult);
			}
			else
				throw new InvalidOperationException($"Unable to create user with email {registrationModel.Username} as it already exists.");

			return user;
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
				{
					await CreateNewMember(user);
				}
				else
					ProcessErrors(nameof(ConfirmEmail), confirmEmailResult);
			}
			else
				ProcessErrors("", IdentityResult.Failed(new IdentityError() { Description = $"Unable to confirm email. User with id {userId} not found." }));
		}

		private async Task CreateNewMember(ApplicationUser user)
		{
			await _memberService.Add(user.Id, user.FirstName, user.LastName);
			await _emailService.SendWelcomeEmail(user.Email, user.FirstName);
		}

		public Task<UserModel> LoginUser(LoginModel loginModel)
		{
			if (loginModel == null)
				throw new ArgumentNullException(nameof(loginModel));
			if (string.IsNullOrEmpty(loginModel.Username))
				throw new ArgumentNullException(nameof(loginModel.Username));
			if (string.IsNullOrEmpty(loginModel.Password))
				throw new ArgumentNullException(nameof(loginModel.Password));

			return LoginUserAsync(loginModel);
		}

		private async Task<UserModel> LoginUserAsync(LoginModel loginModel)
		{
			var user = await _userManager.FindByEmailAsync(loginModel.Username);
			if (user != null)
			{
				var validCredentials = await _userManager.CheckPasswordAsync(user, loginModel.Password);
				if (validCredentials)
				{
					var token = _tokenFactory.GenerateJwtToken(user.Email, user.Id);
					var userModel = new UserModel()
					{
						Id = user.Id,
						FirstName = user.FirstName,
						LastName = user.LastName,
						UserName = user.UserName,
						Token = token
					};

					return userModel;
				}
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
				UserName = registrationModel.Username,
                Email = registrationModel.Username,
                FirstName = registrationModel.FirstName,
                LastName = registrationModel.LastName
            };
        }

        private async Task SendConfirmationEmail(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var encodedToken = System.Web.HttpUtility.UrlEncode(token);
			var confirmEmailUrl = $"{_applicationSettings.ApplicationUrl}/ConfirmEmail/{user.Id}/{encodedToken}";
            await _emailService.SendRegistrationEmail(user.UserName, confirmEmailUrl);
        }

        private void ProcessErrors(string description, IdentityResult identityResult)
        {
			var identityErrors = new StringBuilder();
			foreach (var error in identityResult.Errors)
			{
				identityErrors.AppendLine(error.Description);
				_logger.LogError($"{description} Code:{error.Code} Description:{error.Description}");
			}

            throw new InvalidOperationException(string.IsNullOrEmpty(identityErrors.ToString()) ? description : identityErrors.ToString());
        }

		public Task RegisterInvitedUser(RegisterInvitationModel registerInvitationModel)
		{
			if (registerInvitationModel == null)
				throw new ArgumentNullException(nameof(registerInvitationModel));
			if (string.IsNullOrEmpty(registerInvitationModel.FirstName))
				throw new ArgumentNullException(nameof(registerInvitationModel.FirstName));
			if (string.IsNullOrEmpty(registerInvitationModel.Username))
				throw new ArgumentNullException(nameof(registerInvitationModel.Username));
			if (string.IsNullOrEmpty(registerInvitationModel.Password))
				throw new ArgumentNullException(nameof(registerInvitationModel.Password));
			if (string.IsNullOrEmpty(registerInvitationModel.InvitationId))
				throw new ArgumentNullException(nameof(registerInvitationModel.InvitationId));

			return RegisterInvitedUserAsync(registerInvitationModel);
		}

		private async Task RegisterInvitedUserAsync(RegisterInvitationModel registerInvitationModel)
		{
			var invitation = await _invitationService.GetInvitation(registerInvitationModel.InvitationId);
			if (!invitation.Used)
			{
				var newUser = await CreateNewUser(registerInvitationModel);
				await CreateNewMember(newUser);

				await _invitationService.AcceptInvitation(newUser.Id, registerInvitationModel.InvitationId);
			}
			else
				throw new InvalidOperationException($"Unable to register user. Invitation with id {registerInvitationModel.InvitationId} was already used.");
		}
	}
}
