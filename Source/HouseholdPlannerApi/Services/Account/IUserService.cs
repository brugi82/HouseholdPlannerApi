using HouseholdPlannerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlannerApi.Services.Account
{
    public interface IUserService
    {
        Task RegisterUser(RegistrationModel registrationModel);
    }
}
