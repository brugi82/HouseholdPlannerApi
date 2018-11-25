using HouseholdPlanner.Contracts.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlannerApi.Services.Security
{
    public class TokenFactory : ITokenFactory
    {
        private readonly JwtIssuerOptions _jwtIssuerOptions;

        public TokenFactory(JwtIssuerOptions jwtIssuerOptions)
        {
            if (jwtIssuerOptions == null)
                throw new ArgumentNullException(nameof(jwtIssuerOptions));

            _jwtIssuerOptions = jwtIssuerOptions;
        }

        public string GenerateJwtToken(string userName, string userId)
        {
            var identity = GenerateClaimsIdentity(userName, userId);
            var claims = GenerateClaims(userName, identity);
            var jwt = GenerateJwtToken(claims);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        public ClaimsIdentity GenerateClaimsIdentity(string userName, string userId)
        {
            return new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
            {
                new Claim(JwtConstants.JwtClaimIdentifiers.Id, userId),
                new Claim(JwtConstants.JwtClaimIdentifiers.Rol, JwtConstants.JwtClaims.ApiAccess)
            });
        }

        private Claim[] GenerateClaims(string userName, ClaimsIdentity identity)
        {
            return new[]
               {
                 new Claim(JwtRegisteredClaimNames.Sub, userName),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtIssuerOptions.IssuedAt).ToString(),
                             ClaimValueTypes.Integer64),
                             identity.FindFirst(JwtConstants.JwtClaimIdentifiers.Rol),
                             identity.FindFirst(JwtConstants.JwtClaimIdentifiers.Id)
               };
        }

        private JwtSecurityToken GenerateJwtToken(Claim[] claims)
        {
            return new JwtSecurityToken(
                            issuer: _jwtIssuerOptions.Issuer,
                            audience: _jwtIssuerOptions.Audience,
                            claims: claims,
                            notBefore: _jwtIssuerOptions.NotBefore,
                            expires: _jwtIssuerOptions.Expiration,
                            signingCredentials: _jwtIssuerOptions.SigningCredentials);
        }

        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);
    }
}
