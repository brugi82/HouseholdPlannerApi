using HouseholdPlanner.Contracts.Notification;
using HouseholdPlanner.Contracts.Services;
using HouseholdPlanner.Data.Contracts;
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
        private readonly ILogger<InvitationService> _logger;

        public InvitationService(IUnitOfWorkFactory unitOfWorkFactory, IEmailService emailService, ILogger<InvitationService> logger)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task SendInvitation(string email, string firstName, string inviterId, string familyId)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrEmpty(firstName))
                throw new ArgumentNullException(nameof(firstName));
            if (string.IsNullOrEmpty(inviterId))
                throw new ArgumentNullException(nameof(inviterId));
            if (string.IsNullOrEmpty(familyId))
                throw new ArgumentNullException(nameof(familyId));

            return SendInvitationAsync(email, firstName, inviterId, familyId);
        }

        private async Task SendInvitationAsync(string email, string firstName, string inviterId, string familyId)
        {

        }
    }
}
