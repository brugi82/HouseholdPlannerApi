using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Contracts.Notification
{
    public interface IEmailService
    {
        Task SendRegistrationEmail(string to);
        Task SendWelcomeEmail(string to, string name);
    }
}
