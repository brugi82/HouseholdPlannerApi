using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlannerApi.Services.Security
{
    public static class JwtConstants
    {
        public static class JwtClaimIdentifiers
        {
            public const string Rol = "rol";
            public const string Id = "id";
        }

        public static class JwtClaims
        {
            public const string ApiAccess = "api_access";
        }
    }
}
