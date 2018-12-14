using HouseholdPlanner.Contracts.Notification;
using HouseholdPlanner.Contracts.Services;
using HouseholdPlanner.Data.Contracts;
using HouseholdPlanner.Models.Options;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IEmailService _emailService;
		private readonly ApplicationSettings _applicationSettings;
		private readonly ILogger<InvitationService> _logger;

        public InvitationService(IUnitOfWorkFactory unitOfWorkFactory, IEmailService emailService, ApplicationSettings applicationSettings,
			ILogger<InvitationService> logger)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
			_applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task SendInvitation(string email, string firstName, string inviterId)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrEmpty(firstName))
                throw new ArgumentNullException(nameof(firstName));
            if (string.IsNullOrEmpty(inviterId))
                throw new ArgumentNullException(nameof(inviterId));

            return SendInvitationAsync(email, firstName, inviterId);
        }

        private async Task SendInvitationAsync(string email, string firstName, string inviterId)
        {
			try
			{
				using (var unitOfWork = _unitOfWorkFactory.Create())
				{
					var inviter = await unitOfWork.MemberRepository.GetAsync(inviterId) ?? throw new ArgumentException(nameof(inviterId));
					var family = await unitOfWork.FamilyRepository.GetAsync(inviter.FamilyId) ?? throw new ArgumentException(nameof(inviter.FamilyId));

					var invitation = new Data.Models.Invitation()
					{
						Email = email,
						FirstName = firstName,
						Family = family,
						Member = inviter
					};

					unitOfWork.InvitationRepository.Add(invitation);
					await unitOfWork.SaveAsync();

					var encodedEmail = Convert.ToBase64String(Encoding.UTF8.GetBytes(email));
					var acceptInvitationUri = $"{_applicationSettings.ApplicationUrl}/accounts/AcceptInvitation?d={invitation.Id}&m={encodedEmail}";
					await _emailService.SendInvitation(email, firstName, inviter.FirstName, family.Name, acceptInvitationUri);
				}
			}
			catch(Exception ex)
			{
				_logger.LogError(ex.Message);
				throw;
			}
        }
    }
}
