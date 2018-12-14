using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Contracts.Services
{
    public interface IInvitationService
    {
        Task SendInvitation(string email, string firstName, string inviterId);
    }
}
