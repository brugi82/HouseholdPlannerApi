using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Contracts.Security
{
    public interface ITokenFactory
    {
        string GenerateJwtToken(string userName, string userId);
    }
}
